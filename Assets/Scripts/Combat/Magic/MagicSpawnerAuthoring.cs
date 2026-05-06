using UnityEngine;
using Unity.Entities;
using Components;

public class MagicSpawnerAuthoring : MonoBehaviour
{
    public GameObject MagicFieldPrefab;

    class Baker : Baker<MagicSpawnerAuthoring>
    {
        public override void Bake(MagicSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new MagicFieldPrefabRef
            {
                Value = GetEntity(authoring.MagicFieldPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
