using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] public Tilemap interactableMap;

    [SerializeField] Tile hiddenInteractableTile;
    [SerializeField] List<Tile> interactedTileDict;

    Dictionary<Vector3Int, TileData> tileDict = new Dictionary<Vector3Int, TileData>();

    void Start()
    {
        // Ÿ�ϸ� ���� ��� �� ��ǥ �ϳ��� ��������
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);
            if (tile != null && tile.name == "Visible_InteractableTile")
            {
                interactableMap.SetTile(position, hiddenInteractableTile);

                TileData tileData = new TileData();
                tileDict.Add(position, tileData);
            }
        }
    }

    public void SetInteracted(Vector3 position)
    {
        Vector3Int cellPosition = interactableMap.WorldToCell(position);

        SetTileState(cellPosition);
    }

    void SetTileState(Vector3Int cellPosition)
    {
        if (!tileDict.ContainsKey(cellPosition))
        {
            Debug.LogWarning("TileManager - cellPosition�� Ÿ�� ����");
            return;
        }

        // Ÿ�ϸʿ� Ÿ�� ����, Ÿ�� ���� ����
        tileDict[cellPosition].tileState = TileState.Tilled;
        TileLogicHelper.SetTiles(cellPosition, tileDict, interactableMap, interactedTileDict);
    }

    public string GetTileName(Vector3Int position)
    {
        if(interactableMap != null)
        {
            TileBase tile = interactableMap.GetTile(position);

            if(tile != null)
            {
                return tile.name;
            }
        }
        return "";
    }
}
