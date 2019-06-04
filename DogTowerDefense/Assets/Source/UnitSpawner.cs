using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    /*******************
     * Unity Functions *
     *******************/
    void Update()
    {
        if (!spawningUnit && !ceaseSpawning)
        {
            StartCoroutine(BeginSpawnCycle(3f));
        }

        if (Input.GetKey(KeyCode.Space)) // FOR TESTING WAVES
        {
            if (ceaseSpawning)
            {
                ceaseSpawning = false;
            }
            else
            {
                ceaseSpawning = true;
            }
        }
    }


    /********************
     * Member Variables *
     ********************/
    public GameObject UnitPrefab = null;
    [System.NonSerialized] public Hexmap Hexmap = null;

    private List<Unit> units = null;
    private Dictionary<Unit, GameObject> dictUnitToGameObj = null;
    private bool spawningUnit = false;
    private bool ceaseSpawning = false;


    /******************
     * Public Methods *
     ******************/
    public void SpawnUnitAtHex(Hex hex)
    {
        if (units == null)
        {
            units = new List<Unit>();
            dictUnitToGameObj = new Dictionary<Unit, GameObject>();
        }

        GameObject hexObj = Hexmap.DictHexToGameObj[hex];
        GameObject unitObj = Instantiate(UnitPrefab, hexObj.transform.position, Quaternion.identity, hexObj.transform);

        Unit unit = unitObj.GetComponentInChildren<Unit>();
        unit.SetOccupiedHex(hex);

        units.Add(unit);
        dictUnitToGameObj[unit] = unitObj;
    }


    /*******************
     * Private Methods *
     *******************/
    private IEnumerator BeginSpawnCycle(float spawnDelay)
    {
        spawningUnit = true;
        List<Hex> spawnPoints = Hexmap.GetSpawnPoints();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            SpawnUnitAtHex(spawnPoints[i]);
        }
        yield return new WaitForSeconds(spawnDelay);
        spawningUnit = false;
    }
}
