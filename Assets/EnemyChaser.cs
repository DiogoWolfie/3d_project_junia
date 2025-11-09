using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaser : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform target;
    [SerializeField] private float repathInterval = 0.2f;
    private float timer;

    public void Init(Transform player)
    {
        target = player;
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.isStopped = false;
        agent.updateRotation = true;
        SetNow();
        Debug.Log("[EnemyChaser] Init() target=" + target.name);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Fallback: si nadie llamó a Init, intenta encontrar al jugador por Tag
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) Init(p.transform);
            else Debug.LogWarning("[EnemyChaser] No se encontró Player por Tag. Asigna el target en GameManager o pon el tag 'Player' al jugador.");
        }
    }

    private void Update()
    {
        if (target == null || agent == null || !agent.enabled) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SetNow();
            timer = repathInterval;
        }
    }

    private void SetNow()
    {
        if (target == null) return;
        if (!agent.enabled) agent.enabled = true;
        agent.isStopped = false;
        if (!agent.SetDestination(target.position))
            Debug.LogWarning("[EnemyChaser] SetDestination() devolvió false");
    }
}
