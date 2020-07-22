using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class WorldMenuUI : MonoBehaviour {
    private void Awake() {
        transform.Find("QuitBtn").GetComponent<Button_UI>().ClickFunc = () => {
            Debug.Log("Click Quit");          
            Loader.Load(Loader.Scene.MainMenu);
        };
    }

}
