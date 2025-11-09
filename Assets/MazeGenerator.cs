using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    // >>> NUEVO: evento para avisar que terminó
    public event Action OnMazeBuilt;

    // >>> NUEVO: getters de solo lectura
    public int Width => _mazeWidth;
    public int Depth => _mazeDepth;
    public float CellSize => _cellSize;

    // ---------- NOVO (minimo necessário) ----------
    // nomes dos prefabs variantes conforme você informou
    private readonly string[] _variantNames = new string[] {
        "Maze Cell dec Variant",
        "Maze Cell Horse Variant",
        "Maze Cell paint Variant",
        "Maze Cell Scream Variant"
    };

    private MazeCell[] _detectedVariants = null; // prefabs detectados automaticamente

    // fracão desejada de variantes (30% conforme pedido)
    private const float TARGET_VARIANT_FRACTION = 0.30f;
    // ------------------------------------------------

    void Start()
    {
        // detecta variantes pelo nome (se existirem) - método seguro no Editor/Play
        AutoDetectVariantsByName();

        _mazeGrid = new MazeCell[_mazeWidth, _mazeDepth];

        int totalCells = _mazeWidth * _mazeDepth;
        int remainingSpecialToPlace = Mathf.CeilToInt(totalCells * TARGET_VARIANT_FRACTION);

        // Para garantir espalhamento aleatório e exatamente ~30%, usamos uma técnica de alocação:
        // a cada célula, com probabilidade = remainingSpecialToPlace / remainingCells, colocamos special.
        int cellsLeft = totalCells;

        //int logged = 0;
        int placedVariants = 0;

        for (int x = 0; x < _mazeWidth; x++)
        {
            for (int z = 0; z < _mazeDepth; z++)
            {
                if (x == 0 && z == 0)
                {
                    _mazeGrid[x, z] = Instantiate(_mazeCellPrefab, new Vector3(x * _cellSize, 0f, z * _cellSize), Quaternion.identity, mazeRoot);
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

    private void AutoDetectVariantsByName()
    {
        
        List<MazeCell> candidates = new List<MazeCell>();

        
        try
        {
            var fromResources = Resources.FindObjectsOfTypeAll<MazeCell>();
            if (fromResources != null && fromResources.Length > 0)
                candidates.AddRange(fromResources);
        }
        catch { }

      
        try
        {
            var fromScene = Object.FindObjectsOfType<MazeCell>();
            if (fromScene != null && fromScene.Length > 0)
                candidates.AddRange(fromScene);
        }
        catch { }

       
        candidates = candidates.Distinct().ToList();

        if (candidates == null || candidates.Count == 0)
        {
            _detectedVariants = null;
            Debug.Log("[MazeGenerator] Auto-detect: nenhum MazeCell encontrado nas fontes (Resources / cena). Verifique se os prefabs têm o componente MazeCell.");
            return;
        }


        Debug.Log($"[MazeGenerator] Auto-detect: {candidates.Count} candidate(s) encontrados.");

       
        System.Func<string, string> norm = s =>
        {
            if (string.IsNullOrEmpty(s)) return "";
            string t = s.ToLowerInvariant();
            t = t.Replace(" ", "").Replace("_", "").Replace("-", "");
            return t;
        };

        var normTargets = _variantNames.Select(n => norm(n)).ToArray();

        List<MazeCell> found = new List<MazeCell>();

        
        foreach (var cand in candidates)
        {
            if (cand == null || string.IsNullOrEmpty(cand.name)) continue;
            string nc = norm(cand.name);

            for (int ti = 0; ti < normTargets.Length; ti++)
            {
                if (string.IsNullOrEmpty(normTargets[ti])) continue;
                if (nc.Contains(normTargets[ti]))
                {
                    if (!found.Contains(cand))
                    {
                        found.Add(cand);
                        Debug.Log($"[MazeGenerator] Auto-detect match: '{cand.name}' matched '{_variantNames[ti]}'");
                    }
                    break;
                }
            }
        }

        
        if (found.Count == 0)
        {
            Debug.Log("[MazeGenerator] Auto-detect: nenhuma correspondência por nome. Fazendo fallback: usando quaisquer MazeCell disponíveis (excluindo o base).");
            foreach (var cand in candidates)
            {
             
                if (cand == _mazeCellPrefab) continue;
                if (!found.Contains(cand)) found.Add(cand);
            }
        }

        if (found.Count == 0)
        {
            Debug.Log("[MazeGenerator] Auto-detect: fallback também não encontrou variações (provavelmente só existe o prefab base).");
            _detectedVariants = null;
            return;
        }

        _detectedVariants = found.ToArray();
        Debug.Log($"[MazeGenerator] Auto-detect: {_detectedVariants.Length} variante(s) registradas.");
        
        foreach (var v in _detectedVariants)
            Debug.Log($"[MazeGenerator] Variant registered: {v.name}");
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
