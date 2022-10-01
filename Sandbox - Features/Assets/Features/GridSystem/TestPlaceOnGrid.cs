using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlaceOnGrid : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    private void Update()
    {
        
        if (InputManager.instance.isFiring)
        {
            InputManager.instance.isFiring = false;
            _grid.Clicked();

            if (_grid.isClickedCellOnGrid)
            {

            }
        }
        
    }


    
}
