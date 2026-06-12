using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate how far we moved since the last frame
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        
        // If we moved more than a tiny amount, we are running
        bool isMoving = distanceMoved > 0.001f;

        animator.SetFloat("Run", isMoving ? 1f : 0f);

        // Update lastPosition for the next frame
        lastPosition = transform.position;
    }
}