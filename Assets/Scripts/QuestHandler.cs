using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour {
    public long minutes;
    // Start is called before the first frame update
    void Start() {
        // TODO: load from save file
        minutes = 0;
    }

    public void incrementTime(long increment) {
        minutes += increment;
    }
}
