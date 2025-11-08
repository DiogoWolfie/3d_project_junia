using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void onPlayButton()
    {
        Debug.Log("Play was pressed");
        SceneManager.LoadScene(1);
    }

    public void onResumeButton()
    {
        Debug.Log("Resume was pressed");

        SceneManager.UnloadSceneAsync(2).completed += _ =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        };

        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void onQuitButton()
    {
        Debug.Log("Quit was pressed");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
