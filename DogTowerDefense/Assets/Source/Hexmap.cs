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
        flowField(HexList[Random.Range(0, HexList.Count)]);
        //flowField(GetHexAtCoord(8, 8));

        // For testing unit spawning
        Unit unit = new Unit();
        SpawnUnitAtCoord(unit, enemyPrefab, 8, 8);
    }

    void Update() // TESTING (TODO: remove)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (units != null)
            {
                foreach (Unit unit in units)
                {
                    unit.TriggerMovement();
                }
            }
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
    public GameObject enemyPrefab = null;
    //public Material[] hexMats = null;
    public Mesh terrainMesh = null; // Mats & Meshes
    public Mesh grassMesh = null;
    public Material terrainMat = null;
    public Material grassMat = null;

    private List<Hex> HexList = new List<Hex>();
    //private int hexRows = 30;
    //private int hexCols = 60;
    private Hex[,] hexes = null;
    private Dictionary<Hex, GameObject> dictHexToGameObj = null;
    private List<Unit> units = null;
    private Dictionary<Unit, GameObject> dictUnitToGameObj = null;
    List<Hex> SpawnPoints = new List<Hex>();

    [SerializeField] private int hexagonMapRadius = 8;


    /******************
     * Public Methods *
     ******************/
    public virtual void GenerateTilemap()
    {
        hexes = new Hex[(hexagonMapRadius * 2) + 1, (hexagonMapRadius * ((hexagonMapRadius + 1) * 3)) + 1];
        dictHexToGameObj = new Dictionary<Hex, GameObject>();
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

                dictHexToGameObj[hex] = hexObj;
                hex.HexObject = hexObj;
                //hex.SetHexObj(hexObj); // TODO: verify remove

                // Shows hexes' coordinates relative to center hex
                hexObj.GetComponentInChildren<TextMesh>().text = string.Format("({0}, {1})", xIndex, yIndex);

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

    public Hex GetHexAtCoord(int q, int r)
    {
        if (hexes == null)
        {
            Debug.Log("Hexes not instantiated.");
            return null;
        }

        return hexes[q, r];
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
                hexesList.Add(GetHexAtCoord(centerHex.q + dq, centerHex.r + dr));
            }
        }

        return hexesList.ToArray();
    }

    public void SpawnUnitAtCoord(Unit unitType, GameObject unitPrefab, int q, int r)
    {
        if (units == null)
        {
            units = new List<Unit>();
            dictUnitToGameObj = new Dictionary<Unit, GameObject>();
        }

        Hex hex = GetHexAtCoord(q, r);
        //Debug.Log(hex); // TODO: fix bug.. Hex is null because Hexes[,] isn't instantiated.
        // need to figure out how to initialize Hexes[,] as each Hex is instantiated.
        // see GenerateTilemap_old method (commented)
        GameObject hexObj = dictHexToGameObj[hex];
        unitType.SetOccupiedHex(hex);
        GameObject unitObj = Instantiate(unitPrefab, hexObj.transform.position, Quaternion.identity, hexObj.transform);

        units.Add(unitType);
        // dictUnitToGameObj[unitType] = unitObj;
        dictUnitToGameObj.Add(unitType, unitObj);
    }

    void flowField(Hex goal)
    {
        int frontier = 0;

        List<Hex> queue = new List<Hex>();
        foreach (Hex hex in HexList)
        {
            queue.Add(hex);
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
        }
        foreach (Distance hexDist in distance)
        {
            hexDist.hexagon.HexObject.GetComponentInChildren<TextMesh>().text += string.Format("\n{0}", hexDist.dist);
        }
    }

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
