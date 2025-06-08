using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public enum ItemType
{
    None, Consumable, Seed, Tool
}

[System.Serializable]
public class ItemData
{
    public string itemName = "";
    public Sprite icon = null;
    public ItemType itemType = ItemType.None;
    public int buyPrice = 0;
    public int sellPrice = 0;
    public GameObject itemPrefab;

    public EnemyDropItemData dropItemData = new EnemyDropItemData();
    public CropItemData cropItemData = new CropItemData();

    public void SetItemData(ItemData newItemData)
    {
        if (newItemData == null)
        {
            Debug.Log("itemData 없음");
            return;
        }
        itemName = newItemData.itemName;
        icon = newItemData.icon;
        itemType = newItemData.itemType;
        buyPrice = newItemData.buyPrice;
        sellPrice = newItemData.sellPrice;

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
            Debug.Log("ItemData - GetDropRate dropItemData 없음");
        return dropItemData.rate;
    }

    public void SetEmpty()
    {
        itemName = "";
        icon = null;
        itemType = ItemType.None;
        buyPrice = 0;
        sellPrice = 0;

        dropItemData.SetEmpty();
        cropItemData.SetEmpty();
    }

    public void SetItemData(ScriptableItemData scriptableItemData)
    {
        itemName = scriptableItemData.itemName;
        icon = scriptableItemData.icon;
        itemType = scriptableItemData.itemType;
        buyPrice = scriptableItemData.buyPrice;
        sellPrice = scriptableItemData.sellPrice;
        itemPrefab = scriptableItemData.itemPrefab;

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
public class EnemyDropItemData
{
    public float rate = 0f;

    public bool IsEmpty() => rate == 0f ? true : false;

    public void SetEmpty() => rate = 0f;

    public void SetDropData(EnemyDropItemData dropData) => rate = dropData.rate;
}

[System.Serializable]
public class CropItemData
{
    public string cropName;
    public Tile[] cropTiles;
    public Tile[] wetCropTiles;
    public int[] growthDurations;
    public bool isRegrowable;
    public Sprite harvestedImage;

    [HideInInspector] public int growthLevel;
    [HideInInspector] public int currentGrowthDuration = 0;
    [HideInInspector] public int currentGrowthLevel = 0;
    [HideInInspector] public bool isWatered = false;
    [HideInInspector] public bool canHarvest = false;

    public void SetCropItemData(CropItemData newCropItemData)
    {
        if (newCropItemData.wetCropTiles.Length != newCropItemData.cropTiles.Length)
        {
            Debug.Log("타일 제대로 안넣음");
        }

        cropName = newCropItemData.cropName;
        cropTiles = newCropItemData.cropTiles;
        wetCropTiles = newCropItemData.wetCropTiles;
        growthDurations = newCropItemData.growthDurations;
        isRegrowable = newCropItemData.isRegrowable;

        growthLevel = cropTiles.Length;
        currentGrowthLevel = 1;
    }

    public void SetEmpty()
    {
        cropName = "";
        cropTiles = null;
        wetCropTiles = null;
        growthDurations = null;
        isRegrowable = false;

        growthLevel = 0;
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
