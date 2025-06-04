using UnityEngine;

public class InGameCanvas : MonoBehaviour
{
    private static InGameCanvas instance;
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
}
