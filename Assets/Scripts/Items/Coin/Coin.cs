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
    private Vector3 localScale;

    void Awake() {
        state = State.IDLE;
        pirateCoin = transform.Find("PirateCoin");
        pirateCoin.GetComponent<MeshCollider>().enabled = false;
    }

    void Start() {
        rotateSpeed = 100.0f;
    }

    void Update() {
        if (state == State.ROTATE) {
            Rotate();
        }
    }

    private void Rotate() {
        Vector3 rotation = pirateCoin.eulerAngles;
        rotation.z += Time.deltaTime * rotateSpeed;
        pirateCoin.rotation = Quaternion.Euler(rotation);
    }

    public override void Initialize() {
        state = State.ROTATE;
        pirateCoin.GetComponent<MeshCollider>().enabled = true;
        localScale = transform.localScale;
        origin_position = transform.position;
        origin_rotation = transform.rotation;
    }

    public override void Reset() {
        state = State.IDLE;
        transform.localScale = localScale;
        transform.parent = null;
        pirateCoin.GetComponent<MeshCollider>().enabled = true;
        transform.SetPositionAndRotation(origin_position, origin_rotation);
    }
}
