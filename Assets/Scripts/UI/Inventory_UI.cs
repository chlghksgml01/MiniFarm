using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Inventory;

public class Inventory_UI : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    private static Inventory_UI instance;
    public static Inventory_UI Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("Inventory_UI");
                instance = go.AddComponent<Inventory_UI>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    enum SelectionMode
    {
        SelectAll,
        SelectOne,
        SelectHalf
    }

    [SerializeField] List<Slot_UI> slotsUI = new List<Slot_UI>();

    public int slotCount = 0;

    GameObject inventoryPanel;
    Player player;
    bool isDragging = false;
    bool isClick = false;

    [SerializeField] SelectedItem selectedItem;
    Slot sourceSlot;

    PointerEventData pointerData;

    private void Awake()
    {
        selectedItem.gameObject.SetActive(false);
        pointerData = new PointerEventData(EventSystem.current);

        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        inventoryPanel = transform.Find("Background").gameObject;
        inventoryPanel.SetActive(false);

        for (int i = 0; i < slotsUI.Count; i++)
        {
            slotsUI[i].InitializeSlot(i); // �� ���Կ� �ε��� ����
        }

        slotCount = slotsUI.Count;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (!isDragging)
            KeyInput();
        else
            DragKeyInput();
    }

    private void LateUpdate()
    {
        isClick = false;
    }

    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (!isClick && Input.GetMouseButtonDown(0))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            SelectItemFromSlot(results, SelectionMode.SelectAll);
        }

        else if (!isClick && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            SelectItemFromSlot(results, SelectionMode.SelectHalf);
        }

        else if (!isClick && Input.GetMouseButtonDown(1))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            SelectItemFromSlot(results, SelectionMode.SelectOne);
        }

    }

    void SelectItemFromSlot(List<RaycastResult> results, SelectionMode selectionMode)
    {
        foreach (var result in results)
        {
            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                if (_slotUI.quantity > 0)
                {
                    selectedItem.gameObject.SetActive(true);
                    isClick = true;
                    isDragging = true;

                    // Ŭ���� ���� ����
                    List<Slot> _slots = player.inventory.slots;
                    sourceSlot = _slots[_slotUI.slotIdx];

                    // SelectedItem ����
                    selectedItem.type = sourceSlot.type;
                    selectedItem.iconImage.sprite = sourceSlot.icon;
                    switch (selectionMode)
                    {
                        case SelectionMode.SelectAll:
                            selectedItem.Quantity = sourceSlot.quantity;
                            sourceSlot.quantity = 0;
                            break;

                        case SelectionMode.SelectOne:
                            selectedItem.Quantity = 1;
                            sourceSlot.quantity -= 1;
                            break;

                        case SelectionMode.SelectHalf:
                            selectedItem.Quantity = (int)Math.Ceiling(sourceSlot.quantity / 2f);
                            sourceSlot.quantity -= selectedItem.Quantity;
                            break;
                    }

                    if (sourceSlot.quantity == 0)
                        sourceSlot.type = CollectableType.NONE;
                    Refresh();

                    selectedItem.transform.SetParent(transform.root); // UI �ֻ����� �̵�
                    selectedItem.transform.position = pointerData.position;
                }
            }
        }
    }

    void DragKeyInput()
    {
        pointerData.position = Input.mousePosition;
        selectedItem.transform.position = pointerData.position;

        if (!isClick && Input.GetMouseButtonDown(0))
            MoveItem();
        else if (!isClick && Input.GetMouseButtonDown(1))
            MoveItemOneOrHalf();
    }

    void MoveItem()
    {
        isClick = true;
        List<RaycastResult> results = DetectUIUnderCursor();

        // �����缳��
        foreach (var result in results)
        {
            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                // Ŭ���� ���� ��������
                List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.slots;
                Slot _slot = _slots[_slotUI.slotIdx];

                // ���� ������� ���
                if (_slot.type == CollectableType.NONE)
                {
                    // ���� �� ����
                    _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, selectedItem.Quantity);

                    isDragging = false;
                    selectedItem.Quantity = 0;
                    selectedItem.gameObject.SetActive(false);
                }

                // �ٸ� ������ ���� ���
                else
                {
                    if (_slot.type == selectedItem.type)
                    {
                        isDragging = false;
                        _slot.quantity += selectedItem.Quantity;
                        selectedItem.Quantity = 0;
                        selectedItem.gameObject.SetActive(false);
                    }
                    else
                    {
                        Slot tempslot = new Slot();
                        tempslot.type = _slot.type;
                        tempslot.icon = _slot.icon;
                        tempslot.quantity = _slot.quantity;

                        _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, selectedItem.Quantity);

                        selectedItem.type = tempslot.type;
                        selectedItem.iconImage.sprite = tempslot.icon;
                        selectedItem.Quantity = tempslot.quantity;
                    }
                }
                Refresh();
            }
        }
    }

    void MoveItemOneOrHalf()
    {
        isClick = true;
        List<RaycastResult> results = DetectUIUnderCursor();

        foreach (var result in results)
        {
            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.slots;
                Slot _slot = _slots[_slotUI.slotIdx];

                if (_slot.type != CollectableType.NONE && _slot.type != selectedItem.type)
                    return;

                // LeftShift + ��Ŭ��
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    int tempquantity = selectedItem.Quantity; 
                    selectedItem.Quantity = (int)Math.Ceiling(selectedItem.Quantity / 2f); 
                    _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, _slot.quantity + (tempquantity - selectedItem.Quantity));
                }

                // ��Ŭ��
                else
                {
                    selectedItem.Quantity -= 1;
                    _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, 1);
                }

                Refresh();
            }
        }
    }

    public void ToggleInventory()
    {
        if (!inventoryPanel.activeSelf)
        {
            inventoryPanel.SetActive(true);
            Refresh();
        }
        else
        {
            inventoryPanel.SetActive(false);
            if (isDragging)
            {
                isDragging = false;
                selectedItem.gameObject.SetActive(false);
            }
        }
    }

    void Refresh()
    {
        if (slotsUI.Count != player.inventory.slots.Count)
        {
            Debug.Log("�κ�UI, �κ� ���� �ٸ�");
            return;
        }

        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (player.inventory.slots[i].type != CollectableType.NONE)
            {
                slotsUI[i].SetItem(player.inventory.slots[i]);
            }
            else
            {
                slotsUI[i].SetEmtpy();
            }
        }
    }

    List<RaycastResult> DetectUIUnderCursor()
    {
        List<RaycastResult> results;

        // Unity���� UI �Է��� �����ϴ� �ý����� �����ͼ� ���� UI�� �Է� �̺�Ʈ�� ó���Ϸ��� ����
        pointerData = new PointerEventData(EventSystem.current);
        // ���콺 Ŀ�� ��ġ ����
        pointerData.position = Input.mousePosition;

        // ����ĳ��Ʈ ��� ����� ����
        results = new List<RaycastResult>();
        // pointerData�� �ִ� ���콺 Ŀ�� ��ġ�� �������� UI ��Ҹ� �˻��ϰ� �浹�� ��Ҹ� results�� ����
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    public int GetInventoryCount()
    {
        return slotsUI.Count;
    }
}