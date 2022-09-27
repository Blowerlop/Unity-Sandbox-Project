using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerGrabObjets : MonoBehaviour
{
    #region Variables

    [Header("Grabbling relative")]
    [Tooltip("The point where you grabbed object will position")]
    [SerializeField] private Transform _grabPoint;
    [SerializeField] private float _grabbingDistance = 2.0f;
    [SerializeField] private LayerMask _grabbableMask;
    private Rigidbody _grabbedTarget;
    
    [Header("Drop Force")]
    [Tooltip("Force that you put on the grabbed object when you drop it")]
    [SerializeField] private float _dropForce = 5.0f;

    // Grabbed Target velocity relative
    private Vector3 _velocityOffset = new Vector3(0.1f, 0.1f, 0.1f);
    private Vector3 _angularVelocityOffset = new Vector3(0.1f, 0.1f, 0.1f);

    [Header("Links")]
    [SerializeField] private Transform _camera;
    [SerializeField] private GameObject _grabUI;
    #endregion


    #region Updates
    private void Start()
    {
        _grabUI.SetActive(false);
    }

    private void Update()
    {
        if (_grabbedTarget == null)
        {
            if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _grabbingDistance, _grabbableMask))
            {
                EnableGrabUI(true);
                if (InputManager.instance.isUsing)
                {
                    Grab(hit);
                }
            }
            else
            {
                EnableGrabUI(false);
            }
        }
        else
        {
            if (InputManager.instance.isUsing)
            {
                Drop();
            }
        }
        InputManager.instance.isUsing = false;       
    }

    private void FixedUpdate()
    {
        if (_grabbedTarget != null)
        {
            MoveGrabbedTarget();
        }  
    }
    #endregion


    #region Methods
    private void Grab(RaycastHit hit)
    {
        _grabbedTarget = hit.transform.GetComponent<Rigidbody>();

        _grabbedTarget.useGravity = false;
        _grabbedTarget.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _grabbedTarget.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Drop()
    {
        _grabbedTarget.useGravity = true;
        _grabbedTarget.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _grabbedTarget.interpolation = RigidbodyInterpolation.None;
        _grabbedTarget.AddForce(_camera.forward * _dropForce, ForceMode.Impulse);

        _grabbedTarget = null;
    }

    private void MoveGrabbedTarget()
    {
        Vector3 targetPosition = Vector3.Lerp(_grabbedTarget.position, _grabPoint.position, Time.fixedDeltaTime * 10);

        Vector3 velocity = _grabbedTarget.velocity;
        Vector3 angularVelocity = _grabbedTarget.angularVelocity;


        if (velocity.normalized != Vector3.zero + _velocityOffset)
        {
            velocity = Vector3.Lerp(_grabbedTarget.velocity, Vector3.zero, Time.deltaTime * 10.0f);
        }
        else
        {
            velocity = Vector3.zero;
        }

        if (angularVelocity.normalized != Vector3.zero + _angularVelocityOffset)
        {
            angularVelocity = Vector3.Lerp(_grabbedTarget.angularVelocity, Vector3.zero, Time.deltaTime * 10.0f);

        }
        else
        {
            angularVelocity = Vector3.zero;
        }


        _grabbedTarget.velocity = velocity;
        _grabbedTarget.angularVelocity = angularVelocity;

        _grabbedTarget.MovePosition(targetPosition);
    }

    private void EnableGrabUI(bool state)
    {
        _grabUI.SetActive(state);
    }
    #endregion
}
