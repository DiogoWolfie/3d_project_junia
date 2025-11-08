using System.Collections;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation; // NavMeshSurface

public class BuildNavMeshAfterMaze : MonoBehaviour
{
    public NavMeshSurface surface;
    public EnemySpawnerNavMesh enemySpawner; // usa el spawner de arriba

    void Start()
    {
        StartCoroutine(WaitAndBuild());
    }

    IEnumerator WaitAndBuild()
    {
        // Espera a que el laberinto termine de generarse:
        yield return new WaitUntil(() =>
        {
            var cells = FindObjectsOfType<MazeCell>();
            return cells.Length > 0 && cells.All(c => c.IsVisited);
        });

        // Construye el NavMesh con el laberinto ya creado
        surface.BuildNavMesh();

        // Cuando el NavMesh esté listo, spawnea al enemigo
        if (enemySpawner) enemySpawner.Spawn();
    }
}
