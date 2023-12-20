using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlameController : BaseItem
{
    private enum State { IDLE, FIRE };

    [SerializeField] private float period = 5.0f;       

    private GameObject fire;
    private float timeCounter;
    private State state;

    void Start() {
        fire = transform.Find("Fire").gameObject;
        fire.SetActive(false);
        timeCounter = 0;
    }

    void Update() {
        if (state == State.FIRE) {
            timeCounter += Time.deltaTime;
            if (timeCounter > period) {
                timeCounter = 0;
                fire.SetActive(!fire.activeSelf);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") && state == State.FIRE && fire.activeSelf) {
            Player player = other.gameObject.GetComponent<Player>();
            if (player.state == Player.State.GAME) {
                player.state = Player.State.LOSE;
            }
        }
    }

    public override void Initialize() {
        state = State.FIRE;
        timeCounter = 0;
    }

    public override void Reset() {
        state = State.IDLE;
        fire.SetActive(false);
    }
}
