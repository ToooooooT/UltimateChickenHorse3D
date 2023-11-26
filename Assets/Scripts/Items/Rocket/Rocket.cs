using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : BaseItem  
{
    public enum State { Idle, Equipped, Flying };
    public State state;
    private State lastState;
    private Transform FireTransform;
    private Player player;
    private Vector3 origin_position;
    private Quaternion origin_rotation;

    // Start is called before the first frame update
    void Start() {
        state = State.Idle;
        lastState = State.Idle;
        FireTransform = transform.Find("Fire");
        player = null;
        InitFire();
    }

    // Update is called once per frame
    void Update() {
        HandleFire();
        UpdateState();
        HandleFly();
    }

    private void InitFire() {
        FireTransform.gameObject.SetActive(false);
    }

    private void UpdateState() {
        if (player != null && player.isPressSpace) {
            state = State.Flying;
        } else if (player != null) {
            state = State.Equipped;
        }
    }

    private void HandleFly() {
        if (state == State.Flying) {
            player.exSpeed += new Vector3(0, player.jumpSpeed / 10, 0);
        }
    }

    private void HandleFire() {
        if (lastState == State.Equipped && state == State.Flying) {
            FireTransform.gameObject.SetActive(true);
        }
        if (lastState == State.Flying && state != State.Flying) {
            FireTransform.gameObject.SetActive(false);
        }
        lastState = state;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            state = State.Equipped;
            transform.SetParent(other.transform);
            transform.localPosition = new Vector3(0f, 0f, -1f);
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            GetComponent<BoxCollider>().enabled = false;
            player = GetComponentInParent<Player>();
            Debug.Log("Object collided with Player!");
        }
    }

    public override void Initialize() {
        origin_position = transform.position;
        origin_rotation = transform.rotation;
    }

    public override void Reset() {
        InitFire();
        state = State.Idle;
        transform.parent = null;
        player = null;
        GetComponent<BoxCollider>().enabled = true;
        transform.SetPositionAndRotation(origin_position, origin_rotation);
    }
}
