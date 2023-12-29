using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBomb : Velocity
{
    private enum State { MOVE, EXPLOSION };

    private State state;

    public GameObject cannon;

    void Start() {
        state = State.MOVE;
    }

    void Update() {
        if (state == State.MOVE) {
            Forward();
            Move();
            CheckCollision();
        }
    }

    private void Move() {
        transform.position = transform.position + Time.deltaTime * 40 * velocity;
    }

    private void CheckCollision() {
        if (Physics.SphereCast(transform.position, 1.1f, velocity, out RaycastHit hitInfo, 1f)) {
            GameObject collidedObject = hitInfo.collider.gameObject;
            if (collidedObject != cannon) {
                if (collidedObject.TryGetComponent<Player>(out var player) && 
                player.GetState() == Player.State.GAME) {
                    player.SetDead();
                    Bomb();
                } else if (collidedObject.CompareTag("Wall")) {
                    Bomb();
                }
            }
        }
    }

    private void Bomb() {
        if (state == State.MOVE) {
            state = State.EXPLOSION;
            Destroy(gameObject, 2);
            GetComponent<MeshRenderer>().enabled = false;
            transform.Find("Wind").gameObject.SetActive(false);
            transform.Find("Explosion").gameObject.SetActive(true);
        }
    }
}
