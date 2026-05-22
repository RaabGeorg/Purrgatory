using Components;
using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5.0f;
    public uint randomSeed = 1;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            
            uint seed = authoring.randomSeed == 0 ? 1 : authoring.randomSeed;
            
            AddComponent(entity, new EnemySpawner
            {
                Enemy = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                SpawnInterval = authoring.spawnInterval,
                NextSpawnTime = 0.0f,
                Random = new Unity.Mathematics.Random(seed)
            });
        }
    }
}