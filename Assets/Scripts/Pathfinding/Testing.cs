using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SuperTiled2Unity;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour {
    public SuperMap map;

    public GameObject player;

    private Pathfinding pathfinding;

    private void Start() {
        pathfinding = new Pathfinding(map);
    }

    private void Update() {       
        if (Input.GetMouseButtonDown(0)) {
            StopAllCoroutines();
            pathfinding.GetGridManager().GetXY(player.transform.position, out int px, out int py);
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            pathfinding.GetGridManager().GetXY(mouseWorldPosition, out int x, out int y);
            //Debug.Log("mouseWorldPosition: " + mouseWorldPosition +  ", x: " + x +  ", y: " + y);
            List<GridTile> path = pathfinding.FindPath(px, py, x, y);
            StartCoroutine(TestMove(path));
        }
    }

    IEnumerator TestMove(List<GridTile> path) {
        if (path != null) {
            for (int i=0; i<path.Count; i++) {

                Vector3 target = new Vector3(path[i].x, -path[i].y);
                player.transform.position = target;
                //Debug.Log(target);
                //Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i+1].x, path[i+1].y) * 10f + Vector3.one * 5f, Color.green, 5f);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}