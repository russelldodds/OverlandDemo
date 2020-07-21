using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager> {
    public long minutes = 0;

    void OnEnable() {
        EventManager.StartListening("SaveGame", OnSaveGame);
        EventManager.StartListening("LoadGame", OnLoadGame);
        EventManager.StartListening("IncrementTime", OnIncrementTime);
    }

    void OnDisable() {
        EventManager.StopListening("SaveGame", OnSaveGame);
        EventManager.StopListening("LoadGame", OnLoadGame);
        EventManager.StopListening("IncrementTime", OnIncrementTime);
    }
 
    void OnIncrementTime(Dictionary<string, object> message) {
        minutes += (int)message["minutes"];
    }

    public void OnSaveGame(Dictionary<string, object> message) {
        PlayerPrefsX.SetLong("gameMinutes", minutes);
    }

    public void OnLoadGame(Dictionary<string, object> message) {
        long gameMinutes = PlayerPrefsX.GetLong("gameMinutes");
        minutes = gameMinutes;
    }
}
