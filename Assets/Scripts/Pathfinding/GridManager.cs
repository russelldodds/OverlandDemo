using System;
using UnityEngine;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;

public class GridManager : MonoBehaviour {
    public TilemapGroup tilemaps;
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private GridTile[,] gridArray;

    private void Start() {
        STETilemap tilemap = tilemaps.Tilemaps[0];
        this.width = tilemap.GridWidth;
        this.height = tilemap.GridHeight;
        this.cellSize = tilemap.CellSize.x; // TODO: deal with this
        this.originPosition = tilemap.transform.position;

        gridArray = new GridTile[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                GridTile gridTile = new GridTile(this, x, y);
                if (tilemaps != null) {
                    tilemaps.Tilemaps.Reverse();
                    foreach (STETilemap checkmap in tilemaps.Tilemaps) {
                        Tile tile = checkmap.GetTile(x, y);
                        if (tile != null) {
                            //Debug.Log("Matched Tile Map: " + tilemap.name);
                            gridTile.SetCost(tile.paramContainer.GetIntParam("cost", 0));
                            Enum.TryParse<TileType>(tile.paramContainer.GetStringParam("type", TileType.BASE.ToString()), out TileType tileType);
                            gridTile.SetTileType(tileType);
                            break;
                        }                       
                    }
                }
                gridArray[x, y] = gridTile;
            }
        }

        bool showDebug = false;
        if (showDebug) {
            //TextMesh[,] debugTextArray = new TextMesh[width, height];

             for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    //debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * .5f, 30, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);


            // OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) => {
            //     debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y]?.ToString();
            // };
        }
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public float GetCellSize() {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y) {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetGridTile(int x, int y, GridTile value) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
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
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        } else {
            return default(GridTile);
        }
    }

    public GridTile GetGridTile(Vector3 worldPosition) {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridTile(x, y);
    }

    public bool ValidateMove(Vector3 targetLocation, List<TileType> tileTypes) {
        GridTile tile = GetGridTile(targetLocation);
        //Debug.Log("Matched tile: " + tile);
        if (tile == null || tile.cost < 0 || !tileTypes.Contains(tile.tileType)) {
            return false;
        }  
        return true;                     
    }

}
