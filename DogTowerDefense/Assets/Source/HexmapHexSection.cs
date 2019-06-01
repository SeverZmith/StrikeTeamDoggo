using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexmapHexSection : Hexmap
{
    ///******************
    // * Public Methods *
    // ******************/
    //public override void GenerateTilemap()
    //{
    //    base.GenerateTilemap();

    //    CreateSection(46, 14, 3);
    //    CreateSection(6, 14, 6);

    //    UpdateTileType();

    //    Unit unit = new Unit();
    //    SpawnUnitAtCoord(unit, enemyPrefab, 0, 14);
    //}


    ///*******************
    // * Private Methods *
    // *******************/
    //private void CreateSection(int q, int r, int radius) // @params pass in tile coordinate and radius of section
    //{
    //    // TODO: fix bug... (@param1 + @param3) can not exceed total number of rows in map (needs to account for wrapping)
    //    // TODO: fix bug where center hex origin is X: -1, Y: 0... should be X: 0, Y: 0
    //    Hex centerHex = GetHexAtCoord(q, r);
    //    Hex[] areaOfHexes = GetHexesWithinRadius(centerHex, radius);
    //    // Iterate through each hex in section and set default tile type
    //    foreach (Hex hex in areaOfHexes)
    //    {
    //        if (hex.tileTypeIndex < 0)
    //        {
    //            hex.tileTypeIndex = 1;
    //        }
    //    }
    //}
}
