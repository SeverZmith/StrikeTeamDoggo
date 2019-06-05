using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    /*******************
     * Unity Functions *
     *******************/
    void Start()
    {
        InvokeRepeating("AcquireTarget", 0, 1f);
    }

    void Update()
    {
        if (TargetTransform == null)
        {
            return;
        }

        if (attackCooldown <= 0)
        {
            SprayAttack(6);
            attackCooldown = 1f / AttackRate;
        }
        attackCooldown -= Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, Range);
    }


    /********************
     * Member Variables *
     ********************/
    [System.NonSerialized] public Transform TargetTransform = null;
    public float Range = 4f;
    public float AttackRate = 2f;
    public GameObject BulletPrefab = null;
    public Transform launchPoint = null;

    private float attackCooldown = 0f;
    private bool isShooting = false;


    /******************
     * Public Methods *
     ******************/
    //public void SprayAttack()
    //{

    //}


    /*******************
     * Private Methods *
     *******************/
    private void AcquireTarget()
    {
        if (isShooting)
        {
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Get units tagged 'Enemy' in scene
        float closestDistanceToEnemy = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (GameObject e in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, e.transform.position);
            if (distanceToEnemy < closestDistanceToEnemy) // Set closest enemy
            {
                closestDistanceToEnemy = distanceToEnemy;
                closestEnemy = e;
            }
        }

        if (closestEnemy != null && closestDistanceToEnemy <= Range) // If enemy is targeted and within range
        {
            TargetTransform = closestEnemy.transform; // Set target
        }
        else
        {
            TargetTransform = null; // Otherwise clear target
        }
    }

    private void SprayAttack(int projectilesInSpray) // TODO: fix launch rotation // TODO: try coroutine to space out projectile timings
    {
        if (!isShooting)
        {
            isShooting = true;
            for (int i = 0; i < projectilesInSpray; i++)
            {
                GameObject bulletObj = Instantiate(BulletPrefab, launchPoint.position, launchPoint.rotation);
                Projectile bullet = bulletObj.GetComponentInChildren<Projectile>();

                if (bullet != null)
                {
                    // put spray logic here
                    bullet.Launch(TargetTransform);
                    StartCoroutine(DelayBetweenShots(0.05f));
                }
            }
            StopCoroutine(DelayBetweenShots(0));
        }
    }

    private IEnumerator DelayBetweenShots(float seconds)
    {

        yield return new WaitForSeconds(seconds);
        isShooting = false;
    }
}
