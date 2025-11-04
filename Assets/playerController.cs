using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMoveFPS_Flattened : MonoBehaviour
{
    public float speed = 4f;
    public Transform optionalCameraForForward = null; // opcional: usar a forward da câmera (flattened) ao invés do corpo
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward;
        Vector3 right;

        if (optionalCameraForForward != null)
        {
            // Usa a direção da câmera, mas "achatada" no plano XZ
            forward = optionalCameraForForward.forward;
            forward.y = 0f;
            forward.Normalize();

            right = optionalCameraForForward.right;
            right.y = 0f;
            right.Normalize();
        }
        else
        {
            // Usa a direção do corpo (player), já sem componente Y
            forward = transform.forward;
            forward.y = 0f;
            forward.Normalize();

            right = transform.right;
            right.y = 0f;
            right.Normalize();
        }

        Vector3 dir = forward * v + right * h;
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        // aplica velocidade horizontal mantendo a velocidade Y (gravidade/salto)
        Vector3 newVel = new Vector3(dir.x * speed, rb.velocity.y, dir.z * speed);
        rb.velocity = newVel;
    }
}
