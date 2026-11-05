using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestarter : MonoBehaviour
{
    public void RestartScene()
    {
        PauseController.SetPause(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
