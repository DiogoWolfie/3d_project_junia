using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Tooltip("If true, keep cursor visible every frame (for Pause/Win screens).")]
    public bool forceCursorVisible = true;

    [Tooltip("If true, confine cursor to game window instead of completely free.")]
    public bool confineCursor = false;

    void Start()
    {
        Cursor.lockState = confineCursor ? CursorLockMode.Confined : CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    void LateUpdate()
    {
        if (forceCursorVisible)
        {
            if (!Cursor.visible)
                Cursor.visible = true;

            if (Cursor.lockState != (confineCursor ? CursorLockMode.Confined : CursorLockMode.None))
                Cursor.lockState = confineCursor ? CursorLockMode.Confined : CursorLockMode.None;
        }
    }
}
