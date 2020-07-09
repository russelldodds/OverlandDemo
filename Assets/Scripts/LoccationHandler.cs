using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoccationHandler : MonoBehaviour
{
    public SuperTiled2Unity.SuperMap map;

    public PlayerController playerController;

    public SaveLoadHandler saveLoadHandler;

    public FadeHandler fadeHandler;

    public MovementHandler movementHandler;

    void Start() {
        string locationName = PlayerPrefs.GetString("locationName", "Castle1");
        foreach (Transform location in transform) {
            if (location.name.Equals(locationName)) {
                location.gameObject.SetActive(true);
                map = location.GetComponent<SuperTiled2Unity.SuperMap>();
                movementHandler.grid = map.GetComponentInChildren<Grid>();
                break;
            } else {
                location.gameObject.SetActive(false);
;            }
        }
        saveLoadHandler.Load();        
    }

    // Update is called once per frame
    void Update() {
        if (playerController != null && 
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
