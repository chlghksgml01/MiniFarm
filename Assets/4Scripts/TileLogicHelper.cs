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
        TileManager tileManager = GameManager.Instance.tileManager;

        string cropName = "";
        // 플레이어가 들고있는 씨앗 이름으로 타일 가져오기
        if (GameManager.Instance.player.holdItem.itemData.IsCrop())
            cropName = GameManager.Instance.player.holdItem.itemData.itemName;

        // 씨앗 심기
        if (cropName != "" && tileState == TileState.Tilled || tileState == TileState.Watered)
        {
            Tilemap farmFieldMap = tileManager.farmFieldMap;
            if (farmFieldMap.GetTile(cellPosition) == null)
            {
                Tile cropSeedTile = ScriptableObject.CreateInstance<Tile>();

                // 씨앗 타일 가져오기
                if (tileState == TileState.Tilled)
                    cropSeedTile = tileManager.GetCropSeedTile(cropName, false);
                else if (tileState == TileState.Watered)
                    cropSeedTile = tileManager.GetCropSeedTile(cropName, true);

                if (cropSeedTile != null)
                {
                    farmFieldMap.SetTile(cellPosition, cropSeedTile);

                    tileManager.cropTileDataDict.TryAdd(cellPosition, new CropData());
                    tileManager.cropTileDataDict[cellPosition].SetCropData(GameManager.Instance.player.holdItem.itemData.cropData);
                    tileDict[cellPosition].tileState = TileState.Planted;
                }
            }
        }
        else
        {
            Tilemap interactableMap = tileManager.interactableMap;

            ToolType playerToolType = GameManager.Instance.player.playerToolType;
            switch (playerToolType)
            {
                case ToolType.Hoe:
                    if (tileState == TileState.Empty)
                    {
                        tileDict[cellPosition].tileState = TileState.Tilled;
                        UpdateTiles(cellPosition, tileDict, centerTileData);
                    }
                    else if (tileState == TileState.Planted)
                    {
                        tileDict[cellPosition].tileState = TileState.Tilled;
                        tileManager.farmFieldMap.SetTile(cellPosition, null);
                    }
                    break;
                case ToolType.WateringCan:
                    if (tileState == TileState.Tilled || tileState == TileState.Planted)
                    {
                        Tilemap wateringMap = tileManager.wateringMap;
                        wateringMap.SetTile(cellPosition, tileManager.wateringTile);
                        if (tileState == TileState.Tilled)
                            tileDict[cellPosition].tileState = TileState.Watered;
                        if (tileState == TileState.Planted)
                        {
                            tileManager.WaterCropTile(cellPosition);
                        }
                    }
                    break;
                case ToolType.Pickaxe:
                    if (tileState != TileState.Empty)
                    {
                        tileManager.farmFieldMap.SetTile(cellPosition, null);
                        tileManager.wateringMap.SetTile(cellPosition, null);
                        interactableMap.SetTile(cellPosition, tileManager.emptyTile);

                        tileDict[cellPosition].tileConnectedDir = TileConnectedDir.None;
                        tileDict[cellPosition].tileState = TileState.Empty;

                        ResetConnectedTiles(cellPosition, tileDict);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private static void UpdateTiles(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict, TileData centerTileData)
    {
        Vector3Int rightCellPos = cellPosition + Vector3Int.right;
        Vector3Int leftCellPos = cellPosition + Vector3Int.left;
        Vector3Int upCellPos = cellPosition + Vector3Int.up;
        Vector3Int downCellPos = cellPosition + Vector3Int.down;

        // 오른쪽
        if (tileDict.TryGetValue(rightCellPos, out var rightTileData))
        {
            rightTileData.tileConnectedDir |= TileConnectedDir.Left;
            if (rightTileData.tileState != TileState.Empty)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Right;
                UpdateConnectedTilledTile(rightCellPos, tileDict);
            }
        }

        // 왼쪽
        if (tileDict.TryGetValue(leftCellPos, out var leftTileData))
        {
            leftTileData.tileConnectedDir |= TileConnectedDir.Right;
            if (leftTileData.tileState != TileState.Empty)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Left;
                UpdateConnectedTilledTile(leftCellPos, tileDict);
            }
        }

        // 위쪽
        if (tileDict.TryGetValue(upCellPos, out var upTileData))
        {
            upTileData.tileConnectedDir |= TileConnectedDir.Down;
            if (upTileData.tileState != TileState.Empty)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Up;
                UpdateConnectedTilledTile(upCellPos, tileDict);
            }
        }

        // 아래쪽
        if (tileDict.TryGetValue(downCellPos, out var downTileData))
        {
            downTileData.tileConnectedDir |= TileConnectedDir.Up;
            if (downTileData.tileState != TileState.Empty)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Down;
                UpdateConnectedTilledTile(downCellPos, tileDict);
            }
        }

        UpdateConnectedTilledTile(cellPosition, tileDict, true);
    }

    static void UpdateConnectedTilledTile(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict, bool isInteractedTile = false)
    {
        Tilemap interactableMap = GameManager.Instance.tileManager.interactableMap;
        List<Tile> tilledTileDict = GameManager.Instance.tileManager.tilledTileDict;

        // TileConnectedState 설정
        SetTileConnectedDirection(cellPosition, tileDict);
        int tileConnectedState = (int)tileDict[cellPosition].tileConnectedState;
        interactableMap.SetTile(cellPosition, tilledTileDict[tileConnectedState]);
    }

    static void ResetConnectedTiles(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict)
    {
        Tilemap interactableMap = GameManager.Instance.tileManager.interactableMap;
        List<Tile> tilledTileDict = GameManager.Instance.tileManager.tilledTileDict;

        Vector3Int rightCellPos = cellPosition + Vector3Int.right;
        Vector3Int leftCellPos = cellPosition + Vector3Int.left;
        Vector3Int upCellPos = cellPosition + Vector3Int.up;
        Vector3Int downCellPos = cellPosition + Vector3Int.down;

        if (tileDict.TryGetValue(rightCellPos, out var rightTileData) && rightTileData.tileState != TileState.Empty)
        {
            rightTileData.tileConnectedDir &= ~TileConnectedDir.Left;
            SetTileConnectedDirection(rightCellPos, tileDict);
            int tileConnectedState = (int)rightTileData.tileConnectedState;
            interactableMap.SetTile(rightCellPos, tilledTileDict[tileConnectedState]);
        }
        if (tileDict.TryGetValue(leftCellPos, out var leftTileData) && leftTileData.tileState != TileState.Empty)
        {
            leftTileData.tileConnectedDir &= ~TileConnectedDir.Right;
            SetTileConnectedDirection(leftCellPos, tileDict);
            int tileConnectedState = (int)leftTileData.tileConnectedState;
            interactableMap.SetTile(leftCellPos, tilledTileDict[tileConnectedState]);
        }
        if (tileDict.TryGetValue(upCellPos, out var upTileData) && upTileData.tileState != TileState.Empty)
        {
            upTileData.tileConnectedDir &= ~TileConnectedDir.Down;
            SetTileConnectedDirection(upCellPos, tileDict);
            int tileConnectedState = (int)upTileData.tileConnectedState;
            interactableMap.SetTile(upCellPos, tilledTileDict[tileConnectedState]);
        }
        if (tileDict.TryGetValue(downCellPos, out var downTileData) && downTileData.tileState != TileState.Empty)
        {
            downTileData.tileConnectedDir &= ~TileConnectedDir.Up;
            SetTileConnectedDirection(downCellPos, tileDict);
            int tileConnectedState = (int)downTileData.tileConnectedState;
            interactableMap.SetTile(downCellPos, tilledTileDict[tileConnectedState]);
        }
    }

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
