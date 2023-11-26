using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEditor.Profiling;

public class Player : MonoBehaviour {
   
    public enum State { GAME, SELECT_ITEM, STOP, WIN, LOSE };

    [SerializeField] private float normalMoveSpeed;
    [SerializeField] private float accelerateMoveSpeed;
    [SerializeField] private float moveSpeedJumpWallratio;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravityMaxSpeedWithFriction;
    [SerializeField] private float gravityMaxSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float buttonPressedWindow;


    private bool isWalking = false;
    private bool isJumping = false;
    private float buttonPressedTime;
    private float verticalVelocity;
    private float velocity;
    public State state;
    private CharacterController controller;
    private PlayerInputActions playerInputActions;
    private CinemachineVirtualCamera virtualCamera;
    private string item;
    Vector3 followObjectMove;
    

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        virtualCamera = transform.Find("FollowCamera").GetComponent<CinemachineVirtualCamera>();
        controller = GetComponent<CharacterController>();
    }

    private void Start() {
        normalMoveSpeed = 5f;
        accelerateMoveSpeed = 7f;
        moveSpeedJumpWallratio = 5f;
        rotateSpeed = 10f;
        velocity = normalMoveSpeed;
        jumpSpeed = 25f;
        gravity = 60;
        gravityMaxSpeed = 20f;
        gravityMaxSpeedWithFriction = 5f;
        buttonPressedWindow = .3f;
        item = null;
        state = State.STOP;
    }

    private void Update() {
        if (state == State.GAME || state == State.SELECT_ITEM) {
            HandleMovement();
            HandleJump();
            HandleFacement();
        }
    }

    public void Enable(State new_state) {
        state = new_state;
        playerInputActions.Player.Enable();
        playerInputActions.Player.Jump.started += DoJump;
        playerInputActions.Player.Jump.canceled += CancelJump;
        playerInputActions.Player.Accelerate.started += DoAccelerate;
        playerInputActions.Player.Accelerate.canceled += NoAccelerate;
    }

    public void Disable(State new_state) {
        playerInputActions.Player.Disable();
        state = new_state;
    }

    public bool IsWalking() {
        return isWalking;
    }

    private Vector3 GetMoveDirNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
        Vector3 dir = new(0,0,0);
        if (inputVector.y > 0) {
            dir += virtualCamera.transform.forward;
        } else if (inputVector.y < 0) {
            dir -= virtualCamera.transform.forward;
        }
        if (inputVector.x > 0) {
            dir += virtualCamera.transform.right;
        } else if (inputVector.x < 0) {
            dir -= virtualCamera.transform.right;
        }
        dir.y = 0;
        return dir.normalized;
    }

    private void HandleMovement() {
        Vector3 moveDir = GetMoveDirNormalized();
        Vector3 moveVector = velocity * Time.deltaTime * moveDir;
        controller.Move(moveVector);
        isWalking = moveDir != Vector3.zero;
        // follow object move
        controller.Move(followObjectMove);
        followObjectMove = Vector3.zero;
    }

    public bool IsFlying() {
        float pressJump = playerInputActions.Player.Jump.ReadValue<float>();
        return pressJump > 0;
    }

    private void HandleJump() {
        Transform rocketTransform = transform.Find("Rocket");
        
        if (rocketTransform != null) {
            Vector3 moveVector = Vector3.zero;
            bool isWall = CheckWall();
            if (IsFlying()) {
                buttonPressedTime += Time.deltaTime;
                verticalVelocity = jumpSpeed;
            } else {
                verticalVelocity -= gravity * Time.deltaTime;
            }
            verticalVelocity = Mathf.Clamp(verticalVelocity,
                                            isWall ? -gravityMaxSpeedWithFriction : -gravityMaxSpeed,
                                            float.PositiveInfinity);
            moveVector.y = verticalVelocity;
            controller.Move(moveVector * Time.deltaTime);
        } else {
            Vector3 moveVector = Vector3.zero;
            bool isWall = CheckWall();
            if (isJumping && isWall) {
                WallJump();
                return;
            }
            if (isJumping) {
                buttonPressedTime += Time.deltaTime;
                verticalVelocity = jumpSpeed;
                if (buttonPressedTime > buttonPressedWindow) {
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
    }

    private void DoAccelerate(InputAction.CallbackContext context) {
        velocity = accelerateMoveSpeed;
    }

    private void NoAccelerate(InputAction.CallbackContext context) {
        velocity = normalMoveSpeed;
    }

    private void DoJump(InputAction.CallbackContext context) {
        if (CheckWall() || IsGrounded()) {
            isJumping = true;
            buttonPressedTime = 0;
        }
    }

    private void CancelJump(InputAction.CallbackContext context) {
        isJumping = false;
        verticalVelocity = Mathf.Min(verticalVelocity, 0);
    }

    private void WallJump() {
        Vector3 moveVector = Vector3.zero;
        verticalVelocity = 20;
        Vector3 p1 = transform.position + controller.center + 0.5F * -controller.height * Vector3.up;
        Vector3 p2 = p1 + Vector3.up * controller.height;
        float castDistance = .2f;
        if (Physics.CapsuleCast(p1, p2, controller.radius, transform.forward, out RaycastHit hit, castDistance) 
                && TagCanJump(hit.collider)) {
            moveVector = normalMoveSpeed * moveSpeedJumpWallratio * hit.normal;
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
                && TagCanJump(hit.collider);
    }

    private bool IsGrounded() {
        Vector3 p1 = transform.position + controller.center;
        float castDistance = .2f;
        return Physics.SphereCast(p1, controller.height / 2, Vector3.down, out RaycastHit hit, castDistance) 
                && TagCanJump(hit.collider);
    }

    private void HandleFacement() {
        Vector3 dir = GetMoveDirNormalized();
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
            item = GetTopParentObjectName(hit.gameObject.transform);
            // Remove the odd name ending
            item = item.Replace("(Clone)", "");
            Destroy(hit.gameObject);
            // TODO: move to the start position of stage
            ModifyPosition(Vector3.zero);
            state = State.STOP;
        } else if (state == State.GAME && hit.gameObject.TryGetComponent<PlayerFollowObject>(out var playerFollow)) {
            // follow object move
            followObjectMove = playerFollow.GetDiffPosition();
        }
    }   

    private string GetTopParentObjectName(Transform obj) {
        while (obj.parent) {
            obj = obj.parent;
        }
        return obj.gameObject.name;
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

    private bool TagCanJump(Collider collider) {
        return collider.CompareTag("Airplane") ||
            collider.CompareTag("Wall");
    }
}
