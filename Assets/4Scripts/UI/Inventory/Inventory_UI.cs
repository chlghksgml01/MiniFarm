using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static Inventory;

public class Inventory_UI : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private ToolBar_UI toolBar_UI;

    public List<Slot_UI> slotsUIs = new List<Slot_UI>();

    public SelectedItem_UI selectedItem;


    ISelectionStrategy selectionStrategy;
    LeftClickStrategy leftClick;
    RightClickStrategy rightClick;
    ShiftRightClickStrategy shiftRightClick;

    public DragState dragState = new DragState();
    public class DragState
    {
        public bool isClick = false;
        public bool isDragging = false;
    }

    private void Awake()
    {
        leftClick = new LeftClickStrategy(selectedItem, dragState);
        rightClick = new RightClickStrategy(selectedItem, dragState);
        shiftRightClick = new ShiftRightClickStrategy(selectedItem, dragState);

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            slotsUIs[i].InitializeSlot(i); // 각 슬롯에 인덱스 설정
        }
    }

    private void Start()
    {
        if (selectedItem == null)
        {
            Debug.Log("Inventory_UI - selectedItem 없음");
            return;
        }
        selectedItem.gameObject.SetActive(false);
        inventory = GameManager.Instance.player.inventory;
    }

    void Update()
    {
        KeyInput();
        if (dragState.isDragging)
        {
            selectedItem.transform.position = Input.mousePosition;
        }
    }

    private void LateUpdate()
    {
        dragState.isClick = false;
    }

    void KeyInput()
    {
        if (!GameManager.Instance.uiManager.inventoryPanel.activeSelf)
            return;

        if (!dragState.isClick && Input.GetMouseButtonDown(0))
        {
            SetStrategy(leftClick);
        }

        else if (!dragState.isClick && Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftShift))
        {
            SetStrategy(shiftRightClick);
        }

        else if (!dragState.isClick && Input.GetMouseButtonDown(1))
        {
            SetStrategy(rightClick);
        }
    }
    public void SetStrategy(ISelectionStrategy currentStrategy)
    {
        selectionStrategy = currentStrategy;
        selectionStrategy.ClickHandle();
    }

    public void Refresh()
    {
        if (inventory == null)
        {
            inventory = GameManager.Instance.player.inventory;
        }

        if (slotsUIs.Count != inventory.slots.Count)
        {
            Debug.Log("Inventory_UI - 인벤UI, 인벤 개수 다름");
            return;
        }

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            if (!inventory.slots[i].IsEmpty())
            {
                slotsUIs[i].SetItem(inventory.slots[i]);

                if (i < 9)
                    toolBar_UI.slotsUIs[i].SetItem(inventory.slots[i]);
            }
            else
            {
                slotsUIs[i].SetEmtpy();

                if (i < 9)
                    toolBar_UI.slotsUIs[i].SetEmtpy();
            }
        }
    }

    // 버튼 함수
    public void SortInventory()
    {
        if (dragState.isDragging)
        {
            PlaceItem();
        }

        inventory.SortInventory();
        Refresh();
    }

    void PlaceItem()
    {
        dragState.isDragging = false;

        foreach (Slot slot in inventory.slots)
        {
            if (selectedItem.itemName == slot.itemName)
            {
                slot.count += selectedItem.Count;
                selectedItem.SetEmpty();
                return;
            }
        }

        foreach (Slot slot in inventory.slots)
        {
            if (slot.IsEmpty())
            {
                slot.Refresh(selectedItem.itemName, selectedItem.Icon, selectedItem.Count);
                selectedItem.SetEmpty();
                return;
            }
        }
    }

    public void CloseInventoryUI()
    {
        if (dragState.isDragging)
            DropItem();

        dragState.isDragging = false;
        selectedItem.gameObject.SetActive(false);
    }

    void DropItem()
    {
        GameManager.Instance.player.CreateDropItem(selectedItem, selectedItem.Count);
    }

    public void TrashBin()
    {
        selectedItem.SetEmpty();
        dragState.isDragging = false;
    }
}