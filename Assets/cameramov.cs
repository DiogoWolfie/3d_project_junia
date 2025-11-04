using UnityEngine;

public class MouseLookFPS : MonoBehaviour
{
    public Camera playerCamera; // arraste aqui a Main Camera (filha)
    public float mouseSensitivity = 200f;
    public float pitchMin = -85f;
    public float pitchMax = 85f;

    float yaw;
    float pitch;

    void Start()
    {
        if (playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        yaw = transform.eulerAngles.y;
        pitch = playerCamera.transform.localEulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Aplica yaw só no corpo (eixo Y)
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Aplica pitch só na câmera local (eixo X)
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
