using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInputController : MonoBehaviour
{
    /********************
     * Member Variables *
     ********************/
    private float glideSpeed = 10;


    /*******************
     * Unity Functions *
     *******************/
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Camera Glide
        Vector3 glideDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        this.transform.Translate(glideDirection * glideSpeed * Time.deltaTime, Space.World);

        // Left Mouse Button
        if (Input.GetMouseButtonDown(0))
        {

        }
    }
}
