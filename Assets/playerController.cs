using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Referências")]
    public Camera playerCamera; // arraste a Main Camera aqui (filha do Player)

    [Header("Movimento")]
    public float walkSpeed = 4f;
    public float gravity = 20f;

    [Header("Visão")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0f;
    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        // inicializa rotationX corretamente mesmo que localEulerAngles esteja em 0..360
        if (playerCamera != null)
        {
            float initialPitch = playerCamera.transform.localEulerAngles.x;
            if (initialPitch > 180f) initialPitch -= 360f;
            rotationX = initialPitch;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Mouse look (pitch na câmera, yaw no corpo) ---
        float mx = Input.GetAxis("Mouse X") * lookSpeed;
        float my = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX += -my;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        transform.Rotate(0f, mx, 0f);

        // --- Movimento plano XZ (sem pulo) ---
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float inputV = Input.GetAxis("Vertical");   // W/S
        float inputH = Input.GetAxis("Horizontal"); // A/D

        Vector3 desiredMove = (forward * inputV + right * inputH).normalized * walkSpeed;

        // preserva Y para gravidade
        float y = moveDirection.y;

        if (characterController.isGrounded)
        {
            // mantém um pequeno valor negativo para "grudar" no chão
            y = -1f;
        }
        else
        {
            y -= gravity * Time.deltaTime;
        }

        moveDirection = new Vector3(desiredMove.x, y, desiredMove.z);

        characterController.Move(moveDirection * Time.deltaTime);
    }
}
