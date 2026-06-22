// using Components;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
//
// public class WeaponBridge : MonoBehaviour
// {
//     // Dein Player Transform
//     public Transform playerTransform;
//
//     private EntityManager _entityManager;
//     private Entity _weaponEntity;
//
//     void Start()
//     {
//         _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
//
//         // Weapon Entity per Query finden
//         var query = _entityManager.CreateEntityQuery(
//             ComponentType.ReadOnly<Weapon>(),
//             ComponentType.ReadOnly<PlayerTag>()
//         );
//
//         if (query.CalculateEntityCount() > 0)
//             _weaponEntity = query.GetSingletonEntity();
//     }
//
//     void Update()
//     {
//         if (_weaponEntity == Entity.Null) return;
//
//         // Weapon Entity Position = Player Position
//         var transform = _entityManager.GetComponentData<LocalTransform>(_weaponEntity);
//         transform.Position = playerTransform.position;
//         _entityManager.SetComponentData(_weaponEntity, transform);
//     }
// }
