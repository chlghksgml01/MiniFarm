using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    private string saveRootFolderName = "Saves";
    public string saveFolderName = "TempPath";

    private string path;
    private string playerSaveFileName = "player_save.json";
    private string tileSaveFileName = "tile_save.json";
    private string dropItemSaveFileName = "dropItem_save.json";
    private string giftGetSaveFileName = "giftGet_save.json";

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
                    Debug.Log("씬에 DataManager 없음");
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
    }

    public void CreateFolder()
    {
        path = Path.Combine(Application.persistentDataPath, saveRootFolderName, saveFolderName);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public void SaveData()
    {
        CreateFolder();

        SavePlayer();
        SaveTile();
        SaveDropItem();
        SaveGift();
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

    private void SaveGift()
    {
        string json = JsonUtility.ToJson(GameManager.Instance.giftGet);
        File.WriteAllText(Path.Combine(path, giftGetSaveFileName), json);
    }

    public void LoadData()
    {
        path = Path.Combine(Application.persistentDataPath, saveRootFolderName, saveFolderName);

        LoadPlayer();
        LoadTile();
        LoadDropItem();
        LoadGift();
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
        }
    }

    private void LoadGift()
    {
        string fullPath = Path.Combine(path, giftGetSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            GameManager.Instance.giftGet = JsonUtility.FromJson<GiftGet>(data);
        }
    }

    public bool IsFolderExist(string _saveFolderName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, saveRootFolderName, _saveFolderName);
        return Directory.Exists(folderPath);
    }

    public void DeleteSaveFile(string _saveFolderName)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, saveRootFolderName, _saveFolderName);

        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true);
            Debug.Log("폴더 삭제 완료");
        }
        else
        {
            Debug.Log("삭제할 폴더 없음");
        }
    }
}
