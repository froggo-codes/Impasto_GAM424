using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    void OnCollisionStay2D(Collision2D collision)
    {
        Health health = collision.gameObject.GetComponent<Health>();
        if (health == null) return;

        bool hitLanded = health.TakeDamage(damage);

        if (hitLanded)
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null)
                player.Knockback(transform.position);
        }
    }
}