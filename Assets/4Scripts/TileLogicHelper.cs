using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileLogicHelper
{
    public static void SetTiles(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict)
    {
        if (!tileDict.TryGetValue(cellPosition, out var centerTileData) || centerTileData.tileState == TileState.None)
            return;

        TileState tileState = centerTileData.tileState;
        TileManager tileManager = InGameManager.Instance.tileManager;

        string cropName = "";
        // 플레이어가 들고있는 씨앗 이름으로 타일 가져오기
        if (!InGameManager.Instance.player.holdItem.itemData.IsCropEmpty())
            cropName = InGameManager.Instance.player.holdItem.itemData.cropItemData.cropName;

        // 씨앗 심기
        if (cropName != "" && (tileState == TileState.Tilled || tileState == TileState.Watered))
            PlantCrop(cellPosition, tileDict, tileState, tileManager);
        // 도구 사용
        else
            ApplyTool(cellPosition, tileDict, centerTileData, tileState, tileManager);
    }

    private static void PlantCrop(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict, TileState tileState, TileManager tileManager)
    {
        CropManager cropManager = InGameManager.Instance.cropManager;
        Tilemap farmFieldMap = tileManager.cropTileMap;

        if (farmFieldMap.GetTile(cellPosition) == null)
        {
            Tile cropSeedTile = ScriptableObject.CreateInstance<Tile>();

            // 씨앗 타일 가져오기
            if (tileState == TileState.Tilled)
                cropSeedTile = InGameManager.Instance.player.holdItem.itemData.cropItemData.cropTiles[0];
            else if (tileState == TileState.Watered)
                cropSeedTile = InGameManager.Instance.player.holdItem.itemData.cropItemData.wetCropTiles[0];

            if (cropSeedTile != null)
            {
                farmFieldMap.SetTile(cellPosition, cropSeedTile);

                cropManager.plantedCropsDict.TryAdd(cellPosition, new CropItemData());
                cropManager.plantedCropsDict[cellPosition].SetCropItemData(InGameManager.Instance.player.holdItem.itemData.cropItemData);

                if (tileState == TileState.Watered)
                    cropManager.plantedCropsDict[cellPosition].isWatered = true;
                tileDict[cellPosition].tileState = TileState.Planted;
                InGameManager.Instance.uiManager.toolBar_UI.UseItem();
            }
        }
    }

    private static void ApplyTool(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict, TileData centerTileData, TileState tileState, TileManager tileManager)
    {
        CropManager cropManager = InGameManager.Instance.cropManager;
        Tilemap interactableMap = tileManager.tilledTileMap;

        ToolType playerToolType = InGameManager.Instance.player.playerToolType;
        switch (playerToolType)
        {
            case ToolType.Hoe:
            if (tileState == TileState.Empty)
            {
                tileDict[cellPosition].tileState = TileState.Tilled;
                ConnectSurroundingTiles(cellPosition, tileDict, centerTileData);
            }
            else if (tileState == TileState.Planted)
            {
                if (cropManager.plantedCropsDict.ContainsKey(cellPosition))
                    cropManager.plantedCropsDict.Remove(cellPosition);

                tileManager.cropTileMap.SetTile(cellPosition, null);

                tileDict[cellPosition].tileState = TileState.Tilled;
            }
            break;

            case ToolType.WateringCan:
            if (tileState == TileState.Tilled || tileState == TileState.Planted)
            {
                Tilemap wateringMap = tileManager.wateringTileMap;
                wateringMap.SetTile(cellPosition, tileManager.wateringTile);
                if (tileState == TileState.Tilled)
                    tileDict[cellPosition].tileState = TileState.Watered;
                if (tileState == TileState.Planted)
                {
                    cropManager.WaterCropTile(cellPosition);
                }
            }
            break;

            case ToolType.Pickaxe:
            if (tileState != TileState.Empty)
            {
                if (cropManager.plantedCropsDict.ContainsKey(cellPosition))
                    cropManager.plantedCropsDict.Remove(cellPosition);

                tileManager.cropTileMap.SetTile(cellPosition, null);
                tileManager.wateringTileMap.SetTile(cellPosition, null);
                interactableMap.SetTile(cellPosition, tileManager.emptyTile);

                tileDict[cellPosition].tileConnectedDir = TileConnectedDir.None;
                tileDict[cellPosition].tileState = TileState.Empty;

                DisconnectSurroundingTiles(cellPosition, tileDict);
            }
            break;
            default:
            break;
        }
    }

    // 경작한 주변 타일 잇기
    public static void ConnectSurroundingTiles(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict, TileData centerTileData)
    {
        var dirs = new (Vector3Int delta, TileConnectedDir currentDir, TileConnectedDir neighborDir)[]
        {
            (Vector3Int.right, TileConnectedDir.Right, TileConnectedDir.Left),
            (Vector3Int.left,  TileConnectedDir.Left,  TileConnectedDir.Right),
            (Vector3Int.up,    TileConnectedDir.Up,    TileConnectedDir.Down),
            (Vector3Int.down,  TileConnectedDir.Down,  TileConnectedDir.Up),
        };

        foreach (var dir in dirs)
        {
            var neighborPos = cellPosition + dir.delta;
            if (!tileDict.TryGetValue(neighborPos, out var neighborData))
                continue;

            neighborData.tileConnectedDir |= dir.neighborDir;

            if (neighborData.tileState != TileState.Empty)
            {
                centerTileData.tileConnectedDir |= dir.currentDir;
                UpdateConnectedTilledTile(neighborPos, tileDict);
            }
            else
                centerTileData.tileConnectedDir &= ~dir.currentDir;
        }

        UpdateConnectedTilledTile(cellPosition, tileDict, true);
    }

    // 경작한 타일상태 업데이트
    static private void UpdateConnectedTilledTile(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict, bool isInteractedTile = false)
    {
        Tilemap interactableMap = InGameManager.Instance.tileManager.tilledTileMap;
        List<Tile> tilledTileList = InGameManager.Instance.tileManager.tilledTileList;

        // TileConnectedState 설정
        SetTileConnectedDirection(cellPosition, tileDict);
        int tileConnectedState = (int)tileDict[cellPosition].tileConnectedState;
        interactableMap.SetTile(cellPosition, tilledTileList[tileConnectedState]);
    }

    // 경작한 타일 끊기
    static public void DisconnectSurroundingTiles(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict)
    {
        Tilemap tilledTileMap = InGameManager.Instance.tileManager.tilledTileMap;
        List<Tile> tilledTileList = InGameManager.Instance.tileManager.tilledTileList;

        Vector3Int rightCellPos = cellPosition + Vector3Int.right;
        Vector3Int leftCellPos = cellPosition + Vector3Int.left;
        Vector3Int upCellPos = cellPosition + Vector3Int.up;
        Vector3Int downCellPos = cellPosition + Vector3Int.down;

        if (tileDict.TryGetValue(rightCellPos, out var rightTileData) && rightTileData.tileState != TileState.Empty)
        {
            rightTileData.tileConnectedDir &= ~TileConnectedDir.Left;
            SetTileConnectedDirection(rightCellPos, tileDict);
            int tileConnectedState = (int)rightTileData.tileConnectedState;
            tilledTileMap.SetTile(rightCellPos, tilledTileList[tileConnectedState]);
        }
        if (tileDict.TryGetValue(leftCellPos, out var leftTileData) && leftTileData.tileState != TileState.Empty)
        {
            leftTileData.tileConnectedDir &= ~TileConnectedDir.Right;
            SetTileConnectedDirection(leftCellPos, tileDict);
            int tileConnectedState = (int)leftTileData.tileConnectedState;
            tilledTileMap.SetTile(leftCellPos, tilledTileList[tileConnectedState]);
        }
        if (tileDict.TryGetValue(upCellPos, out var upTileData) && upTileData.tileState != TileState.Empty)
        {
            upTileData.tileConnectedDir &= ~TileConnectedDir.Down;
            SetTileConnectedDirection(upCellPos, tileDict);
            int tileConnectedState = (int)upTileData.tileConnectedState;
            tilledTileMap.SetTile(upCellPos, tilledTileList[tileConnectedState]);
        }
        if (tileDict.TryGetValue(downCellPos, out var downTileData) && downTileData.tileState != TileState.Empty)
        {
            downTileData.tileConnectedDir &= ~TileConnectedDir.Up;
            SetTileConnectedDirection(downCellPos, tileDict);
            int tileConnectedState = (int)downTileData.tileConnectedState;
            tilledTileMap.SetTile(downCellPos, tilledTileList[tileConnectedState]);
        }
    }

    // 타일 연결된 방향 설정
    private static void SetTileConnectedDirection(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict)
    {
        TileConnectedDir dir = tileDict[cellPosition].tileConnectedDir;

        switch (dir)
        {
            // 상하좌우 연결
            case TileConnectedDir.Up | TileConnectedDir.Down | TileConnectedDir.Left | TileConnectedDir.Right:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.Center;
            break;

            // 상하좌 연결
            case TileConnectedDir.Up | TileConnectedDir.Down | TileConnectedDir.Left:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.RightCenter;
            break;

            // 상하우 연결
            case TileConnectedDir.Up | TileConnectedDir.Down | TileConnectedDir.Right:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.LeftCenter;
            break;

            // 상좌우 연결
            case TileConnectedDir.Up | TileConnectedDir.Left | TileConnectedDir.Right:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.DownCenter;
            break;

            // 하좌우 연결
            case TileConnectedDir.Down | TileConnectedDir.Left | TileConnectedDir.Right:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.UpCenter;
            break;

            // 상하 연결
            case TileConnectedDir.Up | TileConnectedDir.Down:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.VerticalCenter;
            break;

            // 상좌 연결
            case TileConnectedDir.Up | TileConnectedDir.Left:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.RightDown;
            break;

            // 상우 연결
            case TileConnectedDir.Up | TileConnectedDir.Right:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.LeftDown;
            break;

            // 하좌 연결
            case TileConnectedDir.Down | TileConnectedDir.Left:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.RightUp;
            break;

            // 하우 연결
            case TileConnectedDir.Down | TileConnectedDir.Right:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.LeftUp;
            break;

            // 좌우 연결
            case TileConnectedDir.Left | TileConnectedDir.Right:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.HorizontalCenter;
            break;

            // 위쪽 연결
            case TileConnectedDir.Up:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.Down;
            break;

            // 아래쪽 연결
            case TileConnectedDir.Down:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.Up;
            break;

            // 오른쪽 연결
            case TileConnectedDir.Right:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.Left;
            break;

            // 왼쪽 연결
            case TileConnectedDir.Left:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.Right;
            break;

            // 연결X
            default:
            tileDict[cellPosition].tileConnectedState = TileConnectedState.One;
            break;
        }
    }
}
