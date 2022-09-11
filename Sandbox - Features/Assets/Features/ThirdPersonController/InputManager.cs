using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Variables
    public Vector2 move;
    public Vector2 look;
    public bool isJumping = false;
    public bool isSprinting = false;
    public bool isFiring = false;



    // Methods
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        isJumping = context.performed;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        isFiring = context.performed;
    }
}
