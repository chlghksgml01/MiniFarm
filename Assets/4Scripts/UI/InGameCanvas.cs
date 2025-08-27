using UnityEngine;
using UnityEngine.UI;

public class InGameCanvas : MonoBehaviour
{
    static InGameCanvas instance;
    [HideInInspector] public StoreUI storeUI;
    [SerializeField] public Slider backgroundSlider;
    [SerializeField] public Slider SFXSlider;

    public static InGameCanvas Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<InGameCanvas>();
                if (instance == null)
                {
                    Debug.Log("¾À¿¡ GameManager ¾øÀ½");
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

        storeUI = GetComponentInChildren<StoreUI>();
        backgroundSlider.onValueChanged.AddListener(SetBGMVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    void OnDestroy()
    {
        backgroundSlider.onValueChanged.RemoveListener(SetBGMVolume);
        SFXSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }

    public void ExitButton()
    {
        SceneLoadManager.Instance.StartLoadScene("Title", false, false);
        Time.timeScale = 1f;
    }

    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.bgmManager.ChangeVolume(volume);
    }
    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance.sfxManager.ChangeVolume(volume);
    }
}
