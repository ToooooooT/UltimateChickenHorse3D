using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCannon : MonoBehaviour
{
    private enum State{idle, shooting, cooldown}
    private State state;
    private const float SHOOTING_SPEED = 500f;
    private float trueSpeed;
    private const float SHOOTING_TIME = 10f;
    private float countdown;
    private GameObject cannon;
    private GameObject shootingObject;
    private float cannonGoUp;
    private Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        cannonGoUp = 1f;
        state = State.idle;
        cannon = transform.Find("cannon").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.idle)
            Rotate();
        else if (state == State.shooting)
            Shooting();
        else if (state == State.cooldown)
            Cooldown();
    }
    void Rotate()
    {
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y += 0.6f;
        transform.eulerAngles = currentRotation;
        cannon.transform.forward += new Vector3(0, cannonGoUp * 0.01f, 0);
        if(cannon.transform.forward.y <= 0) {
            cannonGoUp = 1f;
        } else if(cannon.transform.forward.y >= 0.8) {
            cannonGoUp = -1f;
        }
    }
    void Shooting()
    {
        if(countdown > 5) {
            countdown -= 0.1f;
            shootingObject.transform.position = cannon.transform.position;
        }
        else {
            trueSpeed = SHOOTING_SPEED;
            if (shootingObject.CompareTag("Player")) {
                Player playerScript = shootingObject.GetComponent<Player>();
                playerScript.exSpeed = trueSpeed * cannon.transform.forward;
            }
            else {
                Velocity velocityScript = shootingObject.GetComponent<Velocity>();
                velocityScript.velocity = trueSpeed * cannon.transform.forward;
            }
            shootingObject.transform.localScale = scale;
            state = State.cooldown;
        }
    }
    void Cooldown()
    {
        if (countdown > 0) {
            if (shootingObject.CompareTag("Player")) {
                Player playerScript = shootingObject.GetComponent<Player>();
                playerScript.exSpeed += trueSpeed * cannon.transform.forward + (SHOOTING_TIME - 5 - countdown) * new Vector3(0, -30f, 0);
            }
            else {
                Velocity velocityScript = shootingObject.GetComponent<Velocity>();
                velocityScript.velocity += trueSpeed * cannon.transform.forward + (SHOOTING_TIME - 5 - countdown) * new Vector3(0,-30f,0);
            }
            trueSpeed *= 0.98f;
            countdown -= 0.1f;
        }
        else {
            state = State.idle;
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        GameObject obj = collision.gameObject;
        if (Shootable(obj) && state == State.idle){
            shootingObject = obj;
            countdown = SHOOTING_TIME;
            scale = obj.transform.localScale;
            obj.transform.localScale = new Vector3(0, 0, 0);
            state = State.shooting;
        }
    }
    bool Shootable(GameObject obj)
    {
        return obj.CompareTag("Player")||
            obj.CompareTag("Airplane")||
            obj.CompareTag("CannonBomb");
    }
}
