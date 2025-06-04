using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class DataManager : MonoBehaviour
{
    private string path;
    private string playerSaveFileName = "player_save.json";
    private string cropSaveFileName = "crop_save.json";
    private string saveFiles = "default";
    private Player player;

    private PlayerSaveData playerSaveData = new PlayerSaveData();
    private CropSaveData cropSaveData = new CropSaveData();

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

        path = Path.Combine(Application.dataPath, "..", "Saves", saveFiles);
        CreateFile();
    }

    private void Start()
    {
        player = GameManager.Instance.player;
    }

    public void CreateFile()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public void SaveData()
    {
        SavePlayer();
        SaveCrop();
    }

    private void SavePlayer()
    {
        if (player == null)
        {
            Debug.LogError("DataManager - Player 없음");
            return;
        }

        string json = string.Empty;
        json = JsonUtility.ToJson(playerSaveData);
        File.WriteAllText(Path.Combine(path, playerSaveFileName), json);
    }

    private void SaveCrop()
    {
        cropSaveData = GameManager.Instance.cropManager.GetCropSaveData();
        if (cropSaveData.cropPos.Count == 0)
            return;

        string json = JsonUtility.ToJson(cropSaveData);
        File.WriteAllText(Path.Combine(path, cropSaveFileName), json);
    }

    public void LoadData()
    {
        LoadPlayer();
        LoadCrop();
    }

    private void LoadPlayer()
    {
        string fullPath = Path.Combine(path, playerSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            playerSaveData = JsonUtility.FromJson<PlayerSaveData>(data);
        }
    }

    private void LoadCrop()
    {
        string fullPath = Path.Combine(path, cropSaveFileName);

        if (File.Exists(fullPath))
        {
            string data = File.ReadAllText(fullPath);
            cropSaveData = JsonUtility.FromJson<CropSaveData>(data);
            GameManager.Instance.cropManager.SetCropSaveData(cropSaveData);
        }
    }
}
