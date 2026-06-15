using System.Collections;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [Header("Death")]
    [Tooltip("How long the slime takes to shrink away after dying.")]
    public float deathDuration = 1f;

    private bool isDying = false;

    public void Die()
    {
        if (isDying) return;
        isDying = true;

        // Clean up any active jump attack and its targeting circle.
        var jumpAttack = GetComponent<SlimeJumpAttack>();
        if (jumpAttack != null) jumpAttack.CancelAttack();

        StartCoroutine(ShrinkAndDestroy());
    }

    IEnumerator ShrinkAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < deathDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / deathDuration);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}