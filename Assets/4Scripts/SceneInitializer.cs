using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;
    [SerializeField] private BoxCollider2D spawnBox;

    [Header("TileManager")]
    [SerializeField] public Tilemap farmSelectedTileMap;
    [SerializeField] public Tilemap tilledTileMap;
    [SerializeField] public Tilemap wateringTileMap;
    [SerializeField] public Tilemap cropTileMap;

    private void Awake()
    {
        GameManager.Instance.dayTimeManager.globalLight = globalLight;
        GameManager.Instance.dayTimeManager.UpdateLight();

        if (spawnBox != null)
            GameManager.Instance.slimeSpawnController.spawnBox = spawnBox;

        TileManager tileManager = GameManager.Instance.tileManager;
        if (cropTileMap != null)
        {
            tileManager.cropTileMap = cropTileMap;
            tileManager.tilledTileMap = tilledTileMap;
            tileManager.wateringTileMap = wateringTileMap;
        }
        if (farmSelectedTileMap != null)
            tileManager.farmSelectedTileMap = farmSelectedTileMap;
    }
}
