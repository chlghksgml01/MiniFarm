using UnityEngine;

public enum ItemType
{
    None, Consumable, Tool
}

public class ItemData
{
    public string itemName = "Item Name";
    public Sprite icon = null;
    public int count = 0;
    public ItemType itemType = ItemType.None;

    public void SetItemData(ItemData newItemData, int _count = -99)
    {
        if (_count != -99)
            count = _count;
        else
            count = newItemData.count;

        itemName = newItemData.itemName;
        icon = newItemData.icon;
        itemType = newItemData.itemType;
    }

    public void SetEmpty()
    {
        itemName = "";
        icon = null;
        count = 0;
        itemType = ItemType.None;
    }

    public bool IsEmpty()
    {
        if (itemName == "" || itemType == ItemType.None || count == 0)
            return true;
        return false;
    }
}