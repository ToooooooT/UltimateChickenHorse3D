using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCannon : BaseItem
{
    private enum State {NONE, IDLE, SHOOTING, COOLDOWN}

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
    }

    void Update() {
        if (state == State.SHOOTING) {
            Shooting();
        } else if (state == State.COOLDOWN) {
            Cooldown();
        }
    }

    void Shooting() {
        Debug.Log(shootingObject.name);
        if(countdown > 5) {
            countdown -= 10 * Time.deltaTime;
            shootingObject.transform.position = cannon.transform.position;
        } else {
            trueSpeed = SHOOTING_SPEED;
            if (shootingObject.CompareTag("Player")) {
                shootingObject.GetComponent<Player>().exSpeed = trueSpeed * cannon.transform.forward;
            } else {
                shootingObject.GetComponent<Velocity>().velocity = trueSpeed * cannon.transform.forward;
            }
            shootingObject.transform.localScale = scale;
            state = State.COOLDOWN;
        }
    }

    void Cooldown() {
        Debug.Log(shootingObject.name);
        if (countdown > 0) {
            if (shootingObject.CompareTag("Player")) {
                shootingObject.GetComponent<Player>().exSpeed += trueSpeed * cannon.transform.forward 
                                            + (SHOOTING_TIME - 5 - countdown) * new Vector3(0, -30f, 0);
            } else {
                shootingObject.GetComponent<Velocity>().velocity += trueSpeed * cannon.transform.forward 
                                            + (SHOOTING_TIME - 5 - countdown) * new Vector3(0,-30f,0);
            }
            trueSpeed *= 0.98f;
            countdown -= 10 * Time.deltaTime;
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
            bombParicle.Play();
        }
    }

    bool Shootable(GameObject obj) {
        return obj.CompareTag("Player") || !obj.CompareTag("Wall");
    }

    public override void Initialize() {
        state = State.IDLE;
    }

    public override void Reset() {
        state = State.NONE;
    }
}
