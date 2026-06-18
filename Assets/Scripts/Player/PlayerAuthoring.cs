using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{

    public float InvincibilityDuration = 0.5f;
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new PlayerTag());
            AddComponent(entity, new Health());
            AddComponent<PlayerStatsComponent>(entity);
            AddComponent(entity, new InvincibilityData
            {
                CurrentTime = 0f,
                MaxTime = authoring.InvincibilityDuration,
            });
        }
    }
}
