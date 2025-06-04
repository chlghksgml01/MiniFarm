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
    Inventory_UI inventoryUI = null;

    public BaseClickStrategy(SelectedItem_UI _selectedItem) => selectedItemUI = _selectedItem;

    public virtual void ClickHandle()
    {
        // Canvas�� �ִ� �ֵ� ��������
        List<RaycastResult> results = DetectUIUnderCursor();
        inventoryUI = GameManager.Instance.uiManager.inventory_UI;

        foreach (var result in results)
        {
            if (inventoryUI.isDragging &&
                (result.gameObject.name == "Sort_Button" || result.gameObject.name == "TrashBin"))
                return;

            slotUI = result.gameObject.GetComponent<Slot_UI>();

            if (slotUI != null)
            {
                // Ŭ���� ����UI�� ���� ��������
                if (slots == null)
                    slots = GameManager.Instance.player.playerSaveData.inventory.slots;
                selectedSlot = slots[slotUI.slotIdx];


                // �巡����°� �ƴ϶�� 
                if (!inventoryUI.isDragging)
                {
                    // ��ĭ�̸� return
                    if (selectedSlot.IsEmpty())
                        return;

                    // ������ �巡�� ����
                    StartItemDrag();

                    // ���ø�忡 ���� ���� ����
                    DragStart_SetItemQuantity();
                }
                // �巡����¶�� : ��������
                else
                {
                    DragEnd_SetItemQuantity();
                    CompleteEndDrag();
                }
            }
        }

        if (inventoryUI.isDragging)
        {
            CompleteStartDrag();
        }

        // �κ�â ���ִµ� �κ����� �ٸ��� Ŭ�� -> ������ ���
        if (GameManager.Instance.uiManager.inventoryPanel.activeSelf && !EventSystem.current.IsPointerOverGameObject())
        {
            if (selectedItemUI.IsEmpty())
                return;

            DropItem();

            if (selectedItemUI.IsEmpty())
            {
                inventoryUI.isDragging = false;
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
        inventoryUI.isDragging = true;
        selectedItemUI.gameObject.SetActive(true);
    }

    protected void CompleteStartDrag()
    {
        if (selectedSlot == null)
            return;

        if (selectedSlot.IsEmpty())
            selectedSlot.SetEmpty();

        GameManager.Instance.uiManager.inventory_UI.Refresh();

        selectedItemUI.transform.SetParent(GameManager.Instance.uiManager.inventory_UI.transform.root); // UI �ֻ����� �̵�
        selectedItemUI.transform.position = pointerData.position;
    }

    protected void CompleteEndDrag()
    {
        if (selectedItemUI.IsEmpty())
        {
            inventoryUI.isDragging = false;
        }

        GameManager.Instance.uiManager.inventory_UI.Refresh();
    }
}

public class LeftClickStrategy : BaseClickStrategy
{
    public LeftClickStrategy(SelectedItem_UI selectedItem) : base(selectedItem) { }

    protected override void DragStart_SetItemQuantity()
    {
        selectedItemUI.SetSelectedUIItemData(selectedSlot);
        selectedSlot.itemCount = 0;
    }

    protected override void DragEnd_SetItemQuantity()
    {
        // ���� ������� ���
        if (selectedSlot.IsEmpty())
        {
            selectedSlot.SetSlotItemData(selectedItemUI.selectedSlot);
            selectedItemUI.SetEmpty();
        }

        // �ٸ� ������ ���� ���
        else
        {
            SwapItemWithSlot();
        }
    }

    protected override void DropItem()
    {
        GameManager.Instance.player.CreateDropItem(selectedItemUI, selectedItemUI.selectedSlot.itemCount);
        selectedItemUI.SetEmpty();
    }

    void SwapItemWithSlot()
    {
        // ���� ������
        if (selectedSlot.slotItemData.IsSameItem(selectedItemUI.selectedSlot.slotItemData))
        {
            int itemCount = selectedSlot.itemCount + selectedItemUI.selectedSlot.itemCount;
            selectedSlot.SetSlotItemData(selectedItemUI.selectedSlot, itemCount);

            selectedItemUI.SetEmpty();
        }
        // �ٸ� ������
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
    public RightClickStrategy(SelectedItem_UI selectedItem) : base(selectedItem) { }

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
        GameManager.Instance.player.CreateDropItem(selectedItemUI, 1);
        selectedItemUI.SetCount(selectedItemUI.selectedSlot.itemCount - 1);
    }
}

public class ShiftRightClickStrategy : BaseClickStrategy
{
    public ShiftRightClickStrategy(SelectedItem_UI selectedItem) : base(selectedItem) { }

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

        GameManager.Instance.player.CreateDropItem(selectedItemUI, dropItemCount);

        selectedItemUI.SetCount(selectedUIcount);
    }
}