using System.Collections;
using UnityEngine;

public enum BGMNAME
{
    None,
    Title,
    InGame
}

public class BGMManager : MonoBehaviour
{
    private AudioSource audioSource;
    private float fadeInOutDuration = 0.5f;

    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip inGameBGM;

    private float currentVolume = 1f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = currentVolume;
        audioSource.Play();
    }

    public void ChangeBGM(BGMNAME bgmName = BGMNAME.None)
    {
        switch (bgmName)
        {
            case BGMNAME.None:
            StartCoroutine(StartFadeInOutBGM());
            audioSource.loop = true;
            break;
            case BGMNAME.Title:
            StartCoroutine(StartFadeInOutBGM(titleBGM));
            audioSource.loop = false;
            break;
            case BGMNAME.InGame:
            StartCoroutine(StartFadeInOutBGM(inGameBGM));
            audioSource.loop = true;
            break;
        }
    }

    private IEnumerator StartFadeInOutBGM(AudioClip newAudioclip = null)
    {
        if (newAudioclip == null)
            StartCoroutine(FadeInOutBGM(currentVolume, currentVolume / 2));
        else
            StartCoroutine(FadeInOutBGM(currentVolume, 0f));

        yield return new WaitForSecondsRealtime(fadeInOutDuration + 0.1f);

        if (newAudioclip == null)
            StartCoroutine(FadeInOutBGM(0.5f, 1f));
        else
        {
            audioSource.clip = newAudioclip;
            audioSource.Play();
            StartCoroutine(FadeInOutBGM(0f, currentVolume));
        }
    }

    private IEnumerator FadeInOutBGM(float originVolume, float targetVolume)
    {
        float elapsed = 0f;
        while (elapsed < fadeInOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(originVolume, targetVolume, elapsed / fadeInOutDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    public void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
        currentVolume = volume;
    }
}