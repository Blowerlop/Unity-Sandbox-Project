using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Girod_Nathan.Utilities;



public class Grid : MonoBehaviour
{
    #region Variables

    // All grids relative
    private int[,] _gridArray;
#if UNITY_EDITOR
    private TextMesh[,] _gridText;
#endif
    
    // Try to sf the two dimensional array
    [SerializeField] private GameObject goGameObject;
    private GameObject[,] _gridData;
    
    [SerializeField] private bool _drawGizmos;

    [Header("Grid Relative")] // CHOISIR AVEC GRID NORMAL
    [SerializeField] [Min(1)] private int _width = 7;
    [SerializeField] [Min(1)] private int _depth = 10;
    
    [Header("Cell")]
    [SerializeField] [Min(1)] private Vector3 _size;

    //
    public Vector2 currentSelectedCellIndexes { get; private set; }
    public bool isClickedCellOnGrid { get; private set; }
    
    // 
    private Camera _cam;
    private Vector3 _gridOriginPosition;
    private Plane _plane;

    #endregion


    #region Updates

    private void Start()
    {
        _cam = Camera.main;
        _gridOriginPosition = transform.position;
        _plane = new Plane(Vector3.up, _gridOriginPosition);
        InitializeAllGridRelative();
    }

    #endregion
    
    
    #region Methods
    private void InitializeAllGridRelative()
    {
        // Generate grid array and grid text
        GenerateGrids();
        GenerateDataInGrid();
        GenerateCellsText();
    }

    private void GenerateGrids()
    {
        _gridArray = new int[_width, _depth];
        _gridData = new GameObject[_width, _depth];
        _gridText = new TextMesh[_width, _depth];
    }

    private void GenerateDataInGrid()
    {
        _gridData[0, 1] = goGameObject;
        
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                GameObject cellData = _gridData[x, z];
                if (cellData != null)
                {
                    Instantiate(cellData, GetCellWorldPosition(x, z), Quaternion.identity);
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    private void GenerateCellsText()
    {
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                _gridText[x, z] = Utilities.CreateWorldText($"{x}, {z}", null, 
                    GetCellWorldPosition(x, z) + _size * 0.5f, 30, Color.white,
                    TextAnchor.MiddleCenter, new Vector3(90, 0, 0));
            }
        }
    }
    #endif

    private IEnumerable<Vector3> EvaluateCellsWorldPosition()
    {
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                Vector3 cellPosition = new Vector3(x, 0, z);
                cellPosition = Vector3.Scale(cellPosition, _size) + // Adjust to the grid origin position
                               new Vector3(_gridOriginPosition.x, _gridOriginPosition.y, _gridOriginPosition.z);
                yield return cellPosition;
            }
        }
    }

    public Vector3 GetCurrentSelectedCellWorldPosition()
    {
        return GetCellWorldPosition((int)currentSelectedCellIndexes.x, (int)currentSelectedCellIndexes.y);
        
    }
    
    private Vector3 GetCellWorldPosition(int x, int z)
    {   
        Vector3 cellPosition = new Vector3(x, 0, z);
        cellPosition = Vector3.Scale(cellPosition, _size) + // Adjust to the grid origin position
                       new Vector3(_gridOriginPosition.x, _gridOriginPosition.y, _gridOriginPosition.z);
        return cellPosition;
    }

    public void GetCellIndexes(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt(worldPosition.x / _size.x - _gridOriginPosition.x / _size.x) ;
        z = Mathf.FloorToInt(worldPosition.z / _size.z - _gridOriginPosition.z / _size.z) ;
    }

    private bool IsClickedCellOnGrid()
    {
        return currentSelectedCellIndexes.x >= 0 && currentSelectedCellIndexes.x < _width &&
               currentSelectedCellIndexes.y >= 0 && currentSelectedCellIndexes.y < _depth;
    }

    public void Clicked()
    {
        Ray ray = _cam.ScreenPointToRay(InputManager.instance.mousePosition);
        
        if (_plane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);
            
            GetCellIndexes(mouseWorldPosition, out int x, out int z);
            currentSelectedCellIndexes = new Vector2(x, z);
            isClickedCellOnGrid = IsClickedCellOnGrid();
        }
    }

    private void OnDrawGizmos()
    {
        if (_drawGizmos == false) return;
        
        GenerateGrids();
        
        _gridOriginPosition = transform.position;

        // Draw Gizmos Cells
        foreach (var cellPosition in EvaluateCellsWorldPosition())
        {
            Gizmos.DrawLine(cellPosition, new Vector3(cellPosition.x + _size.x, cellPosition.y, cellPosition.z));
            Gizmos.DrawLine(cellPosition, new Vector3(cellPosition.x, cellPosition.y, cellPosition.z + _size.z));
        }

        // Draw Gizmos top and right borders
        float x, y, z;
        // x origin
        x = _width * _size.z + _gridOriginPosition.x;
        z = _depth * _size.x + _gridOriginPosition.z;
        Gizmos.DrawLine(
            new Vector3(x, _gridOriginPosition.y, _gridOriginPosition.z), 
            new Vector3(x, _gridOriginPosition.y, z)
            );

        // z origin
        Gizmos.DrawLine(
            new Vector3(x, _gridOriginPosition.y, z), 
            new Vector3(_gridOriginPosition.x, _gridOriginPosition.y, z) 
            );
        
        // Move the plane to fit the grid and fix the cells indexes detections
        _plane.SetNormalAndPosition(Vector3.up, new Vector3(0, _gridOriginPosition.y, 0));

        
        // Highlight the cell we are on
        if (Application.isPlaying == false) return;
        if (IsClickedCellOnGrid())
        {
            Vector3 currentCellWorldPosition = GetCurrentSelectedCellWorldPosition();
            x = currentCellWorldPosition.x + (_size.x / 2);
            y = currentCellWorldPosition.y;
            z = currentCellWorldPosition.z + (_size.z / 2);
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(x, y, z) , new Vector3(_size.x, 0.1f, _size.z));
        }
    }
    #endregion
}