using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallWait = 2f;
    public float respawnTime = 1f;
    
    Vector2 defaultPos;
    private Quaternion defaultRot;  // (CALE) remembers the starting rotation so we can restore it

    bool isFalling;
    Rigidbody2D rb;
    Collider2D col;     // (CALE) the platform's own collider, used to find top edge
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultPos = transform.position;
        defaultRot = transform.rotation;        // (CALE) save the starting rotation
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();       // (CALE) grab the collider once
    }

    // Runs once when something first touches the platform
    // (CALE) had to change this one a bit, will explain why on Discord
    private void OnCollisionEnter2D(Collision2D collision)
    {   
        // ignore the hit if we're already falling or it isn't the player
        if (isFalling || !collision.gameObject.CompareTag("Player")) return;
        // (CALE) basically flipped the if statement inside out so that we can check for landed on top
        
        // Only fall if the player landed on TOP (their feet are at or above the top edge)
        float playerFeet = collision.collider.bounds.min.y;     // bottom of the player's collider
        float platformTop = col.bounds.max.y;                   // top of the platform
        if (playerFeet < platformTop - 0.1f) return;            // feet below the top = hit from below/side -> ignore

        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        isFalling = true;
        yield return new WaitForSeconds(fallWait);
        rb.bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(respawnTime);
        ResetPlatform();
    }

    private void ResetPlatform()
    {
        isFalling = false;
        rb.bodyType = RigidbodyType2D.Static;
        transform.position = defaultPos;
        transform.rotation = defaultRot;        // (CALE) undo any tipping from the fall - (bug safeguard)
    }
}
