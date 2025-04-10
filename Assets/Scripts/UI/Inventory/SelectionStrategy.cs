using System.Collections.Generic;
using Unity.Loading;
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
    bool isInventoryAreaClicked = false;

    protected SelectedItem_UI selectedItem;
    protected Slot selectedSlot;
    protected DragState dragState;
    protected List<Slot> slots;
    protected Slot_UI slotUI;

    protected int slotQuantity = 0;
    protected int dropItemQuantity = 0;

    public BaseClickStrategy(SelectedItem_UI _selectedItem, DragState _dragState)
    {
        selectedItem = _selectedItem;
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
                    slots = GameManager.Instance.player.inventoryManager.GetInventoryByName("backpack").slots;
                selectedSlot = slots[slotUI.slotIdx];

                isInventoryAreaClicked = true;
                dragState.isClick = true;

                // 드래깅상태가 아니라면 
                if (!dragState.isDragging)
                {
                    // 빈칸이면 return
                    if (selectedSlot.type == CollectableType.NONE)
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
        if (GameManager.Instance.uiManager.inventoryPanel.activeSelf && !isInventoryAreaClicked)
        {
            if (selectedItem.type == CollectableType.NONE)
                return;

            dropItemQuantity = 0;

            DropItem();
            GameManager.Instance.player.CreateDropItem(selectedItem);

            if (selectedItem.Count == 0)
            {
                dragState.isDragging = false;
                selectedItem.SetEmpty();
            }
        }

        else
            isInventoryAreaClicked = false;
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
        selectedItem.gameObject.SetActive(true);

        // SelectedItem 설정 -> 개수는 선택모드에 따라 나중에 지정
        selectedItem.type = selectedSlot.type;
        selectedItem.Icon = selectedSlot.icon;
    }

    protected void CompleteStartDrag()
    {
        if (selectedSlot == null)
            return;

        if (selectedSlot.count == 0)
            selectedSlot.SetEmpty();

        GameManager.Instance.uiManager.GetInventoryUIByName("backpack").Refresh();

        selectedItem.transform.SetParent(GameManager.Instance.uiManager.GetInventoryUIByName("backpack").transform.root); // UI 최상위로 이동
        selectedItem.transform.position = pointerData.position;
    }

    protected void CompleteEndDrag()
    {
        if (selectedItem.Count == 0)
        {
            dragState.isDragging = false;
            selectedItem.SetEmpty();
        }

        GameManager.Instance.uiManager.GetInventoryUIByName("backpack").Refresh();
    }
}

public class LeftClickStrategy : BaseClickStrategy
{
    public LeftClickStrategy(SelectedItem_UI selectedItem, DragState dragState)
        : base(selectedItem, dragState)
    { }

    protected override void DragStart_SetItemQuantity()
    {
        selectedItem.Count = selectedSlot.count;
        selectedSlot.count = 0;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        // 슬롯 비어있을 경우
        if (selectedSlot.type == CollectableType.NONE)
        {
            slotQuantity = selectedItem.Count;
            selectedItem.Count = 0;
            selectedSlot.Refresh(selectedItem.type, selectedItem.Icon, slotQuantity);
            selectedItem.SetEmpty();
        }

        // 다른 아이템 있을 경우
        else
        {
            SwapItemWithSlot();
        }
    }

    protected override void DropItem()
    {
        dropItemQuantity = selectedItem.Count;
        selectedItem.Count = 0;
    }

    void SwapItemWithSlot()
    {
        // 같은 아이템
        if (selectedSlot.type == selectedItem.type)
        {
            slotQuantity = selectedSlot.count + selectedItem.Count;
            selectedItem.Count = 0;
            slots[slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, slotQuantity);
            selectedItem.SetEmpty();
        }
        // 다른 아이템
        else
        {
            Slot tempslot = new Slot();
            tempslot.type = selectedSlot.type;
            tempslot.icon = selectedSlot.icon;
            tempslot.count = selectedSlot.count;

            slotQuantity = selectedItem.Count;
            slots[slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, selectedItem.Count);

            selectedItem.type = tempslot.type;
            selectedItem.Icon = tempslot.icon;
            selectedItem.Count = tempslot.count;
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
        selectedItem.Count = 1;
        selectedSlot.count -= 1;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        if (selectedSlot.type != CollectableType.NONE && selectedSlot.type != selectedItem.type)
            return;

        selectedItem.Count -= 1;
        slotQuantity = selectedSlot.count + 1;
        selectedSlot.Refresh(selectedItem.type, selectedItem.Icon, slotQuantity);
    }

    protected override void DropItem()
    {
        dropItemQuantity = 1;
        selectedItem.Count--;
    }
}

public class ShiftRightClickStrategy : BaseClickStrategy
{
    public ShiftRightClickStrategy(SelectedItem_UI selectedItem, DragState dragState)
        : base(selectedItem, dragState)
    { }

    protected override void DragStart_SetItemQuantity()
    {
        selectedItem.Count = (int)Mathf.Ceil(selectedSlot.count / 2f);
        selectedSlot.count -= selectedItem.Count;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        if (selectedSlot.type != CollectableType.NONE && selectedSlot.type != selectedItem.type)
            return;

        int tempquantity = selectedItem.Count;
        selectedItem.Count = (int)(selectedItem.Count / 2f);
        slotQuantity = selectedSlot.count + (tempquantity - selectedItem.Count);
        selectedSlot.Refresh(selectedItem.type, selectedItem.Icon, slotQuantity);
    }

    protected override void DropItem()
    {
        dropItemQuantity = (int)Mathf.Ceil(selectedItem.Count / 2f);
        selectedItem.Count -= dropItemQuantity;
    }
}