using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadHandler : MonoBehaviour {

    public MonsterGenerator monsterGenerator;

    public PlayerController playerController;

    private QuestHandler questHandler;

    private PlayerInventory playerInventory;

    public bool loadAtStart = false;

    public bool isProcessing = false;

    // Start is called before the first frame update
    IEnumerator Start() {
        questHandler = GetComponent<QuestHandler>();
        playerInventory = GetComponent<PlayerInventory>();
        if (loadAtStart) {
            // delay to allow map and things to load
            yield return new WaitForEndOfFrame();
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
        //Debug.Log("SAVING");
        isProcessing = true;
        monsterGenerator?.Save();
        playerController?.Save();
        questHandler?.Save();
        playerInventory?.Save();
        isProcessing = false;
    }

    public void Load() {
        //Debug.Log("LOADING");
        isProcessing = true;
        playerController?.Load();
        monsterGenerator?.Load();
        questHandler?.Load();
        playerInventory?.Load();
        isProcessing = false;    
    }

}
