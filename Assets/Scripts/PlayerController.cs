using UnityEngine;

public class PlayerController : MonoBehaviour {

    public MovementHandler movementHandler;

    public float speed = 5f;

    public Transform movePoint;

    // Start is called before the first frame update
    void Start() {
        movePoint.parent = null;          
    }

    // Update is called once per frame
    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f) {
            Vector3 targetLocation = Vector3.forward;
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
                targetLocation = movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);     
            } else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f) {
                targetLocation = movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            }

            if (targetLocation != Vector3.forward && movementHandler.movePlayer(targetLocation)) {
                movePoint.position = targetLocation;       
            }
        }
    }
}
