using UnityEngine;

public class InGameCanvas : MonoBehaviour
{
    static InGameCanvas instance;

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
    }

    public void ExitButton()
    {
        SceneLoadManager.Instance.StartLoadScene("Title", false, false);
    }
}
