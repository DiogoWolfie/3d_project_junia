using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    public MazeCell _mazeCellPrefab;

    public MazeCell[] variantEntries;

    [SerializeField]
    public int _mazeWidth;
    
    [SerializeField]
    public int _mazeDepth;
    
    [SerializeField]
    private float _cellSize = 2f;
    
    [SerializeField]
    private Transform mazeRoot;

    private MazeCell[,] _mazeGrid;

    public event Action OnMazeBuilt;

    public int Width => _mazeWidth;
    public int Depth => _mazeDepth;
    public float CellSize => _cellSize;

    private const float TARGET_VARIANT_FRACTION = 0.30f;

    private MazeCell[] _detectedVariants = null;

    void Start()
    {

        if (variantEntries != null && variantEntries.Length > 0)
        {
            _detectedVariants = variantEntries
                .Where(v => v != null && v != _mazeCellPrefab)
                .ToArray();

            Debug.Log($"[MazeGenerator] Using {_detectedVariants.Length} variant(s) from Inspector.");
        }
        else
        {
            _detectedVariants = null;
            Debug.Log("[MazeGenerator] Nenhuma variante passada no Inspector; gerando apenas com o prefab base.");
        }

        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        int totalCells = _mazeWidth * _mazeDepth;
        int remainingSpecialToPlace = Mathf.CeilToInt(totalCells * TARGET_VARIANT_FRACTION);

        
        int cellsLeft = totalCells;

        int placedVariants = 0;

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                if (x == 0 && z == 0)
                {
                    _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x * _cellSize, 0f, z * _cellSize),
                    Quaternion.identity, mazeRoot);
                    cellsLeft--;
                    continue;
                }
                MazeCell chosenPrefab = _mazeCellPrefab; // default

                if (_detectedVariants != null && _detectedVariants.Length > 0 && remainingSpecialToPlace > 0)
                {
                    float p = (float)remainingSpecialToPlace / (float)cellsLeft;
                    if (Random.value <= p)
                    {
                        
                        int idx = Random.Range(0, _detectedVariants.Length);
                        var variant = _detectedVariants[idx];
                        if (variant != null)
                        {
                            chosenPrefab = variant;
                            remainingSpecialToPlace--;
                            placedVariants++;
                        }
                        else
                        {
                            chosenPrefab = _mazeCellPrefab;
                        }
                    }
                }

                _mazeGrid[x, z] = Instantiate(chosenPrefab, new Vector3(x * _cellSize, 0f, z * _cellSize), Quaternion.identity, mazeRoot);

                cellsLeft--;
            }
        }

        Debug.Log($"[MazeGenerator] finished instantiating. Variants placed (approx): {placedVariants} / target {Mathf.CeilToInt(totalCells * TARGET_VARIANT_FRACTION)}");

        GenerateMaze(null, _mazeGrid[0, 0]);

        // >>> NUEVO: avisar que terminó
        OnMazeBuilt?.Invoke();
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

    void StartPlaceholder() { } // placeholder to keep structure identical if desired

    // private void OnValidate()
    // {
        
    //     if (variantEntries != null && variantEntries.Length > 0)
    //     {
    //         variantEntries = variantEntries.Where(v => v != null).ToArray();
    //     }
    // }

    // (tu Update vacío se puede quedar)
}
