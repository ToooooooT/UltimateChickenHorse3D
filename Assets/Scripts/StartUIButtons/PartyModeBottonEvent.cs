using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PartyModeBottonEvent : MonoBehaviour
{
    public Image image;
    private float alpha = 0.0f;
    void Start() {
        Color newColor = image.color;
        newColor.a = alpha;
        image.color = newColor;
    }

    void Update() {
        Vector3 mousePosition = Input.mousePosition;
        if (image.color.a != 0.0f) {
            if (mousePosition.x < 1110.06f && mousePosition.x > 733.86f && mousePosition.y < 904.58f && mousePosition.y > 765.09f) {
                Color newColor = image.color;
                newColor.a = 1.0f;
                image.color = newColor;
                transform.localScale = new Vector3(12, 4.8f, 0);
                if (Input.GetMouseButtonDown(0)) {
                    StartGame();
                }
            }
            else {
                Color newColor = image.color;
                transform.localScale = new Vector3(10, 4, 0);
                newColor.a = 0.3f;
                image.color = newColor;
            }
        }
        else if (Input.GetMouseButtonDown(0) && mousePosition.x < 1215 && mousePosition.x > 651 && mousePosition.y < 642 && mousePosition.y > 439) {
            Color newColor = image.color;
            transform.localScale = new Vector3(10, 4, 0);
            newColor.a = 0.3f;
            image.color = newColor;
        }
    }

    private void StartGame() {
        SceneManager.LoadScene("GameScene");
        PlayerPrefs.SetString("GameMode", "Party");
        PlayerPrefs.Save();
    }
}
