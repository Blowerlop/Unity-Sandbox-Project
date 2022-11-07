using System.Collections.Generic;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Girod_Nathan.Utilities;



public class Grid : MonoBehaviour
{
    #region Variables
    
    [SerializeField] private bool _drawGizmos;

    [Header("Grid Relative")]
    [SerializeField] [Min(1)] private int _width = 7;
    [SerializeField] [Min(1)] private int _depth = 10;
    private int[,] _gridArray;
#if UNITY_EDITOR
    private TextMesh[,] _gridText;
#endif

    [Header("Tile Relative")]
    [SerializeField] private Tile _tileVisual;
    [SerializeField] [Min(1)] private Vector3 _size;
    [Tooltip("The LayerMask that will permit the raycast to touch the tiles")]
    [SerializeField] private LayerMask _tileLayerMask;
    private Tile[] _tiles;


    [Header("Reference")]
    [Tooltip("The camera from which the raycast will start")]
    [SerializeField] private Camera _cam;


    private Vector3 _gridOriginPosition;
    private Plane _plane;

    #endregion


    #region Updates
    private void Start()
    {
        _gridOriginPosition = transform.position;
        _plane = new Plane(Vector3.up, _gridOriginPosition);
        InitializeAllGridRelative();
    }
    #endregion


    #region Methods
    
    private void InitializeAllGridRelative()
    {
        // Generate grid array and grid text
        InitializeArrays();
        GenerateTiles();
        GenerateTilesText();
        SpawnDataInTiles();
    }

    private void InitializeArrays()
    {
        _gridArray = new int[_width, _depth];
        _gridText = new TextMesh[_width, _depth];
        _tiles = new Tile[_width * _depth];
    }
    
    private void GenerateTiles()
    {
        // Create a parent for the tiles
        
        Transform tilesParent = new GameObject("Tiles Visual").transform;
        tilesParent.position = transform.position;

        // Fit the tiles to the size of the grid
        _tileVisual = Instantiate(_tileVisual);
        _tileVisual.transform.localScale = _size;


        // Spawn tiles
        int index = 0; // To add in the _tiles array each tiles
        foreach (var tileWorldPosition in EvaluateEachTilesWorldPosition())
        {
            Tile ins = Instantiate(_tileVisual, tileWorldPosition, Quaternion.identity, tilesParent);
            ins.name = $"Tile {index}";
            ins.worldPosition = tileWorldPosition;
            
            GetTileIndexes(tileWorldPosition, out int x, out int z);
            ins.inGridIndexes = new Vector2(x, z);


            _tiles[index] = ins;
            index++;
        }
        
        // Destroy the tile prefab copy, we just need the reference
        Destroy(_tileVisual.gameObject);
    }
    
    #if UNITY_EDITOR
    private void GenerateTilesText()
    {
        Transform tilesParent = new GameObject("Tiles Text").transform;
        tilesParent.position = transform.position;

        foreach (var tile in _tiles)
        {
            Vector2 indexes = tile.inGridIndexes;
            int x = (int)indexes.x;
            int z = (int)indexes.y;
            
            _gridText[x, z] = Utilities.CreateWorldText($"{x}, {z}", tilesParent, 
                tile.transform.localPosition + _size * 0.5f, 30, Color.white,
                TextAnchor.MiddleCenter, new Vector3(90, 0, 0));
        }
    }
    #endif
    
    private void SpawnDataInTiles()
    {
        foreach (var tile in _tiles)
        {
            foreach (var data in tile.data)
            {
                Instantiate(data, tile.worldPosition, Quaternion.identity);
            }
            
        }
    }

    private IEnumerable<Vector3> EvaluateEachTilesWorldPosition()
    {
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                Vector3 tilePosition = new Vector3(x, 0, z);
                tilePosition = Vector3.Scale(tilePosition, _size) + // Adjust to the grid origin position
                               new Vector3(_gridOriginPosition.x, _gridOriginPosition.y, _gridOriginPosition.z);
                yield return tilePosition;
            }
        }
        
    }

    private void GetTileIndexes(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.FloorToInt(worldPosition.x / _size.x - _gridOriginPosition.x / _size.x) ;
        z = Mathf.FloorToInt(worldPosition.z / _size.z - _gridOriginPosition.z / _size.z) ;
    }
    
    public bool Raycast(out RaycastHit hitInfo)
    {
        // Get the distance between the camera and the grid (by the plane) according to the mouse world position
        Ray ray = _cam.ScreenPointToRay(InputManager.instance.mousePosition);

        float raycastDistance = 0;
        if (_plane.Raycast(ray, out float distance))
        { 
            raycastDistance = distance;
        }

        // Shoot a ray from the camera to the grid according to the ray and the distance between them
        return Physics.Raycast(ray, out hitInfo, raycastDistance, _tileLayerMask);
    }
    
    private void OnDrawGizmos()
    {
        if (_drawGizmos == false) return;
        
        InitializeArrays();
        
        _gridOriginPosition = transform.position;

        // Draw Gizmos Tiles
        foreach (var tilePosition in EvaluateEachTilesWorldPosition())
        {
            Gizmos.DrawLine(tilePosition, new Vector3(tilePosition.x + _size.x, tilePosition.y, tilePosition.z));
            Gizmos.DrawLine(tilePosition, new Vector3(tilePosition.x, tilePosition.y, tilePosition.z + _size.z));
        }

        // Draw Gizmos top and right borders
        // x origin
        float x = _width * _size.x + _gridOriginPosition.x;
        float z = _depth * _size.z + _gridOriginPosition.z;
        Gizmos.DrawLine(
            new Vector3(x, _gridOriginPosition.y, _gridOriginPosition.z), 
            new Vector3(x, _gridOriginPosition.y, z)
            );

        // z origin
        Gizmos.DrawLine(
            new Vector3(x, _gridOriginPosition.y, z), 
            new Vector3(_gridOriginPosition.x, _gridOriginPosition.y, z) 
            );
        
        // Move the plane to fit the grid and fix the tiles indexes detections
        _plane.SetNormalAndPosition(Vector3.up, new Vector3(0, _gridOriginPosition.y, 0));

        
        // Highlight the tile we are on
        if (Application.isPlaying == false) return;
        
        if (Raycast(out RaycastHit hitInfo))
        {
            Vector3 currentTileWorldPosition = hitInfo.transform.GetComponentInParent<Tile>().worldPosition;
            x = currentTileWorldPosition.x + (_size.x / 2);
            float y = currentTileWorldPosition.y;
            z = currentTileWorldPosition.z + (_size.z / 2);
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(new Vector3(x, y, z) , new Vector3(_size.x, 0.1f, _size.z));
        }
            
        
    }
    #endregion
}