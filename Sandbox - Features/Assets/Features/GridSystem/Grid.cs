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
    private TextMesh[,] _gridText;

    private Vector3 _originPosition;
    private Vector2 _currentSelectedCellIndexes;
    private Vector3 _currentSelectedCellWorldPosition 
        => GetCellWorldPosition((int)_currentSelectedCellIndexes.x, (int)_currentSelectedCellIndexes.y);
    
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
        
        InitializeAllGridRelative();
    }

    [SerializeField] private LayerMask _gridLayerMask;
    RaycastHit hit;
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.mousePosition);
        
        if (_plane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);
            
            GetCellIndexes(mouseWorldPosition, out int x, out int z);
            
            if (x >= 0 && x < _width &&
                z >= 0 && z < _depth)
            {
                _currentSelectedCellIndexes = new Vector2(x, z);
            }
            
            // _currentSelectedCellIndexes = new Vector2(x, z);

        }
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
    
    private Vector3 GetCellWorldPosition(int x, int z)
    {   
        Vector3 cellPosition = new Vector3(x, 0, z);
        cellPosition = Vector3.Scale(cellPosition, _size) + new Vector3(_originPosition.x, _originPosition.y, _originPosition.z);
        return cellPosition;
    }
    
    private Vector3 GetCellWorldPosition(Vector3 mouseWorldPosition)
    {
        GetCellIndexes(mouseWorldPosition, out int x, out int z);
        return GetCellWorldPosition(x, z);
    }
    

    /*
    private void SetText(int x, int z, string text)
    {
        _gridText[x, z].text = text;
    }
    */

    private void GetCellIndexes(Vector3 worldPosition, out int x, out int z)
    {
        // private delegate int FloorToInt(float f);
        //FloorToInt ezafaz = new FloorToInt(Mathf.FloorToInt);
        // Maybe use a delegate instead of multiple functions call in a once
        
        // Need to make it work with a transform deplacement too
        
        x = Mathf.FloorToInt(worldPosition.x / _size.x ) - Mathf.FloorToInt(_originPosition.x / _size.x);
        z = Mathf.FloorToInt(worldPosition.z / _size.z ) - Mathf.FloorToInt(_originPosition.z / _size.z);
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

        x = _currentSelectedCellWorldPosition.x + (_size.x / 2);
        y = _currentSelectedCellWorldPosition.y;
        z = _currentSelectedCellWorldPosition.z + (_size.z / 2);
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(new Vector3(x, y, z) , new Vector3(_size.x, 0.1f, _size.z));
    }

    
    
    
    
    
    
    
    
    
    
    
    
    
    
    public TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3),
        int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, Vector3 rotation = default(Vector3), 
        TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, rotation, textAlignment,
            sortingOrder);
    }
    
    
    public TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color,
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