using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TileLogicHelper
{
    public static void SetTiles(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict, Tilemap interactableMap, List<Tile> interactedTileDict)
    {
        if (tileDict[cellPosition].tileState == TileState.Empty)
            return;

        Vector3Int rightCellPos = cellPosition + Vector3Int.right;
        Vector3Int leftCellPos = cellPosition + Vector3Int.left;
        Vector3Int upCellPos = cellPosition + Vector3Int.up;
        Vector3Int downCellPos = cellPosition + Vector3Int.down;

        tileDict[rightCellPos].tileConnectDir |= TileConnectDir.Left;
        if (tileDict[rightCellPos].tileState != TileState.Empty)
        {
            tileDict[cellPosition].tileConnectDir |= TileConnectDir.Right;
            UpdateConnectState(rightCellPos, tileDict, interactableMap, interactedTileDict);
        }

        tileDict[leftCellPos].tileConnectDir |= TileConnectDir.Right;
        if (tileDict[leftCellPos].tileState != TileState.Empty)
        {
            tileDict[cellPosition].tileConnectDir |= TileConnectDir.Left;
            UpdateConnectState(leftCellPos, tileDict, interactableMap, interactedTileDict);
        }

        tileDict[upCellPos].tileConnectDir |= TileConnectDir.Down;
        if (tileDict[upCellPos].tileState != TileState.Empty)
        {
            tileDict[cellPosition].tileConnectDir |= TileConnectDir.Up;
            UpdateConnectState(upCellPos, tileDict, interactableMap, interactedTileDict);
        }

        tileDict[downCellPos].tileConnectDir |= TileConnectDir.Up;
        if (tileDict[downCellPos].tileState != TileState.Empty)
        {
            tileDict[cellPosition].tileConnectDir |= TileConnectDir.Down;
            UpdateConnectState(downCellPos, tileDict, interactableMap, interactedTileDict);
        }

        UpdateConnectState(cellPosition, tileDict, interactableMap, interactedTileDict);
    }

    static void UpdateConnectState(Vector3Int cellPosition, Dictionary<Vector3Int, TileData> tileDict, Tilemap interactableMap, List<Tile> interactedTileDict)
    {
        TileConnectDir dir = tileDict[cellPosition].tileConnectDir;

        switch (dir)
        {
            // 상하좌우 연결
            case TileConnectDir.Up | TileConnectDir.Down | TileConnectDir.Left | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.Center;
                break;

            // 상하좌 연결
            case TileConnectDir.Up | TileConnectDir.Down | TileConnectDir.Left:
                tileDict[cellPosition].tileConnectState = TileConnectState.RightCenter;
                break;

            // 상하우 연결
            case TileConnectDir.Up | TileConnectDir.Down | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.LeftCenter;
                break;

            // 상좌우 연결
            case TileConnectDir.Up | TileConnectDir.Left | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.DownCenter;
                break;

            // 하좌우 연결
            case TileConnectDir.Down | TileConnectDir.Left | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.UpCenter;
                break;

            // 상하 연결
            case TileConnectDir.Up | TileConnectDir.Down:
                tileDict[cellPosition].tileConnectState = TileConnectState.VerticalCenter;
                break;

            // 상좌 연결
            case TileConnectDir.Up | TileConnectDir.Left:
                tileDict[cellPosition].tileConnectState = TileConnectState.RightDown;
                break;

            // 상우 연결
            case TileConnectDir.Up | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.LeftDown;
                break;

            // 하좌 연결
            case TileConnectDir.Down | TileConnectDir.Left:
                tileDict[cellPosition].tileConnectState = TileConnectState.RightUp;
                break;

            // 하우 연결
            case TileConnectDir.Down | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.LeftUp;
                break;

            // 좌우 연결
            case TileConnectDir.Left | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.HorizontalCenter;
                break;

            // 위쪽 연결
            case TileConnectDir.Up:
                tileDict[cellPosition].tileConnectState = TileConnectState.Down;
                break;

            // 아래쪽 연결
            case TileConnectDir.Down:
                tileDict[cellPosition].tileConnectState = TileConnectState.Up;
                break;

            // 오른쪽 연결
            case TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.Left;
                break;

            // 왼쪽 연결
            case TileConnectDir.Left:
                tileDict[cellPosition].tileConnectState = TileConnectState.Right;
                break;

            // 연결X
            default:
                tileDict[cellPosition].tileConnectState = TileConnectState.One;
                break;
        }

        int _tileState = (int)tileDict[cellPosition].tileConnectState;
        interactableMap.SetTile(cellPosition, interactedTileDict[_tileState]);
    }
}