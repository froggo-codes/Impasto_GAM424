using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour, IInteractable
{
    public string sceneToLoad;
    private bool isPortalUsable;


    //This can be changed for future levels, possibly added on to a player to make it so they can't go back to completed levels but right now its always on. 
    void Start()
    {
        isPortalUsable = true;
    }

    public bool CanInteract()
    {
        return isPortalUsable;
    }

    //Can be used for any future levels and exits
    public void Interact()
    {
        if (isPortalUsable == true)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
