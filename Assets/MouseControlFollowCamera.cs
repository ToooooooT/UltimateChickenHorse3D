using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class MouseControlFollowCamera : MonoBehaviour
{
    private float rotationX = 0;
    private float rotationY = 0;
    private CinemachineVirtualCamera virtualCamera;
    private GameObject[] playerObjects;
    private string objectName;
    private bool FPS = false;
    private float distance = 25.0f;
    private float sensitive;

    void Start() {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        objectName = gameObject.name;
        sensitive = 0.2f;
    }

    private void Update() {
        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWheelInput != 0) {
            if (scrollWheelInput > 0) {
                distance -= 0.5f;
                distance = Mathf.Max(distance, 2.0f);
            } else if (scrollWheelInput < 0) {
                distance += 0.5f;
            }
        }

        if (Input.GetKeyDown(KeyCode.V)) {
            FPS = !FPS;
        }
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        rotationX -= mouseY * sensitive * 5;
        rotationY += mouseX * sensitive * 5;
        rotationX = Mathf.Clamp(rotationX, -90, 90);
        transform.SetPositionAndRotation(new Vector3(0, 6010, 0), Quaternion.Euler(rotationX, rotationY, 0.0f));
        if (virtualCamera.enabled && virtualCamera.Follow == null) {
            // make the virtual camera follow the player
            for (int i = 0; i < playerObjects.Length; ++i) {
                if (FPS) {
                    transform.position = playerObjects[i].transform.position + 0.5f * transform.forward + new Vector3(0,3,0);
                } else {
                    transform.position = playerObjects[i].transform.position - distance * transform.forward;
                }

            }
        }
    }
}
