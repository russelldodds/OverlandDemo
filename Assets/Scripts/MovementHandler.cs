using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MovementHandler : MonoBehaviour {

    private class Rules {
        public int cost;
        public TileType tileType;

        public override string ToString() {
            return "cost: " + cost + ", tileType: " + tileType;
        }
    }

    public PlayerController playerController;

    public Grid grid;

    public Transform locationParent; 

    public FadeHandler fadeHandler;

    private QuestHandler questHandler;

    private SaveLoadHandler saveLoadHandler;

    private List<GameObject> locations = new List<GameObject>();

    private Dictionary<string, Rules> rules = new Dictionary<string, Rules>();

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
        StartCoroutine(LoadLRules());
    }

    public bool ValidateMove(Vector3 targetLocation, bool isPlayer, TileType[] tileTypes) {
        // can be null as it loads
        if (grid == null || rules == null) {
            return false;
        }

        Vector3Int tilePos = grid.WorldToCell(targetLocation); 
        //Debug.Log("***************************************************");
        //Debug.Log("targetLoc: " + targetLocation + " targetCell: " + tilePos + " count: " + grid.transform.childCount);
        
        for (int i = 0; i < grid.transform.childCount; i++) {
            GameObject child = grid.transform.GetChild(i).gameObject;
           
            Rules rule;
            rules.TryGetValue(child.name, out rule);
            if (rule != null) {
                Tilemap tilemap = child.GetComponent<Tilemap>();   
                //Debug.Log("tilemap: " + tilemap.name + ", rule: " + rule);

                TileBase tile = tilemap.GetTile(tilePos);
                if (tile != null) {
                    //Debug.Log("Tile matched");
                    if (IsAllowedTile(rule.tileType, tileTypes)) {
                        if (isPlayer) {
                            if (rule.tileType == TileType.LOCATION) {
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
                                questHandler.IncrementTime(rule.cost);
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
        }
        return false;
    }

    private bool IsAllowedTile(TileType tileType, TileType[] tileTypes) {
        foreach (TileType checkTile in tileTypes) {
            if (checkTile == tileType) {
                return true;
            }
        }
        return false;
    } 

    IEnumerator LoadLRules() {
        while (grid == null) {
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < grid.transform.childCount; i++) {
            Rules rule = new Rules();
            GameObject child = grid.transform.GetChild(i).gameObject;
            SuperCustomProperties props = child.GetComponent<SuperCustomProperties>();

            CustomProperty propCost;
            props.TryGetCustomProperty("cost", out propCost);
            if (propCost != null) {
                rule.cost = propCost.GetValueAsInt();
            } else {
                rule.cost = 0;
            }

            CustomProperty propRequired;
            props.TryGetCustomProperty("tileType", out propRequired);
            if (propRequired != null) {
                rule.tileType = propRequired.GetValueAsEnum<TileType>();
            } else {
                rule.tileType = TileType.BASE;
            }         

            rules.Add(child.name, rule);
            //Debug.Log("tilemap: " + child.name + ", rule: " + rule);
        }
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
