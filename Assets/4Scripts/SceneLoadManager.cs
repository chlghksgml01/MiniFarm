using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private float fadeInOutDuration = 0.5f;
    [SerializeField] private Image fadeInOutImage;

    public IEnumerator LoadScene(string sceneName)
    {
        GameManager.Instance.dayTimeManager.canPassToNextDay = false;

        StartCoroutine(FadeInOut(0f, 1f, fadeInOutDuration));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(fadeInOutDuration);
        asyncLoad.allowSceneActivation = true;

        if (sceneName == "Farm")
            GameManager.Instance.player.transform.position = Vector3.zero;
        else if (sceneName == "House")
            GameManager.Instance.player.transform.position = new Vector3(0.5f, 0f, 0f);

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
