using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MovementHandler : MonoBehaviour {

    protected static MovementHandler _instance = null;

    public static MovementHandler Instance {
        get {
            if (_instance == null) { _instance = FindObjectOfType<MovementHandler>(); }
            return _instance;
        }
        protected set { _instance = value; }
    }

    protected virtual void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            DestroyImmediate(this);
            return;
        }
    }
   
    public PlayerController playerController;

    public Transform locationParent; 

    public FadeHandler fadeHandler;

    private QuestHandler questHandler;

    private SaveLoadHandler saveLoadHandler;

    private List<GameObject> locations = new List<GameObject>();

    public Grid grid;

    public SuperMap map;

    public GridTile[,] gridTiles;

    // Start is called before the first frame update
    void Start() {
        questHandler = this.GetComponent<QuestHandler>(); 
        saveLoadHandler = this.GetComponent<SaveLoadHandler>();
        
        if (locationParent != null) {
            for (int i = 0; i < locationParent.childCount; i++) {
                GameObject location = locationParent.GetChild(i).gameObject;
                locations.Add(location);
            }
        }
        //Debug.Log("Locations: " + locations.Count);
        if (gridTiles == null) {
            StartCoroutine(LoadMapGrid());
        }
    }

    public bool ValidateMove(Vector3 targetLocation, bool isPlayer, List<TileType> tileTypes) {
        // can be null as it loads
        if (grid == null) {
            return false;
        }

        Vector3Int tilePos = grid.WorldToCell(targetLocation); 
        //Debug.Log("***************************************************");
        //Debug.Log("targetLoc: " + targetLocation + " targetCell: " + tilePos + " count: " + grid.transform.childCount);
        for (int i = 0; i < grid.transform.childCount; i++) {
            GameObject child = grid.transform.GetChild(i).gameObject;
            Tilemap tilemap = child.GetComponent<Tilemap>();   
            //Debug.Log("tilemap: " + tilemap.name + ", rule: " + rule);
            SuperTile tile = tilemap.GetTile<SuperTile>(tilePos);
            if (tile != null) {
                GridTile gridTile = getTileProperties(tile);
                //Debug.Log("Tile matched");
                if (gridTile.cost >= 0) {
                    if (isPlayer) {
                        if (gridTile.tileType == TileType.LOCATION) {
                            foreach (GameObject location in locations) {
                                if (Vector3.Distance(location.transform.position, targetLocation) <= 0.05f) {
                                    //Debug.Log("Enter location: " + location.name);
                                    saveLoadHandler.Save();
                                    StartCoroutine(LoadLocationScene(location.name));
                                    return true;
                                }
                            }
                            return false;
                        } else {
                            questHandler.IncrementTime(gridTile.cost);
                            return true;
                        }
                    } else {
                        return true;
                    }                       
                } else {
                    // only allow the first rule to be processed
                    return false;
                }
            }
        }
        return false;
    }

    private GridTile getTileProperties(SuperTile tile) {
        GridTile gridTile = new GridTile();
        foreach (CustomProperty prop in tile.m_CustomProperties) {
            if (prop.m_Name == "cost") {
                gridTile.cost = prop.GetValueAsInt();
            } else if (prop.m_Name == "tileType") {
                gridTile.tileType = prop.GetValueAsEnum<TileType>();
            }
        }
        return gridTile;
    }
    
    IEnumerator LoadMapGrid() {
        gridTiles = new GridTile[map.m_Width, map.m_Height];
        for (int i = 0; i < map.m_Width; i++) {
            for (int j = 0; j < map.m_Height; j++) {
                Vector3Int tilePos = new Vector3Int(i, j, 0);
                //Debug.Log("tilePos: " + tilePos);
                for (int k = 0; k < grid.transform.childCount; k++) {   
                    GameObject child = grid.transform.GetChild(k).gameObject;
                    Tilemap tilemap = child.GetComponent<Tilemap>();   
                    //Debug.Log("tilemap: " + tilemap.name + ", rule: " + rule);
                    SuperTile tile = tilemap.GetTile<SuperTile>(tilePos);
                    if (tile != null) {
                        GridTile gridTile = getTileProperties(tile);
                        gridTiles[i, j] = gridTile;
                        break;
                    }
                }
            }
            // yield for UI updates
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Grid tiles: " + gridTiles.Length);
    }
    
    IEnumerator LoadLocationScene(string locationName) {
        PlayerPrefs.SetString("locationName", locationName);
        while (saveLoadHandler.isProcessing) {
            yield return new WaitForEndOfFrame();
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Location");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
