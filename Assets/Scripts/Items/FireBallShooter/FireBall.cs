using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Velocity
{
    public GameObject parentFireBallShooter;

    private Transform smoke;

    void Start() {
        smoke = transform.Find("Smoke");
    }

    void Update() {
        Move();
        CheckCollision();
        ChangeSmokeDirection();
    }

    private void Move() {
        transform.position = transform.position + Time.deltaTime * 20 * velocity;
        velocity += 0.5f * Time.deltaTime * new Vector3(0, -1, 0);
    }

    private void ChangeSmokeDirection() {
        smoke.forward = (-velocity).normalized;
    }

    private void CheckCollision() {
        if (Physics.SphereCast(transform.position, 1.1f, velocity, out RaycastHit hitInfo, 1f)) {
            GameObject collidedObject = hitInfo.collider.gameObject;
            if (collidedObject != parentFireBallShooter) {
                if (collidedObject.TryGetComponent<Player>(out var player) && 
                player.GetState() == Player.State.GAME) {
                    player.SetDead();
                } else if (collidedObject.CompareTag("Wall")) {
                    Destroy(gameObject);
                }
            }
        }
    }
}
