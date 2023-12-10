using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningText : MonoBehaviour
{
    private Transform mainCameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookAtDirection = transform.position - mainCameraTransform.position;
        lookAtDirection.y = transform.position.y;
        transform.LookAt(lookAtDirection);
    }
}
