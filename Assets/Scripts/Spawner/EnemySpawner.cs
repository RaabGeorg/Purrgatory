using UnityEngine;
using Unity.Entities;

public struct EnemySpawner : IComponentData
{
    public Entity Enemy;
    public float SpawnInterval;
    public float NextSpawnTime;
}