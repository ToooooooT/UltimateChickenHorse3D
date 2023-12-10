using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    private enum State { IDLE, PRESS, PRESS_END, RELEASE };
    private State state;
    private float pressStartTime;
    private float leaveStartTime;
    [SerializeField] private float frequency = 1f;
    private float amplitude;
    private Vector3 originPosition;
    private Transform player = null;
    private Collider[] colliders;
    private float largestDistance;
    

    // Start is called before the first frame update
    void Start() {
        state = State.IDLE;
        pressStartTime = -1;
        leaveStartTime = -1;
        amplitude = transform.localScale.y / 2;
        originPosition = transform.localPosition;
        largestDistance = Mathf.Sqrt((float)((transform.localScale.x / 2 + 0.5) * (transform.localScale.x / 2 + 0.5) + transform.localScale.y * transform.localScale.y));
        GetColliders();
    }

    // Update is called once per frame
    void Update() {
        if (state == State.PRESS) {
            HandlePress();
        } else if (state == State.PRESS_END) {
            CheckLeave();
        } else if (state == State.RELEASE) {
            HandleLeave();
        }
    }

    private void HandlePress() {
        float elapsed = Time.time - pressStartTime;
        
        if (elapsed > 0.33f / frequency) {
            pressStartTime = -1;
            state = State.PRESS_END;
            return;
        }
        float angularFrequency = 2.0f * Mathf.PI * frequency;
        float dy = amplitude * -Mathf.Sin(angularFrequency * elapsed);

        transform.localPosition = originPosition + new Vector3(0, dy, 0);
    }

    private void HandleLeave() {
        float elapsed = Time.time - leaveStartTime;

        if (elapsed > 0.5f / frequency) {
            leaveStartTime = -1;
            transform.localPosition = originPosition;
            state = State.IDLE;
            EnableColliders();
            return;
        }

        float angularFrequency = 2.0f * Mathf.PI * frequency;
        float dy = amplitude * -Mathf.Sin(angularFrequency * elapsed);

        transform.localPosition = originPosition + new Vector3(0, dy, 0);
    }

    private void CheckLeave() {
        if (player.position.y >= transform.position.y + 2
            || (player.position - transform.position).magnitude >= largestDistance) {
            state = State.RELEASE;
            leaveStartTime = Time.time - 0.33f;
        }
    }

    private void GetColliders() {
        if (colliders == null) {
            Collider[] allColliders = GetComponentsInChildren<Collider>();
            colliders = new Collider[allColliders.Length - 1];
            for (int i = 1; i < allColliders.Length; i++) {
                colliders[i - 1] = allColliders[i];
            }
        }
    }

    private void DisableColliders() {
        if (colliders == null) {
            GetColliders();
        }
        foreach (Collider collider in colliders) {
            collider.enabled = false;
        }
    }

    private void EnableColliders() {
        if (colliders == null) {
            GetColliders();
        }
        foreach (Collider collider in colliders) {
            collider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (state == State.IDLE) {
                state = State.PRESS;
                player = other.transform;
                pressStartTime = Time.time;
                DisableColliders();
            }
        }
    }
}
