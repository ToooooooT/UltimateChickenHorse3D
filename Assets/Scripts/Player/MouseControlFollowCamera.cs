using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using UnityEditor.Build.Player;

public class MouseControlFollowCamera : MonoBehaviour
{
    [SerializeField] private float sensitive_rotate = 1.0f;
    [SerializeField] private float sensitive_zoom = 0.5f;

    private float rotationX = 0;
    private float rotationY = 0;
    private CinemachineVirtualCamera virtualCamera;
    private InputActionMap playerInputActionMap;
    private Transform playerTransform;
    private bool FPS;
    private float distance;

    private const float MIN_SENSITIVE_ROTATE = 0.5f;
    private const float MAX_SENSITIVE_ROTATE = 4f;
    private const float MIN_SENSITIVE_ZOOM = 0.3f;
    private const float MAX_SENSITIVE_ZOOM = 3f;

    void Start() {
        playerInputActionMap = transform.parent.gameObject.GetComponent<Player>().GetPlayerInputActionMap();
        playerTransform = transform.parent;
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        FPS = false;
        sensitive_rotate = 1.0f;
        sensitive_zoom = 1.0f;
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
        playerInputActionMap.FindAction("FPS2TPS").started += FPS2TPS;
    }

    public void Disable() {
        playerInputActionMap.FindAction("FPS2TPS").started -= FPS2TPS;
    }


    private void FPS2TPS(InputAction.CallbackContext context) {
        FPS = !FPS;
    }

    private void ZoomCamera() {
        float scrollWheelInput = playerInputActionMap.FindAction("ZoomCamera").ReadValue<float>();
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
        Vector2 inputVector = playerInputActionMap.FindAction("MoveCamera").ReadValue<Vector2>().normalized;
        rotationX -= inputVector.y * sensitive_rotate;
        rotationY += inputVector.x * sensitive_rotate;
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

    public void AdjustSensitiveRotate(float ratio) {
        sensitive_rotate = (1 - ratio) * MIN_SENSITIVE_ROTATE + ratio * MAX_SENSITIVE_ROTATE;
    }

    public void AdjustSensitiveZoom(float ratio) {
        sensitive_zoom = (1 - ratio) * MIN_SENSITIVE_ZOOM + ratio * MAX_SENSITIVE_ZOOM;
    }
}
