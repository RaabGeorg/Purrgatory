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

    void Update()
    {
        if (!_initialized || !_em.Exists(_playerEntity))
        {
            _initialized = false;
            TryInitialize();
            return;
        }

        _em.SetComponentData(_playerEntity, LocalTransform.FromPosition(transform.position));
        var hp = _em.GetComponentData<Health>(_playerEntity);
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
    }
}