using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System;

public class MouseControlFollowCamera : MonoBehaviour
{
    [SerializeField] private float sensitive_move;
    [SerializeField] private float sensitive_zoom;

    private float rotationX = 0;
    private float rotationY = 0;
    private CinemachineVirtualCamera virtualCamera;
    private PlayerInputActions playerInputActions;
    private Transform playerTransform;
    private bool FPS;
    private float distance;

    void Start() {
        playerTransform = transform.parent;
        playerInputActions = transform.parent.gameObject.GetComponent<Player>().GetPlayerInputActions();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        sensitive_move = 1.0f;
        sensitive_zoom = 0.5f;
        FPS = false;
        distance = 25.0f;
        rotationX = 0;
        rotationY = 0;
        Enable();
    }

    private void Update() {
        ZoomCamera();
        FollowPlayer();
    }

    public void Enable() {
        playerInputActions.Player.FPS2TPS.started += FPS2TPS;
    }

    private void FPS2TPS(InputAction.CallbackContext context) {
        FPS = !FPS;
    }

    private void ZoomCamera() {
        float scrollWheelInput = playerInputActions.Player.ZoomCamera.ReadValue<float>();
        if (scrollWheelInput != 0) {
            if (scrollWheelInput > 0) {
                distance -= sensitive_zoom;
                distance = Mathf.Max(distance, 2.0f);
            } else if (scrollWheelInput < 0) {
                distance += sensitive_zoom;
            }
        }
    }

    private void FollowPlayer() {
        Vector2 inputVector = playerInputActions.Player.MoveCamera.ReadValue<Vector2>().normalized;
        rotationX -= inputVector.y * sensitive_move;
        rotationY += inputVector.x * sensitive_move;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        if (virtualCamera.enabled && virtualCamera.Follow == null) {
            // make the virtual camera follow the player
            if (FPS) {
                transform.position = playerTransform.position + new Vector3(0,3,0);
            } else {
                transform.position = playerTransform.position - distance * transform.forward;
            }
        }
    }
}
