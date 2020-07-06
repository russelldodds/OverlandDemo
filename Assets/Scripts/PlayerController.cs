using UnityEngine;

public class PlayerController : MonoBehaviour {

    public MovementHandler movementHandler;

    public float speed = 5f;

    public Transform movePoint;

    public Sprite left;
    public Sprite right;
    public Sprite up;
    public Sprite down;

    enum Direction {
        UP,
        DOWN,
        LEFT, 
        RIGHT
    }

    private Direction prevDirection = Direction.DOWN;

    private Animator animator;

    // Start is called before the first frame update
    void Start() {
        movePoint.parent = null;     
        animator = this.GetComponentInChildren(typeof(Animator)) as Animator;
    }

    // Update is called once per frame
    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f) {
            animator.gameObject.SetActive(false);
            animator.gameObject.SetActive(true);
            
            Vector3 targetLocation = Vector3.forward;
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f) {
                targetLocation = movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f); 
            } else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f) {
                targetLocation = movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
            } 

            if (targetLocation != Vector3.forward && movementHandler.movePlayer(targetLocation)) {
                movePoint.position = targetLocation; 
                // play animations
                if (Input.GetAxisRaw("Horizontal") == 1f) {
                    prevDirection = Direction.RIGHT;
                    animator.Play("Walk_Right");
                } else if (Input.GetAxisRaw("Horizontal") == -1f) {
                    prevDirection = Direction.LEFT;
                    animator.Play("Walk_Left");
                } else if (Input.GetAxisRaw("Vertical") == 1f) {
                    prevDirection = Direction.UP;
                    animator.Play("Walk_Up");
                } else if (Input.GetAxisRaw("Vertical") == -1f) {
                    prevDirection = Direction.DOWN;
                    animator.Play("Walk_Down");
                }  
            } else {
                switch(prevDirection) {
                    case Direction.UP:
                        animator.Play("Walk_Up", -1, 0);
                        break;
                    case Direction.LEFT:
                        animator.Play("Walk_Left", -1, 0);
                        break;
                    case Direction.RIGHT:
                        animator.Play("Walk_Right", -1, 0);
                        break;
                    default:
                        animator.Play("Walk_Down", -1, 0);
                    break;
                }   
            }
        }
    }
}
