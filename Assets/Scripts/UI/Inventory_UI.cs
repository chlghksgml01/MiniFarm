using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
    [SerializeField] GameObject selectedItemPrefab;

    public int slotCount = 0;

    GameObject inventoryPanel;
    Player player;
    bool isDragging = false;
    bool isClick = false;

    GameObject selectedItemUI;
    Slot sourceSlot;

    PointerEventData pointerData;

    private void Awake()
    {
        selectedItemUI = Instantiate(selectedItemPrefab);
        selectedItemUI.transform.SetParent(canvas.transform, false);
        selectedItemUI.SetActive(false);
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
        KeyInput();

        if (isDragging)
            DragItem();
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

        if (!isClick && !isDragging && Input.GetMouseButtonDown(0))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            GetSlotUI(results, SelectionMode.SelectAll);
        }

        else if (!isClick && !isDragging && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            GetSlotUI(results, SelectionMode.SelectHalf);
        }

        else if (!isClick && !isDragging && Input.GetMouseButtonDown(1))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            GetSlotUI(results, SelectionMode.SelectOne);
        }

    }

    void GetSlotUI(List<RaycastResult> results, SelectionMode selectionMode)
    {
        foreach (var result in results)
        {
            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                if (_slotUI.quantity > 0)
                {
                    isClick = true;
                    isDragging = true;
                    selectedItemUI.SetActive(true);

                    // ������ ���� ����
                    List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.slots;
                    sourceSlot = _slots[_slotUI.slotIdx];

                    // Icon �̹��� ��������
                    GameObject _icon = _slotUI.gameObject.transform.Find("Icon").gameObject;
                    Image selectedIcon = selectedItemUI.transform.Find("Icon").gameObject.GetComponent<Image>();
                    if (selectedIcon != null)
                    {
                        selectedIcon.sprite = _slotUI.ItemIcon.sprite;
                        selectedIcon.color = new Color(1, 1, 1, 1);
                    }

                    // Text ��������
                    GameObject _quantity = _slotUI.gameObject.transform.Find("Quantity").gameObject;
                    TextMeshProUGUI selectedQuantityText = selectedItemUI.transform.Find("Quantity").GetComponent<TextMeshProUGUI>();

                    if (selectedQuantityText != null)
                    {
                        switch (selectionMode)
                        {
                            case SelectionMode.SelectAll:
                                selectedQuantityText.text = sourceSlot.quantity.ToString();
                                sourceSlot.quantity = 0;
                                break;

                            case SelectionMode.SelectOne:
                                selectedQuantityText.text = "1";
                                sourceSlot.quantity -= 1;

                                break;

                            case SelectionMode.SelectHalf:
                                selectedQuantityText.text = (sourceSlot.quantity / 2).ToString();
                                sourceSlot.quantity -= (sourceSlot.quantity / 2);
                                break;
                        }
                    }

                    slotCount = int.Parse(selectedQuantityText.text);

                    if (sourceSlot.quantity == 0)
                        sourceSlot.type = CollectableType.NONE;
                    Refresh();

                    selectedItemUI.transform.SetParent(transform.root); // UI �ֻ����� �̵�
                    selectedItemUI.transform.position = pointerData.position;

                    if (_slotUI.quantity == 0)
                        _slotUI.SetEmtpy();

                }
            }
        }
    }

    void DragItem()
    {
        pointerData.position = Input.mousePosition;
        selectedItemUI.transform.position = pointerData.position;

        if (!isClick && Input.GetMouseButtonDown(0))
        {
            isClick = true;
            List<RaycastResult> results = DetectUIUnderCursor();

            // ���� �ٲٱ�
            foreach (var result in results)
            {
                Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
                if (_slotUI != null)
                {
                    isDragging = false;

                    // ���� ����
                    List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.slots;
                    //_slots[_slotUI.slotIdx].Refresh(sourceSlot.type, selectedQuantity, sourceSlot.icon);

                    Refresh();
                    selectedItemUI.SetActive(false);
                }
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
                selectedItemUI.SetActive(false);
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