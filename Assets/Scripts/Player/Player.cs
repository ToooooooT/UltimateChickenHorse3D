using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

public class Player : MonoBehaviour {
   
    public enum State { GAME, SELECT_ITEM, STOP, WIN, LOSE };

    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelerateMoveSpeed;
    [SerializeField] private float moveSpeedJumpWallratio;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravityMaxSpeedWithFriction;
    [SerializeField] private float gravityMaxSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float buttonPressedWindow;
    private CinemachineVirtualCamera virtualCamera;


    private bool isWalking = false;
    private bool isJumping = false;
    private bool canJump = false;
    private float buttonPressedTime;
    private float verticalVelocity;
    public State state;
    private CharacterController controller;
    private GameInput gameInput;
    private string item;
    

    private void Awake() {
        gameInput = GameObject.FindGameObjectWithTag("GameInput").GetComponent<GameInput>();
        GameObject[] virtualCameras = GameObject.FindGameObjectsWithTag("Camera");
        for(int i = 0; i < virtualCameras.Length; i++) {
            if(virtualCameras[i].name == "FollowCamera") {
                virtualCamera = virtualCameras[i].GetComponent<CinemachineVirtualCamera>();
            }
        }
        controller = GetComponent<CharacterController>();
        state = State.STOP;
    }

    private void Start() {
        moveSpeed = 5f;
        accelerateMoveSpeed = 7f;
        moveSpeedJumpWallratio = 5f;
        rotateSpeed = 10f;
        jumpSpeed = 25f;
        gravity = 60;
        gravityMaxSpeed = 20f;
        gravityMaxSpeedWithFriction = 5f;
        buttonPressedWindow = .3f;
        item = null;
    }

    private void Update() {
        if (state == State.GAME || state == State.SELECT_ITEM) {
            HandleMovement();
            HandleJump();
            HandleFacement();
        }
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleMovement() {
        // velocity = sqrt(JumpHeight * (-2) * gravity)
        Vector3 dir = new Vector3(0,0,0);
        if (Input.GetKey(KeyCode.W)) {
            dir += virtualCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            dir += -virtualCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.D)) {
            dir += virtualCamera.transform.right;
        }
        if (Input.GetKey(KeyCode.A)) {
            dir += -virtualCamera.transform.right;
        }

        dir.y = 0;
        dir.Normalize();
        Vector3 moveDir = dir;
        float velocity = gameInput.AccelerateMove() ? accelerateMoveSpeed : moveSpeed;
        Vector3 moveVector = velocity * Time.deltaTime * moveDir;
        controller.Move(moveVector);
        isWalking = moveDir != Vector3.zero;
    }

    private void HandleJump() {
        Vector3 moveVector = Vector3.zero;
        bool isWall = CheckWall();
        bool isGrounded = IsGrounded();
        canJump = isWall || isGrounded;
        if (Input.GetKeyDown(KeyCode.Space) && canJump) {
            isJumping = true;
            buttonPressedTime = 0;
        }
        if (isJumping && isWall) {
            WallJump();
            return;
        }
        if (isJumping) {
            buttonPressedTime += Time.deltaTime;
            verticalVelocity = jumpSpeed;
            if (buttonPressedTime > buttonPressedWindow || Input.GetKeyUp(KeyCode.Space)) {
                isJumping = false;
                verticalVelocity = 0;
            }
        } else {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        verticalVelocity = Mathf.Clamp(verticalVelocity, 
                                        isWall ? -gravityMaxSpeedWithFriction : -gravityMaxSpeed, 
                                        float.PositiveInfinity);
        moveVector.y = verticalVelocity;
        controller.Move(moveVector * Time.deltaTime);
    }

    private void WallJump() {
        Vector3 moveVector = Vector3.zero;
        verticalVelocity = 20;
        Vector3 p1 = transform.position + controller.center + 0.5F * -controller.height * Vector3.up;
        Vector3 p2 = p1 + Vector3.up * controller.height;
        float castDistance = .2f;
        if (Physics.CapsuleCast(p1, p2, controller.radius, transform.forward, out RaycastHit hit, castDistance) 
                && hit.collider.CompareTag("Wall")) {
            moveVector = moveSpeed * moveSpeedJumpWallratio * hit.normal;
        }
        moveVector.y = verticalVelocity;
        controller.Move(moveVector * Time.deltaTime);
        isJumping = false;
    }

    private bool CheckWall() {
        Vector3 p1 = transform.position + controller.center + 0.5F * -controller.height * Vector3.up;
        Vector3 p2 = p1 + Vector3.up * controller.height;
        float castDistance = .2f;
        return Physics.CapsuleCast(p1, p2, controller.radius, transform.forward, out RaycastHit hit, castDistance)
                && hit.collider.CompareTag("Wall");
    }

    private bool IsGrounded() {
        Vector3 p1 = transform.position + controller.center;
        float castDistance = .2f;
        return Physics.SphereCast(p1, controller.height / 2, Vector3.down, out RaycastHit hit, castDistance) 
                && hit.collider.CompareTag("Wall");
    }

    private void HandleFacement() {
        Vector3 dir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) {
            dir += virtualCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            dir += -virtualCamera.transform.forward;
        }
        if (Input.GetKey(KeyCode.D)) {
            dir += virtualCamera.transform.right;
        }
        if (Input.GetKey(KeyCode.A)) {
            dir += -virtualCamera.transform.right;
        }

        dir.y = 0;
        dir.Normalize();
        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotateSpeed);
    }

    public void ModifyPosition(Vector3 newPosition) {
        bool origin = controller.enabled;
        controller.enabled = false;
        transform.position = newPosition;
        controller.enabled = origin;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (state == State.SELECT_ITEM && hit.gameObject.CompareTag("ChoosingItem")) {
            item = hit.gameObject.name;
            // Remove the odd name ending
            item = item.Replace("(Clone)", "");
            Destroy(hit.gameObject);
            // TODO: move to the start position of stage
            ModifyPosition(Vector3.zero);
            state = State.STOP;
        }
    }   

    public string GetItemName() {
        return item;
    }

    public void RemoveItem() {
        item = null;
    }

    public bool HaveItem() {
        return item != null;
    }
}
