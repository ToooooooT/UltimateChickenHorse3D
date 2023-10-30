using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class EnableCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private GameObject[] playerObjects;
    private string objectName;

    void Start() {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        objectName = gameObject.name;
    }

    void Update() {
        if (virtualCamera.enabled && objectName == "FollowCamera" && virtualCamera.Follow == null) {
            // make the virtual camera follow the player
            for (int i = 0; i < playerObjects.Length; ++i) {
                virtualCamera.Follow = playerObjects[i].transform; 
            }
        }
    }

    public void Enable() {
        virtualCamera.enabled = true;
    }

    public void Disable() {
        virtualCamera.enabled = false;
    }
}
