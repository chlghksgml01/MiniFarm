using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    private string path;
    private string playerSaveFileName = "player_save.json";
    private string tileSaveFileName = "tile_save.json";
    private string dropItemSaveFileName = "dropItem_save.json";

    public string saveFileName = "default";

    private TileSaveDatas tileSaveDatas = new TileSaveDatas();

    public static DataManager instance;

    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<DataManager>();
                if (instance == null)
                {
                    Debug.LogError("씬에 DataManager 없음");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this);

        path = Path.Combine(Application.dataPath, "..", "Saves", saveFileName);
        CreateFile();
    }

    public void CreateFile()
    {
        path = Path.Combine(Application.dataPath, "..", "Saves", saveFileName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public void SaveData()
    {
        SavePlayer();
        SaveTile();
        SaveDropItem();
        Debug.Log("Save Data");
    }

    private void SavePlayer()
    {
        if (GameManager.Instance.player == null)
        {
            Debug.LogError("DataManager - Player 없음");
            return;
        }

        string json = JsonUtility.ToJson(GameManager.Instance.player.playerSaveData);
        File.WriteAllText(Path.Combine(path, playerSaveFileName), json);
    }

    private void SaveTile()
    {
        tileSaveDatas = GameManager.Instance.tileManager.GetTileData();

        string json = JsonUtility.ToJson(tileSaveDatas);
        File.WriteAllText(Path.Combine(path, tileSaveFileName), json);
    }

    private void SaveDropItem()
    {
        string json = JsonUtility.ToJson(GameManager.Instance.itemManager.dropItemData);
        File.WriteAllText(Path.Combine(path, dropItemSaveFileName), json);
    }

    public void LoadData()
    {
        LoadPlayer();
        LoadTile();
        LoadDropItem();
        Debug.Log("Load Data");
    }

    private void LoadPlayer()
    {
        string fullPath = Path.Combine(path, playerSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            GameManager.Instance.player.playerSaveData = JsonUtility.FromJson<PlayerSaveData>(data);
        }
    }

    private void LoadTile()
    {
        string fullPath = Path.Combine(path, tileSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            tileSaveDatas = JsonUtility.FromJson<TileSaveDatas>(data);

            GameManager.Instance.tileManager.LoadTileData(tileSaveDatas);
        }
    }

    private void LoadDropItem()
    {
        string fullPath = Path.Combine(path, dropItemSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            GameManager.Instance.itemManager.dropItemData = JsonUtility.FromJson<DropItemDatas>(data);
            GameManager.Instance.itemManager.CreateItem();
        }
    }

    public bool IsFileExist(string _saveFileName)
    {
        path = Path.Combine(Application.dataPath, "..", "Saves", _saveFileName);
        return Directory.Exists(path);
    }
}
