using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorControl : MonoBehaviour
{

    void Start() {

    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
