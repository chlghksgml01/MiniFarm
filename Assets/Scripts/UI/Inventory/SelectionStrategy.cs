using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
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
    CollectableItem dropItem;

    protected SelectedItem_UI selectedItem;
    protected Player player;
    protected Slot slot;
    protected DragState dragState;
    protected List<Slot> _slots;
    protected Slot_UI _slotUI;

    protected int _slotQuantity = 0;
    protected int dropItemQuantity = 0;

    public BaseClickStrategy(SelectedItem_UI _selectedItem, Player _player, DragState _dragState)
    {
        selectedItem = _selectedItem;
        player = _player;
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

            _slotUI = result.gameObject.GetComponent<Slot_UI>();
            if (_slotUI != null)
            {
                GetSlot(_slotUI);

                isInventoryAreaClicked = true;
                dragState.isClick = true;

                if (!dragState.isDragging)
                {
                    if (slot.type == CollectableType.NONE)
                        return;

                    StartItemDrag();

                    DragStart_SetItemQuantity();
                }
                else
                {
                    _slotQuantity = 0;

                    DragEnd_SetItemQuantity();
                    CompleteEndDrag();
                }
            }
        }

        if (dragState.isDragging)
        {
            CompleteStartDrag();
        }

        if (Instance.inventoryPanel.activeSelf && !isInventoryAreaClicked)
        {
            dropItem = Instance.dropItem.GetComponent<CollectableItem>();
            if (selectedItem.type == CollectableType.NONE)
                return;

            dropItemQuantity = 0;

            DropItem();
            CompleteDropItem();
        }

        else
            isInventoryAreaClicked = false;
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

    protected abstract void DragStart_SetItemQuantity();
    protected abstract void DragEnd_SetItemQuantity();
    protected abstract void DropItem();

    void GetSlot(Slot_UI slotUI)
    {
        // 클릭한 슬롯 저장
        _slots = player.PlayerInventory.slots;
        slot = _slots[slotUI.slotIdx];
    }

    void StartItemDrag()
    {
        selectedItem.gameObject.SetActive(true);
        dragState.isDragging = true;

        // SelectedItem 설정
        selectedItem.type = slot.type;
        selectedItem.Icon = slot.icon;
    }

    protected void CompleteStartDrag()
    {
        if (slot == null)
            return;

        if (slot.quantity == 0)
        {
            slot.SetEmpty();
        }

        Instance.Refresh();

        selectedItem.transform.SetParent(Instance.transform.root); // UI 최상위로 이동
        selectedItem.transform.position = pointerData.position;
    }

    protected void CompleteEndDrag()
    {
        if (selectedItem.Quantity == 0)
        {
            dragState.isDragging = false;
            selectedItem.SetEmpty();
        }

        Instance.Refresh();
    }

    void CompleteDropItem()
    {
        Vector3 bounceBasePos = new Vector3(player.transform.position.x + 0.5f, player.transform.position.y - 0.5f);

        ItemDropper.Instance.DropItem(dropItem, bounceBasePos, selectedItem.Icon, selectedItem.type, dropItemQuantity);

        dropItem.Quantity = 0;
        dropItem.Type = CollectableType.NONE;

        if (selectedItem.Quantity == 0)
        {
            dragState.isDragging = false;
            selectedItem.SetEmpty();
        }
    }
}

public class LeftClickStrategy : BaseClickStrategy
{
    public LeftClickStrategy(SelectedItem_UI selectedItem, Player player, DragState dragState)
        : base(selectedItem, player, dragState)
    { }

    protected override void DragStart_SetItemQuantity()
    {
        if (dragState.isDragging)
        {
            selectedItem.Quantity = slot.quantity;
            slot.quantity = 0;
        }
    }

    protected override void DragEnd_SetItemQuantity()
    {
        // 슬롯 비어있을 경우
        if (slot.type == CollectableType.NONE)
        {
            _slotQuantity = selectedItem.Quantity;
            selectedItem.Quantity = 0;
            _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, _slotQuantity);
            selectedItem.SetEmpty();
        }

        // 다른 아이템 있을 경우
        else
        {
            SwapItemWithSlot(_slots, slot, _slotUI, _slotQuantity);
        }
    }

    protected override void DropItem()
    {
        dropItemQuantity = selectedItem.Quantity;
        selectedItem.Quantity = 0;
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
}

public class RightClickStrategy : BaseClickStrategy
{
    public RightClickStrategy(SelectedItem_UI selectedItem, Player player, DragState dragState)
        : base(selectedItem, player, dragState)
    { }

    protected override void DragStart_SetItemQuantity()
    {
        if (dragState.isDragging)
        {
            selectedItem.Quantity = 1;
            slot.quantity -= 1;
        }
    }

    protected override void DragEnd_SetItemQuantity()
    {
        if (slot.type != CollectableType.NONE && slot.type != selectedItem.type)
            return;

        selectedItem.Quantity -= 1;
        _slotQuantity = slot.quantity + 1;
        _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, _slotQuantity);
    }

    protected override void DropItem()
    {
        dropItemQuantity = 1;
        selectedItem.Quantity--;
    }
}


public class ShiftRightClickStrategy : BaseClickStrategy
{
    public ShiftRightClickStrategy(SelectedItem_UI selectedItem, Player player, DragState dragState)
        : base(selectedItem, player, dragState)
    { }

    protected override void DragStart_SetItemQuantity()
    {
        if (dragState.isDragging)
        {
            selectedItem.Quantity = (int)Mathf.Ceil(slot.quantity / 2f);
            slot.quantity -= selectedItem.Quantity;
        }
    }

    protected override void DragEnd_SetItemQuantity()
    {
        if (slot.type != CollectableType.NONE && slot.type != selectedItem.type)
            return;

        int tempquantity = selectedItem.Quantity;
        selectedItem.Quantity = (int)(selectedItem.Quantity / 2f);
        _slotQuantity = slot.quantity + (tempquantity - selectedItem.Quantity);
        _slots[_slotUI.slotIdx].Refresh(selectedItem.type, selectedItem.Icon, _slotQuantity);
    }

    protected override void DropItem()
    {
        dropItemQuantity = (int)Mathf.Ceil(selectedItem.Quantity / 2f);
        selectedItem.Quantity -= dropItemQuantity;
    }
}