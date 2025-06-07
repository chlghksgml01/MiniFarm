using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    private void Update()
    {
        if (audioSource.isPlaying)
            Debug.Log("SFX ¿Áª˝¡ﬂ");
        else
            Debug.Log("SFX ∏ÿ√„");
    }

    public void Play(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public bool isPlaying()
    {
        return audioSource.isPlaying;
    }

    public void PlayOneShot(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
