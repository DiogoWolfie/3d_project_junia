using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void onPlayButton()
    {
        Debug.Log("Play was pressed");
        // SceneManager.LoadScene("");
    }

    public void onResumeButton()
    {
        Debug.Log("Resume was pressed");
         // SceneManager.LoadScene("");
    }

    public void onQuitButton()
    {
        Debug.Log("Quit was pressed");
        // Application.Quit();
    }
}
