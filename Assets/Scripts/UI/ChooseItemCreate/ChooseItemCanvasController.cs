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
        SwipeController swipeController = transform.Find("ScrollView").GetComponent<SwipeController>();
        transform.Find("ScrollView").gameObject.SetActive(active);
        GameObject nextButton = transform.Find("NextButton").gameObject;
        GameObject prevButton = transform.Find("PrevButton").gameObject;
        if (active && swipeController.IsNextButtonActive()) {
            nextButton.SetActive(true);
        } else {
            nextButton.SetActive(false);
        }
        if (active && swipeController.IsPrevButtonActive()) {
            prevButton.SetActive(true);
        } else {
            prevButton.SetActive(false);
        }
    }

    public void Reset() {
        counter = 0;
    }
}
