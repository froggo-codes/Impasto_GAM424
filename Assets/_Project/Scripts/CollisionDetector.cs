using UnityEngine;
using UnityEngine.Events;

//https://www.youtube.com/watch?v=1IJmenD1HOk
//Detects collision with quest objects. 
//For now this only works with a quest that has a single objective. Will need to update with another loop if we want to expand.

public class CollisionDetector : MonoBehaviour
{
    [SerializeField]
    private string _colliderScript;
    
    [SerializeField]
    private UnityEvent _collisionEntered;

    [SerializeField]
    private UnityEvent _collisionExited; 
    public QuestUI questUI; 
    public Item item; 


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent(_colliderScript) != null)
        {
            _collisionEntered.Invoke(); 
            if(item.ID == "2")
            {
                questUI.currentQuests[0].objectives[0].currentAmount++; 
            } 
            if(item.ID == "3") 
            {
                questUI.currentQuests[0].objectives[1].currentAmount++; 
            } 
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
