using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Inventory;

public interface ISelectionStrategy
{
    public void ClickHandle();
}

public abstract class BaseClickStrategy : ISelectionStrategy
{
    PointerEventData pointerData;

    protected SelectedItem_UI selectedItemUI;
    protected Slot selectedSlot;
    protected List<Slot> slots;
    protected Slot_UI slotUI;
    public DragState dragState;

    public BaseClickStrategy(SelectedItem_UI _selectedItem, DragState _dragState)
    {
        selectedItemUI = _selectedItem;
        dragState = _dragState;
    }

    public virtual void ClickHandle()
    {
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
                    slots = InGameManager.Instance.player.playerSaveData.inventory.slots;
                selectedSlot = slots[slotUI.slotIdx];

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
        if (InGameManager.Instance.uiManager.inventoryPanel.activeSelf && !EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedItemUI.IsEmpty())
                return;

            DropItem();

            if (selectedItemUI.IsEmpty())
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

        if (selectedSlot.IsEmpty())
            selectedSlot.SetEmpty();

        InGameManager.Instance.uiManager.inventory_UI.Refresh();

        selectedItemUI.transform.SetParent(InGameManager.Instance.uiManager.inventory_UI.transform.root); // UI 최상위로 이동
        selectedItemUI.transform.position = pointerData.position;
    }

    protected void CompleteEndDrag()
    {
        if (selectedItemUI.IsEmpty())
        {
            dragState.isDragging = false;
        }

        InGameManager.Instance.uiManager.inventory_UI.Refresh();
    }
}

public class LeftClickStrategy : BaseClickStrategy
{
    public LeftClickStrategy(SelectedItem_UI selectedItem, DragState _dragState) : base(selectedItem, _dragState) { }

    protected override void DragStart_SetItemQuantity()
    {
        selectedItemUI.SetSelectedUIItemData(selectedSlot);
        selectedSlot.itemCount = 0;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        // 슬롯 비어있을 경우
        if (selectedSlot.IsEmpty())
        {
            selectedSlot.SetSlotItemData(selectedItemUI.selectedSlot);
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
        InGameManager.Instance.player.CreateDropItem(selectedItemUI, selectedItemUI.selectedSlot.itemCount);
        selectedItemUI.SetEmpty();
    }

    void SwapItemWithSlot()
    {
        // 같은 아이템
        if (selectedSlot.slotItemData.IsSameItem(selectedItemUI.selectedSlot.slotItemData))
        {
            int itemCount = selectedSlot.itemCount + selectedItemUI.selectedSlot.itemCount;
            selectedSlot.SetSlotItemData(selectedItemUI.selectedSlot, itemCount);

            selectedItemUI.SetEmpty();
        }
        // 다른 아이템
        else
        {
            Slot tempslot = new Slot();
            tempslot.itemCount = 1;
            tempslot.slotItemData.SetItemData(selectedItemUI.selectedSlot.slotItemData);

            selectedItemUI.SetSelectedUIItemData(selectedSlot);

            selectedSlot.SetSlotItemData(tempslot);
        }
    }
}

public class RightClickStrategy : BaseClickStrategy
{
    public RightClickStrategy(SelectedItem_UI selectedItem, DragState _dragState) : base(selectedItem, _dragState) { }

    protected override void DragStart_SetItemQuantity()
    {
        selectedItemUI.SetSelectedUIItemData(selectedSlot, 1);
        selectedSlot.itemCount -= 1;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        if (!selectedSlot.IsEmpty() &&
            !selectedSlot.slotItemData.IsSameItem(selectedItemUI.selectedSlot.slotItemData))
            return;

        selectedSlot.SetSlotItemData(selectedItemUI.selectedSlot, selectedSlot.itemCount + 1);
        selectedItemUI.SetCount(selectedItemUI.selectedSlot.itemCount - 1);
    }

    protected override void DropItem()
    {
        InGameManager.Instance.player.CreateDropItem(selectedItemUI, 1);
        selectedItemUI.SetCount(selectedItemUI.selectedSlot.itemCount - 1);
    }
}

public class ShiftRightClickStrategy : BaseClickStrategy
{
    public ShiftRightClickStrategy(SelectedItem_UI selectedItem, DragState _dragState) : base(selectedItem, _dragState) { }

    protected override void DragStart_SetItemQuantity()
    {
        int count = (int)Mathf.Ceil(selectedSlot.itemCount / 2f);
        selectedItemUI.SetSelectedUIItemData(selectedSlot, count);
        selectedSlot.itemCount -= selectedItemUI.selectedSlot.itemCount;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        if (!selectedSlot.IsEmpty() &&
            !selectedSlot.slotItemData.IsSameItem(selectedItemUI.selectedSlot.slotItemData))
            return;

        int selectedSlotCount = (int)Mathf.Ceil(selectedItemUI.selectedSlot.itemCount / 2f);
        int selectedUIcount = selectedItemUI.selectedSlot.itemCount - selectedSlotCount;

        selectedSlot.SetSlotItemData(selectedItemUI.selectedSlot, selectedSlotCount);
        selectedItemUI.SetCount(selectedUIcount);

    }

    protected override void DropItem()
    {
        int dropItemCount = (int)Mathf.Ceil(selectedItemUI.selectedSlot.itemCount / 2f);
        int selectedUIcount = selectedItemUI.selectedSlot.itemCount - dropItemCount;

        InGameManager.Instance.player.CreateDropItem(selectedItemUI, dropItemCount);

        selectedItemUI.SetCount(selectedUIcount);
    }
}