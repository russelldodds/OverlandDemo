using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class WorldMenuUI : MonoBehaviour {
    private void Awake() {
        transform.Find("MainMenuBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Debug.Log("Click Main Menu");
            
            Loader.Load(Loader.Scene.MainMenu);
        };
    }

}
