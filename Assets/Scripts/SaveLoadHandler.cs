using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadHandler : MonoBehaviour
{

    public MonsterGenerator monsterGenerator;

    public PlayerController playerController;

    public QuestHandler questHandler;

    public PlayerInventory playerInventory;

    public bool loadAtStart = false;

    private bool isProcessing = false;

    // Start is called before the first frame update
    IEnumerator Start() {
        if (loadAtStart) {
            // delay to allow map and things to load
            yield return new WaitForSeconds(0.1f);
            Load();
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.Alpha1) && !isProcessing) {
            Save();
        } else if (Input.GetKeyUp(KeyCode.Alpha2) && !isProcessing) {
            Load();
        }       
    }

    public void Save() {
        isProcessing = true;
        monsterGenerator.Save();
        playerController.Save();
        questHandler.Save();
        playerInventory.Save();
        isProcessing = false;
    }

    public void Load() {
        Debug.Log("LOADING");
        playerController.Load();
        monsterGenerator.Load();
        questHandler.Load();
        playerInventory.Load();
        isProcessing = false;    
    }
}
