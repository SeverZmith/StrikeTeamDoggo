using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Hexmap hexmap;
    [SerializeField] LayerMask hexLayer;
    public GameObject SelectedHex = null;
    GameObject previousHex = null;
    Color previousColor;
    float rayDistance = 100.0f;

    private void OnValidate()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        HexMouse();

        if (Input.GetMouseButtonDown(0) && SelectedHex)
        {
            hexmap.ClickOnHex(SelectedHex.transform.parent.gameObject);
            SelectedHex.GetComponent<Renderer>().material.color = Color.red;
        }

    }

    void HexMouse()
    {
        Vector2 mouseScreenPos = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mouseScreenPos);

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
        previousHex = SelectedHex;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, rayDistance, hexLayer))
        {
            //Debug.Log("Hit " + raycastHit.collider.gameObject.name, raycastHit.collider.gameObject);
            SelectedHex = raycastHit.collider.gameObject;
        }
        else
        {
            SelectedHex = null;
        }

        if (previousHex != SelectedHex && previousHex != null)
        {
            previousHex.GetComponent<Renderer>().material.color = previousColor;
        }
        if (SelectedHex != previousHex && SelectedHex != null)
        {
            Hex curHex = hexmap.dictGameObjToHex[SelectedHex.transform.parent.gameObject];
            previousColor = SelectedHex.GetComponent<Renderer>().material.color;
            if (curHex.IsHexEmpty)
            {
                SelectedHex.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                SelectedHex.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }
}