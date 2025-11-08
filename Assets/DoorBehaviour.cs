using System;
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{    
    public void Open()
    {
        Debug.Log("DoorBehaviour: Open() called -> porta aberta (destroy).");
        Destroy(gameObject);
    }
}

