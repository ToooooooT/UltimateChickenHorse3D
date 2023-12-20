using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] private int maxPage;
    [SerializeField] private Vector3 pageStep;
    [SerializeField] private RectTransform ItemPagesRect;

    [SerializeField] private float tweenTime;
    [SerializeField] private LeanTweenType tweenType;

    private GameObject prevButton;
    private GameObject nextButton;

    private int currentPage;
    private Vector3 targetPos;

    private void Awake() {
        currentPage = 1;
        targetPos = ItemPagesRect.localPosition;
        Transform chooseItemCanvas = GameObject.Find("ChooseItemCanvas").transform;
        prevButton = chooseItemCanvas.Find("PrevButton").gameObject;
        nextButton = chooseItemCanvas.Find("NextButton").gameObject;
    }

    public void Next() {
        if (currentPage < maxPage) {
            currentPage++;
            targetPos += pageStep;
            MovePage();
            if (currentPage == 2) {
                prevButton.SetActive(true);
            } else if (currentPage == maxPage) {
                nextButton.SetActive(false);
            }
        }
    }

    public void Previous() {
        if (currentPage > 1) {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
            if (currentPage == 1) {
                prevButton.SetActive(false);
            } else if (currentPage == maxPage - 1) {
                nextButton.SetActive(true);
            }
        }
    }

    private void MovePage() {
        ItemPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
    }
}
