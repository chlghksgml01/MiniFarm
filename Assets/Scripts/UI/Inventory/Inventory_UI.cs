using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static Inventory;

public class Inventory_UI : MonoBehaviour
{
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
    bool isInventoryAreaClicked = false;

    [SerializeField] GameObject dropItem;
    [SerializeField] SelectedItem_UI selectedItem;

    PointerEventData pointerData;



    // 디자인패턴
    ISelectionStrategy selectionStrategy;
    private LeftClickStrategy leftClick;
    private RightClickStrategy rightClick;
    private ShiftRightClickStrategy shiftRightClick;


    public class DragState
    {
        public bool isClick = false;
        public bool isDragging = false;
    }
    DragState dragState = new DragState();

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        selectedItem.gameObject.SetActive(false);

        leftClick = new LeftClickStrategy(selectedItem, player, dragState);
        rightClick = new RightClickStrategy(selectedItem, player, dragState);
        shiftRightClick = new ShiftRightClickStrategy(selectedItem, player, dragState);

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
    }

    void Update()
    {
        if (!dragState.isDragging)
            KeyInput();
        else
            DragKeyInput();

    }

    private void LateUpdate()
    {
        dragState.isClick = false;
    }

    void KeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (!inventoryPanel.activeSelf)
            return;

        if (!dragState.isClick && Input.GetMouseButtonDown(0))
        {
            SetStrategy(leftClick);
            selectionStrategy.ClickHandle();
        }

        else if (!dragState.isClick && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            SetStrategy(shiftRightClick);
            selectionStrategy.ClickHandle();
        }

        else if (!dragState.isClick && Input.GetMouseButtonDown(1))
        {
            SetStrategy(rightClick);
            selectionStrategy.ClickHandle();
        }
    }
    public void SetStrategy(ISelectionStrategy currentStrategy)
    {
        selectionStrategy = currentStrategy;
    }

    void DragKeyInput()
    {
        pointerData.position = Input.mousePosition;
        selectedItem.transform.position = pointerData.position;

        if (!dragState.isClick && Input.GetMouseButtonDown(0))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            MoveItem(results, SelectionMode.All);
        }

        else if (!dragState.isClick && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            MoveItem(results, SelectionMode.Half);
        }

        else if (!dragState.isClick && Input.GetMouseButtonDown(1))
        {
            List<RaycastResult> results = DetectUIUnderCursor();
            MoveItem(results, SelectionMode.One);
        }
    }

    void MoveItem(List<RaycastResult> results, SelectionMode selectionMode)
    {
        dragState.isClick = true;

        // 슬롯재설정
        foreach (var result in results)
        {
            // 쓰레기통, 정리버튼이라면
            if (result.gameObject.name == "Sort_Button" || result.gameObject.name == "TrashBin")
                return;

            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                isInventoryAreaClicked = true;

                // 클릭한 슬롯 가져오기
                List<Slot> _slots = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().PlayerInventory.slots;
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
                            _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, _slotQuantity);
                            selectedItem.SetEmpty();
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
                        _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, _slotQuantity);
                        break;

                    case SelectionMode.One:
                        if (_slot.type != CollectableType.NONE && _slot.type != selectedItem.type)
                            return;

                        selectedItem.Quantity -= 1;
                        _slotQuantity = _slot.quantity + 1;
                        _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, _slotQuantity);
                        break;
                }

                if (selectedItem.Quantity == 0)
                {
                    dragState.isDragging = false;
                    selectedItem.SetEmpty();
                }
                Refresh();
            }
        }

        if (!isInventoryAreaClicked)
        {
            DropItem(selectionMode);
        }

        else
            isInventoryAreaClicked = false;
    }

    void SwapItemWithSlot(List<Slot> _slots, Slot _slot, Slot_UI _slotUI, int _slotQuantity)
    {
        // 같은 아이템
        if (_slot.type == selectedItem.type)
        {
            _slotQuantity = _slot.quantity + selectedItem.Quantity;
            selectedItem.Quantity = 0;
            _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, _slotQuantity);
            selectedItem.SetEmpty();
        }
        // 다른 아이템
        else
        {
            Slot tempslot = new Slot();
            tempslot.type = _slot.type;
            tempslot.icon = _slot.icon;
            tempslot.quantity = _slot.quantity;

            _slotQuantity = selectedItem.Quantity;
            _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, selectedItem.Quantity);

            selectedItem.type = tempslot.type;
            selectedItem.Icon = tempslot.icon;
            selectedItem.Quantity = tempslot.quantity;
        }
    }

    void DropItem(SelectionMode selectionMode)
    {
        CollectableItem _dropItem = dropItem.GetComponent<CollectableItem>();
        if (_dropItem == null)
            return;

        int _dropItemQuantity = 0;
        switch (selectionMode)
        {
            case SelectionMode.All:
                _dropItemQuantity = selectedItem.Quantity;
                selectedItem.Quantity = 0;
                break;
            case SelectionMode.Half:
                _dropItemQuantity = (int)Math.Ceiling(selectedItem.Quantity / 2f);
                selectedItem.Quantity -= _dropItemQuantity;
                break;
            case SelectionMode.One:
                _dropItemQuantity = 1;
                selectedItem.Quantity--;
                break;
        }

        Vector3 bounceBasePos = new Vector3(player.transform.position.x + 0.5f, player.transform.position.y - 0.5f);
        _dropItem = Instantiate(_dropItem, bounceBasePos, Quaternion.identity);

        _dropItem.BounceBasePos = bounceBasePos;
        _dropItem.GetComponent<SpriteRenderer>().sprite = selectedItem.Icon;
        _dropItem.Type = selectedItem.type;
        _dropItem.Quantity = _dropItemQuantity;
        _dropItem.IsBouncing = true;

        if (selectedItem.Quantity == 0)
        {
            dragState.isDragging = false;
            selectedItem.SetEmpty();
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
            if (dragState.isDragging)
            {
                dragState.isDragging = false;
                selectedItem.gameObject.SetActive(false);
            }
        }
    }

    public void Refresh()
    {
        if (slotsUI.Count != player.PlayerInventory.slots.Count)
        {
            Debug.Log("인벤UI, 인벤 개수 다름");
            return;
        }

        for (int i = 0; i < slotsUI.Count; i++)
        {
            if (player.PlayerInventory.slots[i].type != CollectableType.NONE)
            {
                slotsUI[i].SetItem(player.PlayerInventory.slots[i]);
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

        pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    // 버튼 함수
    public void SortInventory()
    {
        if (dragState.isDragging)
        {
            PlaceItem();
        }

        player.PlayerInventory.SortInventory();
        Refresh();
    }

    void PlaceItem()
    {
        dragState.isDragging = false;
        bool hasSameItem = false;

        foreach (Slot slot in player.PlayerInventory.slots)
        {
            if (selectedItem.type == slot.type)
            {
                hasSameItem = true;
                slot.quantity += selectedItem.Quantity;
                selectedItem.SetEmpty();
            }
        }

        if (!hasSameItem)
        {
            foreach (Slot slot in player.PlayerInventory.slots)
            {
                if (slot.type == CollectableType.NONE)
                {
                    slot.Refresh(selectedItem.type, selectedItem.Icon, selectedItem.Quantity);
                    selectedItem.SetEmpty();
                }
            }
        }
    }

    public void TrashBin()
    {
        selectedItem.SetEmpty();
        dragState.isDragging = false;
    }
}