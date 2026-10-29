using UnityEngine;
using UnityEngine.Events;

// Fires an event when the player enters this trigger.
// Reusable: start a boss fight, change music, close a door, etc.
public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] private bool triggerOnce = true; // ignore every entry after the first
    [SerializeField] private UnityEvent onPlayerEntered;

    private bool hasTriggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only react to the player
        if (other.GetComponent<PlayerMovement>() == null) return;
        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;
        onPlayerEntered.Invoke();
    }
}
