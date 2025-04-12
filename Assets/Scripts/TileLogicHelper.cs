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
            // �����¿� ����
            case TileConnectDir.Up | TileConnectDir.Down | TileConnectDir.Left | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.Center;
                break;

            // ������ ����
            case TileConnectDir.Up | TileConnectDir.Down | TileConnectDir.Left:
                tileDict[cellPosition].tileConnectState = TileConnectState.RightCenter;
                break;

            // ���Ͽ� ����
            case TileConnectDir.Up | TileConnectDir.Down | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.LeftCenter;
                break;

            // ���¿� ����
            case TileConnectDir.Up | TileConnectDir.Left | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.DownCenter;
                break;

            // ���¿� ����
            case TileConnectDir.Down | TileConnectDir.Left | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.UpCenter;
                break;

            // ���� ����
            case TileConnectDir.Up | TileConnectDir.Down:
                tileDict[cellPosition].tileConnectState = TileConnectState.VerticalCenter;
                break;

            // ���� ����
            case TileConnectDir.Up | TileConnectDir.Left:
                tileDict[cellPosition].tileConnectState = TileConnectState.RightDown;
                break;

            // ��� ����
            case TileConnectDir.Up | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.LeftDown;
                break;

            // ���� ����
            case TileConnectDir.Down | TileConnectDir.Left:
                tileDict[cellPosition].tileConnectState = TileConnectState.RightUp;
                break;

            // �Ͽ� ����
            case TileConnectDir.Down | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.LeftUp;
                break;

            // �¿� ����
            case TileConnectDir.Left | TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.HorizontalCenter;
                break;

            // ���� ����
            case TileConnectDir.Up:
                tileDict[cellPosition].tileConnectState = TileConnectState.Down;
                break;

            // �Ʒ��� ����
            case TileConnectDir.Down:
                tileDict[cellPosition].tileConnectState = TileConnectState.Up;
                break;

            // ������ ����
            case TileConnectDir.Right:
                tileDict[cellPosition].tileConnectState = TileConnectState.Left;
                break;

            // ���� ����
            case TileConnectDir.Left:
                tileDict[cellPosition].tileConnectState = TileConnectState.Right;
                break;

            // ����X
            default:
                tileDict[cellPosition].tileConnectState = TileConnectState.One;
                break;
        }

        int _tileState = (int)tileDict[cellPosition].tileConnectState;
        interactableMap.SetTile(cellPosition, interactedTileDict[_tileState]);
    }
}