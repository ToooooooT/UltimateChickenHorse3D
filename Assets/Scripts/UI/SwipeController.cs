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

    private int currentPage;
    private Vector3 targetPos;

    private void Awake() {
        currentPage = 1;
        targetPos = ItemPagesRect.localPosition;
    }

    public void Next() {
        if (currentPage < maxPage) {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void Previous() {
        if (currentPage > 1) {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }

    private void MovePage() {
        ItemPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
    }
}
