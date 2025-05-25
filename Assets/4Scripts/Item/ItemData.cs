using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ItemType
{
    None, Consumable, Seed, Tool
}

public class ItemData
{
    public string itemName = "Item Name";
    public Sprite icon = null;
    public int count = 0;
    public ItemType itemType = ItemType.None;
    public CropData cropData = new CropData();

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

    public void SetItemData(ScriptableItemData scriptableItemData, ScriptableCropData scriptableCropData)
    {
        itemName = scriptableItemData.itemName;
        icon = scriptableItemData.icon;
        count = 1;
        itemType = scriptableItemData.itemType;

        if (scriptableCropData != null)
            cropData.SetCropData(scriptableCropData);
        else
            cropData.SetEmpty();
    }

    public void SetEmpty()
    {
        itemName = "";
        icon = null;
        count = 0;
        itemType = ItemType.None;

        cropData.SetEmpty();
    }

    public bool IsEmpty()
    {
        if (itemName == "" || itemType == ItemType.None || count == 0)
            return true;
        return false;
    }

    public bool IsCrop()
    {
        if (itemType == ItemType.Seed && !cropData.IsEmpty())
            return true;
        return false;
    }
}

public class CropData
{
    public string cropName = "";
    public int growthLevel;
    public Tile[] cropTiles;
    public Tile[] wetCropTiles;
    public int[] growthDurations;
    public bool isRegrowable;

    public int currentGrowthDuration = 0;
    public int currentGrowthLevel = 0;
    public bool isWatered = false;

    public void SetCropData(ScriptableCropData scriptableCropData)
    {
        cropName = scriptableCropData.cropName;
        cropTiles = scriptableCropData.cropTiles;
        wetCropTiles = scriptableCropData.wetCropTiles;
        growthLevel = scriptableCropData.growthLevel;
        growthDurations = scriptableCropData.growthDurations;
        isRegrowable = scriptableCropData.isRegrowable;
    }

    public void SetCropData(CropData newCropData)
    {
        cropName = newCropData.cropName;
        cropTiles = newCropData.cropTiles;
        wetCropTiles = newCropData.wetCropTiles;
        growthLevel = newCropData.growthLevel;
        growthDurations = newCropData.growthDurations;
        isRegrowable = newCropData.isRegrowable;
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
    }

    public bool IsEmpty()
    {
        if (cropName == "" || growthLevel == 0 || growthDurations == null)
            return true;
        return false;
    }
}