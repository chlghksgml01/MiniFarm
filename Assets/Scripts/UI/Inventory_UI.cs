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
        All,
        Half,
        One,
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
            slotsUI[i].InitializeSlot(i); // 각 슬롯에 인덱스 설정
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
            SelectItemFromSlot(results, SelectionMode.All);
        }

        else if (!isClick && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            SelectItemFromSlot(results, SelectionMode.Half);
        }

        else if (!isClick && Input.GetMouseButtonDown(1))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            SelectItemFromSlot(results, SelectionMode.One);
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

                    // 클릭한 슬롯 저장
                    List<Slot> _slots = player.inventory.slots;
                    sourceSlot = _slots[_slotUI.slotIdx];

                    // SelectedItem 설정
                    selectedItem.type = sourceSlot.type;
                    selectedItem.iconImage.sprite = sourceSlot.icon;
                    switch (selectionMode)
                    {
                        case SelectionMode.All:
                            selectedItem.Quantity = sourceSlot.quantity;
                            sourceSlot.quantity = 0;
                            break;

                        case SelectionMode.Half:
                            selectedItem.Quantity = (int)Math.Ceiling(sourceSlot.quantity / 2f);
                            sourceSlot.quantity -= selectedItem.Quantity;
                            break;

                        case SelectionMode.One:
                            selectedItem.Quantity = 1;
                            sourceSlot.quantity -= 1;
                            break;
                    }

                    if (sourceSlot.quantity == 0)
                        sourceSlot.type = CollectableType.NONE;
                    Refresh();

                    selectedItem.transform.SetParent(transform.root); // UI 최상위로 이동
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
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            MoveItem(results, SelectionMode.All);
        }

        else if (!isClick && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            MoveItem(results, SelectionMode.Half);
        }

        else if (!isClick && Input.GetMouseButtonDown(1))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            MoveItem(results, SelectionMode.One);
        }
    }

    void MoveItem(List<RaycastResult> results, SelectionMode selectionMode)
    {
        isClick = true;

        // 슬롯재설정
        foreach (var result in results)
        {
            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                // 클릭한 슬롯 가져오기
                List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().inventory.slots;
                Slot _slot = _slots[_slotUI.slotIdx];

                int _slotQuantity = 0;
                switch (selectionMode)
                {
                    case SelectionMode.All:
                        // 슬롯 비어있을 경우
                        if (_slot.type == CollectableType.NONE)
                        {
                            _slotQuantity = selectedItem.Quantity;
                            selectedItem.Quantity = 0;
                            _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, _slotQuantity);
                        }

                        // 다른 아이템 있을 경우
                        else
                        {
                            SwapItemWithSlot(_slots, _slot, _slotUI, _slotQuantity);
                        }
                        break;

                    case SelectionMode.Half:
                        if (_slot.type != CollectableType.NONE && _slot.type != selectedItem.type)
                            return;

                        int tempquantity = selectedItem.Quantity;
                        selectedItem.Quantity = (int)(selectedItem.Quantity / 2f);
                        _slotQuantity = _slot.quantity + (tempquantity - selectedItem.Quantity);
                        _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, _slotQuantity);
                        break;

                    case SelectionMode.One:
                        if (_slot.type != CollectableType.NONE && _slot.type != selectedItem.type)
                            return;

                        selectedItem.Quantity -= 1;
                        _slotQuantity = _slot.quantity + 1;
                        _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, _slotQuantity);
                        break;
                }

                if (selectedItem.Quantity == 0)
                    isDragging = false;
                Refresh();
            }
        }
    }

    void SwapItemWithSlot(List<Slot> _slots, Slot _slot, Slot_UI _slotUI, int _slotQuantity)
    {
        // 같은 아이템
        if (_slot.type == selectedItem.type)
        {
            _slotQuantity = _slot.quantity + selectedItem.Quantity;
            selectedItem.Quantity = 0;
            _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, _slotQuantity);
        }
        // 다른 아이템
        else
        {
            Slot tempslot = new Slot();
            tempslot.type = _slot.type;
            tempslot.icon = _slot.icon;
            tempslot.quantity = _slot.quantity;

            _slotQuantity = selectedItem.Quantity;
            _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.iconImage.sprite, selectedItem.Quantity);

            selectedItem.type = tempslot.type;
            selectedItem.iconImage.sprite = tempslot.icon;
            selectedItem.Quantity = tempslot.quantity;
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
            Debug.Log("인벤UI, 인벤 개수 다름");
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

        // Unity에서 UI 입력을 관리하는 시스템을 가져와서 현재 UI의 입력 이벤트를 처리하려는 변수
        pointerData = new PointerEventData(EventSystem.current);
        // 마우스 커서 위치 저장
        pointerData.position = Input.mousePosition;

        // 레이캐스트 결과 저장용 변수
        results = new List<RaycastResult>();
        // pointerData에 있는 마우스 커서 위치를 기준으로 UI 요소를 검사하고 충돌한 요소를 results에 저장
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    public int GetInventoryCount()
    {
        return slotsUI.Count;
    }
}