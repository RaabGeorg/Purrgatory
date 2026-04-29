using UnityEngine;
using Unity.Entities;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5.0f;

    class Baker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EnemySpawner
            {
                Enemy = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                SpawnInterval = authoring.spawnInterval,
                NextSpawnTime = 0.0f
            });
        }
    }
}