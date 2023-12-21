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
    private float maxY;
    private float minY;
    private Vector3 originPosition;

    void Awake() {
        state = State.IDLE;
        fan = transform.Find("Fan");
    }

    void Start() {
        rotateSpeed = 200.0f;
        maxY = transform.position.y + 20.0f;
        minY = transform.position.y;
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
        Vector3 position = transform.position;
        position.y += moveSpeed * Time.deltaTime;
        if (position.y > maxY || position.y < minY) {
            moveSpeed *= -1;
        }
        position.y = Mathf.Clamp(position.y, minY, maxY);
        transform.position = position;
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
