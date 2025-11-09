using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorBehaviour : MonoBehaviour
{    
    public void Open()
    {
        Debug.Log("DoorBehaviour: Open() called -> porta aberta (destroy).");
        SceneManager.LoadScene(3);
        Destroy(gameObject);
    }
}

