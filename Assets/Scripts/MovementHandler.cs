using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;
using UnityEngine.Tilemaps;

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

    private QuestHandler questHandler;

    private SaveLoadHandler saveLoadHandler;

    private List<GameObject> entrances = new List<GameObject>();

    private Dictionary<string, Rules> rules = new Dictionary<string, Rules>();

    // Start is called before the first frame update
    void Start() {
        questHandler = this.GetComponent(typeof(QuestHandler)) as QuestHandler; 
        saveLoadHandler = this.GetComponent(typeof(SaveLoadHandler)) as SaveLoadHandler;
        
        Transform entranceParent = transform.Find("Entrances");
        for (int i = 0; i < entranceParent.childCount; i++) {
            GameObject child = entranceParent.GetChild(i).gameObject;
            entrances.Add(child);
        }

        for (int i = 0; i < grid.transform.childCount; i++) {
            Rules rule = new Rules();
            GameObject child = grid.transform.GetChild(i).gameObject;
            SuperCustomProperties props = child.GetComponent(typeof(SuperCustomProperties)) as SuperCustomProperties;

            CustomProperty propCost;
            props.TryGetCustomProperty("cost", out propCost);
            if (propCost != null) {
                rule.cost = propCost.GetValueAsInt();
            } else {
                rule.cost = 0;
            }

            CustomProperty propRequired;
            props.TryGetCustomProperty("required", out propRequired);
            if (propRequired != null) {
                rule.tileType = propRequired.GetValueAsEnum<TileType>();
            } else {
                rule.tileType = TileType.NONE;
            }         

            rules.Add(child.name, rule);
            //Debug.Log("tilemap: " + child.name + ", rule: " + rule);
        }
    }

    public bool ValidateMove(Vector3 targetLocation, bool isPlayer, TileType[] tileTypes) {
        Vector3Int tilePos = grid.WorldToCell(targetLocation); 
        //Debug.Log("***************************************************");
        //Debug.Log("targetLoc: " + targetLocation + " targetCell: " + tilePos + " count: " + grid.transform.childCount);
        
        for (int i = 0; i < grid.transform.childCount; i++) {
            GameObject child = grid.transform.GetChild(i).gameObject;
           
            Rules rule;
            rules.TryGetValue(child.name, out rule);
            if (rule != null) {
                Tilemap tilemap = child.GetComponent(typeof(Tilemap)) as Tilemap;   
                //Debug.Log("tilemap: " + tilemap.name + ", rule: " + rule);

                TileBase tile = tilemap.GetTile(tilePos);
                if (tile != null) {
                    //Debug.Log("Tile matched");
                    if (IsAllowedTile(rule.tileType, tileTypes)) {
                        if (isPlayer) {
                            if (rule.tileType == TileType.ENTRANCE) {
                                foreach (GameObject entrance in entrances) {
                                    if (Vector3.Distance(entrance.transform.position, targetLocation) <= 0.05f) {
                                        //Debug.Log("Enter location: " + entrance.name);
                                        saveLoadHandler.Save();
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
}
