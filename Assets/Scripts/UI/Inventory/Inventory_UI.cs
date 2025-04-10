using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static Inventory;

public class Inventory_UI : MonoBehaviour
{
    private Inventory inventory;
    public string inventoryName;

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
        selectedItem.gameObject.SetActive(false);

        leftClick = new LeftClickStrategy(selectedItem, dragState);
        rightClick = new RightClickStrategy(selectedItem, dragState);
        shiftRightClick = new ShiftRightClickStrategy(selectedItem, dragState);

        GameManager.Instance.uiManager.inventoryPanel.SetActive(false);

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            slotsUIs[i].InitializeSlot(i); // 각 슬롯에 인덱스 설정
        }
    }

    private void Start()
    {
        inventory = GameManager.Instance.player.inventoryManager.GetInventoryByName(inventoryName);
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

    public void Refresh()
    {
        if (inventory == null)
        {
            inventory = GameManager.Instance.player.inventoryManager.GetInventoryByName(inventoryName);
        }

        if (slotsUIs.Count != inventory.slots.Count)
        {
            Debug.Log("Inventory_UI - 인벤UI, 인벤 개수 다름");
            return;
        }

        for (int i = 0; i < slotsUIs.Count; i++)
        {
            if (inventory.slots[i].type != CollectableType.NONE)
            {
                slotsUIs[i].SetItem(inventory.slots[i]);
            }
            else
            {
                slotsUIs[i].SetEmtpy();
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
        bool hasSameItem = false;

        foreach (Slot slot in inventory.slots)
        {
            if (selectedItem.type == slot.type)
            {
                hasSameItem = true;
                slot.count += selectedItem.Count;
                selectedItem.SetEmpty();
            }
        }

        if (!hasSameItem)
        {
            foreach (Slot slot in inventory.slots)
            {
                if (slot.type == CollectableType.NONE)
                {
                    slot.Refresh(selectedItem.type, selectedItem.Icon, selectedItem.Count);
                    selectedItem.SetEmpty();
                }
            }
        }
    }

    public void CloseInventoryUI()
    {
        dragState.isDragging = false;
        selectedItem.gameObject.SetActive(false);

        DropItem();
    }

    void DropItem()
    {
        GameManager.Instance.player.CreateDropItem(selectedItem);
    }

    public void TrashBin()
    {
        selectedItem.SetEmpty();
        dragState.isDragging = false;
    }
}