using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private float fadeInOutDuration = 0.5f;
    [SerializeField] private Image fadeInOutImage;

    public event Action SceneLoad = null;

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

        DontDestroyOnLoad(this);
    }

    public void StartLoadScene(string sceneName, bool isGameStart)
    {
        StartCoroutine(LoadScene(sceneName, isGameStart));
        fadeInOutImage.transform.SetAsLastSibling();
    }

    public IEnumerator LoadScene(string sceneName, bool isGameStart)
    {
        if (!isGameStart)
            GameManager.Instance.dayTimeManager.canPassToNextDay = false;

        StartCoroutine(FadeInOut(0f, 1f, fadeInOutDuration));
        yield return new WaitForSecondsRealtime(fadeInOutDuration);

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOp.isDone)
            yield return null;

        yield return new WaitForSecondsRealtime(fadeInOutDuration);

        if (isGameStart)
        {
            DataManager.instance.LoadData();
            GameObject bed = GameObject.Find("BedNextDayCollider");
            bed.GetComponent<Bed>().isPassDay = true;
        }

        SceneLoad?.Invoke();

        Player player = GameManager.Instance.player;
        if (sceneName == "Farm" && !isGameStart)
        {
            player.transform.position = Vector3.zero;
            player.LookDown();
        }
        else if (sceneName == "House" && isGameStart)
        {
            GameManager.Instance.CreateGift();
            player.transform.position = new Vector3(3.32f, 1.4f);
            player.LookDown();
        }
        else if (sceneName == "House")
        {
            GameManager.Instance.CreateGift();
            player.transform.position = new Vector3(0.5f, 0f, 0f);
            player.LookUp();
        }

        StartCoroutine(FadeInOut(1f, 0f, fadeInOutDuration));

        GameManager.Instance.dayTimeManager.canPassToNextDay = true;
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
