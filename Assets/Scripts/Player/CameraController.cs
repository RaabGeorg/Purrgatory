using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] private GameObject player;

    [Header("Camera Settings")]
    [SerializeField] private double cameraAngle = 65;
    [SerializeField] private double cameraDistance = 10;

    private double cameraHeight;
    private double cameraDepth;
    private Vector3 offset;

    void Start()
    {
        cameraHeight = cameraDistance * Math.Sin(cameraAngle * (Math.PI / 180));
        cameraDepth = cameraDistance * Math.Cos(cameraAngle * (Math.PI / 180));
        transform.rotation = Quaternion.Euler((float)cameraAngle, 0, 0);

        offset = new Vector3(0, (float)cameraHeight, -(float)cameraDepth) - player.transform.position;
    }

    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
