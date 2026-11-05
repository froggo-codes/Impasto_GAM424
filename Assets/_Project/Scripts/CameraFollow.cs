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

    [Header("Boss Arena")] 
    [SerializeField] private Transform arenaMin;    // bottom-left of boss arena
    [SerializeField] private Transform arenaMax;    // top-right of boss arena
    [SerializeField] private bool zoomToFitArena = true;    // zoom out so the whole arena is on screen
    [SerializeField] private float zoomSpeed = 3f;      // how fast the zoom changes

    private float targetSize;   // the cam size adjusting to
    private Camera cam;
    private Vector3 velocity;


    void Awake()
    {
        cam = GetComponent<Camera>();
        targetSize = cam.orthographicSize;
    }
    
    void LateUpdate()
    {
        if (target ==null) return;
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

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

    public void UseArenaView()
    {
        boundsMin = arenaMin;
        boundsMax = arenaMax;

        if (zoomToFitArena)
        {
            float arenaWidth = arenaMax.position.x - arenaMin.position.x;
            float arenaHeight = arenaMax.position.y - arenaMin.position.y;
            
            // Size needed to fit the height, or the width, whichever needs more room.
            targetSize = Mathf.Max(arenaHeight / 2f, arenaWidth / 2f / cam.aspect);
        }
    }
}