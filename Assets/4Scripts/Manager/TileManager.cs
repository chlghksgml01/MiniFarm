using System.Collections.Generic;
using Autodesk.Fbx;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MouseDirection
{
    Center,
    Up, Down,
    Right, UpRight, DownRight,
    Left, UpLeft, DownLeft,
}

public class TileManager : MonoBehaviour
{
    [SerializeField] public Tilemap interactableMap;
    [SerializeField] public Tilemap mainTileMap;

    [SerializeField] Tile hiddenInteractableTile;
    [SerializeField] Tile selectedTile;
    [SerializeField] List<Tile> interactedTileDict;

    Dictionary<Vector3Int, TileData> tileDict = new Dictionary<Vector3Int, TileData>();

    private Player player;
    private Vector3Int playerCellPosition;

    private Dictionary<MouseDirection, Vector2Int> mouseDirectionValues = new Dictionary<MouseDirection, Vector2Int>
        {
            { MouseDirection.Center,     Vector2Int.zero },
            { MouseDirection.Up,         Vector2Int.up },
            { MouseDirection.Down,       Vector2Int.down },
            { MouseDirection.Right,      Vector2Int.right },
            { MouseDirection.Left,       Vector2Int.left },
            { MouseDirection.UpRight,    new Vector2Int(1, 1) },
            { MouseDirection.DownRight,  new Vector2Int(1, -1)},
            { MouseDirection.UpLeft,     new Vector2Int(-1, 1)},
            { MouseDirection.DownLeft,   new Vector2Int(-1, -1)}
        };
    private MouseDirection mouseDirection;
    private Vector3Int selectedTilePos;

    void Start()
    {
        InitializeTileData();

        player = GameManager.Instance.player;
    }

    private void InitializeTileData()
    {
        // 타일맵 안의 모든 셀 좌표 하나씩 가져오기
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);
            if (tile != null && tile.name == "Visible_InteractableTile")
            {
                interactableMap.SetTile(position, hiddenInteractableTile);

                TileData tileData = new TileData();
                tileDict.TryAdd(position, tileData);
            }
        }
    }

    private void Update()
    {
        UpdateMouseDirection();
        DrawSelectedTile();
    }

    private void UpdateMouseDirection()
    {
        playerCellPosition = mainTileMap.WorldToCell(player.transform.position);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCellPos = mainTileMap.WorldToCell(mousePos);

        GetMouseDirection(playerCellPosition, mouseCellPos);
    }

    private void GetMouseDirection(Vector3Int playerPos, Vector3 mousePos)
    {
        Vector2 dir = (mousePos - playerPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle < -180f)
            angle += 360f;
        if (angle > 180f)
            angle -= 360f;

        if (angle >= -22.5f && angle < 22.5f)
            mouseDirection = MouseDirection.Right;
        else if (angle >= 22.5f && angle < 67.5f)
            mouseDirection = MouseDirection.UpRight;
        else if (angle >= 67.5f && angle < 112.5f)
            mouseDirection = MouseDirection.Up;
        else if (angle >= 112.5f && angle < 157.5f)
            mouseDirection = MouseDirection.UpLeft;
        else if (angle >= 157.5f || angle < -157.5f)
            mouseDirection = MouseDirection.Left;
        else if (angle >= -157.5f && angle < -112.5f)
            mouseDirection = MouseDirection.DownLeft;
        else if (angle >= -112.5f && angle < -67.5f)
            mouseDirection = MouseDirection.Down;
        else if (angle >= -67.5f && angle < -22.5f)
            mouseDirection = MouseDirection.DownRight;
        else
            mouseDirection = MouseDirection.Right;

        Vector3Int intMousePos = new Vector3Int((int)mousePos.x, (int)mousePos.y, 0);
        if (playerPos == intMousePos)
            mouseDirection = MouseDirection.Center;
    }

    private void DrawSelectedTile()
    {
        Vector3Int _selectedTilePos = playerCellPosition + new Vector3Int(mouseDirectionValues[mouseDirection].x, mouseDirectionValues[mouseDirection].y);

        if (_selectedTilePos != selectedTilePos)
        {
            mainTileMap.SetTile(selectedTilePos, null);
            mainTileMap.SetTile(_selectedTilePos, selectedTile);
            selectedTilePos = _selectedTilePos;
        }
    }

    public void ChangeTileState()
    {
        player.SetPlayerDirection(mouseDirection);

        string tileName = GetTileName(selectedTilePos);

        if (!string.IsNullOrWhiteSpace(tileName))
        {
            if (tileName == "InVisible_InteractableTile")
            {
                SetTileState();
            }
        }
    }

    private void SetTileState()
    {
        if (!tileDict.ContainsKey(selectedTilePos))
        {
            Debug.LogWarning("TileManager - cellPosition에 타일 없음");
            return;
        }

        // 타일맵에 타일 설정, 타일 상태 설정
        tileDict[selectedTilePos].tileState = TileState.Tilled;
        TileLogicHelper.SetTiles(selectedTilePos, tileDict, interactableMap, interactedTileDict);
    }

    public string GetTileName(Vector3Int position)
    {
        if (interactableMap != null)
        {
            TileBase tile = interactableMap.GetTile(position);

            if (tile != null)
            {
                return tile.name;
            }
        }
        return "";
    }
}
