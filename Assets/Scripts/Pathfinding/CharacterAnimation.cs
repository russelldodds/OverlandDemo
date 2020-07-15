using UnityEngine;
using System.Collections;

public class CharacterAnimation : MonoBehaviour {

    public Sprite[] left;
    public Sprite[] right;
    public Sprite[] up;
    public Sprite[] down;

    public float frameDelay = 0.07f;

    private SpriteRenderer spriteRenderer; 

    private Direction currentDirection = Direction.DOWN;

    private Vector3 targetLoc;

    // Start is called before the first frame update
    void Start() {  
        currentDirection = Direction.UP;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AnimateDirection(Vector3 targetLoc, bool isMoving) {
        this.targetLoc = targetLoc;
        Vector3 vec = targetLoc - transform.position;
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
        Debug.Log("facing vec: " + vec + ", direction: " + currentDirection + ", isMoving: " + isMoving);
        AnimateDirection(currentDirection, isMoving);
    }

    public void AnimateDirection(Direction direction, bool isMoving) {
        if (spriteRenderer != null) {
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
        if (isMoving) {
            StartCoroutine(Animate());
        }      
    }

    public void AnimateIdle() {
        StopAllCoroutines();
        AnimateDirection(currentDirection, false);
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
        if (Vector3.Distance(transform.position, targetLoc) == 0f) {
            AnimateIdle();
        } else {
            StartCoroutine(Animate());
        }
    }
}
