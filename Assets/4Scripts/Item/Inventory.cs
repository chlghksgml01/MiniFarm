using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Inventory;

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

            if (item.gameObject.GetComponentInChildren<TextMeshProUGUI>().text != "")
            {
                int count = int.Parse(item.GetComponentInChildren<TextMeshProUGUI>().text);
                itemCount += count;
            }
            else
                itemCount++;

            slotItemData.SetItemData(item.itemData);

            InGameManager.Instance.uiManager.inventory_UI.Refresh();
        }

        public void AddItem(ItemData itemData, int count)
        {
            if (itemData.IsEmpty())
                return;

            itemCount += count;

            slotItemData.SetItemData(itemData);

            InGameManager.Instance.uiManager.inventory_UI.Refresh();
        }

        public void UseItem(int useCount = -99)
        {
            if (itemCount > 0)
            {
                if (useCount == -99)
                    itemCount--;
                else
                    itemCount -= useCount;

                if (itemCount <= 0)
                    SetEmpty();

                InGameManager.Instance.uiManager.inventory_UI.Refresh();
            }
        }

        public void SetEmpty()
        {
            itemCount = 0;
            slotItemData.SetEmpty();

            InGameManager.Instance.uiManager.inventory_UI.Refresh();
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

            InGameManager.Instance.uiManager.inventory_UI.Refresh();
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
                InGameManager.Instance.uiManager.toolBar_UI.CheckSlot();
                return;
            }
        }
    }

    public void AddItem(ItemData itemData, int count)
    {
        foreach (Slot slot in slots)
        {
            if (slot.slotItemData.itemName == itemData.itemName && slot.CanAddItem())
            {
                slot.AddItem(itemData, count);
                return;
            }
        }

        foreach (Slot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.AddItem(itemData, count);
                InGameManager.Instance.uiManager.toolBar_UI.CheckSlot();
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

    public void RemoveItem(ItemData itemData, int count)
    {
        if (itemData.IsEmpty())
            return;

        List<Slot> removeSlots = slots.Where(slot => slot.slotItemData.itemName == itemData.itemName).ToList();
        if (removeSlots.Count == 0)
        {
            Debug.Log("Inventory - RemoveItem 해당 아이템 없음");
            return;
        }

        for (int i = removeSlots.Count - 1; i >= 0; i--)
        {
            Slot removeSlot = removeSlots[i];

            if (removeSlot.itemCount >= count)
            {
                removeSlot.UseItem(count);
                return;
            }
            else
            {
                count -= removeSlot.itemCount;
                removeSlot.SetEmpty();
            }
        }
    }
}
