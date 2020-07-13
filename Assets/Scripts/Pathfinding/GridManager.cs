/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

using SuperTiled2Unity;
using UnityEngine.Tilemaps;
public class GridManager {

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellWidth;
    private float cellHeight;
    private Vector3 originPosition;
    private GridTile[,] gridArray;
    private SuperMap map;

    public GridManager(SuperMap map) {
        this.map = map;
        this.width = map.m_Width;
        this.height = map.m_Height;
        this.cellWidth = 1;//map.m_TileWidth;
        this.cellHeight = 1;//map.m_TileHeight;
        this.originPosition = map.transform.position;

        Grid grid = map.GetComponentInChildren<Grid>();
        gridArray = new GridTile[width, height];

        for (int x = 0; x < map.m_Width; x++) {
            for (int y = 0; y < map.m_Height; y++) {
                Vector3Int tilePos = new Vector3Int(x, -y, 0);
                //Debug.Log("tilePos: " + tilePos);
                for (int t = 0; t < grid.transform.childCount; t++) {   
                    GameObject child = grid.transform.GetChild(t).gameObject;
                    Tilemap tilemap = child.GetComponent<Tilemap>();   
                    //Debug.Log("tilemap: " + tilemap.name);
                    SuperTile tile = tilemap.GetTile<SuperTile>(tilePos);
                    if (tile != null) {
                        //Debug.Log("tilemap: " + tilemap.name + ", tile: " + tile.name);
                        GridTile gridTile = new GridTile(this, x, y, tile);
                        gridArray[x, y] = gridTile;
                        break;
                    }
                }
            }
        }

        bool showDebug = true;
        if (showDebug) {
            TextMesh[,] debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellWidth, cellHeight) * .5f, 6, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y - 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, 0), GetWorldPosition(width, 0), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
                debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            };
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellWidth() {
        return cellWidth;
    }

    public float GetCellHeight() {
        return cellHeight;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        // TODO: deal with different height
        return new Vector3(x, -y - 1) * cellWidth + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellWidth);
        y = Mathf.CeilToInt((worldPosition - originPosition).y / cellHeight);
    }

    public void SetGridTile(int x, int y, GridTile value) {
        if (x >= 0 && y <= 0 && x < width && y > -height) {
            gridArray[x, -y] = value;
            if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
        }
    }

    public void TriggerGridObjectChanged(int x, int y) {
        if (OnGridObjectChanged != null) OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { x = x, y = y });
    }

    public void SetGridObject(Vector3 worldPosition, GridTile value) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridTile(x, y, value);
    }

    public GridTile GetGridTile(int x, int y) {
        if (x >= 0 && y <= 0 && x < width && y > -height) {
            return gridArray[x, -y];
        } else {
            return default(GridTile);
        }
    }

    public GridTile GetGridTile(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridTile(x, y);
    }

}
