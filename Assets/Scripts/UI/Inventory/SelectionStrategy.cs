using System.Collections.Generic;
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
    protected SelectedItem_UI selectedItem;
    protected Player player;
    protected Slot sourceSlot;
    protected DragState dragState;

    public BaseClickStrategy(SelectedItem_UI _selectedItem, Player _player, DragState _dragState)
    {
        selectedItem = _selectedItem;
        player = _player;
        dragState = _dragState;
    }

    public virtual void ClickHandle()
    {
        List<RaycastResult> results = DetectUIUnderCursor();
        foreach (var result in results)
        {
            Slot_UI _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                if (!dragState.isDragging)
                    StartItemDrag(_slotUI);
            }
        }

        SetQuantity();
        OnItemDragEnd();
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

    protected abstract void SetQuantity();

    void StartItemDrag(Slot_UI slotUI)
    {
        selectedItem.gameObject.SetActive(true);
        dragState.isClick = true;
        dragState.isDragging = true;

        // 클릭한 슬롯 저장
        List<Slot> _slots = player.PlayerInventory.slots;
        sourceSlot = _slots[slotUI.slotIdx];

        // SelectedItem 설정
        selectedItem.type = sourceSlot.type;
        selectedItem.Icon = sourceSlot.icon;
    }

    protected void OnItemDragEnd()
    {
        if (sourceSlot == null)
            return;

        if (sourceSlot.quantity == 0)
        {
            sourceSlot.SetEmpty();
        }

        Instance.Refresh();

        selectedItem.transform.SetParent(Instance.transform.root); // UI 최상위로 이동
        selectedItem.transform.position = pointerData.position;
    }
}

public class LeftClickStrategy : BaseClickStrategy
{
    public LeftClickStrategy(SelectedItem_UI selectedItem, Player player, DragState dragState)
        : base(selectedItem, player, dragState)
    { }

    protected override void SetQuantity()
    {
        if (dragState.isDragging)
        {
            selectedItem.Quantity = sourceSlot.quantity;
            sourceSlot.quantity = 0;
        }
    }
}

public class RightClickStrategy : BaseClickStrategy
{
    public RightClickStrategy(SelectedItem_UI selectedItem, Player player, DragState dragState)
        : base(selectedItem, player, dragState)
    { }

    protected override void SetQuantity()
    {
        if (dragState.isDragging)
        {
            selectedItem.Quantity = 1;
            sourceSlot.quantity -= 1;
        }
    }
}

public class ShiftRightClickStrategy : BaseClickStrategy
{
    public ShiftRightClickStrategy(SelectedItem_UI selectedItem, Player player, DragState dragState)
        : base(selectedItem, player, dragState)
    { }

    protected override void SetQuantity()
    {
        if (dragState.isDragging)
        {
            selectedItem.Quantity = (int)Mathf.Ceil(sourceSlot.quantity / 2f);
            sourceSlot.quantity -= selectedItem.Quantity;
        }
    }
}