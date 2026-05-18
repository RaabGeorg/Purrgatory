using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct EnemySpawner : IComponentData
{
    public Entity Enemy;
    public float SpawnInterval;
    public float NextSpawnTime;
    public Unity.Mathematics.Random Random;
}