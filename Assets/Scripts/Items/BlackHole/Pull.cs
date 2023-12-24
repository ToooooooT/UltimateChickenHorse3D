using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pull : BaseItem
{
    private enum State { idle, active}
    private State state;
    [SerializeField] private float FORCE;

    private void Awake() {
        state = State.idle;
    }

    void Start() {
        FORCE = 0.2f;
    }
    
    void Update() {
        if (state == State.active) {
            Pulling();
        }
    }

    private void Pulling() {
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
        for (int i = 0; i < allGameObjects.Length; i++) {
            if (allGameObjects[i].TryGetComponent<Velocity>(out _) && !allGameObjects[i].CompareTag("Player")) {
                Vector3 forceDirection = transform.position - allGameObjects[i].transform.position;
                float distence = forceDirection.magnitude;
                if (distence < 3) {
                    Destroy(allGameObjects[i]);
                } else if (distence < 15) {
                    Velocity objectVelocityScript = allGameObjects[i].GetComponent<Velocity>();
                    objectVelocityScript.velocity += forceDirection * (FORCE / (distence * distence));
                }
            } else if (allGameObjects[i].CompareTag("Player")) {
                Vector3 forceDirection = transform.position - allGameObjects[i].transform.position;
                float distence = forceDirection.magnitude;
                Player playerScript = allGameObjects[i].GetComponent<Player>();
                if (distence < 3 && playerScript.state == Player.State.GAME) {
                    allGameObjects[i].transform.Find("PlayerVisual").GetComponent<PlayerAnimator>().SetDead();
                } else if (distence < 15 && playerScript.state == Player.State.GAME) {
                    playerScript.exSpeed += forceDirection * (FORCE * 500 / (distence * distence * distence));
                }
            }
        }
    }

    public override void Initialize() {
        state = State.active;
    }

    public override void Reset() {
        state = State.idle;
    }
}
