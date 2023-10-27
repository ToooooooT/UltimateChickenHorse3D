using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButtonEvent : MonoBehaviour
{
    private Image image;
    private float alpha = 0.3f;
    // Start is called before the first frame update
    void Start() {
        Color newColor = image.color;
        newColor.a = alpha;
        image.color = newColor;
    }

    // Update is called once per frame
    void Update() {
        if (image.color.a != 0.0f) {
            Vector3 mousePosition = Input.mousePosition;
            if (mousePosition.x < 1215 && mousePosition.x > 651 && mousePosition.y < 642 && mousePosition.y > 439) {
                Color newColor = image.color;
                newColor.a = 1.0f;
                image.color = newColor;
                transform.localScale = new Vector3(66 / 5, 6, 0);
                if (Input.GetMouseButtonDown(0)) {
                    newColor = image.color;
                    newColor.a = 0.0f;
                    image.color = newColor;
                }
            }
            else {
                Color newColor = image.color;
                transform.localScale = new Vector3(11, 5, 0);
                newColor.a = 0.3f;
                image.color = newColor;
            }
        }
    }
}
