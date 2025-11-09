using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MazeGenerator maze;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;

    [Header("Spawn")]
    [SerializeField] private int minCellDistanceFromPlayer = 6;
    [SerializeField] private KeyCode bringEnemyKey = KeyCode.B;
    [SerializeField] private float bringDistanceMeters = 6f;

    [Header("Catch Settings")]
    [SerializeField] private float catchRadius = 1.2f;   // tune to model size

    // >>> ESTO ES LO QUE TE FALTABA <<<
    private Transform _spawnedEnemy;
    private bool gameOverFired = false;

    private void Awake()
    {
        maze.OnMazeBuilt += HandleMazeBuilt;
    }

    private void OnDestroy()
    {
        maze.OnMazeBuilt -= HandleMazeBuilt;
    }

    private void HandleMazeBuilt()
    {
        StartCoroutine(SpawnAfterBuilt());
    }

    private IEnumerator SpawnAfterBuilt()
    {
        yield return null; // esperar 1 frame

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Verificar que el NavMesh existe
        var tri = NavMesh.CalculateTriangulation();
        if (tri.vertices == null || tri.vertices.Length == 0)
        {
            Debug.LogError("[GM] No hay NavMesh. Asegúrate de haber hecho Bake sobre el piso.");
            yield break;
        }

        Vector3 spawnPos = PickSpawnOnNavMeshFarFromPlayer();
        Debug.Log("[GM] Spawn elegido: " + spawnPos);

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        _spawnedEnemy = enemy.transform;

        var agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (!agent.enabled) agent.enabled = true;
            agent.Warp(spawnPos);
            agent.isStopped = false;
        }

        var chaser = enemy.GetComponent<EnemyChaser>();
        if (chaser != null) chaser.Init(player);

        AddBeaconLight(_spawnedEnemy);
    }

    private void Update()
    {
        if (_spawnedEnemy && Input.GetKeyDown(bringEnemyKey)) BringEnemyNearPlayer();

        // Lose detection
        if (!gameOverFired && _spawnedEnemy && player)
        {
            float sq = (_spawnedEnemy.position - player.position).sqrMagnitude;
            if (sq <= catchRadius * catchRadius) TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        if (gameOverFired) return;
        gameOverFired = true;

        var agent = _spawnedEnemy.GetComponent<NavMeshAgent>();
        if (agent) { agent.isStopped = true; agent.ResetPath(); }

        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(4);
    }

    // ---- Spawn helpers ----
    private Vector3 PickSpawnOnNavMeshFarFromPlayer()
    {
        Vector3 playerPos = player.position;
        Vector3 corner = FarthestCornerSampled(playerPos);
        if (DistanceInCells(corner, playerPos) >= minCellDistanceFromPlayer)
            return corner;

        Bounds b = GetMazeBounds();
        for (int i = 0; i < 180; i++)
        {
            var rnd = new Vector3(
                Random.Range(b.min.x, b.max.x),
                0f,
                Random.Range(b.min.z, b.max.z)
            );
            if (NavMesh.SamplePosition(rnd, out NavMeshHit hit, maze.CellSize * 3f, NavMesh.AllAreas))
            {
                if (DistanceInCells(hit.position, playerPos) >= minCellDistanceFromPlayer)
                    return hit.position;
            }
        }

        Debug.LogWarning("[GM] No se encontró spawn lejos, usando esquina igualmente.");
        return corner;
    }

    private Vector3 FarthestCornerSampled(Vector3 from)
    {
        Vector3[] corners =
        {
            CellToWorld(0,0),
            CellToWorld(maze.Width-1,0),
            CellToWorld(0,maze.Depth-1),
            CellToWorld(maze.Width-1,maze.Depth-1)
        };

        Vector3 best = from;
        float bestD = -1f;

        foreach (var c in corners)
        {
            if (NavMesh.SamplePosition(c, out NavMeshHit hit, maze.CellSize * 3f, NavMesh.AllAreas))
            {
                float d = (hit.position - from).sqrMagnitude;
                if (d > bestD)
                {
                    bestD = d;
                    best = hit.position;
                }
            }
        }
        return best;
    }

    private int DistanceInCells(Vector3 a, Vector3 b)
    {
        return Mathf.RoundToInt(Vector3.Distance(a, b) / maze.CellSize);
    }

    private Bounds GetMazeBounds()
    {
        float w = (maze.Width - 1) * maze.CellSize;
        float d = (maze.Depth - 1) * maze.CellSize;
        Vector3 min = new Vector3(0, 0, 0);
        Vector3 max = new Vector3(w, 0, d);
        return new Bounds((min + max) * 0.5f, (max - min));
    }

    private Vector3 CellToWorld(int x, int z)
    {
        return new Vector3(x * maze.CellSize, 0f, z * maze.CellSize);
    }

    // ----------- Beacon -----------

    private void AddBeaconLight(Transform t)
    {
        var l = t.gameObject.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = Color.red;
        l.intensity = 8f;
        l.range = 10f;
    }

    // ----------- Bring Enemy (tecla B) -----------

    private void BringEnemyNearPlayer()
    {
        if (_spawnedEnemy == null || player == null) return;

        Vector2 rnd = Random.insideUnitCircle.normalized * bringDistanceMeters;
        Vector3 candidate = new Vector3(player.position.x + rnd.x, 0f, player.position.z + rnd.y);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, bringDistanceMeters * 2f, NavMesh.AllAreas))
        {
            var agent = _spawnedEnemy.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(hit.position);
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
                _spawnedEnemy.position = hit.position;

            Debug.Log("[GM] Enemigo traído cerca a: " + hit.position);
        }
    }
}
