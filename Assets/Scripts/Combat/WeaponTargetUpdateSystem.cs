using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class WeaponTargetUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreen);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float3 mouseWorldPos = float3.zero;

        if (groundPlane.Raycast(ray, out float distance))
            mouseWorldPos = ray.GetPoint(distance);

        // Player Position holen
        float3 playerPos = float3.zero;
        foreach (var t in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerTag>())
        {
            playerPos = t.ValueRO.Position;
            break;
        }

        // Weapon Target → Maus UND Position → Player Position
        foreach (var (target, transform) in SystemAPI.Query<RefRW<WeaponTarget>, RefRW<LocalTransform>>()
                     .WithAll<WeaponTag>()) // ← WeaponTag statt PlayerTag
        {
            target.ValueRW.Value = mouseWorldPos;
            transform.ValueRW.Position = playerPos;
        }

        // Enemy Target → Player
        foreach (var (target, _) in SystemAPI.Query<RefRW<WeaponTarget>, RefRO<Enemy>>())
            target.ValueRW.Value = playerPos;
    }
}