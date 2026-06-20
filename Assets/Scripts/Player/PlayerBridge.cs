using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Components;

public class PlayerBridge : MonoBehaviour
{
    public static PlayerBridge Instance { get; private set; }

    private Entity _playerEntity;
    private EntityManager _em;
    private EntityQuery _playerQuery;
    private bool _initialized = false;
    private float _lastHealth;
    [SerializeField] private HitIndicator _hitIndicator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        _playerQuery = _em.CreateEntityQuery(typeof(PlayerTag));
        
        TryInitialize();
    }

    void LateUpdate()
    {
        if (!_initialized || !_em.Exists(_playerEntity))
        {
            _initialized = false;
            TryInitialize();
            return;
        }

        var eTransform = _em.GetComponentData<LocalTransform>(_playerEntity);
        
        // 2. Apply it TO the GameObject (The visual shell follows the physics body)
        transform.position = eTransform.Position;

        // Health Sync
        var hp = _em.GetComponentData<Health>(_playerEntity);
        
        if (!Mathf.Approximately(hp.Value, _lastHealth))
        {
            _lastHealth = hp.Value;
            SFXManager.Instance.PlayHurt();
            _hitIndicator.Show();
        }
        _lastHealth = hp.Value;
        
        GameEvents.OnHealthChanged?.Invoke(hp.Value);
    }

    private void TryInitialize()
    {
        if (_playerQuery.IsEmpty) return;

        _playerEntity = _playerQuery.GetSingletonEntity();

        if (!_em.HasComponent<Health>(_playerEntity))
            _em.AddComponent<Health>(_playerEntity);

        _em.SetComponentData(_playerEntity,
            new Health { Value = PlayerStatsManager.Instance.stats.baseHealth.Value });
        
        _lastHealth = PlayerStatsManager.Instance.stats.baseHealth.Value;

        PlayerStatsManager.Instance?.PushToBridge();
        _initialized = true;
    }

    public void ApplyStats(PlayerStatsComponent stats)
    {
        if (!_initialized || !_em.Exists(_playerEntity)) return;
        _em.SetComponentData(_playerEntity, stats);
    }

    public void ApplyHealth(float maxHealth)
    {
        if (!_initialized || !_em.Exists(_playerEntity)) return;
        var hp = _em.GetComponentData<Health>(_playerEntity);
        hp.Value = maxHealth;
        _em.SetComponentData(_playerEntity, hp);
        _lastHealth = hp.Value;
    }
    public Entity GetPlayerEntity() => _playerEntity;
    public EntityManager GetEntityManager() => _em;
    
    
}