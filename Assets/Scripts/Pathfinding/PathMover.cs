using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathMover : MonoBehaviour {

    public float speed = 5f;

    public Vector3 nextTile = Vector3.forward;

    public List<Vector3> path;

    public int index = 0;

    // Update is called once per frame
    void Update() {
        if (nextTile != Vector3.forward) {
            transform.position = Vector3.MoveTowards(transform.position, nextTile, speed * Time.deltaTime);
        }
        
        if (path != null && index <= path.Count - 1) {
            if (Vector3.Distance(transform.position, nextTile) == 0f) {
                nextTile = path[index++];
                // Vector3 vec = nextTile - transform.position;
                // vec.Normalize();
                // if (vec != Vector3.zero) {
                GetComponentInChildren<CharacterAnimation>().AnimateDirection(nextTile, true);
                //}
                Debug.Log("Move to: " + nextTile);
            }
        } 
        // else if (nextTile != Vector3.forward) {
        //     nextTile = Vector3.forward;
        //     GetComponentInChildren<CharacterAnimation>().AnimateIdle();
        // }
    }

    public void SetPath(List<Vector3> path) {
        if (path != null && path.Count > 0) {
            this.path = path;
            index = 0;
            nextTile = path[index++];
        }
    }
}