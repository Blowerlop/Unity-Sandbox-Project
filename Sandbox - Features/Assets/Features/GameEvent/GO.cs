using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GO : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            gameEvent?.Invoke();
        }

    }
}
