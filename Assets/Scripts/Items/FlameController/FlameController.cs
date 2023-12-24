using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlameController : BaseItem
{
    private enum State { IDLE, FIRE };

    [SerializeField] private float period;       

    private GameObject fire;
    private float timeCounter;
    private State state;
    private CapsuleCollider fireCollider;

    void Awake() {
        fire = transform.Find("Fire").gameObject;
        fire.SetActive(false);
        timeCounter = 0;
        CapsuleCollider[] capsuleColliders = GetComponents<CapsuleCollider>();
        foreach (CapsuleCollider capsuleCollider in capsuleColliders) {
            if (capsuleCollider.isTrigger) {
                fireCollider = capsuleCollider;
                fireCollider.enabled = false;
            }
        }
    }

    void Start() {
        period = 5.0f;
    }

    void Update() {
        if (state == State.FIRE) {
            timeCounter += Time.deltaTime;
            if (timeCounter > period) {
                timeCounter = 0;
                fire.SetActive(!fire.activeSelf);
                fireCollider.enabled = fire.activeSelf;
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && state == State.FIRE && fire.activeSelf) {
            Player player = other.GetComponent<Player>();
            if (player.state == Player.State.GAME) {
                other.transform.Find("PlayerVisual").GetComponent<PlayerAnimator>().SetDead();
            }
        }
    }

    public override void Initialize() {
        state = State.FIRE;
        timeCounter = 0;
        fire.SetActive(false);
        fireCollider.enabled = fire.activeSelf;
    }

    public override void Reset() {
        state = State.IDLE;
        fire.SetActive(false);
        fireCollider.enabled = fire.activeSelf;
    }
}
