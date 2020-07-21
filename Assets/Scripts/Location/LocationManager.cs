using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine.SceneManagement;

public class LocationManager : MonoBehaviour {
    public string defaultLocation;
    public string activeLocation;
    private List<GameObject> locations = new List<GameObject>();

    void OnEnable() {
        EventManager.StartListening("LoadScene", OnLoadScene);
    }

    void OnDisable() {
        EventManager.StopListening("LoadScene", OnLoadScene);
    }
 
    void Start() { 
        TilemapGroup[] maps = GetComponentsInChildren<TilemapGroup>(true);
        LocationTarget[] locations = GetComponentsInChildren<LocationTarget>();   
        GridManager.Instance.InitializeScene(maps, locations);

        activeLocation = PlayerPrefs.GetString("locationName", defaultLocation);
        GridManager.Instance.LoadMap(activeLocation);
    }

    void OnLoadScene(Dictionary<string, object> message) {
        string sceneName = (string)message["sceneName"];
        string activeLocation = (string)message["activeLocation"];
        StartCoroutine(LoadScene(sceneName, activeLocation));
    }

    IEnumerator LoadScene(string sceneName, string activeLocation) {
        PlayerPrefs.SetString("locationName", activeLocation);       
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }
}
