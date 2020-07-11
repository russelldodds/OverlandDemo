using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoccationHandler : MonoBehaviour
{
    
    public PlayerController playerController;

    public SaveLoadHandler saveLoadHandler;

    public FadeHandler fadeHandler;

    public MovementHandler movementHandler;

    public List<SuperTiled2Unity.SuperMap> locationPrefabs;

    private SuperTiled2Unity.SuperMap map;

    void Start() {
        string locationName = PlayerPrefs.GetString("locationName", "Castle1");
        
        foreach (SuperTiled2Unity.SuperMap prefab in locationPrefabs) {
            if (prefab.name.Equals(locationName)) {
                map = Instantiate<SuperTiled2Unity.SuperMap>(prefab, Vector3.zero, Quaternion.identity);
                movementHandler.grid = map.GetComponentInChildren<Grid>();
                break;
            } 
        }
        saveLoadHandler.Load();        
    }

    // Update is called once per frame
    void Update() {
        if (playerController != null && map != null && 
                (playerController.transform.position.x <= 0 || 
                playerController.transform.position.x >= map.m_Width - 1 ||
                playerController.transform.position.y >= 0 || 
                playerController.transform.position.y <= -map.m_Height + 1)) {
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
