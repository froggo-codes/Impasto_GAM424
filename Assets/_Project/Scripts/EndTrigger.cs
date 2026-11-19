using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private string _colliderScript;
    [SerializeField] private UnityEvent _collisionEntered;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent(_colliderScript) != null)
        {
            // _collisionEntered.Invoke();
            SceneManager.LoadScene("Credits");
        }
    }
}
