using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject Title;
    [SerializeField] private GameObject Save;
    [SerializeField] private GameObject[] SaveImage;
    [SerializeField] private GameObject[] NewSaveImage;

    private void Awake()
    {
        Title.SetActive(true);
        Save.SetActive(false);
    }

    public void OpenSaveUIButton()
    {
        Title.SetActive(false);
        Save.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            string folderName = $"Save{i + 1}";
            bool exists = DataManager.instance.IsFolderExist(folderName);

            SaveImage[i].SetActive(exists);
            NewSaveImage[i].SetActive(!exists);
        }
    }

    public void StartSaveButton(string saveFolderName)
    {
        DataManager.instance.saveFolderName = saveFolderName;
        SceneLoadManager.Instance.StartLoadScene("House", true, false);
    }

    public void DeleteButton(string saveFolderName)
    {
        DataManager.instance.DeleteSaveFile(saveFolderName);

        int index = saveFolderName[saveFolderName.Length - 1] - '0' - 1;
        SaveImage[index].SetActive(false);
        NewSaveImage[index].SetActive(true);
    }

    public void BackButton()
    {
        Title.SetActive(true);
        Save.SetActive(false);
    }

    public void ExitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
