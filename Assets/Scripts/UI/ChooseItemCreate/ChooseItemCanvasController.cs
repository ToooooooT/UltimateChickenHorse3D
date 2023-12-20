using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseItemCanvasController : MonoBehaviour
{
    private int counter;

    void Awake() {
        counter = 0;
    }

    public void Enable() {
        counter++;
        if (counter == 1) {
            SetActiveAllChild(true);
        }
    }

    public void Disable() {
        counter--;
        if (counter == 0) {
            SetActiveAllChild(false);
        }
    }

    public void SetActiveAllChild(bool active) {
        foreach (Transform child in transform) {
            if (!active || child.gameObject.name != "PrevButton") {
                child.gameObject.SetActive(active);
            }
        }
    }
}
