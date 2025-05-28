
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public Dictionary<Vector3Int, CropItemData> plantedCropsDict = new Dictionary<Vector3Int, CropItemData>();

    private TileManager tileManager;
    private void Start()
    {
        tileManager = GameManager.Instance.tileManager;
        GameManager.Instance.dayTimeManager.OnDayPassed += NewDayCrop;
    }

    private void OnDisable()
    {
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
        Tile wetCropTile = CropItemData.wetCropTiles[CropItemData.currentGrowthLevel];
        tileManager.cropTileMap.SetTile(tileManager.selectedTilePos, wetCropTile);
    }

    public void NewDayCrop()
    {
        foreach (KeyValuePair<Vector3Int, CropItemData> cropTile in plantedCropsDict)
        {
            Vector3Int cropPos = cropTile.Key;
            CropItemData CropItemData = cropTile.Value;

            if (CropItemData.isWatered && CropItemData.currentGrowthLevel < CropItemData.growthLevel - 1)
            {
                CropItemData.currentGrowthDuration++;
                if (CropItemData.currentGrowthLevel >= CropItemData.growthLevel - 1)
                    CropItemData.currentGrowthLevel = CropItemData.growthLevel - 1;

                if (CropItemData.currentGrowthDuration >= CropItemData.growthDurations[CropItemData.currentGrowthLevel])
                {
                    CropItemData.currentGrowthDuration = 0;
                    CropItemData.currentGrowthLevel++;
                    Tile nextLevelCropTile = CropItemData.cropTiles[CropItemData.currentGrowthLevel];
                    tileManager.cropTileMap.SetTile(cropPos, nextLevelCropTile);
                }
            }
            if (CropItemData.currentGrowthLevel >= CropItemData.growthLevel - 1)
                CropItemData.canHarvest = true;

            CropItemData.isWatered = false;
            tileManager.wateringTileMap.SetTile(cropPos, null);
            tileManager.cropTileMap.SetTile(cropPos, CropItemData.cropTiles[CropItemData.currentGrowthLevel]);
        }
    }
}