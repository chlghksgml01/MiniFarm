using UnityEngine;
using System.IO;

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
                    Debug.Log("���� DataManager ����");
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

    public void SaveData()
    {
        CreateFolder();

        SavePlayer();
        SaveTile();
        SaveDropItem();
        SaveGift();

        Debug.Log("Save Data");
    }

    public void CreateFolder()
    {
        path = Path.Combine(Application.persistentDataPath, saveRootFolderName, saveFolderName);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }


    private void SavePlayer()
    {
        if (InGameManager.Instance.player == null)
        {
            Debug.LogError("DataManager - Player ����");
            return;
        }

        string json = JsonUtility.ToJson(InGameManager.Instance.player.playerSaveData);
        File.WriteAllText(Path.Combine(path, playerSaveFileName), json);
    }

    private void SaveTile()
    {
        tileSaveDatas = InGameManager.Instance.tileManager.GetTileData();

        string json = JsonUtility.ToJson(tileSaveDatas);
        File.WriteAllText(Path.Combine(path, tileSaveFileName), json);
    }

    private void SaveDropItem()
    {
        string json = JsonUtility.ToJson(InGameManager.Instance.itemManager.dropItemData);
        File.WriteAllText(Path.Combine(path, dropItemSaveFileName), json);
    }

    private void SaveGift()
    {
        string json = JsonUtility.ToJson(InGameManager.Instance.giftGet);
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
            InGameManager.Instance.player.playerSaveData = JsonUtility.FromJson<PlayerSaveData>(data);
        }
    }

    private void LoadTile()
    {
        string fullPath = Path.Combine(path, tileSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            tileSaveDatas = JsonUtility.FromJson<TileSaveDatas>(data);

            InGameManager.Instance.tileManager.LoadTileData(tileSaveDatas);
        }
    }

    private void LoadDropItem()
    {
        string fullPath = Path.Combine(path, dropItemSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            InGameManager.Instance.itemManager.dropItemData = JsonUtility.FromJson<DropItemDatas>(data);
        }
    }

    private void LoadGift()
    {
        string fullPath = Path.Combine(path, giftGetSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            InGameManager.Instance.giftGet = JsonUtility.FromJson<GiftGet>(data);
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
            Debug.Log("���� ���� �Ϸ�");
        }
        else
        {
            Debug.Log("������ ���� ����");
        }
    }
}
