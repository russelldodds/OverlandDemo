using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CreativeSpore.SuperTilemapEditor;

public class LocationHandler : MonoBehaviour {
    void Start() {
        // activate the right location
        string locationName = PlayerPrefs.GetString("locationName", "Castle1");
        foreach (Transform location in  transform) {
            location.gameObject.SetActive(false);
            if (location.name.Equals(locationName)) {
                location.gameObject.SetActive(true);
            }
        }
    }
}
