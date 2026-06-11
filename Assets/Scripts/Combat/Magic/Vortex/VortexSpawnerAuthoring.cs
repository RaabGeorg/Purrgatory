using Unity.Entities;
using UnityEngine;
using Components;

public class VortexSpawnerAuthoring : MonoBehaviour
{
    public GameObject VortexPrefab;
    
    class Baker : Baker<VortexSpawnerAuthoring>
    {
        public override void Bake(VortexSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new VortexPrefabRef
            {
                Value = GetEntity(authoring.VortexPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}