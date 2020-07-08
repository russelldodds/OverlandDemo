using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour, IDataSerialiizer {
    public long minutes = 0;
 
    public void IncrementTime(long increment) {
        minutes += increment;
    }

    public void Save() {
        PlayerPrefsX.SetLong("gameMinutes", minutes);
    }

    public void Load() {
        long gameMinutes = PlayerPrefsX.GetLong("gameMinutes");
        minutes = gameMinutes;
    }
}
