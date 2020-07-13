using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;

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

    public GridTile(SuperTile tile) {
        cost = 0;
        tileType = TileType.BASE;
        foreach (CustomProperty prop in tile.m_CustomProperties) {
            if (prop.m_Name == "cost") {
                cost = prop.GetValueAsInt();
            } else if (prop.m_Name == "tileType") {
                tileType = prop.GetValueAsEnum<TileType>();
            }
        }
    }
    
    public GridTile(GridManager gridManager, int x, int y, SuperTile tile) {
        this.gridManager = gridManager;
        this.x = x;
        this.y = y;
        isWalkable = true;
        cost = 0;
        tileType = TileType.BASE;
        foreach (CustomProperty prop in tile.m_CustomProperties) {
            if (prop.m_Name == "cost") {
                cost = prop.GetValueAsInt();
            } else if (prop.m_Name == "tileType") {
                tileType = prop.GetValueAsEnum<TileType>();
            }
        }
        if (cost < 0) {
            isWalkable = false;
        }
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
        return x + "," + y;
    }
}
