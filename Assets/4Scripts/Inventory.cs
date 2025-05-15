using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public ItemData slotItemData = new ItemData();
        public int maxAllowed;

        public Slot()
        {
            maxAllowed = 999;
        }

        public bool CanAddItem()
        {
            if (slotItemData.count < maxAllowed)
                return true;
            return false;
        }

        public void AddItem(Item item)
        {
            slotItemData.itemName = item.itemData.itemName;
            slotItemData.icon = item.itemData.icon;
            slotItemData.itemType = item.itemData.itemType;
            slotItemData.count += item.count;
        }

        public void SetEmpty()
        {
            slotItemData.SetEmpty();
        }

        public bool IsEmpty()
        {
            if (slotItemData.IsEmpty())
                return true;
            return false;
        }

        public void SetSlotItemData(ItemData itemData, int _count = -99)
        {
            if(_count!= -99)
                itemData.count = _count;

            slotItemData.SetItemData(itemData);
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
                // �ƹ��͵� ������ �ڷ� ������
                if (slots[i].IsEmpty())
                {
                    var temp = slots[i];
                    slots[i] = slots[j];
                    slots[j] = temp;
                }

                // j�� i���� �ڿ� ������
                else if (slots[j].slotItemData.itemName.CompareTo(slots[i].slotItemData.itemName) > 0 && slots[j].IsEmpty())
                {
                    var temp = slots[i];
                    slots[i] = slots[j];
                    slots[j] = temp;
                }

                // �����Ŷ�� �����ֱ�
                else if (slots[i].slotItemData.itemName.CompareTo(slots[j].slotItemData.itemName) == 0)
                {
                    slots[i].slotItemData.count += slots[j].slotItemData.count;
                    slots[j].SetEmpty();
                }
            }
        }
    }
}