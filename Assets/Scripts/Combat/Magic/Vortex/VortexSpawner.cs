using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Components;
using UnityEngine.Audio;

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
        
        
        cooldownTimer -= Time.deltaTime;
        if (!_controls.Player.Spell2.WasPressedThisFrame()) return;
        if (cooldownTimer > 0f) return;
        
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (!ground.Raycast(ray, out float dist)) return;

        float3 spawnPos = ray.GetPoint(dist);
        var Vortex = _em.Instantiate(_prefab);
        _em.SetComponentData(Vortex, LocalTransform.FromPosition(spawnPos)); 
        _em.SetComponentData(Vortex, new VortexMovement
        {
            Center = spawnPos,
            RadiusX = UnityEngine.Random.Range(2f, 10f),
            RadiusZ = UnityEngine.Random.Range(2f, 10f),
            Speed = UnityEngine.Random.Range(0.5f, 1f),
            Time = UnityEngine.Random.Range(0f, 100f)
        });
        
        float lifetime = _em.GetComponentData<Lifetime>(Vortex).Value;
        var go = new GameObject("VortexAudio");
        go.transform.position = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
        var src = go.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = sfxGroup;
        src.clip = vortexClip;
        src.loop = true;
        src.volume = 0.05f;
        src.Play();
        Destroy(go, lifetime);
        
        cooldownTimer = cooldown;
    }
}