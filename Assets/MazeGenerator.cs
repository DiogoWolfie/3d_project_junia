using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell _mazeCellPrefab;
    [SerializeField] private int _mazeWidth;
    [SerializeField] private int _mazeDepth;
    [SerializeField] private float _cellSize = 2f;
    [SerializeField] private Transform mazeRoot;

    private MazeCell[,] _mazeGrid;

    // >>> NUEVO: evento para avisar que terminó
    public event Action OnMazeBuilt;

    // >>> NUEVO: getters de solo lectura
    public int Width => _mazeWidth;
    public int Depth => _mazeDepth;
    public float CellSize => _cellSize;

    void Start()
    {
        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                _mazeGrid[x, z] = Instantiate(
                    _mazeCellPrefab,
                    new Vector3(x * _cellSize, 0f, z * _cellSize),
                    Quaternion.identity,
                    mazeRoot
                );
            }
        }

        // Generación recursiva (como ya haces)
        GenerateMaze(null, _mazeGrid[0, 0]);

        // >>> NUEVO: avisar que terminó
        OnMazeBuilt?.Invoke();
    }

    private void GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        MazeCell nextCell;
        do
        {
            nextCell = GetNextUnvisitedCell(currentCell);
            if (nextCell != null)
                GenerateMaze(currentCell, nextCell);
        } while (nextCell != null);
    }

    private MazeCell GetNextUnvisitedCell(MazeCell currentCell)
    {
        var unvisited = GetUnvisitedCells(currentCell);
        return unvisited.OrderBy(_ => UnityEngine.Random.Range(1, 10)).FirstOrDefault();
    }

    private IEnumerable<MazeCell> GetUnvisitedCells(MazeCell currentCell)
    {
        int x = Mathf.RoundToInt(currentCell.transform.position.x / _cellSize);
        int z = Mathf.RoundToInt(currentCell.transform.position.z / _cellSize);

        if (x + 1 < _mazeWidth && !_mazeGrid[x + 1, z].IsVisited) yield return _mazeGrid[x + 1, z];
        if (x - 1 >= 0      && !_mazeGrid[x - 1, z].IsVisited)     yield return _mazeGrid[x - 1, z];
        if (z + 1 < _mazeDepth && !_mazeGrid[x, z + 1].IsVisited)  yield return _mazeGrid[x, z + 1];
        if (z - 1 >= 0      && !_mazeGrid[x, z - 1].IsVisited)     yield return _mazeGrid[x, z - 1];
    }

    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if (previousCell == null) return;

        if (previousCell.transform.position.x < currentCell.transform.position.x)
        { previousCell.ClearRightWall(); currentCell.ClearLeftWall(); return; }

        if (previousCell.transform.position.x > currentCell.transform.position.x)
        { previousCell.ClearLeftWall(); currentCell.ClearRightWall(); return; }

        if (previousCell.transform.position.z < currentCell.transform.position.z)
        { previousCell.ClearFrontWall(); currentCell.ClearBackWall(); return; }

        if (previousCell.transform.position.z > currentCell.transform.position.z)
        { previousCell.ClearBackWall(); currentCell.ClearFrontWall(); return; }
    }

    // (tu Update vacío se puede quedar)
}
