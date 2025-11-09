using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaser : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;

    [Header("Pathfinding")]
    [SerializeField] private float repathInterval = 0.2f;
    private float timer;

    [Header("Growl Settings")]
    [SerializeField] private float growlInterval = 6f; // Cada cuántos segundos gruñe
    private float growlTimer = 0f;
    private AudioSource audioSource;

    public void Init(Transform player)
    {
        target = player;
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        agent.enabled = true;
        agent.isStopped = false;
        agent.updateRotation = true;

        SetNow();
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>(); // IMPORTANTE: pon AudioSource en el objeto raíz
    }

    private void Start()
    {
        // Si GameManager no pasó el player, intentamos detectarlo por tag
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) Init(p.transform);
        }
    }

    private void Update()
    {
        if (target == null || agent == null || !agent.enabled)
            return;

        // Recalcular ruta periódicamente (performance-friendly)
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SetNow();
            timer = repathInterval;
        }

        // Reproducir gruñido cada X segundos
        growlTimer -= Time.deltaTime;
        if (growlTimer <= 0f)
        {
            PlayGrowl();
            growlTimer = growlInterval;
        }

        // Ajustar volumen según distancia al jugador (intensidad dinámica 🌡️)
        UpdateGrowlVolume();
    }

    private void SetNow()
    {
        if (!agent.enabled) return;

        agent.isStopped = false;
        agent.SetDestination(target.position);
    }

    private void PlayGrowl()
    {
        if (audioSource == null) return;
        if (!audioSource.isPlaying) // Evita superposición
            audioSource.Play();
    }

    private void UpdateGrowlVolume()
    {
        if (audioSource == null || target == null) return;

        // Distancia actual al jugador
        float dist = Vector3.Distance(transform.position, target.position);

        // Distancias clave
        float maxDist = 5f; // desde aquí casi no se oye
        float minDist = 1f;  // a esta distancia se oye fuerte

        // Convertir distancia en factor 0..1
        float t = Mathf.InverseLerp(maxDist, minDist, dist);

        // Aplicar volumen suavizado
        audioSource.volume = Mathf.Lerp(0.05f, 1.0f, t);
    }
}
