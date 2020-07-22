using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;

public class MonsterManager : MonoBehaviour {
    public int minimumCount = 2;
    public PlayerController playerController;
    public GameObject monsterPrefab;
    public List<Transform> startingLocations;
    private GridManager gridManager;

    // Start is called before the first frame update
    void Start() {
        gridManager = GridManager.Instance;
        PlaceMonsters();
    }

    private void PlaceMonsters() {
        int count = Random.Range(minimumCount, startingLocations.Count + 1);
        
        for (int i = 0; i < count; i++) {
            // TODO: randomize the positions
            InstantiateMonster(monsterPrefab, startingLocations[i].position);
        }

               
    }

    private void InstantiateMonster(GameObject monsterPrefab, Vector3 targetLocation) {
        //Debug.Log("prefab: " + monsterPrefab + ", mover: " + mover);
        GameObject monster = Instantiate(monsterPrefab, targetLocation, Quaternion.identity);
        monster.transform.parent = transform;
    }
}
