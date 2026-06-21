using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Components;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class VortexSpawner : MonoBehaviour
{
    private EntityManager _em;
    private PlayerControls _controls;
    private Entity _prefab;
    
    [SerializeField]
    private float cooldown = 2f;
    private float cooldownTimer = 0f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip vortexClip;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Visual")]
    [SerializeField] private GameObject vortexVFXPrefab;

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        _controls = new PlayerControls();
        _controls.Enable();
    }

    void OnDisable() => _controls.Disable();
    void OnDestroy() => _controls.Dispose();

    void Update()
    {
        if (_prefab == Entity.Null)
        {
            var query = _em.CreateEntityQuery(typeof(VortexPrefabRef));

            if (query.IsEmpty) return;

            _prefab = query.GetSingleton<VortexPrefabRef>().Value;
        }
        
        if (SceneSwitchManager.Instance != null &&
            SceneSwitchManager.Instance.CurrentLevel == SceneSwitchManager.Instance.HeavenScene) return;
        
        cooldownTimer -= Time.deltaTime;
        if (!_controls.Player.Spell2.WasPressedThisFrame()) return;
        if (cooldownTimer > 0f) return;
        
        var cam = Camera.main;
        if (cam == null || Mouse.current == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (!ground.Raycast(ray, out float dist)) return;

        float3 spawnPos = ray.GetPoint(dist);
        
        var pull = _em.HasComponent<PullEffect>(_prefab)
            ? _em.GetComponentData<PullEffect>(_prefab) : default;
        var explosion = _em.HasComponent<Explosion>(_prefab)
            ? _em.GetComponentData<Explosion>(_prefab) : default;
        float lifetime = _em.HasComponent<Lifetime>(_prefab)
            ? _em.GetComponentData<Lifetime>(_prefab).Value : 5f;

        var vortex = _em.CreateEntity();
        _em.AddComponentData(vortex, LocalTransform.FromPosition(spawnPos));
        _em.AddComponentData(vortex, pull);
        _em.AddComponentData(vortex, explosion);
        _em.AddComponentData(vortex, new Lifetime { Value = lifetime });
        _em.AddComponentData(vortex, new VortexMovement
        {
            Center = spawnPos,
            RadiusX = UnityEngine.Random.Range(2f, 10f),
            RadiusZ = UnityEngine.Random.Range(2f, 10f),
            Speed = UnityEngine.Random.Range(0.5f, 1f),
            Time = UnityEngine.Random.Range(0f, 100f)
        });
        
        if (vortexVFXPrefab != null)
        {
            var vfx = Instantiate(vortexVFXPrefab, (Vector3)spawnPos, Quaternion.identity);
            vfx.AddComponent<VortexVisualFollow>().Bind(_em, vortex);
            Destroy(vfx, lifetime + 0.5f);
        }

        var go = new GameObject("VortexAudio");
        go.transform.position = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
        var src = go.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = sfxGroup;
        src.clip = vortexClip;
        src.loop = true;
        src.volume = 0.3f;
        src.Play();
        if (SFXManager.Instance != null) SFXManager.Instance.RegisterSource(src);
        Destroy(go, lifetime);
        
        cooldownTimer = cooldown;
    }
}