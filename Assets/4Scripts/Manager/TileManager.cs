using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autodesk.Fbx;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
    [SerializeField] public Tilemap wateringMap;
    [SerializeField] public Tilemap farmFieldMap;

    [SerializeField] Tile hiddenInteractableTile;
    [SerializeField] Tile selectedTile;
    [SerializeField] public Tile emptyTile;
    [SerializeField] public Tile wateringTile;
    [SerializeField] public List<Tile> tilledTileDict;
    [SerializeField] public Tile[] cropTiles;
    [SerializeField] public Tile[] wetCropTiles;

    private Dictionary<Vector3Int, TileData> tileDict = new Dictionary<Vector3Int, TileData>();
    private Dictionary<string, List<Tile>> cropTileDict = new Dictionary<string, List<Tile>>();
    private Dictionary<string, List<Tile>> wetCropTileDict = new Dictionary<string, List<Tile>>();
    public Dictionary<Vector3Int, CropData> cropTileDataDict = new Dictionary<Vector3Int, CropData>();

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

    private void Awake()
    {
        InitialCropTiles(cropTiles, cropTileDict);
        InitialCropTiles(wetCropTiles, wetCropTileDict);
    }

    private void InitialCropTiles(Tile[] _cropTiles, Dictionary<string, List<Tile>> _cropTileDict)
    {
        for (int i = 0; i < _cropTiles.Length; i++)
        {
            string cropTileName = _cropTiles[i].name;
            cropTileName = cropTileName.Replace("Tile", "");
            cropTileName = Regex.Replace(cropTileName, @"\d", "");

            if (!_cropTileDict.ContainsKey(cropTileName))
            {
                _cropTileDict[cropTileName] = new List<Tile>();
            }
            _cropTileDict[cropTileName].Add(_cropTiles[i]);
        }
    }

    void Start()
    {
        InitializeTileData();
        GameManager.Instance.dayTimeManager.OnDayPassed += CropGrow;

        player = GameManager.Instance.player;
    }

    private void OnDisable()
    {
        GameManager.Instance.dayTimeManager.OnDayPassed -= CropGrow;
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

    public Tile GetCropSeedTile(string cropSeedName, bool isWet)
    {
        string cropName = cropSeedName.Replace("Seed", "");
        if (isWet && wetCropTileDict.ContainsKey("Wet" + cropName))
        {
            return wetCropTileDict["Wet" + cropName][0];
        }
        else if (!isWet && cropTileDict.ContainsKey(cropName))
        {
            return cropTileDict[cropName][0];
        }
        return null;
    }

    public void WaterCropTile(Vector3Int cellPosition)
    {
        cropTileDataDict.TryGetValue(cellPosition, out CropData cropData);
        if (cropData == null)
            return;

        cropData.isWatered = true;
        Tile wetTile = wetCropTileDict["Wet" + cropData.cropName][cropData.currentGrowthLevel];
        farmFieldMap.SetTile(selectedTilePos, wetTile);
    }

    public void CropGrow()
    {
        foreach (KeyValuePair<Vector3Int, CropData> cropTile in cropTileDataDict)
        {
            Vector3Int cropPos = cropTile.Key;
            CropData cropData = cropTile.Value;

            if (cropData.isWatered)
            {
                if (cropData.currentGrowthLevel >= cropData.growthLevel)
                    return;

                cropData.currentGrowthDuration++;
                if (cropData.currentGrowthDuration >= cropData.growthDurations[cropData.currentGrowthLevel])
                {
                    cropData.currentGrowthLevel++;
                    Tile nextLevelCropTile = cropTileDict[cropData.cropName][cropData.currentGrowthLevel];
                    farmFieldMap.SetTile(cropPos, nextLevelCropTile);
                }
            }

            cropData.isWatered = false;
            wateringMap.SetTile(cropPos, null);
        }
    }
}
