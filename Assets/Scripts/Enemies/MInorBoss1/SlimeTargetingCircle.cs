using System.Collections;
using UnityEngine;

// Attach this to the targeting circle prefab (a flat quad/decal/sprite
// lying on the ground, ~1 unit diameter at scale 1). SlimeJumpAttack
// will call Play() with the total time the circle should be visible.
public class SlimeTargetingCircle : MonoBehaviour
{
    [Tooltip("Color when the circle first appears.")]
    public Color startColor = new Color(1f, 1f, 0f, 0.35f);

    [Tooltip("Color right before impact - use this to give a final 'get out' warning.")]
    public Color warningColor = new Color(1f, 0f, 0f, 0.6f);

    private SpriteRenderer spriteRenderer;
    private Renderer rend;
    private MaterialPropertyBlock mpb;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void Play(float duration)
    {
        StartCoroutine(Animate(duration));
    }

    IEnumerator Animate(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Color c = Color.Lerp(startColor, warningColor, t);
            ApplyColor(c);
            yield return null;
        }
    }

    void ApplyColor(Color c)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = c;
            return;
        }

        if (rend != null)
        {
            rend.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", c);
            rend.SetPropertyBlock(mpb);
        }
    }
}
