using StarterAssets;
using UnityEngine;


public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    private StarterAssetsInputs inputs;

    private Collider[] hitColliders;
    private Transform _camTransform;
    private bool drawGizmos = false;

    private Vector3 explosionOrigin; // Del later, only debug

    private void Start()
    {
        _camTransform = Camera.main.transform;
        inputs = GetComponent<StarterAssetsInputs>();
    }

    


    private void FixedUpdate()
    {
        if (inputs.isShooting)
        {
            inputs.isShooting = false;

            if (Physics.Raycast(_camTransform.position, _camTransform.forward, out RaycastHit hit, 999))
            {
                explosionOrigin = hit.point;
                Explosion();


                drawGizmos = true;
            }

            
        }
    }

    private void Explosion()
    {
        hitColliders = Physics.OverlapSphere(explosionOrigin, 2.5f);
        Damage();
    }

    private void Damage()
    {
        foreach (var hitCollider in hitColliders)
        {
            //Vector3 distance = Vector3.dis
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos == false) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(explosionOrigin, 2.5f);

        Gizmos.color = Color.red;
        foreach (var hitCollider in hitColliders)
        {

            Gizmos.DrawLine(explosionOrigin, hitCollider.bounds.center);
        }
    }
}
