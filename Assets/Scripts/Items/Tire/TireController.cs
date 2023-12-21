using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireController : BaseItem
{
    private enum State { IDLE, ROTATE };

    [SerializeField] private float rotateSpeed;

    private Transform tire;
    private State state;

    void Awake() {
        state = State.IDLE;
        tire = transform.Find("tire");
    }

    void Start() {
        rotateSpeed = 30.0f;
    }

    void Update() {
        if (state == State.ROTATE) {
            Rotate();
        }
    }

    private void Rotate() {
        tire.Rotate(Vector3.right, rotateSpeed * Time.deltaTime);
    }

    public override void Initialize() {
        state = State.ROTATE;
    }

    public override void Reset() {
        state = State.IDLE;
    }
}
