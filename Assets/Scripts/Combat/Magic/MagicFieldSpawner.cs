using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Components;
public class MagicFieldSpawner : MonoBehaviour
{
    private EntityManager _em;
    private PlayerControls _controls;
    private Entity _prefab;

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = _em.CreateEntityQuery(typeof(MagicFieldPrefabRef));
        _prefab = query.GetSingleton<MagicFieldPrefabRef>().Value;

        _controls = new PlayerControls();
        _controls.Enable();
    }

    void OnDestroy() => _controls.Disable();

    void Update()
    {
        if (!_controls.Player.Spell.WasPressedThisFrame()) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        if (!ground.Raycast(ray, out float dist)) return;

        float3 spawnPos = ray.GetPoint(dist);

        var field = _em.Instantiate(_prefab);
        _em.SetComponentData(field, LocalTransform.FromPosition(spawnPos));
    }
}