using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using CreativeSpore.SuperTilemapEditor;

public class MovementHandler : MonoBehaviour {
    public PlayerController playerController;

    public Transform locationParent; 

    public FadeHandler fadeHandler;

    private QuestHandler questHandler;

    private SaveLoadHandler saveLoadHandler;

    private List<GameObject> locations = new List<GameObject>();

    public STETilemap map;

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
    }

    public bool ValidateMove(Vector3 targetLocation, bool isPlayer, List<TileType> tileTypes) {
        // can be null as it loads
        // if (grid == null) {
        //     return false;
        // }

        // Vector3Int tilePos = grid.WorldToCell(targetLocation); 
        // //Debug.Log("***************************************************");
        // //Debug.Log("targetLoc: " + targetLocation + " targetCell: " + tilePos + " count: " + grid.transform.childCount);
        // for (int i = 0; i < grid.transform.childCount; i++) {
        //     GameObject child = grid.transform.GetChild(i).gameObject;
        //     Tilemap tilemap = child.GetComponent<Tilemap>();   
        //     //Debug.Log("tilemap: " + tilemap.name + ", rule: " + rule);
        //     SuperTile tile = tilemap.GetTile<SuperTile>(tilePos);
        //     if (tile != null) {
        //         //GridTile gridTile = getTileProperties(tile);
        //         //Debug.Log("Tile matched");
        //         // if (gridTile.cost >= 0) {
        //         //     if (isPlayer) {
        //         //         if (gridTile.tileType == TileType.LOCATION) {
        //         //             foreach (GameObject location in locations) {
        //         //                 if (Vector3.Distance(location.transform.position, targetLocation) <= 0.05f) {
        //         //                     //Debug.Log("Enter location: " + location.name);
        //         //                     saveLoadHandler.Save();
        //         //                     StartCoroutine(LoadLocationScene(location.name));
        //         //                     return true;
        //         //                 }
        //         //             }
        //         //             return false;
        //         //         } else {
        //         //             questHandler.IncrementTime(gridTile.cost);
        //         //             return true;
        //         //         }
        //         //     } else {
        //         //         return true;
        //         //     }                       
        //         // } else {
        //         //     // only allow the first rule to be processed
        //         //     return false;
        //         // }
        //         return true;
        //     }
        // }
        return true;
    }

    // private GridTile getTileProperties(SuperTile tile) {
    //     GridTile gridTile = new GridTile(tile);
    //     foreach (CustomProperty prop in tile.m_CustomProperties) {
    //         if (prop.m_Name == "cost") {
    //             gridTile.cost = prop.GetValueAsInt();
    //         } else if (prop.m_Name == "tileType") {
    //             gridTile.tileType = prop.GetValueAsEnum<TileType>();
    //         }
    //     }
    //     return gridTile;
    // }
    
    IEnumerator LoadMapGrid() {
        gridTiles = new GridTile[map.GridWidth, map.GridHeight];
        for (int i = 0; i < map.GridWidth; i++) {
            for (int j = 0; j < map.GridHeight; j++) {
                // Vector3Int tilePos = new Vector3Int(i, j, 0);
                // //Debug.Log("tilePos: " + tilePos);
                // for (int k = 0; k < grid.transform.childCount; k++) {   
                //     GameObject child = grid.transform.GetChild(k).gameObject;
                //     Tilemap tilemap = child.GetComponent<Tilemap>();   
                //     //Debug.Log("tilemap: " + tilemap.name + ", rule: " + rule);
                //     SuperTile tile = tilemap.GetTile<SuperTile>(tilePos);
                //     if (tile != null) {
                //         GridTile gridTile = new GridTile();
                //         gridTiles[i, j] = gridTile;
                //         break;
                //     }
                // }
            }
            // yield for UI updates
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Grid tiles: " + gridTiles.Length);
    }
    

}
