using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    /********************
     * Member Variables *
     ********************/
    public Hex OccupiedHex { get; protected set; }
    public string name = "Unit-Name";
    public int health = 100;


    /******************
     * Public Methods *
     ******************/
    public void SetOccupiedHex(Hex hex)
    {
        if (hex != null)
        {
            hex.RemoveUnit(this);
        }

        OccupiedHex = hex;

        hex.AddUnit(this);
    }

    public void StartMovement()
    {
        Debug.Log("Moving");
    }
}
