
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    public int gold = 500;
    public Inventory inventory;
}

[System.Serializable]
public class CropSaveData
{
    public string cropName;
    public int currentGrowthDuration = 0;
    public int currentGrowthLevel = 0;
    public bool canHarvest = false;
}

[System.Serializable]
public class TileSaveData
{
    public Vector3Int tilePos = new Vector3Int();
    public TileData tileData = new TileData();
    public CropSaveData cropSaveData = new CropSaveData();
}

[System.Serializable]
public class DropItemData
{
    public ItemData itemData = new ItemData();
    public Vector3 pos = Vector3.zero;
    public int count = 0;
}

[System.Serializable]
public class DropItemDatas
{
    public List<DropItemData> houseDropItems = new List<DropItemData>();
    public List<DropItemData> farmDropItems = new List<DropItemData>();
}