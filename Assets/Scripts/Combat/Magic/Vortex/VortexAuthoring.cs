using Unity.Entities;
using UnityEngine;
using Components;
public class VortexAuthoring : MonoBehaviour
{
    public float pullStrength;
    public float radius;
    
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
        }
    }
}