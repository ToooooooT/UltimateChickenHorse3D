using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : BaseItem
{
    private enum State { IDLE, ROTATE };

    [SerializeField] private float rotateSpeed;

    private Vector3 origin_position;
    private Quaternion origin_rotation;
    private State state;
    private Transform pirateCoin;

    void Awake() {
        state = State.IDLE;
        pirateCoin = transform.Find("PirateCoin");
    }

    void Start() {
        rotateSpeed = 100.0f;
    }

    void Update() {
        if (state == State.ROTATE) {
            Rotate();
            Debug.Log(transform.position);
        }
    }

    private void Rotate() {
        Vector3 rotation = pirateCoin.eulerAngles;
        rotation.z += Time.deltaTime * rotateSpeed;
        pirateCoin.rotation = Quaternion.Euler(rotation);
    }

    public override void Initialize() {
        state = State.ROTATE;
        origin_position = transform.position;
        origin_rotation = transform.rotation;
    }

    public override void Reset() {
        transform.parent = null;
        pirateCoin.GetComponent<MeshCollider>().enabled = true;
        transform.SetPositionAndRotation(origin_position, origin_rotation);
    }
}
