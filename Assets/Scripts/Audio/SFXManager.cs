using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Audio Clips")] [SerializeField]
    private AudioMixerGroup sfxGroup;

    private AudioSource sfxSource;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip bulletClip;
    [SerializeField] private AudioClip laserClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip lvlUpClip;
    [SerializeField] private AudioClip hurtClip;

    [Header("Background Music")] [SerializeField]
    private AudioMixerGroup musicGroup;
    private List<AudioSource> activeSources = new List<AudioSource>();

    private AudioSource musicSource;
    [SerializeField] public AudioClip hellBackground;
    [SerializeField] public AudioClip heavenBackground;
    [SerializeField] public AudioClip menuBackground;
    
    
    
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
        sfxSource.volume = 0.1f;
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;
        Instance.PlayMusic(Instance.menuBackground);
    }

    private void OnEnable()
    {
        GameEvents.OnLevelUp += PlayLvlUp;
        GameEvents.OnUpgradeShow += PauseAllSFX;
        GameEvents.OnUpgradeHide += ResumeAllSFX;
    }

    private void OnDisable()
    {
        GameEvents.OnLevelUp -= PlayLvlUp;
        GameEvents.OnUpgradeShow -= PauseAllSFX;
        GameEvents.OnUpgradeHide -= ResumeAllSFX;
    } 

    private void Start()
    {
        
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayExplosion()
    {
        sfxSource.PlayOneShot(explosionClip, 0.2f);
    }

    public void PlayDash()
    {
        sfxSource.PlayOneShot(dashClip, 0.5f);
    }

    public void ShootBullet()
    {
        sfxSource.PlayOneShot(bulletClip, 0.5f);
    }

    public void ShootLaser()
    {
        sfxSource.PlayOneShot(laserClip);
    }
    public void PlayLvlUp(int x)
    {
        sfxSource.PlayOneShot(lvlUpClip, 0.5f);
    }

    public void PlayHurt()
    {
        sfxSource.PlayOneShot(hurtClip, 0.5f);
    }

    public void RegisterSource(AudioSource source)
    {
        activeSources.RemoveAll(s => s == null);
        activeSources.Add(source);
    }
    
    public void PauseAllSFX()
    {
        activeSources.RemoveAll(s => s == null);

        foreach (var source in activeSources)
        {
            if (source.isPlaying)
                source.Pause();
        }
    }

    public void ResumeAllSFX()
    {
        activeSources.RemoveAll(s => s == null);

        foreach (var source in activeSources)
        {
            source.UnPause();
        }
    }
}
