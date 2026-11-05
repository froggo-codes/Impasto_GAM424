using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);

    [Header("Level Bounds (optional)")]
    [SerializeField] private Transform levelBoundsMin;  // bottom-left corner of the level
    [SerializeField] private Transform levelBoundsMax;  // top-right corner of the level

    [Header("Boss Arena")]
    [SerializeField] private Transform arenaMin;            // bottom-left corner of the boss arena
    [SerializeField] private Transform arenaMax;            // top-right corner of the boss arena
    [SerializeField] private bool zoomToFitArena = true;    // zoom out so the whole arena is on screen
    [SerializeField] private float zoomSpeed = 3f;          // how fast the zoom changes

    // The bounds the camera is using RIGHT NOW (switches between level and arena)
    private Transform boundsMin;
    private Transform boundsMax;

    private Camera cam;
    private Vector3 velocity;
    private float normalSize;  // the level's zoom, saved so we can return to it
    private float targetSize;  // the zoom we're easing toward

    void Awake()
    {
        cam = GetComponent<Camera>();

        // Start in level view
        normalSize = cam.orthographicSize;
        targetSize = normalSize;
        boundsMin = levelBoundsMin;
        boundsMax = levelBoundsMax;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Ease the zoom toward the target size instead of snapping
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);

        // Where the camera wants to go: the player plus the offset
        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y,
            transform.position.z);

        // Keep the view inside the active bounds
        if (boundsMin != null && boundsMax != null)
        {
            float halfHeight = cam.orthographicSize;    // Size = half the visible height
            float halfWidth = halfHeight * cam.aspect;  // aspect = width ÷ height of the screen

            targetPosition.x = Mathf.Clamp(targetPosition.x, boundsMin.position.x + halfWidth, boundsMax.position.x - halfWidth);
            targetPosition.y = Mathf.Clamp(targetPosition.y, boundsMin.position.y + halfHeight, boundsMax.position.y - halfHeight);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    // Called when the player enters the arena: lock to the arena and zoom to fit it
    public void UseArenaView()
    {
        if (arenaMin == null || arenaMax == null) return;

        boundsMin = arenaMin;
        boundsMax = arenaMax;

        if (zoomToFitArena)
        {
            float arenaWidth = arenaMax.position.x - arenaMin.position.x;
            float arenaHeight = arenaMax.position.y - arenaMin.position.y;

            // Size needed to fit the height, or the width, whichever needs more room
            targetSize = Mathf.Max(arenaHeight / 2f, arenaWidth / 2f / cam.aspect);
        }
    }

    // Called when the player leaves the arena: back to the level's bounds and normal zoom
    public void UseLevelView()
    {
        boundsMin = levelBoundsMin;
        boundsMax = levelBoundsMax;
        targetSize = normalSize;
    }
}