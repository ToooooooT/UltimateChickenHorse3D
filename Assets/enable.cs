using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class enable : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            virtualCamera.enabled = !virtualCamera.enabled;
        }
    }
}
