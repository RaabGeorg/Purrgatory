using Unity.Entities;
using UnityEngine;
using Components;
public class VortexAuthoring : MonoBehaviour
{
    public float pullStrength;
    public float radius;
    public float lifetime;
    public float damage;
    
    class Baker : Baker<VortexAuthoring>
    {
        public override void Bake(VortexAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new Vortex
            {
                PullStrength = authoring.pullStrength,
                Radius = authoring.radius,
            });
            AddComponent(entity, new Lifetime
            {
                Value = authoring.lifetime,
            });
            AddComponent(entity, new VortexMovement());
            
            AddComponent(entity, new Explosion
            {
                Radius = authoring.radius,
                Damage = authoring.damage
            });
            
        }
    }
}