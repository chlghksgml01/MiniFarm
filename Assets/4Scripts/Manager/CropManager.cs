
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public Dictionary<Vector3Int, CropData> plantedCropsDict = new Dictionary<Vector3Int, CropData>();

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
        if (plantedCropsDict.TryGetValue(cellPosition, out CropData cropData))
        {
            if (isWet && cropData.cropTiles.Length > 0)
                return cropData.wetCropTiles[0];
            else if (!isWet && cropData.cropTiles.Length > 0)
                return cropData.cropTiles[0];
        }

        return null;
    }

    public void WaterCropTile(Vector3Int cellPosition)
    {
        plantedCropsDict.TryGetValue(cellPosition, out CropData cropData);
        if (cropData == null)
            return;

        cropData.isWatered = true;
        Tile wetCropTile = cropData.wetCropTiles[cropData.currentGrowthLevel];
        tileManager.cropTileMap.SetTile(tileManager.selectedTilePos, wetCropTile);
    }

    public void NewDayCrop()
    {
        foreach (KeyValuePair<Vector3Int, CropData> cropTile in plantedCropsDict)
        {
            Vector3Int cropPos = cropTile.Key;
            CropData cropData = cropTile.Value;

            if (cropData.isWatered && cropData.currentGrowthLevel < cropData.growthLevel - 1)
            {
                cropData.currentGrowthDuration++;
                if (cropData.currentGrowthLevel >= cropData.growthLevel - 1)
                    cropData.currentGrowthLevel = cropData.growthLevel - 1;

                if (cropData.currentGrowthDuration >= cropData.growthDurations[cropData.currentGrowthLevel])
                {
                    cropData.currentGrowthDuration = 0;
                    cropData.currentGrowthLevel++;
                    Tile nextLevelCropTile = cropData.cropTiles[cropData.currentGrowthLevel];
                    tileManager.cropTileMap.SetTile(cropPos, nextLevelCropTile);
                }
            }
            if (cropData.currentGrowthLevel >= cropData.growthLevel - 1)
                cropData.canHarvest = true;

            cropData.isWatered = false;
            tileManager.wateringTileMap.SetTile(cropPos, null);
            tileManager.cropTileMap.SetTile(cropPos, cropData.cropTiles[cropData.currentGrowthLevel]);
        }
    }
}