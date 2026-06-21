using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Components;
using Unity.VectorGraphics;

public class BossTeleportCheck : MonoBehaviour
{
    [SerializeField] private int requiredSouls = 10;
    [SerializeField] private int requiredCondensedSouls = 0;
    
    [SerializeField] private Transform destinationTarget;


    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (QuestManager.Instance.BossUnlocked)
        {
            if (!other.CompareTag("Player")) return;

            var wallet = PlayerWallet.Instance;
            if (wallet == null) return;

            Teleport(other.transform);
        }
    }

    private void Teleport(Transform playerTransform)
    {
        if (destinationTarget == null) return;

        CharacterController controller = playerTransform.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        playerTransform.position = destinationTarget.position;
        playerTransform.rotation = destinationTarget.rotation;

        if (controller != null)
        {
            controller.enabled = true;
        }
        SceneSwitchManager.inBossRoom = true;
    }
}