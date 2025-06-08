using UnityEngine;

public class InGameCanvas : MonoBehaviour
{
    static InGameCanvas instance;
    [HideInInspector] public StoreUI storeUI;

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
    }

    public void ExitButton()
    {
        SceneLoadManager.Instance.StartLoadScene("Title", false, false);
        Time.timeScale = 1f;
    }
}
