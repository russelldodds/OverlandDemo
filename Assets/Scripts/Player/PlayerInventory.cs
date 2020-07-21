using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    void OnEnable() {
        EventManager.StartListening("SaveGame", OnSaveGame);
        EventManager.StartListening("LoadGame", OnLoadGame);
    }

    void OnDisable() {
        EventManager.StopListening("SaveGame", OnSaveGame);
        EventManager.StopListening("LoadGame", OnLoadGame);
    }
 
    public void OnSaveGame(Dictionary<string, object> message) {
    }

    public void OnLoadGame(Dictionary<string, object> message) {
    }
}
