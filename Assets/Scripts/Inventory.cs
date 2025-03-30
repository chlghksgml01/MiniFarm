using System;
using System.Collections.Generic;
using static UnityEditor.Timeline.Actions.MenuPriority;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public CollectableType type;
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

        public void AddItem(CollectableType _type)
        {
            type = _type;
            count++;
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

    public void AddItem(CollectableType _type)
    {
        foreach (Slot slot in slots)
        {
            if (slot.type == _type && slot.CanAddItem())
            {
                slot.AddItem(_type);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.type == CollectableType.NONE)
            {
                slot.AddItem(_type);
                return;
            }
        }
    }
}
