using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseKeyListener : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetSceneByBuildIndex(2).isLoaded)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive).completed += _ =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));
        };
 
    }

    void ResumeGame()
    {
        SceneManager.UnloadSceneAsync(2).completed += _ =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        };

    }
}
