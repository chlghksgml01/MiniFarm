using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public ItemManager itemManager;
    public UI_Manager uiManager;
    public Player player;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                if (instance == null)
                {
                    Debug.LogError("¾À¿¡ GameManager ¾øÀ½");
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
        }
        instance = this;

        DontDestroyOnLoad(this);

        itemManager = gameObject.GetComponent<ItemManager>();
        uiManager = gameObject.GetComponent<UI_Manager>();

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
}
