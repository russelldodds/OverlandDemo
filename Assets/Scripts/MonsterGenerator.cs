using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour, IDataSerialiizer {

    public int maximumMonsters;

    public int respawnRate;

    public PlayerController playerController;

    public GameObject[] monsterPrefabs;

    private MovementHandler movementHandler;

    public SuperTiled2Unity.SuperMap map;

    // Start is called before the first frame update
    IEnumerator Start() {
        movementHandler = GetComponentInParent(typeof(MovementHandler)) as MovementHandler;
        yield return new WaitForSeconds(respawnRate);
        StartCoroutine(Respawn());
    }

    private void GenerateRandomMonster() {
        int monsterType = Random.Range(0, 1000 * monsterPrefabs.Length) % monsterPrefabs.Length;
        
        GameObject monsterPrefab = monsterPrefabs[monsterType];
        MonsterMover mover = monsterPrefab.GetComponent(typeof(MonsterMover)) as MonsterMover;

        int count = 0;
        Vector3 targetLocation = Vector3.forward;
        while (count < 100 && targetLocation == Vector3.forward) {
            targetLocation = GetValidTile(mover.allowedTiles);
            count++;
        }

        InstantiateMonster(monsterPrefab, mover, targetLocation, monsterType);       
    }

    private void InstantiateMonster(GameObject monsterPrefab, MonsterMover mover, Vector3 targetLocation, int monsterType) {
        //Debug.Log("prefab: " + monsterPrefab + ", mover: " + mover);
        GameObject monster = Instantiate(monsterPrefab, targetLocation, Quaternion.identity);
        monster.transform.parent = transform;
        mover = monster.GetComponent(typeof(MonsterMover)) as MonsterMover;
        mover.player = playerController.transform;
        mover.movementHandler = movementHandler;
        mover.monsterType = monsterType;
    }

    private Vector3 GetValidTile(TileType[] allowedTiles) {
        int randX = Random.Range(0, map.m_Width);
        int randY = Random.Range(0, map.m_Height);
        Vector3 targetLocation  = new Vector3(randX, -randY, 0);
        if (movementHandler.ValidateMove(targetLocation, false, allowedTiles)) {
            return targetLocation;
        } else {
            return Vector3.forward;
        }
    }

    IEnumerator Respawn() {      
        if (transform.childCount < maximumMonsters) {
            GenerateRandomMonster();
        }
        yield return new WaitForSeconds(respawnRate);
        StartCoroutine(Respawn());
    }

    public void Save() {
        Vector3[] monsterLocations = new Vector3[transform.childCount];
        int[] monsterTypes = new int[transform.childCount];

        for (int i = 0; i < transform.childCount; i++) {
            MonsterMover mover = transform.GetChild(i).GetComponent(typeof(MonsterMover)) as MonsterMover;
            monsterLocations[i] = mover.transform.position;
            monsterTypes[i] = mover.monsterType;
        }

        PlayerPrefsX.SetVector3Array("monsterLocations", monsterLocations);
        PlayerPrefsX.SetIntArray("monsterTypes", monsterTypes);
    }

    public void Load() {
        StopAllCoroutines();
        Vector3[] monsterLocations = PlayerPrefsX.GetVector3Array("monsterLocations");
        int[] monsterTypes = PlayerPrefsX.GetIntArray("monsterTypes");
        if (monsterLocations != null && monsterTypes != null) {
            foreach (Transform child in transform) {
                GameObject.Destroy(child.gameObject);
            }

            for (int i = 0; i < monsterLocations.Length; i++) {
                int monsterType = monsterTypes[i];
                Vector3 targetLocation = monsterLocations[i];

                GameObject monsterPrefab = monsterPrefabs[monsterType];
                MonsterMover mover = monsterPrefab.GetComponent(typeof(MonsterMover)) as MonsterMover;

                InstantiateMonster(monsterPrefab, mover, targetLocation, monsterType); 
            }
        }
        StartCoroutine(Respawn());
    }
}
