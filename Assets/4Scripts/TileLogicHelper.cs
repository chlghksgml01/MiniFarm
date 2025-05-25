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
        // �÷��̾ ����ִ� ���� �̸����� Ÿ�� ��������
        if (GameManager.Instance.player.holdItem.itemData.IsCrop())
            cropName = GameManager.Instance.player.holdItem.itemData.itemName;

        // ���� �ɱ�
        if (cropName != "" && tileState == TileState.Tilled || tileState == TileState.Watered)
        {
            Tilemap farmFieldMap = tileManager.farmFieldMap;
            if (farmFieldMap.GetTile(cellPosition) == null)
            {
                Tile cropSeedTile = ScriptableObject.CreateInstance<Tile>();

                // ���� Ÿ�� ��������
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

        // ������
        if (tileDict.TryGetValue(rightCellPos, out var rightTileData))
        {
            rightTileData.tileConnectedDir |= TileConnectedDir.Left;
            if (rightTileData.tileState != TileState.Empty)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Right;
                UpdateConnectedTilledTile(rightCellPos, tileDict);
            }
        }

        // ����
        if (tileDict.TryGetValue(leftCellPos, out var leftTileData))
        {
            leftTileData.tileConnectedDir |= TileConnectedDir.Right;
            if (leftTileData.tileState != TileState.Empty)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Left;
                UpdateConnectedTilledTile(leftCellPos, tileDict);
            }
        }

        // ����
        if (tileDict.TryGetValue(upCellPos, out var upTileData))
        {
            upTileData.tileConnectedDir |= TileConnectedDir.Down;
            if (upTileData.tileState != TileState.Empty)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Up;
                UpdateConnectedTilledTile(upCellPos, tileDict);
            }
        }

        // �Ʒ���
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

        // TileConnectedState ����
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
            // �����¿� ����
            case TileConnectedDir.Up | TileConnectedDir.Down | TileConnectedDir.Left | TileConnectedDir.Right:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.Center;
                break;

            // ������ ����
            case TileConnectedDir.Up | TileConnectedDir.Down | TileConnectedDir.Left:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.RightCenter;
                break;

            // ���Ͽ� ����
            case TileConnectedDir.Up | TileConnectedDir.Down | TileConnectedDir.Right:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.LeftCenter;
                break;

            // ���¿� ����
            case TileConnectedDir.Up | TileConnectedDir.Left | TileConnectedDir.Right:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.DownCenter;
                break;

            // ���¿� ����
            case TileConnectedDir.Down | TileConnectedDir.Left | TileConnectedDir.Right:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.UpCenter;
                break;

            // ���� ����
            case TileConnectedDir.Up | TileConnectedDir.Down:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.VerticalCenter;
                break;

            // ���� ����
            case TileConnectedDir.Up | TileConnectedDir.Left:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.RightDown;
                break;

            // ��� ����
            case TileConnectedDir.Up | TileConnectedDir.Right:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.LeftDown;
                break;

            // ���� ����
            case TileConnectedDir.Down | TileConnectedDir.Left:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.RightUp;
                break;

            // �Ͽ� ����
            case TileConnectedDir.Down | TileConnectedDir.Right:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.LeftUp;
                break;

            // �¿� ����
            case TileConnectedDir.Left | TileConnectedDir.Right:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.HorizontalCenter;
                break;

            // ���� ����
            case TileConnectedDir.Up:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.Down;
                break;

            // �Ʒ��� ����
            case TileConnectedDir.Down:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.Up;
                break;

            // ������ ����
            case TileConnectedDir.Right:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.Left;
                break;

            // ���� ����
            case TileConnectedDir.Left:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.Right;
                break;

            // ����X
            default:
                tileDict[cellPosition].tileConnectedState = TileConnectedState.One;
                break;
        }
    }
}
