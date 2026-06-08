using UnityEngine;
using UnityEngine.InputSystem;

public class LookAt : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (PauseLogic.isPaused) return;
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 lookTarget = ray.GetPoint(distance);
            lookTarget.y = transform.position.y;
            transform.LookAt(lookTarget);
        }
    }
}