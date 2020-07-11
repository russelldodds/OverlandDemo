using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    protected static GameHandler _instance = null;

    public static GameHandler Instance {
        get {
            if (_instance == null) { _instance = FindObjectOfType<GameHandler>(); }
            return _instance;
        }
        protected set { _instance = value; }
    }

    protected virtual void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            DestroyImmediate(this);
            return;
        }
    }
}
