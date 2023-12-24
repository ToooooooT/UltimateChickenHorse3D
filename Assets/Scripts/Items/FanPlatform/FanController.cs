using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanController : BaseItem
{
    private enum State { IDLE, MOVE };

    [SerializeField] private float rotateSpeed;
    [SerializeField] private float moveSpeed;

    private State state;
    private Transform fan;
    private Transform middle;
    private float maxY;
    private float minY;
    private Vector3 originPosition;

    void Awake() {
        state = State.IDLE;
        fan = transform.Find("middle").Find("Fan");
        middle = transform.Find("middle");
    }

    void Start() {
        rotateSpeed = 200.0f;
        maxY = middle.position.y + 20.0f;
        minY = middle.position.y;
        moveSpeed = 10.0f;
    }

    void Update() {
        if (state == State.MOVE) {
            Rotate();
            Move();
        }
    }

    private void Rotate() {
        Vector3 rotation = fan.eulerAngles;
        rotation.z += Time.deltaTime * rotateSpeed;
        fan.rotation = Quaternion.Euler(rotation);
    }

    private void Move() {
        Vector3 position = middle.localPosition;
        position.y += moveSpeed * Time.deltaTime;
        if (position.y > maxY || position.y < minY) {
            moveSpeed *= -1;
        }
        position.y = Mathf.Clamp(position.y, minY, maxY);
        middle.localPosition = position;
    }

    public override void Initialize() {
        state = State.MOVE;
        originPosition = transform.position;
    }

    public override void Reset() {
        state = State.IDLE;
        fan.GetComponent<MeshCollider>().enabled = true;
        transform.position = originPosition;
    }
}
