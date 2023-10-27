using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCursorControl : MonoBehaviour
{
    void Start() {
    }

    void Update() {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
