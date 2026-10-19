using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityTime = 1f;

    [SerializeField] private UnityEvent onDamaged;
    [SerializeField] private UnityEvent onDied;

    private int currentHealth;
    private float invincibilityTimer;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;
    }

    public bool TakeDamage(int amount)
    {
        if (currentHealth <= 0) return false;
        if (invincibilityTimer > 0f) return false;

        currentHealth -= amount;
        invincibilityTimer = invincibilityTime;
        Debug.Log(gameObject.name + " health: " + currentHealth);

        if (currentHealth <= 0)
            onDied.Invoke();
        else
            onDamaged.Invoke();

        return true;

    }
}