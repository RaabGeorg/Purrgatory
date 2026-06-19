using System.Collections;
using UnityEngine;

public class HitIndicator : MonoBehaviour
{
    [SerializeField] private GameObject hitPanel;
    [SerializeField] private float duration = 0.3f;
    private Coroutine currentRoutine;

    public void Show()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        hitPanel.SetActive(true);

        yield return new WaitForSeconds(duration);

        hitPanel.SetActive(false);
        currentRoutine = null;
    }
}