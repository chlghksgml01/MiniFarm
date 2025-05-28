using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public ItemData slotItemData = new ItemData();
        public int itemCount = 0;
        public int maxAllowed;

        public Slot()
        {
            maxAllowed = 999;
        }

        public bool CanAddItem()
        {
            if (itemCount < maxAllowed)
                return true;
            return false;
        }

        public void AddItem(Item item)
        {
            if (item.IsEmpty())
                return;

            itemCount++;
            slotItemData.SetItemData(item.itemData);

            GameManager.Instance.uiManager.inventory_UI.Refresh();
        }

        public void UseItem()
        {
            if (itemCount > 0)
            {
                itemCount--;
                if (itemCount <= 0)
                    SetEmpty();
            }
        }

        public void SetEmpty()
        {
            itemCount = 0;
            slotItemData.SetEmpty();
        }

        public bool IsEmpty()
        {
            if (slotItemData.IsEmpty() || itemCount <= 0)
                return true;
            return false;
        }

        public void SetSlotItemData(Slot _slot, int _count = -99)
        {
            slotItemData.SetItemData(_slot.slotItemData);

            if (_count != -99)
                itemCount = _count;
            else
                itemCount = _slot.itemCount;

            GameManager.Instance.uiManager.inventory_UI.Refresh();
        }
    }

    public List<Slot> slots = new List<Slot>();

    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    public void AddItem(Item item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.slotItemData.itemName == item.itemData.itemName && slot.CanAddItem())
            {
                slot.AddItem(item);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.AddItem(item);
                GameManager.Instance.uiManager.toolBar_UI.CheckSlot();
                return;
            }
        }
    }

    public void SortInventory()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            for (int j = i + 1; j < slots.Count; j++)
            {
                // 아무것도 없으면 뒤로 보내기
                if (slots[i].IsEmpty())
                {
                    var temp = slots[i];
                    slots[i] = slots[j];
                    slots[j] = temp;
                }

                // j가 i보다 뒤에 있으면
                else if (slots[j].slotItemData.itemName.CompareTo(slots[i].slotItemData.itemName) > 0 && slots[j].IsEmpty())
                {
                    var temp = slots[i];
                    slots[i] = slots[j];
                    slots[j] = temp;
                }

                // 같은거라면 묶어주기
                else if (slots[i].slotItemData.itemName.CompareTo(slots[j].slotItemData.itemName) == 0)
                {
                    slots[i].itemCount += slots[j].itemCount;
                    slots[j].SetEmpty();
                }
            }
        }
    }
}
