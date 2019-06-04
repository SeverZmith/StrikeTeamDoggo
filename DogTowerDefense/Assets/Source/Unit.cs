using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    /*******************
     * Unity Functions *
     *******************/
    void Start()
    {
        prevPosition = transform.position;
        nextPosition = transform.position;
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, nextPosition, ref velocity, 1f);
        CalculatePath(OccupiedHex);
        if (Vector3.Distance(transform.position, nextPosition) <= OccupiedHex.radius * 2)
        {
            TriggerMovement();
        }
    }


    /********************
     * Member Variables *
     ********************/
    public Hex OccupiedHex { get; protected set; }
    public string Name = "Unit-Name";
    public int Health = 100;

    private Vector3 prevPosition = Vector3.zero;
    private Vector3 nextPosition = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private List<Hex> pathToGoal = null;
    private Objective currentObjective = null;
    private bool reachedDestination = false;


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

    public void SetCurrentObjective(Objective objective)
    {
        currentObjective = objective;
    }

    public void TriggerMovement()
    {
        if (pathToGoal.Count > 0)
        {
            Hex nextHex = pathToGoal[0];
            pathToGoal.RemoveAt(0);

            float parentY = GetComponentInParent<Transform>().position.y;
            nextPosition = nextHex.GetWorldPosition() + new Vector3(0, parentY, 0);

            OccupiedHex = nextHex;
        }
    }

    public void CalculatePath(Hex startingHex)
    {
        pathToGoal = new List<Hex>();

        for (int i = 0; i < startingHex.Neighbors.Count; i++)
        {
            if (startingHex.Neighbors[i].HexDistanceToObjective == startingHex.HexDistanceToObjective - 1)
            {
                pathToGoal.Add(startingHex.Neighbors[i]);
            }
        }
    }
}
