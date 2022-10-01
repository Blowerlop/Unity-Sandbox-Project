using UnityEngine;

public class TestPlaceOnGrid : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _objectToPlaceTest;
    
    private void Update()
    {
        
        if (InputManager.instance.isFiring)
        {
            InputManager.instance.isFiring = false;
            
            if (_grid.Raycast(out RaycastHit hitInfo))
            {
                Debug.Log("Hit");
            }
        }
        
    }


    
}
