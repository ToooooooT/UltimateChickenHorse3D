using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireController : BaseItem
{
    private enum State { IDLE, ROTATE };

    [SerializeField] private float rotateSpeed;

    private Transform tireTransform;
    private State state;
    public float theta;

    void Awake() {
        state = State.IDLE;
        tireTransform = transform.Find("tire");
        theta = 0;
    }

    void Start() {
        rotateSpeed = 240.0f;
        theta = 0;
    }

    void Update() {
        if (state == State.ROTATE) {
            Rotate();
        }
    }
    private void Rotate() {
        theta = rotateSpeed * Time.deltaTime;
        
        tireTransform.Rotate(Vector3.right, rotateSpeed * Time.deltaTime);
    }

    public override void Initialize() {
        state = State.ROTATE;
    }

    public override void Reset() {
        state = State.IDLE;
    }
    /*
    private void OnTriggerStay(Collider collision)
    {
        Vector3 objectPosition = collision.gameObject.transform.position;
        Vector3 datumPoint = tireTransform.position + Vector3.Dot(tireTransform.right, (objectPosition - tireTransform.position)) * tireTransform.right;
        Vector3 lookAt = objectPosition - datumPoint;
        Vector3 movingPosition = datumPoint + lookAt + lookAt.magnitude * (Mathf.Tan(theta) * lookAt.magnitude * Vector3.Cross(tireTransform.right, lookAt).normalized).normalized;
        if (collision.gameObject.transform.CompareTag("Player")) {
            collision.gameObject.GetComponent<Player>().ModifyPosition(movingPosition);
        }
        else {
            collision.gameObject.transform.position = movingPosition;
        }
    }
    */
}
