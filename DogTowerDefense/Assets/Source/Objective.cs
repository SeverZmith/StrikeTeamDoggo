using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    float lookSpeed = 2.0f;
    Vector3 lastHit;

    private Hexmap hexmap = null;
    private Main main = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            lastHit = new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
            // TODO: Deduct from lives
            Destroy(other.gameObject);
            main.UpdateHealth(1);
        }
            }
    private void Start()
    {
        lastHit = transform.position;
        hexmap = GetComponentInParent<Hexmap>();
        main = hexmap.main.GetComponentInChildren<Main>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lastHit - transform.position + new Vector3(0.0001f, 0, 0)), lookSpeed * Time.deltaTime);
    }
}
