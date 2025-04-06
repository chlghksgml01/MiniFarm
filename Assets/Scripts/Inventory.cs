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
        public Sprite icon;
        public int quantity;
        public int maxAllowed;

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

        public void AddItem(CollectableItem item)
        {
            type = item.Type;
            icon = item.Icon;
            quantity++;
        }

        public void SetEmpty()
        {
            quantity = 0;
            icon = null;
            type = CollectableType.NONE;
        }

        public void Refresh(CollectableType _type, Sprite _icon, int _count)
        {
            type = _type;
            icon = _icon;
            quantity = _count;
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

    public void AddItem(CollectableItem item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.type == item.Type && slot.CanAddItem())
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
