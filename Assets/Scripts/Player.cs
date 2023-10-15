using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour {
   
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float jumpSpeed = 30f;
    [SerializeField] private float gravitySpeed = 20f;
    [SerializeField] private float buttonPressedWindow = .3f;
    [SerializeField] private GameInput gameInput;

    private bool isWalking = false;
    private bool isJumping = false;
    private bool canJump = false;
    private float buttonPressedTime;
    // private CapsuleCollider capsuleCollider;
    private Rigidbody rigidbody_;

    public void Start() {
        rigidbody_ = GetComponent<Rigidbody>();
        rigidbody_.useGravity = false;
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

        rigidbody_.velocity = gravitySpeed * Vector3.down;
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
        }

        isWalking = moveDir != Vector3.zero;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Collider>() != null) {
            canJump = true;
            gravitySpeed = 5;
        }
    }

    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.GetComponent<Collider>() != null) {
            canJump = true;
            gravitySpeed = 5;
        }
    }

    void OnCollisionExit(Collision collision) {
        canJump = false;
        gravitySpeed = 20;
    }

    private void HandleFacement() {
        Vector3 moveDir = gameInput.GetMovementVectorNormalized();
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }
}
