using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private float fadeInOutDuration = 0.5f;
    [SerializeField] private Image fadeInOutImage;

    [HideInInspector] public string prevSceneName = string.Empty;
    [HideInInspector] public bool isSceneLoading = false;
    [HideInInspector] public bool isFarmToHouse = false;

    public event Action SceneLoad = null;
    private Coroutine sceneLoadCoroutine = null;

    private static SceneLoadManager instance;

    public static SceneLoadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<SceneLoadManager>();
                if (instance == null)
                {
                    Debug.Log("SceneLoadManager 없음");
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

    public void StartLoadScene(string sceneName, bool isGameStart, bool isNextDay)
    {
        if (sceneLoadCoroutine == null)
            sceneLoadCoroutine = StartCoroutine(LoadScene(sceneName, isGameStart, isNextDay));

        if (TryEnsureFadeImage())
            fadeInOutImage.transform.SetAsLastSibling();
    }

    private IEnumerator LoadScene(string sceneName, bool isGameStart, bool isNextDay)
    {
        isSceneLoading = true;
        prevSceneName = SceneManager.GetActiveScene().name;
        if (!isGameStart)
        {
            if (prevSceneName == "Farm" && sceneName == "House" && !InGameManager.Instance.player.isDead)
                isFarmToHouse = true;
            InGameManager.Instance.itemManager.SaveDropItem();
        }

        SetBGM(sceneName);

        if (!isGameStart)
        {
            InGameManager.Instance.player.StartSceneLoad();
            InGameManager.Instance.dayTimeManager.SetTimeStop(true);
        }

        StartCoroutine(FadeInOut(0f, 1f, fadeInOutDuration));
        yield return new WaitForSecondsRealtime(fadeInOutDuration);

        if (sceneName == "Title")
        {
            Destroy(InGameManager.Instance.player.gameObject);
            Destroy(InGameManager.Instance.gameObject);
            InGameCanvas inGameCanvas = FindFirstObjectByType<InGameCanvas>(FindObjectsInactive.Include);
            if (inGameCanvas != null)
                Destroy(inGameCanvas.gameObject);
        }

        if (!isGameStart && isNextDay)
            InGameManager.Instance.dayTimeManager.StartOnDayFinishedEvent();

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOp.isDone)
            yield return null;

        if (sceneName != "Title")
            SceneLoad?.Invoke();

        if (isNextDay)
            StartCoroutine(InGameManager.Instance.dayTimeManager.StartNewDay());

        yield return new WaitForSecondsRealtime(fadeInOutDuration);

        if (isGameStart)
        {
            DataManager.instance.LoadData();
            InGameManager.Instance.player.InitializePlayerData();
            InGameManager.Instance.player.gameObject.SetActive(true);
            InGameManager.Instance.uiManager.RefreshInventoryUI();

        }
        if (InGameManager.Instance != null)
        {
            InGameManager.Instance.itemManager.CreateItem();
            SetSceneLoadData(sceneName, isNextDay);
        }

        StartCoroutine(FadeInOut(1f, 0f, fadeInOutDuration));

        isSceneLoading = false;
        if (InGameManager.Instance != null)
        {
            InGameManager.Instance.dayTimeManager.SetTimeStop(false);
            InGameManager.Instance.uiManager.InitializeUI();
        }
        sceneLoadCoroutine = null;
    }

    private void SetBGM(string sceneName)
    {
        if (prevSceneName == "Title")
            SoundManager.Instance.bgmManager.ChangeBGM(BGMNAME.InGame);
        else if (sceneName == "Title")
            SoundManager.Instance.bgmManager.ChangeBGM(BGMNAME.Title);
        else
            SoundManager.Instance.bgmManager.ChangeBGM(BGMNAME.None);
    }

    private void SetSceneLoadData(string sceneName, bool isNextDay)
    {
        Player player = InGameManager.Instance.player;


        if (isNextDay)
        {
            player.transform.position = new Vector3(3.32f, 1.4f);
            player.LookDown();
            InGameManager.Instance.CreateGift();
        }        
        else if (sceneName == "Farm" && prevSceneName == "House")
        {
            player.transform.position = Vector3.zero;
            player.LookDown();
        }        
        else if (sceneName == "House" && prevSceneName == "Farm")
        {
            InGameManager.Instance.CreateGift();
            player.transform.position = new Vector3(0.5f, 0f);
            player.LookUp();
        }
        else if (sceneName == "House" && (prevSceneName == "Title" || prevSceneName == "House"))
        {
            InGameManager.Instance.CreateGift();
            player.transform.position = new Vector3(3.32f, 1.4f);
            player.LookDown();
        }
    }

    private IEnumerator FadeInOut(float startAlpha, float endAlpha, float fadeInOutDuration)
    {
        if (!TryEnsureFadeImage())
            yield break;

        float elapsed = 0;
        while (elapsed <= fadeInOutDuration)
        {
            if (fadeInOutImage == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeInOutDuration);

            Color imageColor = fadeInOutImage.color;
            imageColor.a = alpha;
            fadeInOutImage.color = imageColor;

            yield return null;
        }
    }

    private bool TryEnsureFadeImage()
    {
        if (fadeInOutImage != null)
            return true;

        fadeInOutImage = GetComponentInChildren<Image>(true);
        if (fadeInOutImage != null)
            return true;

        Image[] images = FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Image image in images)
        {
            if (image != null && image.name.IndexOf("fade", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                fadeInOutImage = image;
                break;
            }
        }

        if (fadeInOutImage == null)
        {
            Debug.LogWarning("SceneLoadManager - fadeInOutImage 없음");
            return false;
        }

        return true;
    }
}
