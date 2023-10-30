using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Enable : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private GameObject[] playerObjects;

    void Start() {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.C)) {
            virtualCamera.enabled = !virtualCamera.enabled;
        } 
        if (virtualCamera.enabled && virtualCamera.Follow == null) {
            // make the virtual camera follow the player
            for (int i = 0; i < playerObjects.Length; ++i) {
                virtualCamera.Follow = playerObjects[i].transform; 
            }
        }
    }
}
