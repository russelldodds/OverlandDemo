using System;
using UnityEngine;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;

public class GridManager : Singleton<GridManager> {
    private const string WORLD = "World";
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs {
        public int x;
        public int y;
    }
    static int width;
    static int height;
    static float cellSize;
    static Vector3 originPosition;
    static GridTile[,] gridArray;
    static Dictionary<string, GridTile[,]> grids = new Dictionary<string, GridTile[,]>();
    static Dictionary<string, LocationTarget> entrances = new Dictionary<string, LocationTarget>();
    static Dictionary<string, TilemapGroup> maps = new Dictionary<string, TilemapGroup>();
    static TilemapGroup tilemapGroup;

    private void Start() {
    }

    public void InitializeScene(TilemapGroup[] groups, LocationTarget[] locations) {
        grids.Clear();
        entrances.Clear();
        maps.Clear();
        tilemapGroup = null;

        if (groups != null && groups.Length > 0) {
            foreach (TilemapGroup group in groups) {
                maps.Add(group.name, group);           
            }
        } 
        Debug.Log("Map count: " + maps.Count);

        if (locations != null && locations.Length > 0) {
            foreach (LocationTarget location in locations) {
                entrances.Add(location.name, location);  
            }
        } 
        Debug.Log("Entrance count: " + entrances.Count);      
    }

    public void LoadMap(string locationName) {
        if (!maps.ContainsKey(locationName)) {
            Dictionary<string, TilemapGroup>.Enumerator itr = maps.GetEnumerator();
            itr.MoveNext();
            KeyValuePair<string, TilemapGroup> entry = itr.Current;
            Debug.Log(entry);
            locationName = entry.Key;
            tilemapGroup = entry.Value;
        } else {
            maps.TryGetValue(locationName, out tilemapGroup);        
        }
    
        // load the grid
        if (tilemapGroup == null) {
            // something went horribly wrong
            Debug.Log("Something went horribly wrong");
            return;
        }

        Debug.Log("Load Location: " + locationName + ", tilemap: " + tilemapGroup);

        // toggle active state
        tilemapGroup.gameObject.SetActive(true);
        foreach (TilemapGroup location in maps.Values) {
            if (!location.name.Equals(locationName)) {
                location.gameObject.SetActive(false);          
            }
        }       

        // place the player
        LocationData locationData = tilemapGroup.GetComponent<LocationData>();
        if (locationName.Equals(WORLD)) {
            // load from prefs, otherwise starting location
            Vector3 playerPosition = PlayerPrefsX.GetVector3("playerPosition", locationData.startingLocation);
            // seems like the laoding causes float errors
            playerPosition.x = Mathf.FloorToInt(playerPosition.x) + 0.5f;
            playerPosition.y = Mathf.FloorToInt(playerPosition.y) + 0.5f;
            EventManager.TriggerEvent("SetPlayerLocation", new Dictionary<string, object> { 
                { "position", playerPosition },
                { "direction", Direction.DOWN }
            });
        } else {
            EventManager.TriggerEvent("SetPlayerLocation", new Dictionary<string, object> { 
                { "position", locationData.startingLocation }, 
                { "direction", Direction.UP } 
            });
        }
            
        STETilemap tilemap = tilemapGroup.Tilemaps[0];
        width = tilemap.GridWidth;
        height = tilemap.GridHeight;
        cellSize = tilemap.CellSize.x; // TODO: deal with this
        originPosition = tilemap.transform.position;

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

            bool showDebug = true;
            if (showDebug) {
                for (int x = 0; x < gridArray.GetLength(0); x++) {
                    for (int y = 0; y < gridArray.GetLength(1); y++) {
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.gray, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.gray, 100f);
                    }
                }
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.gray, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.gray, 100f);
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
            EventManager.TriggerEvent("IncrementTime", new Dictionary<string, object> { 
                { "minutes", tile.cost }
            });
        }               
    }

    public void CheckEntrance(Vector3 targetLocation) {
        Debug.Log("Active tilemapGroup: " + tilemapGroup);
        
        LocationData locationData = tilemapGroup.GetComponent<LocationData>();       
        bool savePlayerPosition = false;
        if (locationData.sceneName.Equals(WORLD)) {
            savePlayerPosition = true;
        }
        Debug.Log("Check Entrances at: " + targetLocation);
        foreach (KeyValuePair<string, LocationTarget> entry in entrances) {
            Debug.Log("Check location: " + entry);  
            if (Vector3.Distance(entry.Value.transform.position, targetLocation) <= 0.5f) {
                Debug.Log("Enter location: " + entry.Key);                
                EventManager.TriggerEvent("SaveGame", new Dictionary<string, object> { 
                    { "savePlayerPosition", savePlayerPosition }
                });
                if (entry.Value.sceneName.Equals(locationData.sceneName)) {
                    PlayerPrefs.SetString("locationName", entry.Value.activeLocation);
                    LoadMap(entry.Key);
                } else {
                    EventManager.TriggerEvent("LoadScene", new Dictionary<string, object> { 
                        { "sceneName", entry.Value.sceneName }, 
                        { "activeLocation", entry.Value.activeLocation }
                    });
                }  
                break;             
            }
        }

        // check location edges
        if (locationData.exitAtEdge && 
                (targetLocation.x < 1 || targetLocation.x > width - 1 || 
                targetLocation.y < 1 || targetLocation.y > height - 1)) {
            EventManager.TriggerEvent("SaveGame", new Dictionary<string, object> { 
                { "savePlayerPosition", false }
            });
            EventManager.TriggerEvent("LoadScene", new Dictionary<string, object> { 
                { "sceneName", WORLD }, 
                { "activeLocation", WORLD }
            });
        }        
    }
}
