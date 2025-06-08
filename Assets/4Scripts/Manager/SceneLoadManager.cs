using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private float fadeInOutDuration = 0.5f;
    [SerializeField] private Image fadeInOutImage;

    public string prevSceneName = string.Empty;
    public bool isSceneLoading = false;

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
                    Debug.Log("씬에 SceneLoadManager 없음");
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
        fadeInOutImage.transform.SetAsLastSibling();
    }

    private IEnumerator LoadScene(string sceneName, bool isGameStart, bool isNextDay)
    {
        isSceneLoading = true;
        prevSceneName = SceneManager.GetActiveScene().name;
        SetBGM(sceneName);

        if (GameManager.Instance != null)
            GameManager.Instance.dayTimeManager.SetTimeStop(true);

        StartCoroutine(FadeInOut(0f, 1f, fadeInOutDuration));
        yield return new WaitForSecondsRealtime(fadeInOutDuration);

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOp.isDone)
            yield return null;

        if (isNextDay)
            StartCoroutine(GameManager.Instance.dayTimeManager.StartNewDay());

        yield return new WaitForSecondsRealtime(fadeInOutDuration);

        DataManager.instance.LoadData();

        if (isGameStart)
        {
            GameManager.Instance.player.InitializePlayerData();
        }

        SetSceneLoadData(sceneName, isNextDay);

        StartCoroutine(FadeInOut(1f, 0f, fadeInOutDuration));

        isSceneLoading = false;
        GameManager.Instance.dayTimeManager.SetTimeStop(false);
        GameManager.Instance.uiManager.InitializeUI();
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
        Player player = GameManager.Instance.player;

        // 다음날 -> 침대에서 시작
        if (isNextDay)
        {
            player.transform.position = new Vector3(3.32f, 1.4f);
            player.LookDown();
        }
        // 집 -> 농장 씬 전환
        else if (sceneName == "Farm" && prevSceneName == "House")
        {
            player.transform.position = Vector3.zero;
            player.LookDown();
        }
        // 농장 -> 집 씬 전환
        else if (sceneName == "House" && prevSceneName == "Farm")
        {
            GameManager.Instance.CreateGift();
            player.transform.position = new Vector3(0.5f, 0f);
            player.LookUp();
        }
        // 이전 씬이 Title, 집이거나
        else if (sceneName == "House" && (prevSceneName == "Title" || prevSceneName == "House"))
        {
            GameManager.Instance.CreateGift();
            player.transform.position = new Vector3(3.32f, 1.4f);
            player.LookDown();
        }
    }

    private IEnumerator FadeInOut(float startAlpha, float endAlpha, float fadeInOutDuration)
    {
        float elapsed = 0;
        while (elapsed <= fadeInOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeInOutDuration);

            Color imageColor = fadeInOutImage.color;
            imageColor.a = alpha;
            fadeInOutImage.color = imageColor;

            yield return null;
        }
    }
}
