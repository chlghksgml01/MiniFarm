using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType
{
    None, Consumable, Seed, Tool
}

public class ItemData
{
    public string itemName = "";
    public Sprite icon = null;
    public ItemType itemType = ItemType.None;

    public DropItemData dropItemData = new DropItemData();
    public CropItemData cropItemData = new CropItemData();

    public void SetItemData(ItemData newItemData)
    {
        itemName = newItemData.itemName;
        icon = newItemData.icon;
        itemType = newItemData.itemType;

        if (!newItemData.IsDropEmpty())
            dropItemData.SetDropData(newItemData.dropItemData);

        if (!newItemData.IsCropEmpty())
            cropItemData.SetCropItemData(newItemData.cropItemData);
    }

    public bool IsCropEmpty()
    {
        if (itemType != ItemType.Seed || cropItemData.IsEmpty())
            return true;
        return false;
    }

    public bool IsDropEmpty() => dropItemData.IsEmpty();

    public float GetDropRate()
    {
        if (dropItemData.IsEmpty())
            Debug.Log("ItemData - GetDropRate dropItemData ¾øÀ½");
        return dropItemData.rate;
    }

    public void SetEmpty()
    {
        itemName = "";
        icon = null;
        itemType = ItemType.None;

        dropItemData.SetEmpty();
        cropItemData.SetEmpty();
    }

    public void SetItemData(ScriptableItemData scriptableItemData)
    {
        itemName = scriptableItemData.itemName;
        icon = scriptableItemData.icon;
        itemType = scriptableItemData.itemType;

        if (!scriptableItemData.dropItemData.IsEmpty())
            dropItemData.SetDropData(scriptableItemData.dropItemData);
        else
            dropItemData.SetEmpty();

        if (!scriptableItemData.cropItemData.IsEmpty())
            cropItemData.SetCropItemData(scriptableItemData.cropItemData);
        else
            cropItemData.SetEmpty();
    }

    public bool IsEmpty()
    {
        if (itemName == "" || itemType == ItemType.None)
            return true;
        return false;
    }

    public bool IsSameItem(ItemData otherData) => itemName == otherData.itemName ? true : false;
}


[System.Serializable]
public class DropItemData
{
    public float rate = 0f;

    public bool IsEmpty() => rate == 0f ? true : false;

    public void SetEmpty() => rate = 0f;

    public void SetDropData(DropItemData dropData) => rate = dropData.rate;
}

[System.Serializable]
public class CropItemData
{
    public string cropName;
    public Tile[] cropTiles;
    public Tile[] wetCropTiles;
    public int growthLevel;
    public int[] growthDurations;
    public bool isRegrowable;
    public Sprite harvestedImage;

    [HideInInspector] public int currentGrowthDuration = 0;
    [HideInInspector] public int currentGrowthLevel = 0;
    [HideInInspector] public bool isWatered = false;
    [HideInInspector] public bool canHarvest = false;

    public void SetCropItemData(CropItemData newCropItemData)
    {
        cropName = newCropItemData.cropName;
        cropTiles = newCropItemData.cropTiles;
        wetCropTiles = newCropItemData.wetCropTiles;
        growthLevel = newCropItemData.growthLevel;
        growthDurations = newCropItemData.growthDurations;
        isRegrowable = newCropItemData.isRegrowable;
    }

    public void SetEmpty()
    {
        cropName = "";
        cropTiles = null;
        wetCropTiles = null;
        growthLevel = 0;
        growthDurations = null;
        isRegrowable = false;

        currentGrowthDuration = 0;
        currentGrowthLevel = 0;
        isWatered = false;
        canHarvest = false;
    }

    public bool IsEmpty()
    {
        if (cropName == "" || growthLevel == 0 || growthDurations == null)
            return true;
        return false;
    }
}
