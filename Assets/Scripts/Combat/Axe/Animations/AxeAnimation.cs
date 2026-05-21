using UnityEngine;

public class AxeAnimation : MonoBehaviour
{
    private Animator _axeAnimator;

    void Start()
    {
        _axeAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _axeAnimator.SetTrigger("BackhandChop");
        }
    }
}
