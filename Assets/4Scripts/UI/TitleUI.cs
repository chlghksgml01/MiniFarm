using UnityEngine;
using UnityEngine.SceneManagement;

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
            string fileName = $"Save{i + 1}";
            bool exists = DataManager.instance.IsFileExist(fileName);

            SaveImage[i].SetActive(exists);
            NewSaveImage[i].SetActive(!exists);
        }
    }

    public void StartSaveButton(string saveFileName)
    {
        DataManager.instance.saveFileName = saveFileName;
        DataManager.instance.CreateFile();
        SceneLoadManager.instance.StartLoadScene("House", true);
    }

    public void DeleteButton()
    {

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
