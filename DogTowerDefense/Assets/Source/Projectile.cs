using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /*******************
     * Unity Functions *
     *******************/
    void Start()
    {
        // Move this to update for seeking
        //direction = targetTransform.position - transform.position;
    }

    void Update()
    {
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        direction = targetTransform.position - transform.position;
        float deltaDistance = TravelSpeed * Time.deltaTime;

        //if (direction.magnitude <= deltaDistance) // For tracking seek hits
        //{
        //    OnHit();
        //}

        transform.Translate(direction.normalized * deltaDistance, Space.World);
    }


    /********************
     * Member Variables *
     ********************/
    public float TravelSpeed = 15f;
    public float Damage = 5.5f;


    private Transform targetTransform = null;
    private Vector3 direction = Vector3.zero;


    /******************
     * Public Methods *
     ******************/
    public void Launch(Transform target)
    {
        targetTransform = target;
        //direction = targetTransform.position - transform.position;
    }

    // TODO: make seek method so projectiles can seek target if necessary


    /*******************
     * Private Methods *
     *******************/
    private void OnHit()
    {
        Debug.Log("ON HIT!");
        ApplyDamage(targetTransform);
        Destroy(gameObject);
    }

    private void ApplyDamage(Transform target)
    {
        Unit e = target.GetComponentInChildren<Unit>();
        if (e != null)
        {
            e.TakeDamage(Damage);
        }
    }
}
