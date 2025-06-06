using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        InitializeTileData();
        GameManager.Instance.dayTimeManager.OnDayPassed += NewDayTile;

        player = GameManager.Instance.player;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null)
            return;
        GameManager.Instance.dayTimeManager.OnDayPassed -= NewDayTile;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeTileData();

        RefreshCropTileStates();
    }

    private void RefreshCropTileStates()
    {
        if (tilledTileMap == null)
            return;
        CropManager cropManager = GameManager.Instance.cropManager;

        // 괭이질/물 준 타일
        foreach (KeyValuePair<Vector3Int, TileData> tile in tileDict)
        {
            if (tile.Value.tileState != TileState.None && tile.Value.tileState != TileState.Empty)
            {
                tileDict[tile.Key].tileState = TileState.Tilled;
                TileLogicHelper.UpdateTiles(tile.Key, tileDict, tileDict[tile.Key]);
            }

            else if (tile.Value.tileState == TileState.Watered)
            {
                wateringTileMap.SetTile(tile.Key, wateringTile);
                tileDict[tile.Key].tileState = TileState.Watered;
            }
        }

        // 작물 심은 타일
        foreach (KeyValuePair<Vector3Int, CropItemData> plantedCropData in cropManager.plantedCropsDict)
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
        Vector3Int _selectedTilePos = playerCellPosition + new Vector3Int(mouseDirectionValues[mouseDirection].x, mouseDirectionValues[mouseDirection].y);

        if (_selectedTilePos != selectedTilePos)
        {
            farmSelectedTileMap.SetTile(selectedTilePos, null);
            farmSelectedTileMap.SetTile(_selectedTilePos, selectedTile);
            selectedTilePos = _selectedTilePos;
        }
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
        GameManager.Instance.cropManager.plantedCropsDict.TryGetValue(selectedTilePos, out CropItemData CropItemData);
        return CropItemData;
    }

    private void NewDayTile()
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
                TileLogicHelper.ResetConnectedTiles(tilledTile.Key, tileDict);
            }
        }
    }

    public string GetSelectedCropName()
    {
        var cropDict = GameManager.Instance.cropManager.plantedCropsDict;
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

    public List<TileSaveData> GetTileData()
    {
        List<TileSaveData> tileSaveDatas = new List<TileSaveData>();
        foreach (KeyValuePair<Vector3Int, TileData> pair in tileDict)
        {
            if (pair.Value.tileState == TileState.Empty || pair.Value.tileState == TileState.None)
                continue;
            TileSaveData tileSaveData = new TileSaveData();

            tileSaveData.tilePos = pair.Key;
            tileSaveData.tileData = pair.Value;
            if (pair.Value.tileState == TileState.Planted)
            {
                CropItemData cropItemData = GameManager.Instance.cropManager.GetCropData(pair.Key);
                tileSaveData.cropItemData = cropItemData;
            }

            tileSaveDatas.Add(tileSaveData);
        }
        return tileSaveDatas;
    }

    public void LoadTileData(List<TileSaveData> tileSaveDatas)
    {
        foreach (TileSaveData tileSaveData in tileSaveDatas)
        {
            tileDict.Add(tileSaveData.tilePos, tileSaveData.tileData);
            if (tileSaveData.cropItemData != null && tileSaveData.tileData.tileState == TileState.Planted)
            {
                GameManager.Instance.cropManager.SetCropData(tileSaveData.tilePos, tileSaveData.cropItemData);
            }
        }
    }
}
