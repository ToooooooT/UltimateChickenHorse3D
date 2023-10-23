using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour {
   
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravityMaxSpeed;
    [SerializeField] private float buttonPressedWindow;
    [SerializeField] private GameInput gameInput;

    private bool isWalking = false;
    private bool isJumping = false;
    private bool canJump = false;
    private float buttonPressedTime;
    // private CapsuleCollider capsuleCollider;
    private Rigidbody rigidbody_;

    public void Start() {
        moveSpeed = 7f;
        rotateSpeed = 10f;
        jumpSpeed = 25f;
        gravityMaxSpeed = 20f;
        buttonPressedWindow = .3f;
        rigidbody_ = GetComponent<Rigidbody>();
        rigidbody_.useGravity = true;
        // capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Update() {
        HandleMovement();
        HandleFacement();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleMovement() {
        Vector3 moveDir = gameInput.GetMovementVectorNormalized();
        transform.position += moveSpeed * Time.deltaTime * moveDir;

        if (Input.GetKeyDown(KeyCode.Space) && canJump) {
            isJumping = true;
            buttonPressedTime = 0;
        }
        if (isJumping) {
            buttonPressedTime += Time.deltaTime;
            rigidbody_.velocity = jumpSpeed * Vector3.up;
            if (buttonPressedTime > buttonPressedWindow || Input.GetKeyUp(KeyCode.Space)) {
                isJumping = false;
            }
        } else if (rigidbody_.velocity.y < -gravityMaxSpeed) {
            rigidbody_.velocity = gravityMaxSpeed * Vector3.down;
        }

        isWalking = moveDir != Vector3.zero;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Collider>() != null) {
            canJump = true;
            gravityMaxSpeed = 5;
        }
    }

    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.GetComponent<Collider>() != null) {
            canJump = true;
            gravityMaxSpeed = 5;
        }
    }

    void OnCollisionExit(Collision collision) {
        canJump = false;
        gravityMaxSpeed = 20;
    }

    private void HandleFacement() {
        Vector3 moveDir = gameInput.GetMovementVectorNormalized();
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
}
