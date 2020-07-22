using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine.SceneManagement;

public class LocationManager : MonoBehaviour {
    public string defaultLocation;
    public string activeLocation;
    private List<GameObject> locations = new List<GameObject>();
 
    void Start() { 
        TilemapGroup[] maps = GetComponentsInChildren<TilemapGroup>(true);
        LocationTarget[] locations = GetComponentsInChildren<LocationTarget>();   
        GridManager.Instance.InitializeScene(maps, locations);

        activeLocation = PlayerPrefs.GetString("locationName", defaultLocation);
        GridManager.Instance.LoadMap(activeLocation);
    }
}
