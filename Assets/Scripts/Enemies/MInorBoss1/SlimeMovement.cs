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

    [Header("Idle Hop")]
    [Tooltip("Set to 0 to disable the hop.")]
    public float hopAmplitude = 0.6f;
    public float hopSpeed = 5f;

    [Tooltip("How many seconds after spawn before we consider the boss settled on the ground.")]
    public float settleDuration = 0.5f;

    [Tooltip("Set to false to freeze movement (used by SlimeJumpAttack during its leap).")]
    [HideInInspector] public bool CanMove = true;

    private Rigidbody rb;
    private float groundY;
    private bool settled = false;
    private float settleTimer = 0f;

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
        float hop = Mathf.Abs(Mathf.Sin(Time.time * hopSpeed)) * hopAmplitude;

        if (dist > stopDistance)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            dir.y = 0f;

            float hopPeak = Mathf.Abs(Mathf.Sin(Time.time * hopSpeed));
            float speedBoost = 1f + hopPeak * 0.6f;
            Vector3 target = transform.position + dir * moveSpeed * speedBoost * Time.deltaTime;
            target.y = groundY + hop;

            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5f);
        }
        else
        {
            Vector3 idle = transform.position;
            idle.y = groundY + hop;
            transform.position = idle;
        }
    }

    public void RefreshGroundY()
    {
        groundY = transform.position.y;
    }
}