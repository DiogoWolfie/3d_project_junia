using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnerNavMesh : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float minDistanceFromPlayer = 8f; // no nacer demasiado cerca
    public int maxTries = 30;

    Transform player;

    void Awake()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
    }

    public GameObject Spawn()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("EnemySpawnerNavMesh: falta Enemy Prefab.");
            return null;
        }

        if (!NavMesh.CalculateTriangulation().vertices?.Length.Equals(null) == true)
        {
            // Nada: solo para evitar warnings en ciertas versiones
        }

        for (int i = 0; i < maxTries; i++)
        {
            if (TryGetRandomPointOnNavMesh(out Vector3 pos))
            {
                if (player == null || Vector3.Distance(pos, player.position) >= minDistanceFromPlayer)
                {
                    var enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);

                    var ai = enemy.GetComponent<EnemyAI>();
                    if (ai && ai.player == null && player != null)
                        ai.player = player;

                    return enemy;
                }
            }
        }

        Debug.LogWarning("EnemySpawnerNavMesh: no se encontró punto válido en el NavMesh.");
        return null;
    }

    bool TryGetRandomPointOnNavMesh(out Vector3 result)
    {
        // Escoge un punto aleatorio usando la triangulación del NavMesh
        var triangulation = NavMesh.CalculateTriangulation();
        if (triangulation.vertices == null || triangulation.indices == null || triangulation.indices.Length < 3)
        {
            result = Vector3.zero;
            return false;
        }

        int tri = Random.Range(0, triangulation.indices.Length / 3) * 3;
        Vector3 v0 = triangulation.vertices[ triangulation.indices[tri] ];
        Vector3 v1 = triangulation.vertices[ triangulation.indices[tri + 1] ];
        Vector3 v2 = triangulation.vertices[ triangulation.indices[tri + 2] ];

        // Coordenadas baricéntricas aleatorias
        float r1 = Random.value;
        float r2 = Random.value;
        float sqrtR1 = Mathf.Sqrt(r1);
        Vector3 point = (1 - sqrtR1) * v0 + (sqrtR1 * (1 - r2)) * v1 + (sqrtR1 * r2) * v2;

        // Ajuste final al NavMesh por seguridad
        if (NavMesh.SamplePosition(point, out var hit, 2f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
