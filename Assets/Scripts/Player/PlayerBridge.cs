using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Components;

public class PlayerBridge : MonoBehaviour
{
    public static PlayerBridge Instance { get; private set; }
    
    private Entity _playerEntity;
    private EntityManager _em;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        _playerEntity = _em.CreateEntity(
            typeof(LocalTransform),
            typeof(PlayerTag),
            typeof(PlayerStatsComponent),
            typeof(Health));
        _em.SetComponentData(_playerEntity, new Health
        {
            Value = PlayerStatsManager.Instance.stats.baseHealth.Value
        });
        PlayerStatsManager.Instance?.PushToBridge();
    }

    void Update()
    {
        _em.SetComponentData(_playerEntity,
            LocalTransform.FromPosition(transform.position));
        var hp = _em.GetComponentData<Health>(_playerEntity);
        GameEvents.OnHealthChanged?.Invoke(hp.Value);
    }
    public void ApplyStats(PlayerStatsComponent stats) =>
        _em.SetComponentData(_playerEntity, stats);

    public void ApplyHealth(float maxHealth) 
    {
        var hp = _em.GetComponentData<Health>(_playerEntity);
        hp.Value = maxHealth;
        _em.SetComponentData(_playerEntity, hp);
    }
        
}