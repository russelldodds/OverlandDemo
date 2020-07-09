using UnityEngine;
using System.Collections;

public class SimpleAnimation : MonoBehaviour {

    public Sprite[] left;
    public Sprite[] right;
    public Sprite[] up;
    public Sprite[] down;

    public Transform movePoint;

    public float frameDelay = 0.07f;

    private SpriteRenderer spriteRenderer; 

    private Direction currentDirection = Direction.DOWN;

    private bool isMoving = false;

    // Start is called before the first frame update
    void Start() {  
        isMoving = false;
        currentDirection = Direction.UP;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log("char: " + transform.parent.position + ", target: " + movePoint.position + ", dist: " + Vector3.Distance(transform.parent.position, movePoint.position));           
        if (isMoving && Vector3.Distance(transform.parent.position, movePoint.position) == 0) {
            // end move  
            isMoving = false;
            StopAllCoroutines(); 
            FaceDirection(currentDirection); 
        } else if (!isMoving && Vector3.Distance(transform.parent.position, movePoint.position) >= 0.8f) {
            // start move
            isMoving = true;
            Vector3 vec = movePoint.position - transform.parent.position;
            vec.Normalize();
            if (vec == Vector3.right) {
                currentDirection = Direction.RIGHT;
            } else if (vec == Vector3.left) {
                currentDirection = Direction.LEFT;                              
            } else if (vec == Vector3.up) {
                currentDirection = Direction.UP;
            } else {
                currentDirection = Direction.DOWN;
            }  
            //Debug.Log("Vector: " + vec + ", currentDirection: " + currentDirection); 
            FaceDirection(currentDirection);      
            StopAllCoroutines();
            StartCoroutine(Animate());
        }
    }
    public void FaceDirection(Direction direction) {
        if (!isMoving && spriteRenderer != null) {
            currentDirection = direction;
            switch(currentDirection) {
                case Direction.UP:
                    spriteRenderer.sprite = up[0];
                    break;
                case Direction.LEFT:
                    spriteRenderer.sprite = left[0];
                    break;
                case Direction.RIGHT:
                    spriteRenderer.sprite = right[0];
                    break;
                default:
                    spriteRenderer.sprite = down[0];
                break;
            }
        }
    }

    IEnumerator Animate() {
        Sprite[] sprites;
        switch(currentDirection) {
            case Direction.UP:
                sprites = up;
                break;
            case Direction.LEFT:
                sprites = left;
                break;
            case Direction.RIGHT:
                sprites = right;
                break;
            default:
                sprites = down;
            break;
        }  
        int i = 0;
        while (i < left.Length) {
            spriteRenderer.sprite = sprites[i++];
            yield return new WaitForSeconds(frameDelay);
            yield return 0;
        }
        StartCoroutine(Animate());
    }
}
