using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IDataSerialiizer {

    public MovementHandler movementHandler;

    public List<TileType> allowedTiles;

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

            if (targetLocation != Vector3.forward) {
                Vector3 vec = movePoint.position - transform.position;
                vec.Normalize();
                GetComponentInChildren<SimpleAnimation>().FaceDirection(vec);
                if (movementHandler.ValidateMove(targetLocation, true, allowedTiles)) {                
                    movePoint.position = targetLocation; 
                } 
            }
        }
    }

    public void Save() {
        PlayerPrefsX.SetVector3("playerPosition", transform.position);

        string [] strAllowedTiles = new string[allowedTiles.Count];
        for (int i = 0; i < allowedTiles.Count; i++) {
            strAllowedTiles[i] = allowedTiles[i].ToString();
        }
        PlayerPrefsX.SetStringArray("playerTiles", strAllowedTiles);
    }

    public void Load() {
        Vector3 playerPosition = PlayerPrefsX.GetVector3("playerPosition");
        if (playerPosition != null && playerPosition != Vector3.zero) {
            // seems like the laoding causes float errors
            playerPosition.x = Mathf.RoundToInt(playerPosition.x);
            playerPosition.y = Mathf.RoundToInt(playerPosition.y);
            transform.position = playerPosition;
            movePoint.position = playerPosition;
        }
        string [] strAllowedTiles = PlayerPrefsX.GetStringArray("playerTiles");
        if (strAllowedTiles != null) {
            allowedTiles = new List<TileType>();
            for (int i = 0; i < allowedTiles.Count; i++) {
                allowedTiles.Add( (TileType)System.Enum.Parse(typeof(TileType), strAllowedTiles[i]));
            }
        }

        Debug.Log("LOADED position: " + playerPosition);
    }
}
