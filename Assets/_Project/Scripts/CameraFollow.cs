using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);

    [Header("Bounds (optional)")] 
    [SerializeField] private Transform boundsMin; // bottom-left corner of level
    [SerializeField] private Transform boundsMax; // top-right corner of level

    private Camera cam;
    private Vector3 velocity;


    void Awake()
    {
        cam = GetComponent<Camera>();
    }
    
    void LateUpdate()
    {
        if (target ==null) return;

        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z);
        
        // Keep the view inside the level: stop following before the camera shows past the edges
        if (boundsMin != null && boundsMax != null)
        {
            float halfHeight = cam.orthographicSize;    // Size = half the visible height
            float halfWidth = halfHeight * cam.aspect;  // aspect = width / height of the screen

            targetPosition.x = Mathf.Clamp(targetPosition.x, boundsMin.position.x + halfWidth, boundsMax.position.x - halfWidth);
            targetPosition.y = Mathf.Clamp(targetPosition.y, boundsMin.position.y + halfHeight, boundsMax.position.y - halfHeight);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}