using UnityEngine;

public class SFXManager : MonoBehaviour
{
    private AudioSource playAudioSource;
    private AudioSource playOneShotAudioSource;

    [Header("SFX")]
    [SerializeField] public AudioClip houseFootsteps;
    [SerializeField] public AudioClip farmFootsteps;
    [SerializeField] public AudioClip slimeJump;
    [SerializeField] public AudioClip storeEntrance;
    [SerializeField] public AudioClip purchase;
    [SerializeField] public AudioClip purchaseFail;

    private float currentVolume = 1f;

    private void Awake()
    {
        playAudioSource = GetComponents<AudioSource>()[0];
        playOneShotAudioSource = GetComponents<AudioSource>()[1];

        playAudioSource.playOnAwake = false;
        playAudioSource.loop = true;

        playOneShotAudioSource.playOnAwake = false;
        playOneShotAudioSource.loop = false;
    }

    public void Play(AudioClip clip)
    {
        playAudioSource.clip = clip;
        playAudioSource.Play();
    }

    public void Stop()
    {
        playAudioSource.Stop();
    }

    public bool isPlaying()
    {
        return playAudioSource.isPlaying;
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        playOneShotAudioSource.PlayOneShot(clip, volume * currentVolume);
    }

    public void ChangeVolume(float volume)
    {
        currentVolume = volume;
        playAudioSource.volume = currentVolume;
    }
}
