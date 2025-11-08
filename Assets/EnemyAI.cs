using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 12f;
    public float chaseSpeed = 4f;
    public float patrolSpeed = 2f;

    private NavMeshAgent agent;
    private Vector3 lastKnownPlayerPos;
    private bool chasing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        agent.speed = patrolSpeed;
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRadius)
        {
            chasing = true;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            lastKnownPlayerPos = player.position;
        }
        else if (chasing)
        {
            agent.SetDestination(lastKnownPlayerPos);

            if (Vector3.Distance(transform.position, lastKnownPlayerPos) < 1f)
            {
                chasing = false;
                agent.speed = patrolSpeed;
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag("Player"))
        {
            Debug.Log("Jugador atrapado!");
        }
    }
}
