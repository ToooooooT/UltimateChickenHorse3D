using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : BaseItem
{
    private enum State { IDLE, CHANGE_SIZE };

    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private float enlargeSpeed;

    private Vector3 originLocalScale;
    private State state;

    void Awake() {
        state = State.IDLE;
    }

    void Start() {
        minSize = transform.localScale.x;
        maxSize = 5f;
        enlargeSpeed = 1.0f;
        originLocalScale = transform.localScale;
    }

    void Update() {
        if (state == State.CHANGE_SIZE) {
            ChangeSize();
        }
    }

    private void ChangeSize() {
        float size = transform.localScale.x;
        size += enlargeSpeed * Time.deltaTime;
        if (size > maxSize || size < minSize) {
            enlargeSpeed *= -1;
        }
        size = Mathf.Clamp(size, minSize, maxSize);
        transform.localScale = new Vector3(size, size, size);
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            if (player.GetState() == Player.State.GAME) {
                player.SetDead();
            }
        }
    }

    public override void Initialize() {
        state = State.CHANGE_SIZE;
    }   

    public override void Reset() {
        state = State.IDLE;
        transform.localScale = originLocalScale;
        if (enlargeSpeed < 0) {
            enlargeSpeed *= -1;
        }
    }   
}
