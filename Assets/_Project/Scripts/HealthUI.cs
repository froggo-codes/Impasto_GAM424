using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;   // drag player in here
    [SerializeField] private Image[] hearts;        // the heart images, in order
    
    void Start()
    {
        Refresh();  //draw the correct number of hearts as soon as level starts
    }
    
    //Public so the health component's event can call it from the inspector
    //Runs every time health changes
    public void Refresh()
    {
        int current = playerHealth.CurrentHealth;   //how much health player has right now
        
        // go through each heart, one at a time
        for (int i = 0; i < hearts.Length; i++)
        {
            //Show this heart only if we have MORE health than its index
            // 5 health -> 0,1,2,3,4 all show. 2 health -> only 0,1
            hearts[i].enabled = i < current;
        }
    }
}
