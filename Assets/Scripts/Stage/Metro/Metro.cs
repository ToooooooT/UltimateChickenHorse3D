using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metro : MonoBehaviour
{
    private enum State { IDLE, READY, MOVING, FINISH };
    private State state;
    private float speed = 80f;
    private float minWaitTime = 0f;
    private float maxWaitTime = 20f;
    public Vector3 direction = new Vector3(0, 0, 1);
    private Vector3 startPosition;
    private float currentDistance = 0;
    private float totalDistance = 235f;
    private AudioManager audioManager;

    void Start() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        state = State.IDLE;
        
        startPosition = transform.position;
    }

    void Update() {
        if (state == State.IDLE) {
            state = State.READY;
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            Invoke(nameof(StartMoving), waitTime);
        }
        if (state == State.MOVING) {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            currentDistance = (transform.position - startPosition).magnitude;
            audioManager.PlaySE("metroTrain");
            if (currentDistance >= totalDistance) {
                state = State.FINISH;
            }
        }
        if (state == State.FINISH) {
            state = State.IDLE;
            transform.position = startPosition;
        }
    }

    void StartMoving() {
        state = State.MOVING;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && other.GetComponent<Player>().GetState() == Player.State.GAME) {
            other.GetComponent<Player>().SetDead(); 
        }
    }
}
