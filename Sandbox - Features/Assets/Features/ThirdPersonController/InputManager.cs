using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Variables
    public Vector2 move;
    public Vector2 look;
    public bool isSprinting = false;
    public bool isJumping = false;



    // Methods
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
        
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed) { Debug.Log("Fire"); }
    }
}
