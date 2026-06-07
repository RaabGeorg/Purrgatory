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

    [Header("Background Music")] [SerializeField]
    private AudioMixerGroup musicGroup;

    private AudioSource musicSource;
    [SerializeField] private AudioClip hellBackground;
    [SerializeField] private AudioClip heavenBackground;
    
    
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
        
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.clip = hellBackground;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void Start()
    {
        
    }

    public void PlayExplosion(float3 pos)
    {
        AudioSource.PlayClipAtPoint(explosionClip, new Vector3(pos.x, pos.y, pos.z));
    }

    public void PlayDash()
    {
        sfxSource.PlayOneShot(dashClip, 0.05f);
    }

    public void ShootBullet()
    {
        sfxSource.PlayOneShot(bulletClip);
    }

    public void ShootLaser()
    {
        sfxSource.PlayOneShot(laserClip);
    }
    public void PlayLvlUp()
    {
        sfxSource.PlayOneShot(lvlUpClip);
    }
}
