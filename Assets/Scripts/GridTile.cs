using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile {
    public GridTile() {
        cost = 0;
        tileType = TileType.BASE;
    }
    public int cost;

    public TileType tileType;
}
