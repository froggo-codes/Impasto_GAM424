using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public Animator transition;
    public void OnStartClick()
    {
        //changes scene to museum hub and fades screen to black
        transition.SetTrigger("Start");

        SceneManager.LoadScene("Museum_Hub");
        
    }

    public void OnExitClick()
    {
        //Ends the Scene player when clicked
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        //ends application in builds
        Application.Quit();
    }
}
