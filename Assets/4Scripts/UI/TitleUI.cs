using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private GameObject Title;
    [SerializeField] private GameObject Save;

    private void Awake()
    {
        Title.SetActive(true);
        Save.SetActive(false);
    }

    public void OpenSaveUIButton()
    {
        Title.SetActive(false);
        Save.SetActive(true);
    }

    public void StartSaveButton()
    {

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
