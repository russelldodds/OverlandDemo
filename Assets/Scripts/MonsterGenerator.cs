using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour {

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

    private void generateRandomMonster() {
        int rand = Random.Range(0, 1000 * monsterPrefabs.Length) % monsterPrefabs.Length;
        GameObject monsterPrefab = monsterPrefabs[rand];
        MonsterMover mover = monsterPrefab.GetComponent(typeof(MonsterMover)) as MonsterMover;

        Debug.Log("prefab: " + monsterPrefab + ", mover: " + mover);

        int count = 0;
        Vector3 targetLocation = Vector3.forward;
        while (count < 100 && targetLocation == Vector3.forward) {
            targetLocation = getValidTile(mover.allowedTiles);
            count++;
        }

        GameObject monster = Instantiate(monsterPrefab, targetLocation, Quaternion.identity);
        monster.transform.parent = transform;
        mover = monster.GetComponent(typeof(MonsterMover)) as MonsterMover;
        mover.player = playerController.transform;
        mover.movementHandler = movementHandler;
    }

    private Vector3 getValidTile(TileType[] allowedTiles) {
        int randX = Random.Range(0, map.m_Width);
        int randY = Random.Range(0, map.m_Height);
        Vector3 targetLocation  = new Vector3(randX, -randY, 0);
        if (movementHandler.validateMove(targetLocation, false, allowedTiles)) {
            return targetLocation;
        } else {
            return Vector3.forward;
        }
    }

    IEnumerator Respawn() {      
        if (transform.childCount < maximumMonsters) {
            generateRandomMonster();
        }
        yield return new WaitForSeconds(respawnRate);
        StartCoroutine(Respawn());
    }
}
