using Components;
using Unity.Entities;
using UnityEngine;

public class ChunkAuthoring : MonoBehaviour
{
    public GameObject chunkPrefab;
    public int chunkSize = 80;
    public int viewDistance = 1;

    class Baker : Baker<ChunkAuthoring>
    {
        public override void Bake(ChunkAuthoring authoring)
        {
            DependsOn(authoring.chunkPrefab);

            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ChunkConfigData
            {
                ChunkSize = authoring.chunkSize,
                ViewDistance = authoring.viewDistance
            });

            AddComponent(entity, new ChunkPrefabData
            {
                Prefab = GetEntity(authoring.chunkPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}