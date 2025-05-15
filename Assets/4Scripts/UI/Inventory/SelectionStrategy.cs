using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Inventory;
using static Inventory_UI;

public interface ISelectionStrategy
{
    public void ClickHandle();
}

public abstract class BaseClickStrategy : ISelectionStrategy
{
    PointerEventData pointerData;

    protected SelectedItem_UI selectedItemUI;
    protected Slot selectedSlot;
    protected DragState dragState;
    protected List<Slot> slots;
    protected Slot_UI slotUI;

    protected int slotQuantity = 0;
    protected int dropItemQuantity = 0;

    public BaseClickStrategy(SelectedItem_UI _selectedItem, DragState _dragState)
    {
        selectedItemUI = _selectedItem;
        dragState = _dragState;
    }

    public virtual void ClickHandle()
    {
        dragState.isClick = true;

        // Canvas에 있는 애들 가져오기
        List<RaycastResult> results = DetectUIUnderCursor();

        foreach (var result in results)
        {
            if (dragState.isDragging &&
                (result.gameObject.name == "Sort_Button" || result.gameObject.name == "TrashBin"))
                return;

            slotUI = result.gameObject.GetComponent<Slot_UI>();

            if (slotUI != null)
            {
                // 클릭한 슬롯UI의 슬롯 가져오기
                if (slots == null)
                    slots = GameManager.Instance.player.inventory.slots;
                selectedSlot = slots[slotUI.slotIdx];

                dragState.isClick = true;

                // 드래깅상태가 아니라면 
                if (!dragState.isDragging)
                {
                    // 빈칸이면 return
                    if (selectedSlot.IsEmpty())
                        return;

                    // 아이템 드래그 시작
                    StartItemDrag();

                    // 선택모드에 따라 개수 설정
                    DragStart_SetItemQuantity();
                    selectedItemUI.SetSelectedUIItemData(selectedSlot);
                }
                // 드래깅상태라면 : 내려놓기
                else
                {
                    DragEnd_SetItemQuantity();
                    CompleteEndDrag();
                }
            }
        }

        if (dragState.isDragging)
        {
            CompleteStartDrag();
        }

        // 인벤창 켜있는데 인벤말고 다른거 클릭 -> 아이템 드랍
        if (GameManager.Instance.uiManager.inventoryPanel.activeSelf && !EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedItemUI.IsEmpty())
                return;

            DropItem();
            GameManager.Instance.player.CreateDropItem(selectedItemUI, dropItemQuantity);

            if (selectedItemUI.selectedItemData.count == 0)
            {
                dragState.isDragging = false;
                selectedItemUI.SetEmpty();
            }
        }
    }

    protected abstract void DragStart_SetItemQuantity();
    protected abstract void DragEnd_SetItemQuantity();
    protected abstract void DropItem();

    List<RaycastResult> DetectUIUnderCursor()
    {
        List<RaycastResult> results;

        pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    void StartItemDrag()
    {
        dragState.isDragging = true;
        selectedItemUI.gameObject.SetActive(true);
    }

    protected void CompleteStartDrag()
    {
        if (selectedSlot == null)
            return;

        if (selectedSlot.slotItemData.count == 0)
            selectedSlot.SetEmpty();

        GameManager.Instance.uiManager.inventory_UI.Refresh();

        selectedItemUI.transform.SetParent(GameManager.Instance.uiManager.inventory_UI.transform.root); // UI 최상위로 이동
        selectedItemUI.transform.position = pointerData.position;
    }

    protected void CompleteEndDrag()
    {
        if (selectedItemUI.selectedItemData.count == 0)
        {
            dragState.isDragging = false;
            selectedItemUI.SetEmpty();
        }

        GameManager.Instance.uiManager.inventory_UI.Refresh();
    }
}

public class LeftClickStrategy : BaseClickStrategy
{
    public LeftClickStrategy(SelectedItem_UI selectedItem, DragState dragState)
        : base(selectedItem, dragState)
    { }

    protected override void DragStart_SetItemQuantity()
    {
        selectedItemUI.SetCount(selectedSlot.slotItemData.count);
        selectedSlot.slotItemData.count = 0;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        // 슬롯 비어있을 경우
        if (selectedSlot.IsEmpty())
        {
            selectedSlot.SetSlotItemData(selectedItemUI.selectedItemData);
            selectedItemUI.SetEmpty();
        }

        // 다른 아이템 있을 경우
        else
        {
            SwapItemWithSlot();
        }
    }

    protected override void DropItem()
    {
        dropItemQuantity = selectedItemUI.selectedItemData.count;
        selectedItemUI.SetCount(0);
    }

    void SwapItemWithSlot()
    {
        // 같은 아이템
        if (selectedSlot.slotItemData.itemName == selectedItemUI.selectedItemData.itemName)
        {
            slotQuantity = selectedSlot.slotItemData.count + selectedItemUI.selectedItemData.count;
            selectedItemUI.SetCount(0);
            selectedSlot.SetSlotItemData(selectedItemUI.selectedItemData, slotQuantity);
            selectedItemUI.SetEmpty();
        }
        // 다른 아이템
        else
        {
            Slot tempslot = new Slot();
            tempslot.slotItemData.SetItemData(selectedItemUI.selectedItemData);

            selectedItemUI.SetSelectedUIItemData(selectedSlot, true);

            selectedSlot.SetSlotItemData(tempslot.slotItemData);
        }
    }
}

public class RightClickStrategy : BaseClickStrategy
{
    public RightClickStrategy(SelectedItem_UI selectedItem, DragState dragState)
        : base(selectedItem, dragState)
    { }

    protected override void DragStart_SetItemQuantity()
    {
        selectedItemUI.SetCount(1);
        selectedSlot.slotItemData.count -= 1;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        if (!selectedSlot.IsEmpty() && selectedSlot.slotItemData.itemName != selectedItemUI.selectedItemData.itemName)
            return;

        selectedItemUI.SetCount(selectedItemUI.selectedItemData.count - 1);
        slotQuantity = selectedSlot.slotItemData.count + 1;
        selectedSlot.SetSlotItemData(selectedItemUI.selectedItemData, slotQuantity);
    }

    protected override void DropItem()
    {
        dropItemQuantity = 1;
        selectedItemUI.SetCount(selectedItemUI.selectedItemData.count - 1);
    }
}

public class ShiftRightClickStrategy : BaseClickStrategy
{
    public ShiftRightClickStrategy(SelectedItem_UI selectedItem, DragState dragState)
        : base(selectedItem, dragState)
    { }

    protected override void DragStart_SetItemQuantity()
    {
        int count = (int)Mathf.Ceil(selectedSlot.slotItemData.count / 2f);
        selectedItemUI.SetCount(count);
        selectedSlot.slotItemData.count -= selectedItemUI.selectedItemData.count;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        if (!selectedSlot.IsEmpty() && selectedSlot.slotItemData.itemName != selectedItemUI.selectedItemData.itemName)
            return;

        int tempquantity = selectedItemUI.selectedItemData.count;
        int count = (int)(selectedItemUI.selectedItemData.count / 2f);
        selectedItemUI.SetCount(count);
        slotQuantity = selectedSlot.slotItemData.count + (tempquantity - selectedItemUI.selectedItemData.count);
        selectedSlot.SetSlotItemData(selectedItemUI.selectedItemData, slotQuantity);
    }

    protected override void DropItem()
    {
        dropItemQuantity = (int)Mathf.Ceil(selectedItemUI.selectedItemData.count / 2f);
        int count = selectedItemUI.selectedItemData.count - dropItemQuantity;
        selectedItemUI.SetCount(count);
    }
}