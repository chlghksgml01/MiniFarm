using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    [HideInInspector] public ItemManager itemManager;
    [HideInInspector] public UI_Manager uiManager;
    [HideInInspector] public DayTimeManager dayTimeManager;
    [HideInInspector] public TileManager tileManager;
    [HideInInspector] public CropManager cropManager;
    [HideInInspector] public SlimeSpawnController slimeSpawnController;
    [HideInInspector] public SceneLoadManager sceneLoadManager;

    [Header("Player")]
    [SerializeField] public Image healthBar;
    [SerializeField] public Image staminaBar;
    [HideInInspector] public Player player;
    [SerializeField] public GameObject playerPrefab;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
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

        itemManager = gameObject.GetComponent<ItemManager>();
        uiManager = gameObject.GetComponent<UI_Manager>();
        dayTimeManager = gameObject.GetComponent<DayTimeManager>();
        tileManager = gameObject.GetComponent<TileManager>();
        cropManager = gameObject.GetComponent<CropManager>();
        slimeSpawnController = gameObject.GetComponent<SlimeSpawnController>();
        sceneLoadManager = gameObject.GetComponent<SceneLoadManager>();

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "House")
            Instantiate(playerPrefab, new Vector3(0.5f, 0f, 0f), Quaternion.identity);
        else if (sceneName == "Farm")
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.healthBar = healthBar;
        player.staminaBar = staminaBar;
    }
}
