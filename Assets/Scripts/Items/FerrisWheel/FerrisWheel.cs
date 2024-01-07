using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerrisWheel : BaseItem
{
    private enum State { IDLE, ROTATE };

    [SerializeField] private float rotateSpeed;

    private State state;

    void Awake() {
        state = State.IDLE;
    }

    void Start() {
        rotateSpeed = 10.0f;
    }

    void Update() {
        if (state == State.ROTATE) {
            Rotate();
        }
    }

    void Rotate() {
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

    public override void Initialize() {
        state = State.ROTATE;
    }

    public override void Reset() {
        state = State.IDLE;
    }
}



