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

    }

    public void Enable() {
        virtualCamera.enabled = true;
    }

    public void Disable() {
        virtualCamera.enabled = false;
    }
}
