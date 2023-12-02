using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinBenScript : MonoBehaviour
{
    public enum State { IDLE, POINTING, FINISH_POINTING };
    [Header("Custom Event")]
    public UnityEvent myEvents;
    public State state;

    // Start is called before the first frame update
    void Start() {
        state = State.IDLE;
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && state == State.IDLE) {
            state = State.POINTING;
            Vector3 targetPosition = other.transform.position;
            targetPosition.y = transform.position.y; // Maintain the same Y level
            transform.LookAt(targetPosition);

            myEvents?.Invoke();
            other.GetComponent<Player>().state = Player.State.WIN;
        }
    }

    private void OnAnimationEnd() {
        state = State.FINISH_POINTING;
    }
}
