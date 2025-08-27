using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-10000)]
[RequireComponent(typeof(ItemManager))]
[RequireComponent(typeof(UI_Manager))]
[RequireComponent(typeof(DayTimeManager))]
[RequireComponent(typeof(TileManager))]
[RequireComponent(typeof(CropManager))]
[RequireComponent(typeof(SlimeSpawnController))]
public class InGameManager : MonoBehaviour
{
    static InGameManager instance;
    public ItemManager itemManager { get; private set; }
    public UI_Manager uiManager { get; private set; }
    public DayTimeManager dayTimeManager { get; private set; }
    public TileManager tileManager { get; private set; }
    public CropManager cropManager { get; private set; }
    public SlimeSpawnController slimeSpawnController { get; private set; }
    public Player player { get; private set; }

    [Header("Player")]
    [SerializeField] public Image healthBar;
    [SerializeField] public Image staminaBar;
    [SerializeField] public GameObject playerPrefab;

    [Header("Gift")]
    [SerializeField] private GameObject Gift;

    [HideInInspector] public GiftGet giftGet;

    public static InGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<InGameManager>();
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

        InitManagers();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        var current = SceneManager.GetActiveScene();
        OnSceneLoaded(current, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitPlayer(scene.name);
    }

    private void InitPlayer(string sceneName)
    {
        if (player != null)
            return;

        Vector3 pos = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if (sceneName == "House")
            pos = new Vector3(0.5f, 0f, 0f);
        else if (sceneName == "Farm")
            pos = Vector3.zero;

        player = Instantiate(playerPrefab, pos, rot).GetComponent<Player>();
    }

    private void InitManagers()
    {
        itemManager = gameObject.GetComponent<ItemManager>();
        uiManager = gameObject.GetComponent<UI_Manager>();
        dayTimeManager = gameObject.GetComponent<DayTimeManager>();
        tileManager = gameObject.GetComponent<TileManager>();
        cropManager = gameObject.GetComponent<CropManager>();
        slimeSpawnController = gameObject.GetComponent<SlimeSpawnController>();
    }

    public void CreateGift()
    {
        if (!giftGet.isGiftGet)
            Instantiate(Gift);
    }
}
