using System;
using UnityEngine;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;

public class GridManager : Singleton<GridManager> {
    private const string WORLD = "World";
    public PlayerController playerController;
    public GameObject[] locations;
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }
    public bool isLoading = false;

    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPosition;
    private GridTile[,] gridArray;
    private Dictionary<string, GridTile[,]> grids = new Dictionary<string, GridTile[,]>();
    private Dictionary<string, Vector3> entrances = new Dictionary<string, Vector3>();
    private Dictionary<string, TilemapGroup> maps = new Dictionary<string, TilemapGroup>();
    private SaveLoadHandler saveLoadHandler;
    private QuestHandler questHandler;

    private void Start() {
        PlayerPrefs.SetString("locationName", WORLD);
        questHandler = GetComponent<QuestHandler>();
        saveLoadHandler = GetComponent<SaveLoadHandler>();
        Debug.Log("Start GridManager");
        if (locations != null && locations.Length > 0) {
            foreach (GameObject location in locations) {
                entrances.Add(location.name, location.transform.position); 
                maps.Add(location.name, location.GetComponentInChildren<TilemapGroup>());        
            }
        }
        Debug.Log(maps.Count);

        // Load the world by default
        LoadMap(WORLD);
    }

    public void LoadMap(string locationName) {
        isLoading = true;
        //GetComponent<FadeHandler>().FadeOut();
        StartCoroutine(GetComponent<FadeHandler>().FadeOut());
        if (!maps.ContainsKey(locationName)) {
            locationName = WORLD;
        }

        Debug.Log("Load Location: " + locationName);

        // toggle active state
        foreach (TilemapGroup location in maps.Values) {
            if (location.name.Equals(locationName)) {
                location.gameObject.SetActive(true);
            } else {
                location.gameObject.SetActive(false);          
            }
        }

        // place the player
        if (locationName.Equals(WORLD)) {
            // load from prefs, otherwise starting location
            Vector3 playerPosition = PlayerPrefsX.GetVector3("playerPosition", playerController.startingLocation);
            // seems like the laoding causes float errors
            playerPosition.x = Mathf.FloorToInt(playerPosition.x) + 0.5f;
            playerPosition.y = Mathf.FloorToInt(playerPosition.y) + 0.5f;
            playerController.transform.position = playerPosition;
        } else {
            playerController.transform.position = new Vector3(31.5f, 2.5f, 0.5f);
        }
            
        // load the grid
        PlayerPrefs.SetString("locationName", locationName);       
        maps.TryGetValue(locationName, out TilemapGroup tilemapGroup);

        STETilemap tilemap = tilemapGroup.Tilemaps[0];
        this.width = tilemap.GridWidth;
        this.height = tilemap.GridHeight;
        this.cellSize = tilemap.CellSize.x; // TODO: deal with this
        this.originPosition = tilemap.transform.position;

        grids.TryGetValue(tilemapGroup.name, out gridArray);
        if (gridArray == null || gridArray.Length <= 0) {
            gridArray = new GridTile[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    GridTile gridTile = new GridTile(this, x, y);
                    if (tilemapGroup != null) {
                        tilemapGroup.Tilemaps.Reverse();
                        foreach (STETilemap checkmap in tilemapGroup.Tilemaps) {
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

            // cache for reuse
            grids.Add(tilemapGroup.name, gridArray);

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

    public void IncrementTime(Vector3 location) {
        GridTile tile = GetGridTile(location);
        if (tile != null && tile.cost > 0) {
            questHandler.IncrementTime(tile.cost);
        }               
    }

    public void CheckEntrance(Vector3 targetLocation) {
        Debug.Log("Check Entrance: " + targetLocation);
        if (PlayerPrefs.GetString("locationName", WORLD).Equals(WORLD)) {
            foreach (KeyValuePair<string, Vector3> entry in entrances) {
                if (Vector3.Distance(entry.Value, targetLocation) <= 0.5f) {
                    Debug.Log("Enter location: " + entry.Key);                
                    saveLoadHandler.Save(true);
                    LoadMap(entry.Key);
                }
            }
        } else  {
            if (targetLocation.x < 1 || targetLocation.x > width - 1 || 
            targetLocation.y < 1 || targetLocation.y > height - 1) {
                saveLoadHandler.Save(false);
                LoadMap(WORLD);
            }
        }        
    }
}
