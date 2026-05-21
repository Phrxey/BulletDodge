using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound")]
    public AudioClip playerShoot;
    public AudioClip bossShoot;
    public AudioClip playerHit;
    public AudioClip bossHit;
    public AudioClip slowTimeActivate;
    public AudioClip explosion;
    public AudioClip victory;
    public AudioClip gameOver;

    [Header("BGM")]
    public AudioClip bgm;

    private AudioSource sfxSource;
    private AudioSource bgmSource;
    private AudioSource slowTimeSource;

    void Awake()
    {
        Instance = this;
        sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = 0.5f;
        bgmSource.priority = 0;

        slowTimeSource = gameObject.AddComponent<AudioSource>();
        slowTimeSource.loop = true;
    }

    void Start()
    {
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (bgm == null) return;
        bgmSource.clip = bgm;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void StartSlowTime()
    {
        if (slowTimeActivate == null) return;
        if (slowTimeSource.isPlaying) return;
        slowTimeSource.clip = slowTimeActivate;
        slowTimeSource.Play();

        // 子弹时间激活时背景音乐降pitch，增强效果
        bgmSource.pitch = 0.7f;
    }

    public void StopSlowTime()
    {
        slowTimeSource.Stop();
        bgmSource.pitch = 1f;
    }
}