using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Velocity
{
    public GameObject parentFireBallShooter;

    [SerializeField] private float shrinkSpeed;

    private Transform smoke;
    private Transform fire;
    private bool isBurning;


    void Start() {
        smoke = transform.Find("Smoke");
        fire = transform.Find("Fire");
        isBurning = false;
        shrinkSpeed = 0.2f;
    }

    void Update() {
        if (isBurning) {
            float shrink = shrinkSpeed * Time.deltaTime;
            fire.localScale -= new Vector3(shrink, shrink, shrink);
            fire.position -= shrinkSpeed * Time.deltaTime * Vector3.up;
        } else {
            Move();
            CheckCollision();
            ChangeSmokeDirection();
        }
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
                    Burn();
                } else if (collidedObject.CompareTag("Wall")) {
                    Burn();
                }
            }
        }
    }

    private void Burn() {
        isBurning = true;
        Destroy(gameObject, 10);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
        smoke.gameObject.SetActive(false);
        transform.Find("Sun").gameObject.SetActive(false);
        fire.gameObject.SetActive(true);
    }
}
