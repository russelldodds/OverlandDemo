using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 5f;
    public Transform movePoint;

    public LayerMask collisionLayer;

    // Start is called before the first frame update
    void Start() {
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f) {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
                Vector3 loc = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                if (!Physics2D.OverlapCircle(movePoint.position + loc, 0.2f, collisionLayer)) {
                    movePoint.position += loc;
                }               
            } else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f) {
                Vector3 loc = new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                if (!Physics2D.OverlapCircle(movePoint.position + loc, 0.2f, collisionLayer)) {
                    movePoint.position += loc;
                }
            }
        }
    }
}
