using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public CollectableType type;
        public Sprite icon;
        public int count;
        public int maxAllowed;

        public Slot()
        {
            type = CollectableType.NONE;
            count = 0;
            maxAllowed = 999;
        }

        public bool CanAddItem()
        {
            if (count < maxAllowed)
                return true;
            return false;
        }

        public void AddItem(Item item)
        {
            type = item.type;
            icon = item.icon;
            count += item.count;
        }

        public void SetEmpty()
        {
            count = 0;
            icon = null;
            type = CollectableType.NONE;
        }

        public void Refresh(CollectableType _type, Sprite _icon, int _quantity)
        {
            type = _type;
            icon = _icon;
            count = _quantity;
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
            if (slot.type == item.type && slot.CanAddItem())
            {
                slot.AddItem(item);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.type == CollectableType.NONE)
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
                // none이 앞에 있으면 뒤로 보내기
                if (slots[i].type == CollectableType.NONE)
                {
                    var temp = slots[i];
                    slots[i] = slots[j];
                    slots[j] = temp;
                }

                else if (slots[i].type > slots[j].type && slots[j].type != CollectableType.NONE)
                {
                    var temp = slots[i];
                    slots[i] = slots[j];
                    slots[j] = temp;
                }

                // 같은거라면 묶어주기
                else if (slots[i].type.CompareTo(slots[j].type) == 0)
                {
                    slots[i].count += slots[j].count;
                    slots[j].SetEmpty();
                }
            }
        }
    }

    public void Remove(int index)
    {
        //if();
    }
}