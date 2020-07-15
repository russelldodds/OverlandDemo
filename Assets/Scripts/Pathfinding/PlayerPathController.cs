using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine.SceneManagement;

public class PlayerPathController : MonoBehaviour, IDataSerialiizer {
    public TilemapGroup tilemaps;
    private Pathfinding pathfinding;
    private SaveLoadHandler saveLoadHandler;
    private QuestHandler questHandler;
    private GridManager gridManager;
    private void Start() {
        questHandler = this.GetComponent<QuestHandler>(); 
        saveLoadHandler = this.GetComponent<SaveLoadHandler>();
        gridManager = new GridManager(tilemaps);
        pathfinding = new Pathfinding();
    }
    private void Update() {       
        if (Input.GetMouseButtonDown(0)) {
            StopAllCoroutines();
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            Debug.Log("mouseWorldPosition: " + mouseWorldPosition);
            List<Vector3> path = pathfinding.FindPath(transform.position, mouseWorldPosition);
            GetComponent<PathMover>().SetPath(path);
        }
    }

    public void Save() {
        PlayerPrefsX.SetVector3("playerPosition", transform.position);

        // string [] strAllowedTiles = new string[allowedTiles.Count];
        // for (int i = 0; i < allowedTiles.Count; i++) {
        //     strAllowedTiles[i] = allowedTiles[i].ToString();
        // }
        // PlayerPrefsX.SetStringArray("playerTiles", strAllowedTiles);
    }

    public void Load() {
        Vector3 playerPosition = PlayerPrefsX.GetVector3("playerPosition");
        if (playerPosition != null && playerPosition != Vector3.zero) {
            // seems like the laoding causes float errors
            playerPosition.x = Mathf.RoundToInt(playerPosition.x);
            playerPosition.y = Mathf.RoundToInt(playerPosition.y);
            transform.position = playerPosition;
        }
        // string [] strAllowedTiles = PlayerPrefsX.GetStringArray("playerTiles");
        // if (strAllowedTiles != null) {
        //     allowedTiles = new List<TileType>();
        //     for (int i = 0; i < allowedTiles.Count; i++) {
        //         allowedTiles.Add( (TileType)System.Enum.Parse(typeof(TileType), strAllowedTiles[i]));
        //     }
        // }

        Debug.Log("LOADED position: " + playerPosition);
    }

    IEnumerator LoadLocationScene(string locationName) {
        PlayerPrefs.SetString("locationName", locationName);
        while (saveLoadHandler.isProcessing) {
            yield return new WaitForEndOfFrame();
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Location");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}