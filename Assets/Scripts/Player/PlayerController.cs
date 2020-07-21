using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CreativeSpore.SuperTilemapEditor;


public class PlayerController : MonoBehaviour {
    public List<TileType> allowedTiles;
    private Pathfinding pathfinding;
    private CharacterAnimation characterAnimation;

    void OnEnable() {
        pathfinding = new Pathfinding();
        characterAnimation = GetComponentInChildren<CharacterAnimation>();

        EventManager.StartListening("SaveGame", OnSaveGame);
        EventManager.StartListening("LoadGame", OnLoadGame);
        EventManager.StartListening("SetPlayerLocation", OnSetPlayerLocation);
    }

    void OnDisable() {
        EventManager.StopListening("SaveGame", OnSaveGame);
        EventManager.StopListening("LoadGame", OnLoadGame);
        EventManager.StopListening("SetPlayerLocation", OnSetPlayerLocation);
    }

    private void Update() {       
        if (Input.GetMouseButtonDown(0)) {
            StopAllCoroutines();
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            Debug.Log("mouseWorldPosition: " + mouseWorldPosition);
            List<Vector3> path = pathfinding.FindPath(transform.position, mouseWorldPosition);
            GetComponent<PlayerMover>().SetPath(path);
        }
    }

    public void OnSaveGame(Dictionary<string, object> message) {
        bool savePlayerPosition = (bool)message["savePlayerPosition"];
        if (savePlayerPosition) {
            PlayerPrefsX.SetVector3("playerPosition", transform.position);
        }
        
        string [] strAllowedTiles = new string[allowedTiles.Count];
        for (int i = 0; i < allowedTiles.Count; i++) {
            strAllowedTiles[i] = allowedTiles[i].ToString();
        }
        PlayerPrefsX.SetStringArray("playerTiles", strAllowedTiles);
    }

    public void OnLoadGame(Dictionary<string, object> message) {
        Vector3 playerPosition = PlayerPrefsX.GetVector3("playerPosition", Vector3.zero);
        if (playerPosition != null && playerPosition != Vector3.zero) {
            // seems like the laoding causes float errors
            playerPosition.x = Mathf.FloorToInt(playerPosition.x) + 0.5f;
            playerPosition.y = Mathf.FloorToInt(playerPosition.y) + 0.5f;
            transform.position = playerPosition;
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

    void OnSetPlayerLocation(Dictionary<string, object> message) {
        transform.position = (Vector3)message["position"];
        Direction direction = (Direction)message["direction"];
        characterAnimation.AnimateDirection(direction, false);
    }
}