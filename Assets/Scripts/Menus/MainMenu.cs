using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class MainMenu : MonoBehaviour {
    private void Awake() {
        transform.Find("ContinueBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Debug.Log("Click Continue");
            Loader.Load(Loader.Scene.World);
        };
        transform.Find("NewGameBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Debug.Log("Click New Game");
            PlayerPrefs.DeleteAll();
            Loader.Load(Loader.Scene.World);
        };
    }
}
