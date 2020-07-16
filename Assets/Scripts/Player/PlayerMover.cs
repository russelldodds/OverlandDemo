using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerMover : MonoBehaviour {

    private int locationSize = 64;
    public float speed = 5f;

    public Vector3 nextTile = Vector3.forward;

    public Vector3 finalTile = Vector3.forward;

    public List<Vector3> path;

    public int index = 0;

    public GameObject gameManager;

    private SaveLoadHandler saveLoadHandler;

    private QuestHandler questHandler;

    void Start() {
        saveLoadHandler = gameManager.GetComponent<SaveLoadHandler>();
        questHandler = gameManager.GetComponent<QuestHandler>();
    }
    // Update is called once per frame
    void Update() {
        if (nextTile != Vector3.forward) {
            transform.position = Vector3.MoveTowards(transform.position, nextTile, speed * Time.deltaTime);
         
            if (path != null && index <= path.Count - 1) {
                if (Vector3.Distance(transform.position, nextTile) == 0f) {
                    IncrementTime(transform.position);
                    nextTile = path[index++];
                    GetComponentInChildren<CharacterAnimation>().AnimateDirection(nextTile, true);
                    Debug.Log("Move to: " + nextTile);
                }
            } else if (Vector3.Distance(transform.position, finalTile) == 0f) {
                ResetPath();
                CheckLocations();
            }
        }
    }

    void IncrementTime(Vector3 location) {
        GridTile tile = gameManager.GetComponent<GridManager>().GetGridTile(location);
        if (tile != null && tile.cost > 0) {
            questHandler.IncrementTime(tile.cost);
        }
    }

    void CheckLocations() {
        if (PlayerPrefs.GetString("locationName", "World").Equals("World")) {
            foreach (Transform location in gameManager.transform) {
                Debug.Log("Location check: " + location.position + ", " + transform.position);
                if (Vector3.Distance(location.position, transform.position) <= 0.5f) {
                    Debug.Log("Enter location: " + location.name);                
                    saveLoadHandler.Save();
                    StartCoroutine(LoadScene("Location", location.name));
                    return;
                }
            }
        } else {
            if (transform.position.x < 1 || transform.position.x > locationSize - 1 || 
            transform.position.y < 1 || transform.position.y > locationSize - 1) {
                saveLoadHandler.Save();
                StartCoroutine(LoadScene("World", "World"));
            }
        }       
    }

    void ResetPath() {
        path.Clear();
        index = 0;
        nextTile = Vector3.forward;
        finalTile = Vector3.forward;
        GetComponentInChildren<CharacterAnimation>().AnimateIdle();
    }

    public void SetPath(List<Vector3> path) {
        if (path != null && path.Count > 0) {
            this.path = path;
            index = 0;
            nextTile = path[index++];
            finalTile = path[path.Count - 1];
        }
    }

    IEnumerator LoadScene(string sceneName, string locationName) {
        PlayerPrefs.SetString("locationName", locationName);
        while (saveLoadHandler.isProcessing) {
            yield return new WaitForEndOfFrame();
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}