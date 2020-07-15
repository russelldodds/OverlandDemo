using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CreativeSpore.SuperTilemapEditor;

public class LocationHandler : MonoBehaviour
{
    
    public PlayerController playerController;

    public SaveLoadHandler saveLoadHandler;

    public FadeHandler fadeHandler;

    public MovementHandler movementHandler;

    public List<STETilemap> locationPrefabs;

    private STETilemap map;

    void Start() {
        string locationName = PlayerPrefs.GetString("locationName", "Castle1");
        
        foreach (STETilemap prefab in locationPrefabs) {
            if (prefab.name.Equals(locationName)) {
                map = Instantiate<STETilemap>(prefab, Vector3.zero, Quaternion.identity);
                //movementHandler.grid = map.GetComponentInChildren<Grid>();
                break;
            } 
        }
        saveLoadHandler.Load();        
    }

    // Update is called once per frame
    void Update() {
        if (playerController != null && map != null && 
                (playerController.transform.position.x <= 0 || 
                playerController.transform.position.x >= map.GridWidth - 1 ||
                playerController.transform.position.y >= 0 || 
                playerController.transform.position.y <= -map.GridHeight + 1)) {
            saveLoadHandler.Save();
            StartCoroutine(ExitLocation());
        }
    }

    IEnumerator ExitLocation() {    
        while (saveLoadHandler.isProcessing){
            yield return new WaitForEndOfFrame();
        }
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("World");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
