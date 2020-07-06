using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    public QuestItem[] questItems;

    public bool hasRequiredItem(QuestItem item) {
        foreach (QuestItem qi in questItems) {
            if (qi == item) {
                return true;
            }
        }
        return false;
    }
}
