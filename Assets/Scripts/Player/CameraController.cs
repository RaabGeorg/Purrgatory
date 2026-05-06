using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [Header("Camera Settings")]
    [SerializeField] private double cameraAngle    = 65;
    [SerializeField] private double cameraDistance = 10;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed   = 2f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 25f;

    private Vector3 offset;

    void Start()
    {
        UpdateOffset();
    }

    void LateUpdate()
    {
        // Neues Input System → Mouse.current.scroll
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0f)
        {
            cameraDistance -= scroll * zoomSpeed * 0.01f * cameraDistance;
            cameraDistance  = Math.Clamp(cameraDistance, minDistance, maxDistance);
            UpdateOffset();
        }

        transform.position = player.transform.position + offset;
    }

    private void UpdateOffset()
    {
        double cameraHeight = cameraDistance * Math.Sin(cameraAngle * (Math.PI / 180));
        double cameraDepth  = cameraDistance * Math.Cos(cameraAngle * (Math.PI / 180));
        transform.rotation  = Quaternion.Euler((float)cameraAngle, 0, 0);
        offset = new Vector3(0, (float)cameraHeight, -(float)cameraDepth);
    }
}
