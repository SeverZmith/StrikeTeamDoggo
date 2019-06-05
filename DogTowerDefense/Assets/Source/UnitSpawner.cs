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
        if (Input.GetKeyDown(KeyCode.Space)) // FOR SPAWNING TEST
        {
            if (!ceaseSpawning)
            {
                EndSpawnCycle();
            }
            else
            {
                StartSpawnCycle(2f);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha6)) // FOR WAVE SPAWNING TEST
        {
            SpawnWave(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SpawnWave(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SpawnWave(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SpawnWave(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SpawnWave(5);
        }
    }


    /********************
     * Member Variables *
     ********************/
    public GameObject UnitPrefab = null;
    [System.NonSerialized] public Hexmap Hexmap = null;

    private List<Unit> units = null;
    private Dictionary<Unit, GameObject> dictUnitToGameObj = null;
    private bool spawningWave = false;
    private bool ceaseSpawning = true;


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

    public void StartSpawnCycle(float frequency)
    {
        ceaseSpawning = false;
        InvokeRepeating("TriggerSpawners", 0, frequency);
    }

    public void EndSpawnCycle()
    {
        ceaseSpawning = true;
        CancelInvoke();
    }

    public void SpawnWave(int unitsInWave)
    {
        StartCoroutine(SpawnSetOfUnits(unitsInWave));
    }


    /*******************
     * Private Methods *
     *******************/
    private void TriggerSpawners()
    {
        List<Hex> spawnPoints = Hexmap.GetSpawnPoints();
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            SpawnUnitAtHex(spawnPoints[i]);
        }
    }

    private IEnumerator SpawnSetOfUnits(int numUnits)
    {
        spawningWave = true;
        InvokeRepeating("TriggerSpawners", 0, 1f);
        yield return new WaitForSeconds(numUnits - 1);
        CancelInvoke();
        spawningWave = false;
    }
}
