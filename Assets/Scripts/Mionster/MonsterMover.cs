using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;

public class MonsterMover : MonoBehaviour {

    public int range = 10;
    public float lerpSpeed = 5f;
    public float attackDelay = 0.25f;
    public Transform player;
    public Transform movePoint;
    public List<TileType> allowedTiles;
    public GridManager gridManager;
    public int monsterType;
    private bool isMoving = true;

    // Start is called before the first frame update
    void Start() {
        movePoint.parent = null;  
        StartCoroutine(HandleMove());   
    }

    // Update is called once per frame
    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, lerpSpeed * Time.deltaTime);

        // check for fight
        if (Vector3.Distance(transform.position, player.position) <= 0.01f) {
            isMoving = false;
            Debug.Log("**********************  FIGHT  **********************");
            StopAllCoroutines();
            KillMonster();
        } else if (!isMoving && Vector3.Distance(transform.position, player.position) > 2f) {
            isMoving = true;
            StartCoroutine(HandleMove()); 
        }
    }

    IEnumerator HandleMove() {
        float delay = attackDelay;   
        Vector3 targetLocation = Vector3.forward;   
        if (Vector3.Distance(transform.position, player.position) <= range) {
            // move towards the player
            Vector3 vec = transform.position - player.position;
            vec.Normalize();
            //Debug.Log("Vector: " + vec);     
            if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y)) {
                // use x
                if (vec.x > 0) {
                    targetLocation = movePoint.position + Vector3.left;
                } else {
                    targetLocation = movePoint.position + Vector3.right;
                }
            } else {
                // use y
                if (vec.y > 0) {
                    targetLocation = movePoint.position + Vector3.down;
                } else {
                    targetLocation = movePoint.position + Vector3.up;
                }
            }
        } else {  
            // mill around
            int dir = Random.Range(0, 300) % 4;
            delay = Random.Range(100f, 600f) / 100;
            switch(dir) {
                case 0:
                    targetLocation = movePoint.position + Vector3.up;
                    break;
                case 1:
                    targetLocation = movePoint.position + Vector3.left;
                    break;
                case 2:
                    targetLocation = movePoint.position + Vector3.right;
                    break;
                case 3:
                    targetLocation = movePoint.position + Vector3.down;
                break;
            }
        }    

        if (targetLocation != Vector3.forward && gridManager.ValidateMove(targetLocation, allowedTiles)) {
            movePoint.position = targetLocation;
        } else {
            // illegal move, try agian next frame
            delay = 0;
        }    
               
        //Debug.Log("Target: " + movePoint.position + ", delay: " + delay);                      
        yield return new WaitForSeconds(delay);
        StartCoroutine(HandleMove());
    }

    private void KillMonster() {
        Destroy(movePoint.gameObject);
        Destroy(gameObject);
    }

}
