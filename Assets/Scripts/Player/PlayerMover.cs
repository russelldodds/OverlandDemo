using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerMover : MonoBehaviour {
    public float speed = 5f;
    public Vector3 nextTile = Vector3.forward;
    public Vector3 finalTile = Vector3.forward;
    public List<Vector3> path;
    public int index = 0;
  
    void Start() {
    }
    // Update is called once per frame
    void Update() {
        if (nextTile != Vector3.forward) {
            transform.position = Vector3.MoveTowards(transform.position, nextTile, speed * Time.deltaTime);
         
            if (path != null && index <= path.Count - 1) {
                if (Vector3.Distance(transform.position, nextTile) == 0f) {
                    GridManager.Instance.IncrementTime(transform.position);
                    nextTile = path[index++];
                    GetComponentInChildren<CharacterAnimation>().AnimateDirection(nextTile, true);
                    Debug.Log("Move to: " + nextTile);
                }
            } else if (Vector3.Distance(transform.position, finalTile) == 0f) {
                ResetPath();
                GridManager.Instance.CheckEntrance(transform.position);
            }
        }
    }

    void ResetPath() {
        path.Clear();
        index = 0;
        nextTile = Vector3.forward;
        finalTile = Vector3.forward;
        GetComponentInChildren<CharacterAnimation>().AnimateIdle();
    }

    public void SetPath(List<Vector3> path) {
        if (path != null && path.Count > 0) {
            this.path = path;
            index = 0;
            nextTile = path[index++];
            finalTile = path[path.Count - 1];
        }
    }
}