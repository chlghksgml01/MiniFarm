using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public Dictionary<Vector3Int, CropItemData> plantedCropsDict = new Dictionary<Vector3Int, CropItemData>();

    private void Start()
    {
        GameManager.Instance.dayTimeManager.OnDayPassed += NewDayCrop;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null)
            return;
        GameManager.Instance.dayTimeManager.OnDayPassed -= NewDayCrop;
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
        GameManager.Instance.tileManager.cropTileMap.SetTile(GameManager.Instance.tileManager.selectedTilePos, wetCropTile);
    }

    public void NewDayCrop()
    {
        if (GameManager.Instance.tileManager == null || GameManager.Instance.cropManager.plantedCropsDict.Count == 0)
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
                    GameManager.Instance.tileManager.cropTileMap.SetTile(cropPos, nextLevelCropTile);
                }
            }
            if (cropItemData.currentGrowthLevel >= cropItemData.growthLevel)
                cropItemData.canHarvest = true;

            cropItemData.isWatered = false;
            GameManager.Instance.tileManager.wateringTileMap.SetTile(cropPos, null);
            GameManager.Instance.tileManager.cropTileMap.SetTile(cropPos, cropItemData.cropTiles[cropItemData.currentGrowthLevel - 1]);
        }
    }

    public bool CanHarvest()
    {
        if (GameManager.Instance.tileManager == null)
            return false;
        TileManager tileManager = GameManager.Instance.tileManager;
        Vector3Int selectedTilePos = tileManager.selectedTilePos;

        var cropDict = GameManager.Instance.cropManager.plantedCropsDict;
        cropDict.TryGetValue(selectedTilePos, out CropItemData CropItemData);
        if (CropItemData == null)
            return false;

        if (CropItemData.canHarvest == true)
            return true;

        return false;
    }

    public void HarvestCrop()
    {
        TileManager tileManager = GameManager.Instance.tileManager;
        Vector3Int selectedTilePos = tileManager.selectedTilePos;

        if (!CanHarvest())
        {
            Debug.LogWarning("TileManager - 수확할 작물 없음");
            return;
        }

        GameManager.Instance.player.SetPlayerDirection(tileManager.mouseDirection);

        CropItemData cropItemData = GameManager.Instance.cropManager.plantedCropsDict[selectedTilePos];

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

        GameManager.Instance.cropManager.plantedCropsDict.Remove(selectedTilePos);
        tileManager.SetTile(tileManager.cropTileMap, selectedTilePos, null);
    }

    public CropSaveData GetCropSaveData()
    {
        CropSaveData cropSaveData = new CropSaveData();

        foreach (KeyValuePair<Vector3Int, CropItemData> cropTile in plantedCropsDict)
        {
            Vector3Int cropPos = cropTile.Key;
            CropItemData cropItemData = cropTile.Value;
            if (cropItemData == null || cropItemData.IsEmpty())
                continue;

            cropSaveData.cropPos.Add(cropPos);
            cropSaveData.cropItemData.Add(cropItemData);
        }

        return cropSaveData;
    }

    public void SetCropSaveData(CropSaveData cropSaveData)
    {
        if (GameManager.Instance.tileManager == null)
            return;

        if (cropSaveData == null || cropSaveData.cropPos.Count == 0)
            return;

        plantedCropsDict.Clear();

        for (int i = 0; i < cropSaveData.cropPos.Count; i++)
        {
            Vector3Int cropPos = cropSaveData.cropPos[i];
            CropItemData cropItemData = cropSaveData.cropItemData[i];
            if (cropItemData == null || cropItemData.IsEmpty())
                continue;

            plantedCropsDict.Add(cropPos, cropItemData);
            GameManager.Instance.tileManager.SetTile(GameManager.Instance.tileManager.cropTileMap, cropPos, cropItemData.cropTiles[cropItemData.currentGrowthLevel - 1]);
        }
    }
}