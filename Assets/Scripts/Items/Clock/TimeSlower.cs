using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSlower : BaseItem
{
    private enum State { IDLE, SLOW, STOP };

    [SerializeField] private float period;

    private float slowCounter = 0;
    private State state;

    void Awake() {
        state = State.STOP;
    }

    // Start is called before the first frame update
    void Start() {
        period = 2.0f;
    }

    // Update is called once per frame
    void Update() {
        if (state == State.SLOW) {
            if (gameObject.TryGetComponent<Renderer>(out var renderer) && renderer.enabled) {
                renderer.enabled = false;
            }
            slowCounter += Time.deltaTime;
            if (slowCounter > period) {
                gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player") && state == State.IDLE) {
            Time.timeScale = 0.2f;
            state = State.SLOW;
        }
    }

    public override void Initialize() {
        if (gameObject.TryGetComponent<Renderer>(out var renderer)) {
            renderer.enabled = true;
        }
        slowCounter = 0;
        gameObject.SetActive(true);
        state = State.IDLE;
    }


    public override void Reset() {
        if (gameObject.TryGetComponent<Renderer>(out var renderer)) {
            renderer.enabled = true;
        }
        Time.timeScale = 1f;
        slowCounter = 0;
        gameObject.SetActive(true);
        state = State.IDLE;
    }
}
