using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public CollectableType type;
        public int quantity;
        public int maxAllowed;
        public Sprite icon;

        public Slot()
        {
            type = CollectableType.NONE;
            quantity = 0;
            maxAllowed = 999;
        }

        public bool CanAddItem()
        {
            if (quantity < maxAllowed)
                return true;
            return false;
        }

        public void AddItem(Collectable item)
        {
            type = item.type;
            icon = item.icon;
            quantity++;
        }

        public void SetEmpty()
        {
            quantity = 0;
            icon = null;
            type = CollectableType.NONE;
        }

        public void Refresh(CollectableType _type, int _count, Sprite _icon)
        {
            type = _type;
            quantity = _count;
            icon = _icon;
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

    public void AddItem(Collectable item)
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
}
