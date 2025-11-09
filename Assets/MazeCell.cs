using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject _leftWall;
        [SerializeField] private GameObject _rightWall;
        [SerializeField] private GameObject _frontWall;
        [SerializeField] private GameObject _backWall;
        [SerializeField] private GameObject _unvisitedBlock;

        private void Awake()
        {
            // Asegura obstáculos con carve en cada pared
            AddObstacle(_leftWall);
            AddObstacle(_rightWall);
            AddObstacle(_frontWall);
            AddObstacle(_backWall);
        }

        private void AddObstacle(GameObject wall)
        {
            if (wall == null) return;
            var obs = wall.GetComponent<NavMeshObstacle>();
            if (obs == null) obs = wall.AddComponent<NavMeshObstacle>();
            obs.carving = true;
            obs.shape = NavMeshObstacleShape.Box;

            // Ajusta tamaño/centro con el collider si existe
            var col = wall.GetComponent<Collider>();
            if (col != null)
            {
                // Caja estándar
                if (col is BoxCollider box)
                {
                    obs.size = box.size;
                    obs.center = box.center;
                }
                else
                {
                    // fallback aproximado si no es BoxCollider
                    var r = col.bounds.extents;
                    obs.size = r * 2f;
                    obs.center = wall.transform.InverseTransformPoint(col.bounds.center);
                }
            }
            else
            {
                // sin collider: tamaño por defecto razonable
                obs.size = new Vector3(1f, 2f, 0.2f);
                obs.center = Vector3.zero;
            }
        }

    //booleano para saber se a célula foi visitada
    public bool IsVisited {get; private set;}

    //Método para saber se a célula foi visitada
    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);

    }

    //Métodos para apagar as paredes, criando o labirinto
    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }
}
