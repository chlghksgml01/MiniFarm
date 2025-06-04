
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