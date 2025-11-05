using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    //references
    public Camera playerCamera;
    private CharacterController characterController;
    private AudioSource audioSource;

    //movement
    public float walkSpeed = 4f;
    public float gravity = 20f;

    //vision
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    //audiow
    public AudioClip[] footstepClips;    
    public float stepInterval = 0.45f;  
    [Range(0.5f, 1.5f)]
    public float stepVolume = 1f;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0f;
    float stepTimer = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; 
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;

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
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        float mx = Input.GetAxis("Mouse X") * lookSpeed;
        float my = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX += -my;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        transform.Rotate(0f, mx, 0f);
    }

    void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float inputV = Input.GetAxis("Vertical");
        float inputH = Input.GetAxis("Horizontal");

        Vector3 desiredMove = (forward * inputV + right * inputH);
        if (desiredMove.sqrMagnitude > 1f) desiredMove.Normalize();
        desiredMove *= walkSpeed;

        
        float y = moveDirection.y;
        if (characterController.isGrounded)
        {
            y = -1f;
        }
        else
        {
            y -= gravity * Time.deltaTime;
        }

        moveDirection = new Vector3(desiredMove.x, y, desiredMove.z);
        characterController.Move(moveDirection * Time.deltaTime);

        // --- passos ---
        stepTimer -= Time.deltaTime;

        // considera que o jogador está "andando" se houver input significativo e estiver no chão
        bool isMoving = characterController.isGrounded && (Mathf.Abs(inputV) > 0.1f || Mathf.Abs(inputH) > 0.1f);

        if (isMoving && stepTimer <= 0f)
        {
            PlayFootstep();
            stepTimer = stepInterval;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;

        int idx = Random.Range(0, footstepClips.Length);
        AudioClip clip = footstepClips[idx];

        // PlayOneShot para permitir sobreposição (caso tenha mais de um som tocando)
        audioSource.PlayOneShot(clip, stepVolume);
    }
}
