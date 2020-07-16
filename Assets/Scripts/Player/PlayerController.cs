using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CreativeSpore.SuperTilemapEditor;


public class PlayerController : MonoBehaviour, IDataSerialiizer {
    public TilemapGroup tilemaps;
    public List<TileType> allowedTiles;
    private Pathfinding pathfinding;
    public GridManager gridManager;
    private void Start() {
        pathfinding = new Pathfinding(gridManager);
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
}