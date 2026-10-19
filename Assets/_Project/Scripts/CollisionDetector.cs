using UnityEngine;
using UnityEngine.Events;


public class CollisionDetector : MonoBehaviour
{
    [SerializeField]
    private string _colliderScript;
    
    [SerializeField]
    private UnityEvent _collisionEntered;

    [SerializeField]
    private UnityEvent _collisionExited; 
    public QuestUI questUI;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent(_colliderScript) != null)
        {
            _collisionEntered.Invoke();
            questUI.currentQuests[0].objectives[0].currentAmount++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent(_colliderScript) != null)
        {
            _collisionExited.Invoke();
        }
    }
}
