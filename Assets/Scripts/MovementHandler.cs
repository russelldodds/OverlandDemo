using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;
using UnityEngine.Tilemaps;

public class MovementHandler : MonoBehaviour {

    private class Rules {
        public int cost;
        public QuestItem requiredItem;

        public override string ToString() {
            return "cost: " + cost + ", requiredItem: " + requiredItem;
        }
    }

    public Grid grid;

    private PlayerInventory inventory;

    private QuestHandler questHandler;

    private List<GameObject> entrances = new List<GameObject>();

    private Dictionary<string, Rules> rules = new Dictionary<string, Rules>();

    // Start is called before the first frame update
    void Start() {
        inventory = this.GetComponent(typeof(PlayerInventory)) as PlayerInventory; 
        questHandler = this.GetComponent(typeof(QuestHandler)) as QuestHandler; 
        
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
                rule.requiredItem = propRequired.GetValueAsEnum<QuestItem>();
            } else {
                rule.requiredItem = QuestItem.NONE;
            }         

            rules.Add(child.name, rule);
            //Debug.Log("tilemap: " + child.name + ", rule: " + rule);
        }
    }

    public bool movePlayer(Vector3 targetLocation) {
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

                    if (inventory.hasRequiredItem(rule.requiredItem)) {
                        questHandler.incrementTime(rule.cost);
                        return true;
                    } else if (rule.requiredItem == QuestItem.ENTRANCE) {
                        foreach (GameObject entrance in entrances) {
                            if (Vector3.Distance(entrance.transform.position, targetLocation) <= 0.05f) {
                                //Debug.Log("Enter location: " + entrance.name);
                                return true;
                            }
                        }
                        return false;
                    } else {
                        // only allow the first rule to be processed
                        return false;
                    }
                }
            }
        }
        return false;
    }
}
