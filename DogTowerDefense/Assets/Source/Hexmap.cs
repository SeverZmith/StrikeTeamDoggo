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
    }


    /********************
     * Member Variables *
     ********************/
    public GameObject hexPrefab = null;
    //public Material[] hexMats = null;
    public Mesh terrainMesh = null;
    public Mesh grassMesh = null;
    public Material terrainMat = null;
    public Material grassMat = null;

    private int hexRows = 30;
    private int hexCols = 60;
    private Hex[ , ] hexes = null;
    private Dictionary<Hex, GameObject> dictHexToGameObj;


    /******************
     * Public Methods *
     ******************/
    public virtual void GenerateTilemap()
    {
        hexes = new Hex[hexCols, hexRows];
        dictHexToGameObj = new Dictionary<Hex, GameObject>();

        for (int col = 0; col < hexCols; col++)
        {
            for (int row = 0; row < hexRows; row++)
            {
                // Create new Hex w/ radius of 1f
                Hex hex = new Hex(col, row, 1f);
                hex.tileTypeIndex = -1;

                // Assign created hex into spot in hexes array
                hexes[col, row] = hex;

                // Position hexes based on camera's position (this is essentially an early stage of world wrapping)
                Vector3 hexPosition = hex.GetPositionRelativeToCamera(Camera.main.transform.position, hexRows, hexCols);

                // Instantiate hex tiles (parent transform to this Tilemap)
                GameObject hexObj = Instantiate(hexPrefab, hexPosition, Quaternion.identity, this.transform);

                dictHexToGameObj[hex] = hexObj; // TODO: use add call instead?

                // Show hex coordinate text
                hexObj.GetComponentInChildren<TextMesh>().text = string.Format("[{0}, {1}]", col, row);
            }
        }

        UpdateTileType();

        // Static objects can be batched together to reduce the number of draw calls
        //StaticBatchingUtility.Combine(this.gameObject);
    }

    public Hex GetHexAtCoord(int x, int y)
    {
        if (hexes == null)
        {
            Debug.Log("Hexes not instantiated.");
            return null;
        }

        return hexes[x, y];
    }

    public Hex[] GetHexesWithinRadius(Hex centerHex, int radius)
    {
        List<Hex> hexesList = new List<Hex>();

        // dx and dy are the changes in horizontal/vertical distances from the center hex
        for (int dx = -radius; dx < radius - 1; dx++)
        {
            // Each time dx loop is incremented, this loop iterates from the lowest to the highest vertical offset from origin
            // See debug log
            // TODO: fix bug where center hex x is equal to -1... should be 0
            // See CreateSection method in HexmapHexSection
            for (int dy = Mathf.Max(-radius + 1, -dx - radius); dy < Mathf.Min(radius, -dx + radius - 1); dy++)
            {
                Debug.Log("Hex: " + hexesList.Count + " X: " + dx + " Y: " + dy);
                hexesList.Add(GetHexAtCoord(centerHex.q + dx, centerHex.r + dy));
            }
        }

        return hexesList.ToArray();
    }

    public void UpdateTileType()
    {
        for (int col = 0; col < hexCols; col++)
        {
            for (int row = 0; row < hexRows; row++)
            {
                Hex hex = hexes[col, row];
                GameObject hexObj = dictHexToGameObj[hex];

                // Change hex material and mesh
                MeshRenderer meshRenderer = hexObj.GetComponentInChildren<MeshRenderer>();
                MeshFilter meshFilter = hexObj.GetComponentInChildren<MeshFilter>();
                switch (hex.tileTypeIndex)
                {
                    case 0:
                        meshRenderer.material = terrainMat;
                        meshFilter.mesh = terrainMesh;
                        break;

                    case 1:
                        meshRenderer.material = grassMat;
                        meshFilter.mesh = grassMesh;
                        break;

                    default:
                        meshRenderer.material = terrainMat;
                        meshFilter.mesh = terrainMesh;
                        break;
                }
            }
        }
    }
}
