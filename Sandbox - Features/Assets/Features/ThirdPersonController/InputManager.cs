using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public void Fire(InputAction.CallbackContext context)
    {
        if (context.performed) { Debug.Log("Fire"); }
    }
}
