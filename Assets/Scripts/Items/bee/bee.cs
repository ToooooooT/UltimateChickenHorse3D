using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    private enum State { Idle, Move, Attack };

    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float accelerate;
    [SerializeField] private float max_speed;

    private Animator animator;
    private State state;
    private GameObject player2follow;

    void Start() {
        animator = GetComponent<Animator>(); 
        state = State.Idle;
        speed = 0f;
        rotateSpeed = 10.0f;
        accelerate = 1.0f;
        max_speed = 7.0f;
        player2follow = null;
    }

    void Update() {
        if (state == State.Move) {
            FollowPlayer();
        }       
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (state == State.Idle) {
                animator.SetTrigger("Move");
                state = State.Move;
                player2follow = other.gameObject;
            } else if (state == State.Move) {
                animator.SetTrigger("Attack");
                state = State.Attack;
                player2follow.GetComponent<Player>().state = Player.State.STOP;
            }
        }
    } 

    private void FollowPlayer() {
        speed = speed + accelerate * Time.deltaTime;
        speed = Mathf.Clamp(speed, 0, max_speed);
        Vector3 dir = (player2follow.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        // handle face
        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotateSpeed);
    }
}
