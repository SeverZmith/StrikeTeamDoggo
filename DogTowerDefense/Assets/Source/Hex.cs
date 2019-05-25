using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Hex Base Class
 * 
 * Contains properties and positions of each hex.
 */
public class Hex
{
    /******************
     * Constructor(s) *
     ******************/
    public Hex(int q, int r, float radius)
    {
        // since:       q + r + s = 0
        // therefore:   s = -(q + r)
        this.q = q;
        this.r = r;
        s = -(q + r);
        this.radius = radius;
    }


    /********************
     * Member Variables *
     ********************/
    public readonly int q = 0; // col
    public readonly int r = 0; // row
    public readonly int s = 0;
    public readonly float radius = 0;

    // TODO: change to enum
    public int tileTypeIndex = -1; // -1: undefined, 0: terrain, 1: grass

    private static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;


    /******************
     * Public Methods *
     ******************/
    public Vector3 GetWorldPosition()
    {
        // Return world position of tile based on its col and row on grid
        return new Vector3(GetMapHorizontalOffset() * (q + r / 2f), 0, GetMapVerticalOffset() * r);
    }

    public float GetHexHeight()
    {
        // Our hex is pointed top and has a radius of 1
        return radius * 2;
    }

    public float GetHexWidth()
    {
        // width = WIDTH_MULTIPLIER * height
        // this is because the inner radius of a hexagon is equal to sqrt(3)/2 times the outer radius
        return WIDTH_MULTIPLIER * GetHexHeight();
    }

    public float GetMapVerticalOffset()
    {
        // Vertical offset is 3/4 of the height of a hex
        return GetHexHeight() * 0.75f;
    }

    public float GetMapHorizontalOffset() // TODO: remove redundant calls or figure out a way to toggle between pointed and flat topped hexes
    {
        return GetHexWidth();
    }

    public Vector3 GetPositionRelativeToCamera(Vector3 cameraPosition, float hexRows, float hexCols)
    {
        float mapHeight = hexRows * GetMapVerticalOffset(); // TODO: extrapolate into member vars
        float mapWidth = hexCols * GetMapHorizontalOffset();

        Vector3 position = GetWorldPosition();

        // Calculates hex's horizontal distance from camera
        float distanceFromCamera = (position.x - cameraPosition.x) / mapWidth;

        // If hex is within camera's horizontal constraint return its position
        if (Mathf.Abs(distanceFromCamera) <= 0.5f)
        {
            return position;
        }

        // If the hex's position hasn't been returned, it needs to be relocated either left or right
        if (distanceFromCamera > 0)
        {
            distanceFromCamera += 0.5f;
        }
        else
        {
            distanceFromCamera -= 0.5f;
        }

        // Relocate hex's horizontal position by its horizontal distance from the camera's
        // We only want to move the tiles in increments exactly equal to their width, so we don't need the decimal
        int numOfTilesToAdjust = (int)distanceFromCamera;
        position.x -= numOfTilesToAdjust * mapWidth;

        return position;
    }

    public static float GetDistanceBetweenHexes(Hex a, Hex b)
    {
        //https://www.redblobgames.com/grids/hexagons/
        // See 'Distances' section under cube coordinates on redblobgames' hexagons article
        return Mathf.Max(Mathf.Abs(a.q - b.q), Mathf.Abs(a.r - b.r), Mathf.Abs(a.s - b.s));
    }
}
