using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartClick()
    {
        //changes scene to museum hub
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
