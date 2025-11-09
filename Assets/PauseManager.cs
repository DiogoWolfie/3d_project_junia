using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseKeyListener : MonoBehaviour
{
    [Header("Build indices")]
    public int gameSceneIndex = 1;   // SampleScene
    public int pauseSceneIndex = 2;  // PauseScreen

    [Header("Optional")]
    public bool hideGameCameraWhilePaused = false;
    Camera gameCam;

    void Awake() { gameCam = Camera.main; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetSceneByBuildIndex(pauseSceneIndex).isLoaded)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void PauseGame()
    {
        // Load pause scene additively and set it active so its UI is focused
        SceneManager.LoadSceneAsync(pauseSceneIndex, LoadSceneMode.Additive).completed += _ =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(pauseSceneIndex));
        };

        // Freeze gameplay & show cursor
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // (optional) stop rendering maze entirely
        if (hideGameCameraWhilePaused && gameCam) gameCam.enabled = false;
    }

    void ResumeGame()
    {
        SceneManager.UnloadSceneAsync(pauseSceneIndex).completed += _ =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(gameSceneIndex));
        };

        // Unfreeze & relock cursor
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (hideGameCameraWhilePaused && gameCam) gameCam.enabled = true;
    }
}
