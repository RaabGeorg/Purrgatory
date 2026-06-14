using System.Collections;
using UnityEngine;

/// <summary>
/// Laser with a lock-on tracking phase.
///
/// Flow:
///   1. TRACKING  — thin beam follows the player (they can dodge out of range to cancel)
///   2. LOCK      — beam turns solid/bright, position is frozen on where player was
///   3. FIRE      — full damage beam fires at the locked position
///   4. FADE      — beam fades out
///
/// If the player escapes range during TRACKING → attack is cancelled.
/// </summary>
public class EyeLaser : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave empty to auto-find by tag.")]
    public Transform player;
    public string playerTag = "Player";

    [Tooltip("Transform at the pupil — where the laser fires from.")]
    public Transform laserOrigin;

    [Tooltip("LineRenderer for the beam. Auto-fetched if empty.")]
    public LineRenderer laserLine;

    [Header("Range")]
    [Tooltip("Player must be within this distance for the attack to start AND during tracking.")]
    public float shootRange = 12f;

    [Header("Timing")]
    [Tooltip("How long the beam tracks the player before locking on.")]
    public float trackingTime = 2.0f;

    [Tooltip("Brief flash when lock-on is confirmed.")]
    public float lockTime = 0.3f;

    [Tooltip("How long the real damage beam fires.")]
    public float fireDuration = 1.2f;

    [Tooltip("Cooldown between attacks.")]
    public float shootCooldown = 4f;

    [Header("Laser")]
    public float laserDamage = 10f;
    public float laserMaxLength = 25f;
    public LayerMask laserHitMask;

    [Header("Colors")]
    [Tooltip("Tracking beam — thin, dim, follows player.")]
    public Color trackingColor = new Color(1f, 0.8f, 0.2f, 0.45f);

    [Tooltip("Lock-on flash — warns the player the beam is about to fire.")]
    public Color lockColor = new Color(1f, 1f, 1f, 1f);

    [Tooltip("Full damage beam.")]
    public Color fireColor = new Color(1f, 0.05f, 0.05f, 0.5f);

    // ── Private ──────────────────────────────────────────────────
    private float shootTimer;
    private bool isShooting;
    private EyeMovement eyeMovement;

    public bool IsShooting => isShooting;
    
    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
            else Debug.LogWarning("[EyeLaser] No player found with tag: " + playerTag);
        }

        if (laserOrigin == null) laserOrigin = transform;

        if (laserLine == null) laserLine = GetComponent<LineRenderer>();
        if (laserLine != null)
        {
            laserLine.positionCount = 2;
            laserLine.enabled = false;
        }
        else Debug.LogWarning("[EyeLaser] No LineRenderer found!");

        eyeMovement  = GetComponent<EyeMovement>();
        shootTimer   = shootCooldown;
    }

    void Update()
    {
        if (player == null || isShooting) return;

        // Only start attack when player is in range
        bool inRange = Vector3.Distance(transform.position, player.position) <= shootRange;
        if (!inRange) return;

        // Also respect EyeMovement's stop-distance if present
        if (eyeMovement != null && !eyeMovement.IsInShootRange) return;

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f)
        {
            shootTimer = shootCooldown;
            StartCoroutine(LaserSequence());
        }
    }

    // ─── Main Sequence ────────────────────────────────────────────
    IEnumerator LaserSequence()
    {
        if (laserLine == null) yield break;

        
        laserLine.enabled = true;

        // ── Phase 1: TRACKING ─────────────────────────────────────
        // Beam follows the player. If they run out of range → cancel.
        float elapsed = 0f;
        bool escaped = false;

        while (elapsed < trackingTime)
        {
            elapsed += Time.deltaTime;

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist > shootRange)
            {
                escaped = true;
                break;
            }

            // Beam tracks live player position, grows slightly over time
            float t = elapsed / trackingTime;
            SetBeam(player.position, 0.7f, trackingColor);

            yield return null;
        }

        // Player escaped → cancel with a quick fade
        if (escaped)
        {
            yield return FadeOut(0.25f, trackingColor);
            laserLine.enabled = false;
            isShooting = false;
            shootTimer = shootCooldown * 0.5f; // shorter cooldown on cancel
            yield break;
        }

        // ── Phase 2: LOCK-ON ──────────────────────────────────────
        // Freeze the target position at where the player currently is.
        Vector3 lockedTarget = player.position;
        isShooting = true;
        elapsed = 0f;
        while (elapsed < lockTime)
        {
            elapsed += Time.deltaTime;

            // Flash between lockColor and fireColor for visual warning
            Color flashColor = (Mathf.FloorToInt(elapsed / 0.08f) % 2 == 0) ? lockColor : fireColor;
            SetBeam(lockedTarget, 1f, flashColor);

            yield return null;
        }

        // ── Phase 3: FIRE ─────────────────────────────────────────
        // Full damage beam — position is LOCKED, no longer tracks player.
        elapsed = 0f;
        while (elapsed < fireDuration)
        {
            elapsed += Time.deltaTime;

            SetBeam(lockedTarget, 10f, fireColor);
            DealDamage(lockedTarget);

            yield return null;
        }

        // ── Phase 4: FADE OUT ─────────────────────────────────────
        yield return FadeOut(1f, fireColor);

        laserLine.enabled = false;
        isShooting = false;
    }

    // ─── Fade Coroutine ───────────────────────────────────────────
    System.Collections.IEnumerator FadeOut(float duration, Color fromColor)
    {
        float elapsed = 0f;
        // Cache the last beam end so it doesn't jump during fade
        Vector3 lastEnd = laserLine.GetPosition(1);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / duration);
            Color faded = new Color(fromColor.r, fromColor.g, fromColor.b, alpha);
            SetBeam(lastEnd, alpha, faded);
            yield return null;
        }
    }

    // ─── Set Beam Visuals ─────────────────────────────────────────
    void SetBeam(Vector3 targetPos, float widthMultiplier, Color color)
    {
        Vector3 dir = (targetPos - laserOrigin.position).normalized;

        Vector3 endPoint;
        if (Physics.Raycast(laserOrigin.position, dir, out RaycastHit hit, laserMaxLength, laserHitMask))
            endPoint = hit.point;
        else
            endPoint = laserOrigin.position + dir * laserMaxLength;

        laserLine.SetPosition(0, laserOrigin.position);
        laserLine.SetPosition(1, endPoint);

        laserLine.startWidth = 0.15f * widthMultiplier;
        laserLine.endWidth   = 0.15f * widthMultiplier;
        laserLine.startColor = color;
        laserLine.endColor   = color;
    }

    // ─── Damage ───────────────────────────────────────────────────
    void DealDamage(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - laserOrigin.position).normalized;

        if (Physics.Raycast(laserOrigin.position, dir, out RaycastHit hit, laserMaxLength, laserHitMask))
        {
            
        }
    }


}

