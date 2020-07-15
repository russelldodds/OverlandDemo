using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile {
    private GridManager gridManager;
    public int x;
    public int y; 
    public int gCost;
    public int hCost;
    public int fCost;
    public bool isWalkable;
    public GridTile cameFromTile;
    public int cost;
    public TileType tileType;
    
    public GridTile() {

    }
    
    public GridTile(GridManager gridManager, int x, int y) {
        this.gridManager = gridManager;
        this.x = x;
        this.y = y;
        cost = 0;
        tileType = TileType.BASE;  
        isWalkable = true;
    }

    public void SetCost(int cost) {
        this.cost = cost;
        if (cost < 0) {
            isWalkable = false;
        } else {
            isWalkable = true;
        }
    }


    public void SetTileType(TileType tileType) {
        this.tileType = tileType;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
        gridManager.TriggerGridObjectChanged(x, y);
    }

    public override string ToString() {
        //return cost.ToString();
        return "cost: " + cost + ", type: " + tileType + ", pos(" + x + "," + y + ")";
    }
}
