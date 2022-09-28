using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Variables
    public static InputManager instance;

    public void Start()
    {
        instance = this;
    }

    public Vector2 move;
    public Vector2 look;
    public bool isJumping = false;
    public bool isSprinting = false;
    public bool isFiring = false;
    public bool isUsing = false;
    public Vector3 mousePosition;



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
        isJumping = context.started;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        isFiring = context.performed;
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        isUsing = context.performed;
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        Vector2 position2D = context.ReadValue<Vector2>();
        mousePosition = new Vector3(position2D.x, position2D.y, 0);
    }
}
