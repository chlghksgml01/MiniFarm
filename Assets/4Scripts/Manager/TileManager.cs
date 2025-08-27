using System.Collections.Generic;
using System.Linq;
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
    [Header("타일맵")]
    [SerializeField] public Tilemap farmSelectedTileMap;
    [SerializeField] public Tilemap tilledTileMap;
    [SerializeField] public Tilemap wateringTileMap;
    [SerializeField] public Tilemap cropTileMap;

    [Header("타일")]
    [SerializeField] Tile hiddenInteractableTile;
    [SerializeField] Tile selectedTile;
    [SerializeField] public Tile emptyTile;
    [SerializeField] public Tile wateringTile;
    [SerializeField] public List<Tile> tilledTileDict;

    [Space]
    [SerializeField] private int resetTileChance = 30;

    public Dictionary<Vector3Int, TileData> tileDict = new Dictionary<Vector3Int, TileData>();

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
    public MouseDirection mouseDirection;
    public Vector3Int selectedTilePos { get; set; }

    void Start()
    {
        player = InGameManager.Instance.player;

        InitializeTileData();
        InGameManager.Instance.dayTimeManager.OnDayFinished += PrepareNewDayTile;

        if (SceneLoadManager.Instance == null)
        {
            Debug.Log("Inventory_UI - SceneLoadManager 없음");
            return;
        }
        SceneLoadManager.Instance.SceneLoad += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (InGameManager.Instance == null)
            return;
        InGameManager.Instance.dayTimeManager.OnDayFinished -= PrepareNewDayTile;

        if (SceneLoadManager.Instance == null)
            return;
        SceneLoadManager.Instance.SceneLoad -= OnSceneLoaded;
    }

    private void PrepareNewDayTile()
    {
        var wateredTiles = tileDict.Where(pair => pair.Value.tileState == TileState.Watered);
        foreach (var wateredTile in wateredTiles)
        {
            if (wateringTileMap != null)
                wateringTileMap.SetTile(wateredTile.Key, null);
            wateredTile.Value.tileState = TileState.Tilled;
        }

        var tilledTiles = tileDict.Where(pair => pair.Value.tileState == TileState.Tilled);
        foreach (var tilledTile in tilledTiles)
        {
            bool isReset = Random.Range(0, 100) <= resetTileChance;
            if (isReset)
            {
                if (tilledTileMap != null)
                    tilledTileMap.SetTile(tilledTile.Key, null);
                tilledTile.Value.tileState = TileState.Empty;
            }
        }
    }

    void OnSceneLoaded()
    {
        InitializeTileData();
        RefreshTileStates();
    }

    private void RefreshTileStates()
    {
        if (tilledTileMap == null)
            return;

        // 괭이질/물 준 타일
        foreach (KeyValuePair<Vector3Int, TileData> tile in tileDict)
        {
            if (tile.Value.tileState != TileState.None && tile.Value.tileState != TileState.Empty)
            {
                TileLogicHelper.ConnectSurroundginTiles(tile.Key, tileDict, tileDict[tile.Key]);

                if (tile.Value.tileState == TileState.Watered)
                {
                    wateringTileMap.SetTile(tile.Key, wateringTile);
                    tileDict[tile.Key].tileState = TileState.Watered;
                }
                else
                    tileDict[tile.Key].tileState = TileState.Tilled;
            }
        }
        // 작물 심은 타일
        foreach (KeyValuePair<Vector3Int, CropItemData> plantedCropData in InGameManager.Instance.cropManager.plantedCropsDict)
        {
            if (tileDict.ContainsKey(plantedCropData.Key))
            {
                if (plantedCropData.Value.isWatered)
                {
                    int growthIndex = plantedCropData.Value.currentGrowthLevel - 1;
                    cropTileMap.SetTile(plantedCropData.Key, plantedCropData.Value.wetCropTiles[growthIndex]);
                    wateringTileMap.SetTile(plantedCropData.Key, wateringTile);
                }

                else
                {
                    int growthIndex = plantedCropData.Value.currentGrowthLevel - 1;
                    cropTileMap.SetTile(plantedCropData.Key, plantedCropData.Value.cropTiles[growthIndex]);

                    tileDict[plantedCropData.Key].tileState = TileState.Planted;
                }
            }
        }
    }

    private void InitializeTileData()
    {
        if (tilledTileMap == null)
            return;
        // 타일맵 안의 모든 셀 좌표 하나씩 가져오기
        foreach (var position in tilledTileMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilledTileMap.GetTile(position);
            if (tile != null && tile.name == "Visible_InteractableTile")
            {
                tilledTileMap.SetTile(position, hiddenInteractableTile);

                TileData tileData = new TileData();
                tileDict.TryAdd(position, tileData);
            }
        }
    }

    private void Update()
    {
        if (farmSelectedTileMap == null)
            return;

        UpdateMouseDirection();
        DrawSelectedTile();
    }

    private void UpdateMouseDirection()
    {
        playerCellPosition = farmSelectedTileMap.WorldToCell(player.transform.position);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int mouseCellPos = farmSelectedTileMap.WorldToCell(mousePos);

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
        Vector3Int _selectedTilePos = playerCellPosition
            + new Vector3Int(mouseDirectionValues[mouseDirection].x, mouseDirectionValues[mouseDirection].y);

        if (_selectedTilePos != selectedTilePos)
        {
            farmSelectedTileMap.SetTile(selectedTilePos, null);
            farmSelectedTileMap.SetTile(_selectedTilePos, selectedTile);
            selectedTilePos = _selectedTilePos;
        }
        Debug.Log("Selected Tile Pos: " + selectedTilePos);
    }

    public void ChangeTileState()
    {
        player.SetPlayerDirection(mouseDirection);

        if (tileDict.ContainsKey(selectedTilePos))
            SetTileState();
    }

    private void SetTileState()
    {
        if (!tileDict.ContainsKey(selectedTilePos))
        {
            Debug.LogWarning("TileManager - cellPosition에 타일 없음");
            return;
        }

        TileLogicHelper.SetTiles(selectedTilePos, tileDict);
    }

    public CropItemData GetSelectedCropItemData()
    {
        InGameManager.Instance.cropManager.plantedCropsDict.TryGetValue(selectedTilePos, out CropItemData CropItemData);
        return CropItemData;
    }

    public string GetSelectedCropName()
    {
        var cropDict = InGameManager.Instance.cropManager.plantedCropsDict;
        cropDict.TryGetValue(selectedTilePos, out CropItemData CropItemData);

        if (CropItemData == null)
            return "";

        return CropItemData.cropName;
    }

    public void SetTile(Tilemap tileMap, Vector3Int tilePos, TileBase tile)
    {
        if (tileMap != null)
            tileMap.SetTile(tilePos, tile);
    }

    public TileSaveDatas GetTileData()
    {
        TileSaveDatas tileSaveDatas = new TileSaveDatas();
        foreach (KeyValuePair<Vector3Int, TileData> pair in tileDict)
        {
            if (pair.Value.tileState == TileState.Empty || pair.Value.tileState == TileState.None)
                continue;
            TileSaveData tileSaveData = new TileSaveData();

            tileSaveData.tilePos = pair.Key;
            tileSaveData.tileData = pair.Value;

            if (pair.Value.tileState == TileState.Planted)
                InGameManager.Instance.cropManager.SaveCropData(tileSaveData.cropSaveData, pair.Key);

            tileSaveDatas.tileSaveDatas.Add(tileSaveData);
        }
        return tileSaveDatas;
    }

    public void LoadTileData(TileSaveDatas tileSaveDatas)
    {
        foreach (TileSaveData tileSaveData in tileSaveDatas.tileSaveDatas)
        {
            if (!tileDict.ContainsKey(tileSaveData.tilePos))
                tileDict.Add(tileSaveData.tilePos, tileSaveData.tileData);
            else
                tileDict[tileSaveData.tilePos] = tileSaveData.tileData;
            if (tileSaveData.cropSaveData != null && tileSaveData.tileData.tileState == TileState.Planted)
            {
                InGameManager.Instance.cropManager.LoadCropData(tileSaveData.tilePos, tileSaveData.cropSaveData);
            }
        }
    }
}
