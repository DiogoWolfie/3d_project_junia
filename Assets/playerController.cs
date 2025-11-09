using UnityEngine;
using TMPro; 

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    //references

    public TextMeshProUGUI introText; // Opção A: TextMeshPro (recomendado)
    public float introDuration = 4f; 
    private float introTimer = 0f;
    private bool showingIntro = true;
    public Camera playerCamera;
    private CharacterController characterController;
    private AudioSource audioSource;

    //movement
    public float walkSpeed = 1f;
    public float runSpeed = 2f;
    public float gravity = 20f;

    //vision
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    //audiow
    public AudioClip[] footstepClips;    
    public float stepInterval = 0.4f;
    [Range(0.5f, 1.5f)]
    public float stepVolume = 0.2f;
    
    //key
    public bool hasKey = false;

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

        if (introText != null)
        {
            introText.gameObject.SetActive(true);
            introTimer = introDuration;
            showingIntro = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        
        if (showingIntro && introText != null)
        {
            introTimer -= Time.deltaTime;
            if (introTimer <= 0f)
            {
                introText.gameObject.SetActive(false);
                showingIntro = false;
            }
        }
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

        float currentSpeed = Input.GetMouseButton(0) ? runSpeed : walkSpeed;
        stepInterval = Input.GetMouseButton(0) ? 0.3f : 0.5f; 

        Vector3 desiredMove = (forward * inputV + right * inputH).normalized*currentSpeed;
        // if (desiredMove.sqrMagnitude > 1f) desiredMove.Normalize();
        // desiredMove *= walkSpeed;

        
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

    public void OnKeyCollected()
    {
        hasKey = true;
        Debug.Log("Player got the key!");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider == null) return;
        if (!hit.collider.CompareTag("Door")) return;

        DoorBehaviour door = hit.collider.GetComponent<DoorBehaviour>();
        if (door == null) return;

        if (hasKey)
            door.Open();
        else
            Debug.Log("door lock!");
    }
}
