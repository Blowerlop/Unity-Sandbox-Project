using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerGrabObjets : MonoBehaviour
{
    private Transform _camTransform;
    [SerializeField] private LayerMask _grabbableMask;
    [SerializeField] private float _grabbingDistance = 2.0f;
    [SerializeField] private Transform _grabPoint;

    private bool _isTryingToGrab = false;
    private Rigidbody _grabTarget;

    private float _dragForce = 10.0f;
    private float _defaultDragForce;
    private float _angularDragForce = 10.0f;
    private float _defaultAngularDragForce;
    [SerializeField] private float _dropForce = 5.0f;

    private void Start()
    {
        _camTransform = Camera.main.transform;
    }

    private void Update()
    {
        // FAIRE APPARAITRE UI --> GRAB THE OBJECT


        if (_isTryingToGrab)
        {
            if (_grabTarget == null)
            {
                if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, _grabbingDistance, _grabbableMask))
                {
                    Grab(hit);
                }
            }
            else
            {
                Drop();
            }

            _isTryingToGrab = false;
        }

        
    }


    private void FixedUpdate()
    {
        if (_grabTarget != null)
        {
            Vector3 targetPosition = Vector3.Lerp(_grabTarget.position, _grabPoint.position, Time.fixedDeltaTime * 10);
            _grabTarget.MovePosition(targetPosition);

            // A MODIFIER --> CHECK SI BON
            Vector3 velocity = _grabTarget.velocity;
            Vector3 angularVelocity = _grabTarget.angularVelocity;

            Vector3 newObjectVelocity = Vector3.zero;
            Vector3 newObjectAngularVelocity = Vector3.zero;

            if (velocity.normalized != Vector3.zero)
            {
                newObjectVelocity = Vector3.Lerp(_grabTarget.velocity, Vector3.zero, Time.deltaTime * 10.0f);
            }
            if (angularVelocity.normalized != Vector3.zero)
            {
                newObjectAngularVelocity = Vector3.Lerp(_grabTarget.angularVelocity, Vector3.zero, Time.deltaTime * 10.0f);

            }

            _grabTarget.velocity = newObjectVelocity;
            _grabTarget.angularVelocity = newObjectAngularVelocity;

            //
        }
    }



    public void OnGrab(InputValue value)
    {
        _isTryingToGrab = value.isPressed;
    }


    private void Grab(RaycastHit hit)
    {
        _grabTarget = hit.transform.GetComponent<Rigidbody>();
        _grabTarget.useGravity = false;
        //
        
  
        //
        //
        //_defaultDragForce = _grabTarget.drag;
        //_defaultAngularDragForce = _grabTarget.angularDrag;

        //_grabTarget.drag = _angularDragForce;
        //_grabTarget.angularDrag = _angularDragForce;
        //////////////
        _grabTarget.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _grabTarget.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Drop()
    {
        _grabTarget.useGravity = true;
        //
        
        //_grabTarget.drag = _defaultDragForce;
        //_grabTarget.angularDrag = _defaultAngularDragForce;
        //
        _grabTarget.collisionDetectionMode = CollisionDetectionMode.Discrete;
        _grabTarget.interpolation = RigidbodyInterpolation.None;
        _grabTarget.AddForce(_camTransform.forward * _dropForce, ForceMode.Impulse);

        _grabTarget = null;

    }
}
