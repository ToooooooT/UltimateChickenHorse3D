using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bee : BaseItem
{
    private enum State { Idle, Move, Attack };

    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float accelerate;
    [SerializeField] private float max_speed;

    private Animator animator;
    private State state;
    private GameObject player2follow;
    private Vector3 origin_position;
    private Quaternion origin_rotation;

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
        if (other.CompareTag("Player")) {
            if (state == State.Idle) {
                animator.SetTrigger("Move");
                state = State.Move;
                player2follow = other.gameObject;
            } else if (state == State.Move) {
                if (player2follow.GetComponent<Player>().GetState() == Player.State.GAME) {
                    animator.SetTrigger("Attack");
                    state = State.Attack;
                    player2follow.GetComponent<Player>().SetDead();
                }
            }
        }
    } 

    private void FollowPlayer() {
        speed += accelerate * Time.deltaTime;
        speed = Mathf.Clamp(speed, 0, max_speed);
        Vector3 dir = (player2follow.transform.position - transform.position).normalized;
        transform.position += speed * Time.deltaTime * dir;
        // handle face
        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotateSpeed);
    }

    public override void Initialize() {
        origin_position = transform.position;
        origin_rotation = transform.rotation;
    }

    public override void Reset() {
        animator.SetTrigger("Idle");
        state = State.Idle;
        player2follow = null;
        transform.SetPositionAndRotation(origin_position, origin_rotation);
    }
}
