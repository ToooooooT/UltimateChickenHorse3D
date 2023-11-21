using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerrisWheel : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    // Start is called before the first frame update
    void Start() {
        rotateSpeed = 10.0f;
    }

    // Update is called once per frame
    void Update() {
        Rotate();
    }

    void Rotate() {
        // Assuming the child object has a specific name, adjust it accordingly
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z += Time.deltaTime * rotateSpeed;
        transform.rotation = Quaternion.Euler(rotation);

        string[] sphere_name = {"RightUpSphere", "RightDownSphere", "LeftDownSphere", "LeftUpSphere"};
        for (int i = 0; i < sphere_name.Length; ++i) {
            Transform cube_transform = transform.Find(sphere_name[i]);
            rotation = cube_transform.rotation.eulerAngles;
            rotation.z -= Time.deltaTime * rotateSpeed;
            cube_transform.rotation = Quaternion.Euler(rotation);
        }
    }
}



