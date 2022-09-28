using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class Grid : MonoBehaviour
{
    #region Variables
    
    private int[,] _gridArray;
    #if UNITY_EDITOR
    private TextMesh[,] _gridText;
    #endif

    private Vector3 _originPosition;

    private Vector2 _currentSelectedCellIndexes;
    private Camera _cam;
    [SerializeField] private LayerMask _gridLayerMask;


    private Plane _plane;

    [SerializeField] private bool _drawGizmos;


    [Header("Grid Relative")]
    [SerializeField] [Min(1)] private int _width = 7;

    [SerializeField] [Min(1)] private int _depth = 10;


    [Header("Cell Relative")]
    [SerializeField] [Min(1)] private Vector3 _size;

    #endregion


    #region Updates

    private void Start()
    {
        _originPosition = transform.position;
        _plane = new Plane(Vector3.up, _originPosition);
        _cam = Camera.main;
        InitializeAllGridRelative();
    }

    void Update()
    {
        // Ray ray = _cam.ScreenPointToRay(InputManager.instance.mousePosition);
        //
        // if (_plane.Raycast(ray, out float distance))
        // {
        //     Vector3 mouseWorldPosition = ray.GetPoint(distance);
        //     
        //     GetCellIndexes(mouseWorldPosition, out int x, out int z);
        //     _currentSelectedCellIndexes = new Vector2(x, z);
        // }
    }

    #endregion
    
    
    #region Methods
    private void InitializeAllGridRelative()
    {
        // Generate grid array and grid text
        GenerateGrids();
        
        GenerateCellsText();
    }

    private void GenerateGrids()
    {
        _gridArray = new int[_width, _depth];
        _gridText = new TextMesh[_width, _depth];
    }
    
    #if UNITY_EDITOR
    private void GenerateCellsText()
    {
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                _gridText[x, z] = CreateWorldText($"{x}, {z}", null, 
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
                cellPosition = Vector3.Scale(cellPosition, _size) + new Vector3(_originPosition.x, _originPosition.y, _originPosition.z);
                yield return cellPosition;
            }
        }
    }

    public Vector3 GetCurrentSelectedCellWorldPosition()
    {
        return GetCellWorldPosition((int)_currentSelectedCellIndexes.x, (int)_currentSelectedCellIndexes.y);
        
    }
    
    private Vector3 GetCellWorldPosition(int x, int z)
    {   
        Vector3 cellPosition = new Vector3(x, 0, z);
        cellPosition = Vector3.Scale(cellPosition, _size) + new Vector3(_originPosition.x, _originPosition.y, _originPosition.z);
        return cellPosition;
    }

    private void GetCellIndexes(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt(worldPosition.x / _size.x - _originPosition.x / _size.x) ;
        z = Mathf.FloorToInt(worldPosition.z / _size.z - _originPosition.z / _size.z) ;
    }

    public bool IsCurrentCellOnGrid()
    {
        CheckCurrentCell();
        
        return _currentSelectedCellIndexes.x >= 0 && _currentSelectedCellIndexes.x < _width &&
               _currentSelectedCellIndexes.y >= 0 && _currentSelectedCellIndexes.y < _depth;
    }

    private void CheckCurrentCell()
    {
        
        Ray ray = _cam.ScreenPointToRay(InputManager.instance.mousePosition);
        
        if (_plane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);
            
            GetCellIndexes(mouseWorldPosition, out int x, out int z);
            _currentSelectedCellIndexes = new Vector2(x, z);
        }
    }

    
    private void OnDrawGizmos()
    {
        if (_drawGizmos == false) return;
        
        GenerateGrids();
        
        _originPosition = transform.position;

        // Draw Gizmos Cells
        foreach (var cellPosition in EvaluateCellsWorldPosition())
        {
            Gizmos.DrawLine(cellPosition, new Vector3(cellPosition.x + _size.x, cellPosition.y, cellPosition.z));
            Gizmos.DrawLine(cellPosition, new Vector3(cellPosition.x, cellPosition.y, cellPosition.z + _size.z));
        }

        float x, y, z;
        // x origin
        x = _width * _size.z + _originPosition.x;
        z = _depth * _size.x + _originPosition.z;
        Gizmos.DrawLine(
            new Vector3(x, _originPosition.y, _originPosition.z), 
            new Vector3(x, _originPosition.y, z)
            );

        // z origin
        Gizmos.DrawLine(
            new Vector3(x, _originPosition.y, z), 
            new Vector3(_originPosition.x, _originPosition.y, z) 
            );
        
        // Move the plane to fit the grid and fix the cells indexes detections
        _plane.SetNormalAndPosition(Vector3.up, new Vector3(0, _originPosition.y, 0));
        
        if (IsCurrentCellOnGrid() && InputManager.instance.isFiring)
        {
            x = GetCurrentSelectedCellWorldPosition().x + (_size.x / 2);
            y = GetCurrentSelectedCellWorldPosition().y;
            z = GetCurrentSelectedCellWorldPosition().z + (_size.z / 2);
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(x, y, z) , new Vector3(_size.x, 0.1f, _size.z));
        }
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    private TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3),
        int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, Vector3 rotation = default(Vector3), 
        TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, rotation, textAlignment,
            sortingOrder);
    }
    
    
    private TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color,
        TextAnchor textAnchor, Vector3 rotation, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject go = new GameObject("World Text", typeof(TextMesh));
        Transform transfo = go.transform;
        transfo.SetParent(parent, false);
        transfo.localPosition = localPosition;
        transfo.rotation = Quaternion.Euler(rotation);
        TextMesh textMesh = go.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
    
    #endregion
}