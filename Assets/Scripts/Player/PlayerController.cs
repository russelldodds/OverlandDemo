using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CreativeSpore.SuperTilemapEditor;


public class PlayerController : MonoBehaviour, IDataSerialiizer {
    public List<TileType> allowedTiles;
    private Pathfinding pathfinding;
    public Vector3 startingLocation = new Vector3(12.5f, 114.5f, 0.5f);
    private void Start() {
        pathfinding = new Pathfinding();
    }
    private void Update() {       
        if (!GridManager.Instance.isLoading && Input.GetMouseButtonDown(0)) {
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
        Vector3 playerPosition = PlayerPrefsX.GetVector3("playerPosition", startingLocation);
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