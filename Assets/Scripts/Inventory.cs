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
        public string itemName;
        public Sprite icon;
        public int count;
        public int maxAllowed;

        public Slot()
        {
            itemName = "";
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
            itemName = item.itemData.itemName;
            icon = item.itemData.icon;
            count += item.count;
        }

        public void SetEmpty()
        {
            itemName = "";
            count = 0;
            icon = null;
        }

        public bool IsEmpty()
        {
            if (itemName == "")
                return true;
            return false;
        }

        public void Refresh(string _name, Sprite _icon, int _quantity)
        {
            itemName = _name;
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
            if (slot.itemName == item.itemData.itemName && slot.CanAddItem())
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
                // 아무것도 없으면 뒤로 보내기
                if (slots[i].IsEmpty())
                {
                    var temp = slots[i];
                    slots[i] = slots[j];
                    slots[j] = temp;
                }

                // j가 i보다 뒤에 있으면
                else if (slots[j].itemName.CompareTo(slots[i].itemName) > 0 && slots[j].IsEmpty())
                {
                    var temp = slots[i];
                    slots[i] = slots[j];
                    slots[j] = temp;
                }

                // 같은거라면 묶어주기
                else if (slots[i].itemName.CompareTo(slots[j].itemName) == 0)
                {
                    slots[i].count += slots[j].count;
                    slots[j].SetEmpty();
                }
            }
        }
    }
}