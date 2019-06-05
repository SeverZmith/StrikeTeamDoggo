using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexmap : MonoBehaviour
{
    /*******************
     * Unity Functions *
     *******************/
    void Start()
    {
        GenerateTilemap();

        // Create Objective
        CreateObjectiveAtHexIndex(objectivePrefab, ObjectiveXIndex, ObjectiveYIndex);

        // Calculate flowField
        flowField(GetHexAtIndex(ObjectiveXIndex, ObjectiveYIndex));

        // Create Unit Spawners
        for (int i = 0; i < SpawnPoints.Count; i++)
        {
            CreateUnitSpawnerAtHex(SpawnPoints[i]);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetBuildingSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetBuildingSelection(2);
        }
    }


    /********************
     * Member Variables *
     ********************/
    public struct Distance
    {
        public Hex hexagon;
        public int dist;

        public Distance(Hex hex, int distance)
        {
            hexagon = hex;
            dist = distance;
        }
    }

    public GameObject hexPrefab = null; // Prefab
    public Wall wallPrefab = null;
    public GameObject towerPrefab = null;
    public GameObject objectivePrefab = null;
    public GameObject unitSpawnerPrefab = null;
    public Mesh terrainMesh = null; // Mats & Meshes
    public Mesh grassMesh = null;
    public Material terrainMat = null;
    public Material grassMat = null;
    public Dictionary<Hex, GameObject> DictHexToGameObj = new Dictionary<Hex, GameObject>();
    public int ObjectiveXIndex = 0;
    public int ObjectiveYIndex = 0;

    private List<Hex> HexList = new List<Hex>();
    private Hex[,] hexes = null;
    public Dictionary<GameObject, Hex> dictGameObjToHex = new Dictionary<GameObject, Hex>();
    private List<Hex> SpawnPoints = new List<Hex>();

    private int buildingSelection = 1;

    [SerializeField] private int hexagonMapRadius = 8;


    /******************
     * Public Methods *
     ******************/
    public virtual void GenerateTilemap()
    {
        hexes = new Hex[(hexagonMapRadius * 2) + 1, (hexagonMapRadius * ((hexagonMapRadius + 1) * 3)) + 1];
        DictHexToGameObj = new Dictionary<Hex, GameObject>();
        int xIndex = 0;
        int yIndex = 0;

        for (int q = hexagonMapRadius; q >= -hexagonMapRadius; q--)
        {
            int qSign;
            if (q != 0)
                qSign = Mathf.Abs(q) / q;
            else
                qSign = 1;

            yIndex = 0;

            for (int r = hexagonMapRadius * qSign - q; r != hexagonMapRadius * -qSign - qSign; r -= qSign)
            {
                Hex hex = new Hex(this, q, r, 1f);
                hex.tileTypeIndex = -1;

                hexes[xIndex, yIndex] = hex;

                // Instantiate hex tiles (parent transform to this Tilemap)
                GameObject hexObj = Instantiate(hexPrefab, hex.GetWorldPosition(), Quaternion.identity, this.transform);

                DictHexToGameObj[hex] = hexObj;
                dictGameObjToHex[hexObj] = hex;
                hex.HexObject = hexObj;

                // Shows hexes' position in hexes 2d array
                //hexObj.GetComponentInChildren<TextMesh>().text = string.Format("({0}, {1})", xIndex, yIndex);

                MeshRenderer meshRenderer = hexObj.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material = terrainMat;

                HexList.Add(hex);

                // Set Spawnpoints
                if ((hex.q == hexagonMapRadius / 2 && (hex.r == -hexagonMapRadius || hex.r == hexagonMapRadius / 2))
                    || (hex.q == -hexagonMapRadius / 2 && (hex.r == hexagonMapRadius || hex.r == -hexagonMapRadius / 2))
                    || (hex.q == hexagonMapRadius && hex.r == -hexagonMapRadius / 2)
                    || (hex.q == -hexagonMapRadius && hex.r == hexagonMapRadius / 2))
                {
                    SpawnPoints.Add(hex);
                }

                yIndex++;
            }
            xIndex++;
        }

        // Check for and store neighbors in Hex
        foreach (Hex hexagon in HexList)
        {
            foreach (Hex neighbor in HexList)
            {
                if ((neighbor.q == hexagon.q + 1 && (neighbor.r == hexagon.r || neighbor.r == hexagon.r - 1))
                    || (neighbor.q == hexagon.q - 1 && (neighbor.r == hexagon.r || neighbor.r == hexagon.r + 1))
                    || (neighbor.q == hexagon.q && (neighbor.r == hexagon.r - 1 || neighbor.r == hexagon.r + 1)))
                {
                    hexagon.AddNeighbor(neighbor);
                }
            }
        }

        // Static objects can be batched together to reduce the number of draw calls
        //StaticBatchingUtility.Combine(this.gameObject);
    }

    public Hex GetHexAtIndex(int xIndex, int yIndex)
    {
        if (hexes == null)
        {
            Debug.Log("Hexes not instantiated.");
            return null;
        }

        return hexes[xIndex, yIndex];
    }

    public Vector3 GetHexPosition(Hex hex)
    {
        return hex.GetPositionRelativeToCamera(Camera.main.transform.position, (hexagonMapRadius * 2) + 1, (hexagonMapRadius * ((hexagonMapRadius + 1) * 3)) + 1);
    }

    public Hex[] GetHexesWithinRadius(Hex centerHex, int radius)
    {
        List<Hex> hexesList = new List<Hex>();

        // dx and dy are the changes in horizontal/vertical distances from the center hex
        for (int dq = -radius; dq < radius - 1; dq++)
        {
            // Each time dx loop is incremented, this loop iterates from the lowest to the highest vertical offset from origin
            // See debug log
            // TODO: fix bug where center hex x is equal to -1... should be 0
            // See CreateSection method in HexmapHexSection
            for (int dr = Mathf.Max(-radius + 1, -dq - radius); dr < Mathf.Min(radius, -dq + radius - 1); dr++)
            {
                Debug.Log("Hex: " + hexesList.Count + " X: " + dq + " Y: " + dr);
                hexesList.Add(GetHexAtIndex(centerHex.q + dq, centerHex.r + dr));
            }
        }

        return hexesList.ToArray();
    }

    public void CreateObjectiveAtHexIndex(GameObject objectivePrefab, int xIndex, int yIndex)
    {
        Hex hex = GetHexAtIndex(xIndex, yIndex);
        GameObject hexObj = DictHexToGameObj[hex];
        Vector3 spawnPosition = hexObj.transform.position + new Vector3(0, 1.2f, 0);
        GameObject objectiveObj = Instantiate(objectivePrefab, spawnPosition, Quaternion.identity, hexObj.transform);
    }

    public void CreateObjectiveAtHex(GameObject objectivePrefab, Hex hex)
    {
        GameObject hexObj = DictHexToGameObj[hex];
        Vector3 spawnPosition = hexObj.transform.position + new Vector3(0, 0.7f, 0);
        GameObject objectiveObj = Instantiate(objectivePrefab, spawnPosition, Quaternion.identity, hexObj.transform);
    }

    public void CreateUnitSpawnerAtHex(Hex hex)
    {
        GameObject hexObj = DictHexToGameObj[hex];
        Vector3 spawnPosition = hexObj.transform.position + new Vector3(0, 0.7f, 0);
        GameObject unitSpawnerObj = Instantiate(unitSpawnerPrefab, spawnPosition, Quaternion.identity, hexObj.transform);
        unitSpawnerObj.GetComponentInChildren<UnitSpawner>().Hexmap = this;
    }

    public List<Hex> GetSpawnPoints()
    {
        return SpawnPoints;
    }

    public void ClickOnHex(GameObject hexGameObj)
    {
        Hex hex = dictGameObjToHex[hexGameObj];
        // Debug.Log("Hex Clicked: " + hex.q + "," + hex.r + "," + hex.s);

        // If the hex being clicked on is occupied by the objective, do nothing
        if (hex.GetWorldPosition() != GetHexAtIndex(ObjectiveXIndex, ObjectiveYIndex).GetWorldPosition())
        {
            // Build
            switch (buildingSelection)
            {
                case 1:
                    AddWall(hex);
                    break;

                case 2:
                    AddTower(hex);
                    break;

                default:
                    break;
            }
        }
    }

    public void SetBuildingSelection(int selection)
    {
        buildingSelection = selection;
    }


    /*******************
     * Private Methods *
     *******************/
    public void flowField(Hex goal)
    {
        int frontier = 0;

        List<Hex> queue = new List<Hex>();
        foreach (Hex hex in HexList)
        {
            if (hex.IsHexEmpty)  // removes obstacles from queue
                queue.Add(hex);
            else
                hex.HexDistanceToObjective = 999;
        }
        List<Distance> distance = new List<Distance>();
        Distance curDistance = new Distance(goal, frontier);
        distance.Add(curDistance);
        queue.Remove(goal);

        List<Hex> currentGroup = new List<Hex>();
        List<Hex> nextGroup = new List<Hex>();
        currentGroup.Add(goal);

        while (queue.Count > 0)
        {
            frontier++;
            foreach (Hex currentHex in currentGroup)
            {
                foreach (Hex neighbor in currentHex.Neighbors)
                {
                    if (queue.Contains(neighbor)) // TODO: also check if obtacle 
                    {
                        nextGroup.Add(neighbor);
                        curDistance = new Distance(neighbor, frontier);
                        distance.Add(curDistance);

                        // TEST PATHFINDING
                        neighbor.HexDistanceToObjective = curDistance.dist;
                        // TEST PATHFINDING

                        queue.Remove(neighbor);
                    }
                }
            }
            currentGroup.Clear();
            foreach (Hex hex in nextGroup)
            {
                currentGroup.Add(hex);
            }
            nextGroup.Clear();

            if (currentGroup.Count == 0) // clears queue of unreachable tiles
            {
                queue.Clear();
            }
        }
        foreach (Distance hexDist in distance)
        {
            //hexDist.hexagon.HexObject.GetComponentInChildren<TextMesh>().text = string.Format("{0}", hexDist.dist);
        }
    }

    private void AddWall(Hex hex)
    {
        Quaternion wallRotation = new Quaternion();
        if (hex.IsHexEmpty)
        {
            if (hex.q == 0)
            {
                wallRotation = Quaternion.Euler(0, 30, 0);
            }
            else if (hex.r == 0)
            {
                wallRotation = Quaternion.Euler(0, 90, 0);
            }
            else if (hex.s == 0)
            {
                wallRotation = Quaternion.Euler(0, 150, 0);
            }
            else if ((hex.q > 0 && hex.r < 0 && hex.s > 0) || (hex.q < 0 && hex.r > 0 && hex.s < 0)) // flat
            {
                wallRotation = Quaternion.Euler(0, 0, 0);
            }
            else if ((hex.q < 0 && hex.r > 0 && hex.s > 0) || (hex.q > 0 && hex.r < 0 && hex.s < 0)) // up left, down right
            {
                wallRotation = Quaternion.Euler(0, 120, 0);
            }
            else if ((hex.q > 0 && hex.r > 0 && hex.s < 0) || (hex.q < 0 && hex.r < 0 && hex.s > 0)) // up right, down left
            {
                wallRotation = Quaternion.Euler(0, 60, 0);
            }
            GameObject hexObj = DictHexToGameObj[hex];
            Vector3 wallOffset = new Vector3(0, 0.3f, 0);
            Wall wall = Instantiate(wallPrefab, hex.HexObject.transform.position + wallOffset, wallRotation);
            hex.IsHexEmpty = false;
            flowField(GetHexAtIndex(ObjectiveXIndex, ObjectiveYIndex));
            wall.hex = hex;
            wall.hexMap = this;
        }
    }

    private void AddTower(Hex hex)
    {
        if (hex.IsHexEmpty)
        {
            GameObject hexObj = DictHexToGameObj[hex];
            Vector3 spawnPosition = hexObj.transform.position + new Vector3(0, 0.7f, 0);
            Instantiate(towerPrefab, hexObj.transform.position, Quaternion.identity, hexObj.transform);
            hex.IsHexEmpty = false;
            flowField(GetHexAtIndex(ObjectiveXIndex, ObjectiveYIndex));
        }
    }







    //public void SpawnUnitAtCoord(GameObject unitPrefab, int q, int r)
    //{
    //    if (units == null)
    //    {
    //        units = new List<Unit>();
    //        dictUnitToGameObj = new Dictionary<Unit, GameObject>();
    //    }

    //    Hex hex = GetHexAtCoord(q, r);
    //    GameObject hexObj = DictHexToGameObj[hex];
    //    GameObject unitObj = Instantiate(unitPrefab, hexObj.transform.position, Quaternion.identity, hexObj.transform);

    //    Unit unit = unitObj.GetComponentInChildren<Unit>();
    //    unit.SetOccupiedHex(hex);

    //    units.Add(unit);
    //    dictUnitToGameObj[unit] = unitObj;
    //}

    //public void SpawnUnitAtHex(GameObject unitPrefab, Hex hex)
    //{
    //    if (units == null)
    //    {
    //        units = new List<Unit>();
    //        dictUnitToGameObj = new Dictionary<Unit, GameObject>();
    //    }

    //    GameObject hexObj = DictHexToGameObj[hex];
    //    GameObject unitObj = Instantiate(unitPrefab, hexObj.transform.position, Quaternion.identity, hexObj.transform);

    //    Unit unit = unitObj.GetComponentInChildren<Unit>();
    //    unit.SetOccupiedHex(hex);

    //    units.Add(unit);
    //    dictUnitToGameObj[unit] = unitObj;
    //}



    //public virtual void GenerateTilemap_old()
    //{
    //    hexes = new Hex[hexCols, hexRows];
    //    dictHexToGameObj = new Dictionary<Hex, GameObject>();

    //    for (int col = 0; col < hexCols; col++)
    //    {
    //        for (int row = 0; row < hexRows; row++)
    //        {
    //            // Create new Hex w/ radius of 1f
    //            Hex hex = new Hex(this, col, row, 1f);
    //            hex.tileTypeIndex = -1;

    //            // Assign created hex into spot in hexes array
    //            hexes[col, row] = hex;

    //            // Position hexes based on camera's position (this is essentially an early stage of world wrapping)
    //            Vector3 hexPosition = hex.GetPositionRelativeToCamera(Camera.main.transform.position, hexRows, hexCols);

    //            // Instantiate hex tiles (parent transform to this Tilemap)
    //            GameObject hexObj = Instantiate(hexPrefab, hexPosition, Quaternion.identity, this.transform);

    //            dictHexToGameObj[hex] = hexObj; // TODO: use add call instead?

    //            // Show hex coordinate text
    //            hexObj.GetComponentInChildren<TextMesh>().text = string.Format("({0}, {1})", col, row);

    //            //hex.SetHexObj(hexObj); // TODO: verify remove
    //            hex.HexObject = hexObj;
    //            HexList.Add(hex);
    //        }

    //        // Check for and store neighbors in Hex
    //        foreach (Hex hex in HexList)
    //        {
    //            foreach (Hex neighbor in HexList)
    //            {
    //                if ((neighbor.q == hex.q + 1 && (neighbor.r == hex.r || neighbor.r == hex.r - 1))
    //                    || (neighbor.q == hex.q - 1 && (neighbor.r == hex.r || neighbor.r == hex.r + 1))
    //                    || (neighbor.q == hex.q && (neighbor.r == hex.r - 1 || neighbor.r == hex.r + 1)))
    //                {
    //                    hex.AddNeighbor(neighbor);
    //                }
    //            }
    //        }

    //        UpdateTileType();

    //        // Static objects can be batched together to reduce the number of draw calls
    //        //StaticBatchingUtility.Combine(this.gameObject);
    //    }
    //}

    //public void UpdateTileType()
    //{
    //    for (int col = 0; col < hexCols; col++)
    //    {
    //        for (int row = 0; row < hexRows; row++)
    //        {
    //            Hex hex = hexes[col, row];
    //            GameObject hexObj = dictHexToGameObj[hex];

    //            // Change hex material and mesh
    //            MeshRenderer meshRenderer = hexObj.GetComponentInChildren<MeshRenderer>();
    //            MeshFilter meshFilter = hexObj.GetComponentInChildren<MeshFilter>();
    //            switch (hex.tileTypeIndex)
    //            {
    //                case 0:
    //                    meshRenderer.material = terrainMat;
    //                    meshFilter.mesh = terrainMesh;
    //                    break;

    //                case 1:
    //                    meshRenderer.material = grassMat;
    //                    meshFilter.mesh = grassMesh;
    //                    break;

    //                default:
    //                    meshRenderer.material = terrainMat;
    //                    meshFilter.mesh = terrainMesh;
    //                    break;
    //            }
    //        }
    //    }
    //}
}
