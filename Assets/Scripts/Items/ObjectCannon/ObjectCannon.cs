using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCannon : BaseItem
{
    private enum State {NONE, IDLE, SHOOTING, COOLDOWN}

    [SerializeField] private float bombManRatio;

    private State state;
    private const float SHOOTING_SPEED = 500f;
    private float trueSpeed;
    private const float SHOOTING_TIME = 10f;
    private float countdown;
    private GameObject cannon;
    private GameObject shootingObject;
    private Vector3 scale;
    private ParticleSystem bombParicle;

    void Awake() {
        state = State.NONE;
        cannon = transform.Find("cannon").gameObject;
        bombParicle = transform.Find("cannon").Find("Explosion").GetComponent<ParticleSystem>();
        bombManRatio = 0.03f;
    }

    void Update() {
        if (state == State.SHOOTING) {
            Shooting();
        } else if (state == State.COOLDOWN) {
            Cooldown();
        }
    }

    void Shooting() {
        if(countdown > 5) {
            countdown -= 10 * Time.deltaTime;
            if (shootingObject == null) {
                // object in cannon may explode
                return;
            }
            if (!shootingObject.CompareTag("Player")) {
                shootingObject.GetComponent<Velocity>().velocity = cannon.transform.forward;
                shootingObject.transform.forward = cannon.transform.forward;
            }
            shootingObject.transform.position = cannon.transform.position;
        } else {
            if (shootingObject == null) {
                // object in cannon may explode
                return;
            }
            trueSpeed = SHOOTING_SPEED;
            if (shootingObject.CompareTag("Player")) {
                shootingObject.GetComponent<Player>().exSpeed = trueSpeed * cannon.transform.forward;
            } else {
                Vector3 velocity = shootingObject.GetComponent<Velocity>().velocity;
                shootingObject.GetComponent<Velocity>().velocity = velocity.magnitude * cannon.transform.forward.normalized;
            }
            shootingObject.transform.localScale = scale;
            state = State.COOLDOWN;
            bombParicle.Play();
        }
    }

    void Cooldown() {
        if (countdown > 0) {
            countdown -= 10 * Time.deltaTime;
            if (shootingObject == null) {
                // object in cannon may explode
                return;
            }
            if (shootingObject.CompareTag("Player")) {
                shootingObject.GetComponent<Player>().exSpeed += trueSpeed * cannon.transform.forward 
                                            + (SHOOTING_TIME - 5 - countdown) * new Vector3(0, -30f, 0);
            }
            if (shootingObject.CompareTag("BombMan")) {
                shootingObject.GetComponent<Velocity>().velocity += bombManRatio * trueSpeed * cannon.transform.forward; 
                shootingObject.GetComponent<Rigidbody>().velocity = new Vector3(0, shootingObject.GetComponent<Velocity>().velocity.y, 0);
            }
            trueSpeed *= 0.98f;
        } else {
            state = State.IDLE;
        }
    }

    void OnTriggerStay(Collider collision) {
        GameObject obj = collision.gameObject;
        if (Shootable(obj) && state == State.IDLE){
            shootingObject = obj;
            countdown = SHOOTING_TIME;
            scale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(0, 0, 0);
            state = State.SHOOTING;
        }
    }

    bool Shootable(GameObject obj) {
        return obj.CompareTag("Player") || obj.TryGetComponent<Velocity>(out var _);
    }

    public override void Initialize() {
        state = State.IDLE;
    }

    public override void Reset() {
        state = State.NONE;
    }
}
