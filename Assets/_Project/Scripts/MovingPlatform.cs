using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Vector2 nextPosition;       // (CALE) Vector3 -> Vector2 (2d game)
    private Rigidbody2D rb;
    private Collider2D col;

    // How fast the platform moved this physics step. Player reads this value.
    // anyone can READ this value, only this script can change it.
    public Vector2 Velocity { get; private set; }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        nextPosition = pointB.position;
    }

    // CHANGED Update -> FixedUpdate, and we move through the rigidbody instead of the transform
    void FixedUpdate()
    {
        // Where we'll be after this physics step
        Vector2 newPos = Vector2.MoveTowards(rb.position, nextPosition, moveSpeed * Time.fixedDeltaTime);
        
        // Speed = distance/time
        Velocity = (newPos - rb.position) / Time.fixedDeltaTime;
        
        // Tell physics to move us there (so it can carry/push the player correctly)
        rb.MovePosition(newPos);
        
        // reached end? swap to the other point
        if (newPos == nextPosition)
        {
            nextPosition = (nextPosition == (Vector2)pointA.position) ? pointB.position : pointA.position;
        }
    }

    // CHANGED Enter -> Stay, no more parenting (parenting is what caused the bug, at least with how our movement is coded)
    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player == null) return;
        
        // Same top-only check as the falling platform: side or bottom touches don't carry
        float playerFeet = collision.collider.bounds.min.y;
        float platformTop = col.bounds.max.y;
        if (playerFeet < platformTop - 0.1f) return;

        player.SetPlatform(this);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
        if (player == null) return;

        player.ClearPlatform(this);     // CHANGED was transform.parent = null
    }
}
