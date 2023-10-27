using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeModeBottonEvent : MonoBehaviour
{
    public Image image;
    private bool inactive = false;
    private float alpha = 0.0f;
    // Start is called before the first frame update
    void Start() {
        Color newColor = image.color;
        newColor.a = alpha;
        image.color = newColor;
    }

    // Update is called once per frame
    void Update() {
        Vector3 mousePosition = Input.mousePosition;
        if (image.color.a != 0.0f) {
            if (mousePosition.x < 1099.49f && mousePosition.x > 742.31f && mousePosition.y < 403.68f && mousePosition.y > 266.3f) {
                Color newColor = image.color;
                newColor.a = 1.0f;
                image.color = newColor;
                transform.localScale = new Vector3(12, 4.8f, 0);
                if (Input.GetMouseButtonDown(0)) {
                    newColor = image.color;
                    transform.localScale = new Vector3(10, 4, 0);
                    newColor.a = 0.0f;
                    image.color = newColor;
                    inactive = true;
                }
            }
            else {
                Color newColor = image.color;
                transform.localScale = new Vector3(10, 4, 0);
                newColor.a = 0.3f;
                image.color = newColor;
            }
        }
        else if (!inactive && Input.GetMouseButtonDown(0) && mousePosition.x < 1215 && mousePosition.x > 651 && mousePosition.y < 642 && mousePosition.y > 439) {
            Color newColor = image.color;
            transform.localScale = new Vector3(10, 4, 0);
            newColor.a = 0.3f;
            image.color = newColor;
        }
    }
}