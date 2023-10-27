using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCursorControl : MonoBehaviour
{
    private bool isCursorLocked = true;

    void Start()
    {
        // ?定鼠?在屏幕中央
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = true;
    }

    void Update()
    {
        // ??鼠?左???
        if (Input.GetMouseButtonDown(1))
        {
            isCursorLocked = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        // ??ESC?按下
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
