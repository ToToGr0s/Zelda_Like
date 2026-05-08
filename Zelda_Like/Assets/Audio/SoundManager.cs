using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] public AudioClip mainMenuMusic;
    [SerializeField] public AudioClip dungeonMusic;
    [SerializeField] public AudioClip bossMusic;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] public AudioClip coinPickup;
    [SerializeField] public AudioClip playerHit;
    [SerializeField] public AudioClip playerDeath;
    [SerializeField] public AudioClip enemyDeath;
    [SerializeField] public AudioClip swordSwing;
    [SerializeField] public AudioClip arrowShoot;
    [SerializeField] public AudioClip doorOpen;
    [SerializeField] public AudioClip purchase;
    [SerializeField] public AudioClip bossRoar;
    [SerializeField] public AudioClip explosion;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = Mathf.Clamp01(volume);
    }
}
