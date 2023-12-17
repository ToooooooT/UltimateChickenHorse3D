using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;
using UnityEditor.Rendering;

public class PlayerCursor : MonoBehaviour
{
    private GameObject cursor;
    private InputActionMap cursorInputActionMap;
    private float cursorMaxX;
    private float cursorMaxY;
    [SerializeField] private float moveSpeed;

    void Start() {
        moveSpeed = 15f;
        cursorInputActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Cursor");
        cursorMaxX = transform.Find("Canvas").GetComponent<RectTransform>().rect.width;
        cursorMaxY = transform.Find("Canvas").GetComponent<RectTransform>().rect.height;
        cursor = transform.Find("Canvas").Find("Cursor").gameObject;
        cursor.SetActive(false);
    }

    void Update() {
        Vector3 position = cursor.GetComponent<RectTransform>().position;
        Vector2 moveVector2 = moveSpeed * cursorInputActionMap.FindAction("Move").ReadValue<Vector2>().normalized;
        Vector3 moveVector3 = new(moveVector2.x, moveVector2.y, 0);
        position += moveVector3;
        position.x = Mathf.Clamp(position.x, 0, cursorMaxX);
        position.y = Mathf.Clamp(position.y, 0, cursorMaxY);
        cursor.GetComponent<RectTransform>().position = position;
    }

    private void RaycastUI(InputAction.CallbackContext context) {
        Ray ray = Camera.main.ScreenPointToRay(cursor.transform.position);
        PointerEventData pointerEventData = new(EventSystem.current) {
            position = cursor.transform.position
        };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerEventData, results);
        if (results.Count > 0) {
            foreach (RaycastResult hit in results) {
                GameObject obj = hit.gameObject;
                if (obj.GetComponent<UIBehaviour>()) {
                    if (obj.CompareTag("ItemButton")) {
                        GetComponent<Player>().SetItem(obj.GetComponent<UnityEngine.UI.Image>().sprite.name);
                    }
                    ExecuteEvents.Execute(obj, pointerEventData, ExecuteEvents.pointerClickHandler);
                }
            }
        }
    }

    public void Enable() {
        cursor.SetActive(true);
        cursorInputActionMap.Enable();
        InputAction click = cursorInputActionMap.FindAction("Click");
        click.performed += RaycastUI;
    }

    public void Disable() {
        cursor.SetActive(false);
        cursorInputActionMap.Disable();
        InputAction click = cursorInputActionMap.FindAction("Click");
        click.performed -= RaycastUI;
    }
}
