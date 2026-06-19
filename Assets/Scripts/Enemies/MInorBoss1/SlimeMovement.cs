using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlimeMovement : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave empty to auto-find by tag.")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Tooltip("Stops moving once within this distance from the player.")]
    public float stopDistance = 2f;

    [Tooltip("How many seconds after spawn before we consider the boss settled on the ground.")]
    public float settleDuration = 0.5f;

    [HideInInspector] public bool CanMove = true;

    private Rigidbody rb;
    private bool settled = false;
    private float settleTimer = 0f;
    private float groundY;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else Debug.LogWarning("[SlimeMovement] No player found with tag: " + playerTag);
        }

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    void Update()
    {
        if (!settled)
        {
            settleTimer += Time.deltaTime;
            if (settleTimer >= settleDuration)
            {
                groundY = transform.position.y;
                rb.isKinematic = true;
                settled = true;
            }
            return;
        }

        if (player == null || !CanMove) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= stopDistance) return;

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;

        Vector3 pos = transform.position + dir * moveSpeed * Time.deltaTime;
        pos.y = groundY;
        transform.position = pos;
    }

    public void RefreshGroundY()
    {
        groundY = transform.position.y;
    }
}