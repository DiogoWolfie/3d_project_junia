using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField] 
    private GameObject _leftWall;
    [SerializeField] 
    private GameObject _rightWall;
    [SerializeField] 
    private GameObject _frontWall;
    [SerializeField] 
    private GameObject _backWall;
    [SerializeField] 
    private GameObject _unvisitedBlock;

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
