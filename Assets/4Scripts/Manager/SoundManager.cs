
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] public BGMManager bgmManager;
    [SerializeField] public SFXManager sfxManager;

    private static SoundManager instance;

    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SoundManager>();
                if (instance == null)
                {
                    Debug.Log("¾À¿¡ SceneLoadManager ¾øÀ½");
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

        DontDestroyOnLoad(gameObject);
    }
}
