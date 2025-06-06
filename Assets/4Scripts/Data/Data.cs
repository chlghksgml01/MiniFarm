
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
    public List<Vector3Int> cropPos = new List<Vector3Int>();
    public List<CropItemData> cropItemData = new List<CropItemData>();
}

public class DropItemData
{
    public ItemData itemData = new ItemData();
    public Vector3 pos = Vector3.zero;
    public int count = 0;
}

[System.Serializable]
public class DropItem
{
    public List<DropItemData> houseDropItems = new List<DropItemData>();
    public List<DropItemData> farmDropItems = new List<DropItemData>();
}