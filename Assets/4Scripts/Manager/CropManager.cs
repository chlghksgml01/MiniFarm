using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public Dictionary<Vector3Int, CropItemData> plantedCropsDict = new Dictionary<Vector3Int, CropItemData>();

    private void Start()
    {
        InGameManager.Instance.dayTimeManager.OnDayFinished += PrepareNewDayCrop;
    }

    private void OnDisable()
    {
        if (InGameManager.Instance == null)
            return;
        InGameManager.Instance.dayTimeManager.OnDayFinished -= PrepareNewDayCrop;
    }

    public Tile GetCropSeedTile(Vector3Int cellPosition, bool isWet)
    {
        if (plantedCropsDict.TryGetValue(cellPosition, out CropItemData CropItemData))
        {
            if (isWet && CropItemData.cropTiles.Length > 0)
                return CropItemData.wetCropTiles[0];
            else if (!isWet && CropItemData.cropTiles.Length > 0)
                return CropItemData.cropTiles[0];
        }

        return null;
    }

    public void WaterCropTile(Vector3Int cellPosition)
    {
        plantedCropsDict.TryGetValue(cellPosition, out CropItemData CropItemData);
        if (CropItemData == null)
            return;

        CropItemData.isWatered = true;
        Tile wetCropTile = CropItemData.wetCropTiles[CropItemData.currentGrowthLevel - 1];
        InGameManager.Instance.tileManager.cropTileMap.SetTile(InGameManager.Instance.tileManager.selectedTilePos, wetCropTile);
    }

    public void PrepareNewDayCrop()
    {
        TileManager tileManager = InGameManager.Instance.tileManager;
        if (tileManager == null || InGameManager.Instance.cropManager.plantedCropsDict.Count == 0)
            return;

        foreach (KeyValuePair<Vector3Int, CropItemData> cropTile in plantedCropsDict)
        {
            Vector3Int cropPos = cropTile.Key;
            CropItemData cropItemData = cropTile.Value;

            if (cropItemData.isWatered && cropItemData.currentGrowthLevel < cropItemData.growthLevel)
            {
                cropItemData.currentGrowthDuration++;
                if (cropItemData.currentGrowthLevel >= cropItemData.growthLevel)
                    cropItemData.currentGrowthLevel = cropItemData.growthLevel;

                if (cropItemData.currentGrowthDuration >= cropItemData.growthDurations[cropItemData.currentGrowthLevel])
                {
                    cropItemData.currentGrowthDuration = 0;
                    cropItemData.currentGrowthLevel++;
                    Tile nextLevelCropTile = cropItemData.cropTiles[cropItemData.currentGrowthLevel - 1];
                    tileManager.SetTile(tileManager.cropTileMap, cropPos, nextLevelCropTile);
                }
            }
            if (cropItemData.currentGrowthLevel >= cropItemData.growthLevel)
                cropItemData.canHarvest = true;

            cropItemData.isWatered = false;
            tileManager.SetTile(tileManager.wateringTileMap, cropPos, null);
            tileManager.SetTile(tileManager.cropTileMap, cropPos, cropItemData.cropTiles[cropItemData.currentGrowthLevel - 1]);
        }
    }

    public bool CanHarvest()
    {
        if (InGameManager.Instance.tileManager == null)
            return false;
        TileManager tileManager = InGameManager.Instance.tileManager;
        Vector3Int selectedTilePos = tileManager.selectedTilePos;

        var cropDict = InGameManager.Instance.cropManager.plantedCropsDict;
        cropDict.TryGetValue(selectedTilePos, out CropItemData CropItemData);
        if (CropItemData == null)
            return false;

        if (CropItemData.canHarvest == true)
            return true;

        return false;
    }

    public void HarvestCrop()
    {
        TileManager tileManager = InGameManager.Instance.tileManager;
        Vector3Int selectedTilePos = tileManager.selectedTilePos;

        if (!CanHarvest())
        {
            Debug.LogWarning("TileManager - 수확할 작물 없음");
            return;
        }

        InGameManager.Instance.player.SetPlayerDirection(tileManager.mouseDirection);

        CropItemData cropItemData = InGameManager.Instance.cropManager.plantedCropsDict[selectedTilePos];

        if (cropItemData.isRegrowable)
        {
            cropItemData.canHarvest = false;
            cropItemData.currentGrowthLevel = cropItemData.growthLevel - 1;
            cropItemData.currentGrowthDuration = 0;

            if (cropItemData.isWatered)
                tileManager.SetTile(tileManager.cropTileMap, selectedTilePos, cropItemData.wetCropTiles[cropItemData.wetCropTiles.Length - 2]);
            else
                tileManager.SetTile(tileManager.cropTileMap, selectedTilePos, cropItemData.cropTiles[cropItemData.cropTiles.Length - 2]);

            return;
        }

        else
        {
            if (cropItemData.isWatered)
                tileManager.tileDict[selectedTilePos].tileState = TileState.Watered;
            else
                tileManager.tileDict[selectedTilePos].tileState = TileState.Tilled;
        }

        InGameManager.Instance.cropManager.plantedCropsDict.Remove(selectedTilePos);
        tileManager.SetTile(tileManager.cropTileMap, selectedTilePos, null);
    }

    public void SaveCropData(CropSaveData cropSaveData, Vector3Int pos)
    {
        CropItemData cropItemData = GetCropData(pos);

        cropSaveData.cropName = cropItemData.cropName + "Seed";
        cropSaveData.currentGrowthDuration = cropItemData.currentGrowthDuration;
        cropSaveData.currentGrowthLevel = cropItemData.currentGrowthLevel;
        cropSaveData.canHarvest = cropItemData.canHarvest;
    }

    private CropItemData GetCropData(Vector3Int pos)
    {
        plantedCropsDict.TryGetValue(pos, out CropItemData cropItemData);
        return cropItemData;
    }

    public void LoadCropData(Vector3Int pos, CropSaveData cropSaveData)
    {
        ItemData itemData = new ItemData();
        itemData.SetItemData(InGameManager.Instance.itemManager.GetItemData(cropSaveData.cropName));
        itemData.cropItemData.SetCropItemData(InGameManager.Instance.itemManager.GetItemData(cropSaveData.cropName).cropItemData);

        itemData.cropItemData.currentGrowthDuration = cropSaveData.currentGrowthDuration;
        itemData.cropItemData.currentGrowthLevel = cropSaveData.currentGrowthLevel;
        itemData.cropItemData.canHarvest = cropSaveData.canHarvest;

        if (!plantedCropsDict.ContainsKey(pos))
            plantedCropsDict.Add(pos, itemData.cropItemData);
        else
            plantedCropsDict[pos] = itemData.cropItemData;
    }
}