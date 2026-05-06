using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public float damage = 10f;
    public float speed  = 15f;

    class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Bullet { Damage = authoring.damage });
            AddComponent(entity, new Speed  { Value  = float3.zero }); // wird vom WeaponJob gesetzt
        }
    }
}