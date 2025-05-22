using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileLogicHelper
{
    public static void SetTiles(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict)
    {
        Tilemap interactableMap = GameManager.Instance.tileManager.interactableMap;
        List<Tile> interactedTileDict = GameManager.Instance.tileManager.tilledTileDict;

        if (!tileDict.TryGetValue(cellPosition, out var centerTileData) || centerTileData.tileState == TileState.None)
            return;

        TileState tileState = tileDict[cellPosition].tileState;
        ToolType playerToolType = GameManager.Instance.player.toolType;
        switch (playerToolType)
        {
            case ToolType.Hoe:
                if (centerTileData.tileState == TileState.Empty)
                {
                    UpdateTiles(cellPosition, tileDict, centerTileData);
                }
                break;
            case ToolType.WateringCan:
                {
                    Tilemap wateringMap = GameManager.Instance.tileManager.wateringMap;
                    wateringMap.SetTile(cellPosition, GameManager.Instance.tileManager.wateringTile);
                    tileDict[cellPosition].tileState = TileState.Watered;
                }
                break;
            case ToolType.Pickaxe:
                if (tileState != TileState.Empty)
                {
                    GameManager.Instance.tileManager.wateringMap.SetTile(cellPosition, null);
                    interactableMap.SetTile(cellPosition, GameManager.Instance.tileManager.emptyTile);

                    tileDict[cellPosition].tileConnectedDir = TileConnectedDir.None;
                    tileDict[cellPosition].tileState = TileState.Empty;

                    ResetConnectedTiles(cellPosition, tileDict);
                }
                break;
            default:
                break;
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
            if (rightTileData.tileState == TileState.Tilled)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Right;
                UpdateConnectedTilledTile(rightCellPos, tileDict);
            }
        }

        // 왼쪽
        if (tileDict.TryGetValue(leftCellPos, out var leftTileData))
        {
            leftTileData.tileConnectedDir |= TileConnectedDir.Right;
            if (leftTileData.tileState == TileState.Tilled)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Left;
                UpdateConnectedTilledTile(leftCellPos, tileDict);
            }
        }

        // 위쪽
        if (tileDict.TryGetValue(upCellPos, out var upTileData))
        {
            upTileData.tileConnectedDir |= TileConnectedDir.Down;
            if (upTileData.tileState == TileState.Tilled)
            {
                centerTileData.tileConnectedDir |= TileConnectedDir.Up;
                UpdateConnectedTilledTile(upCellPos, tileDict);
            }
        }

        // 아래쪽
        if (tileDict.TryGetValue(downCellPos, out var downTileData))
        {
            downTileData.tileConnectedDir |= TileConnectedDir.Up;
            if (downTileData.tileState == TileState.Tilled)
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
        List<Tile> interactedTileDict = GameManager.Instance.tileManager.tilledTileDict;

        // TileConnectedState 설정
        SetTileConnectedDirection(cellPosition, tileDict);
        int tileConnectedState = (int)tileDict[cellPosition].tileConnectedState;
        interactableMap.SetTile(cellPosition, interactedTileDict[tileConnectedState]);

        // TileState 설정
        tileDict[cellPosition].tileState = TileState.Tilled;
    }

    static void ResetConnectedTiles(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict)
    {
        Tilemap interactableMap = GameManager.Instance.tileManager.interactableMap;
        List<Tile> interactedTileDict = GameManager.Instance.tileManager.tilledTileDict;

        Vector3Int rightCellPos = cellPosition + Vector3Int.right;
        Vector3Int leftCellPos = cellPosition + Vector3Int.left;
        Vector3Int upCellPos = cellPosition + Vector3Int.up;
        Vector3Int downCellPos = cellPosition + Vector3Int.down;

        if (tileDict[rightCellPos].tileState == TileState.Tilled)
        {
            tileDict[rightCellPos].tileConnectedDir &= ~TileConnectedDir.Left;
            SetTileConnectedDirection(rightCellPos, tileDict);
            int tileConnectedState = (int)tileDict[rightCellPos].tileConnectedState;
            interactableMap.SetTile(rightCellPos, interactedTileDict[tileConnectedState]);
        }
        if (tileDict[leftCellPos].tileState == TileState.Tilled)
        {
            tileDict[leftCellPos].tileConnectedDir &= ~TileConnectedDir.Right;
            SetTileConnectedDirection(leftCellPos, tileDict);
            int tileConnectedState = (int)tileDict[leftCellPos].tileConnectedState;
            interactableMap.SetTile(leftCellPos, interactedTileDict[tileConnectedState]);
        }
        if (tileDict[upCellPos].tileState == TileState.Tilled)
        {
            tileDict[upCellPos].tileConnectedDir &= ~TileConnectedDir.Down;
            SetTileConnectedDirection(upCellPos, tileDict);
            int tileConnectedState = (int)tileDict[upCellPos].tileConnectedState;
            interactableMap.SetTile(upCellPos, interactedTileDict[tileConnectedState]);
        }
        if (tileDict[downCellPos].tileState == TileState.Tilled)
        {
            tileDict[downCellPos].tileConnectedDir &= ~TileConnectedDir.Up;
            SetTileConnectedDirection(downCellPos, tileDict);
            int tileConnectedState = (int)tileDict[downCellPos].tileConnectedState;
            interactableMap.SetTile(downCellPos, interactedTileDict[tileConnectedState]);
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
