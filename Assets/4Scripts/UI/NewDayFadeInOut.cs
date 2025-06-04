using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NewDayFadeInOut : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetNewDay(float fadeInOutDuration, float fadeWaitTime)
    {
        StartCoroutine(NewDay(fadeInOutDuration, fadeWaitTime));
    }

    public IEnumerator NewDay(float fadeInOutDuration, float fadeWaitTime)
    {
        Time.timeScale = 0f;
        StartCoroutine(FadeInOut(0f, 1f, fadeInOutDuration));

        yield return new WaitForSecondsRealtime(fadeInOutDuration);

        DataManager.instance.SaveData();
        DataManager.instance.LoadData();

        yield return new WaitForSecondsRealtime(fadeWaitTime);

        StartCoroutine(FadeInOut(1f, 0f, fadeInOutDuration));
        Time.timeScale = 1f;
    }

    private IEnumerator FadeInOut(float startAlpha, float endAlpha, float fadeInOutDuration)
    {
        float elapsed = 0;
        while (elapsed <= fadeInOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeInOutDuration);

            Color imageColor = image.color;
            imageColor.a = alpha;
            image.color = imageColor;

            yield return null;
        }
    }
}
