using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player == null) return;

        Health health = other.GetComponent<Health>();
        if (health != null)
            health.TakeDamage(1);

        player.Respawn();
    }
}