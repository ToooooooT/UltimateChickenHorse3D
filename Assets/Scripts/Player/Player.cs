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
   
    public enum State { MOVE, GAME, SELECT_ITEM, STOP, WIN, LOSE };

    public float jumpSpeed;
    public bool isPressSpace = false;
    public float verticalVelocity;
    public State state;
    public Vector3 exSpeed;

    [SerializeField] private float normalMoveSpeed;
    [SerializeField] private float accelerateMoveSpeed;
    [SerializeField] private float moveSpeedJumpWallratio;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float gravityMaxSpeedWithFriction;
    [SerializeField] private float gravityMaxSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float buttonPressedWindow;
    [SerializeField] private float resistanceRatio;
    [SerializeField] private float exSpeedThreshold;

    private bool isWalking = false;
    private bool isJumping = false;
    private float buttonPressedTime;
    private float velocity;
    public Vector3 lastExSpeed;
    private CharacterController controller;
    private CinemachineVirtualCamera virtualCamera;
    private InputActionMap playerInputActionMap;
    private InputActionMap placeObjectInputActionMap;
    private string item;
    private Vector3 followObjectMove;
    

    private void Awake() {
        // playerInputActions = new PlayerInputActions();
        virtualCamera = transform.Find("Camera").GetComponent<CinemachineVirtualCamera>();
        controller = GetComponent<CharacterController>();
        playerInputActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        placeObjectInputActionMap = GetComponent<PlayerInput>().actions.FindActionMap("PlaceObject");
    }

    private void Start() {
        normalMoveSpeed = 5f;
        accelerateMoveSpeed = 7f;
        moveSpeedJumpWallratio = 10f;
        rotateSpeed = 10f;
        velocity = normalMoveSpeed;
        jumpSpeed = 25f;
        gravity = 60;
        gravityMaxSpeed = 20f;
        gravityMaxSpeedWithFriction = 5f;
        buttonPressedWindow = .3f;
        item = null;
        exSpeed = Vector3.zero;
        Enable(State.MOVE);
        resistanceRatio = 0.7f;
        exSpeedThreshold = 55f;
    }

    private void Update() {
        if (state == State.GAME || state == State.SELECT_ITEM || state == State.MOVE) {
            HandleMovement();
            HandleJump();
            HandleFacement();
        }
    }

    public void Enable(State new_state) {
        state = new_state;
        playerInputActionMap.Enable();
        InputAction jump = playerInputActionMap.FindAction("Jump");
        jump.started += DoJump;
        jump.canceled += CancelJump;
        InputAction accelerate = playerInputActionMap.FindAction("Accelerate");
        accelerate.started += DoAccelerate;
        accelerate.canceled += NoAccelerate;
        InputAction giveup = playerInputActionMap.FindAction("GiveUp");
        giveup.performed += GiveUp;
    }

    public void Disable(State new_state) {
        playerInputActionMap.Disable();
        InputAction jump = playerInputActionMap.FindAction("Jump");
        jump.started -= DoJump;
        jump.canceled -= CancelJump;
        InputAction accelerate = playerInputActionMap.FindAction("Accelerate");
        accelerate.started -= DoAccelerate;
        accelerate.canceled -= NoAccelerate;
        state = new_state;
    }

    public bool IsWalking() {
        return isWalking;
    }

    private Vector3 GetMoveDirNormalized() {
        Vector2 inputVector = playerInputActionMap.FindAction("Move").ReadValue<Vector2>().normalized;
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
        moveVector += lastExSpeed * Time.deltaTime;
        lastExSpeed = resistanceRatio * new Vector3(lastExSpeed.x, 0, lastExSpeed.z) 
                    + new Vector3(0, Mathf.Max(lastExSpeed.y - gravity * Time.deltaTime, 0), 0) + exSpeed;
        if (lastExSpeed.magnitude > exSpeedThreshold) {
            lastExSpeed = exSpeedThreshold * lastExSpeed.normalized;
        }
        exSpeed = Vector3.zero;
        controller.Move(moveVector);
        isWalking = moveDir != Vector3.zero;
        // follow object move
        controller.Move(followObjectMove);
        followObjectMove = Vector3.zero;
    }

    private void HandleJump() {
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

    private void GiveUp(InputAction.CallbackContext context) {
        if (state == State.GAME) {
            Disable(State.LOSE);
        }
    }

    private void DoAccelerate(InputAction.CallbackContext context) {
        velocity = accelerateMoveSpeed;
    }

    private void NoAccelerate(InputAction.CallbackContext context) {
        velocity = normalMoveSpeed;
    }

    private void DoJump(InputAction.CallbackContext context) {
        isPressSpace = true;
        if (CheckWall() || IsGrounded()) {
            isJumping = true;
            buttonPressedTime = 0;
        }
    }

    private void CancelJump(InputAction.CallbackContext context) {
        isPressSpace = false;
        isJumping = false;
        verticalVelocity = Mathf.Min(verticalVelocity, 0);
    }

    private void WallJump() {
        verticalVelocity = 20;
        Vector3 p1 = transform.position + controller.center + 0.5F * -controller.height * Vector3.up;
        Vector3 p2 = p1 + Vector3.up * controller.height;
        float castDistance = .2f;
        if (Physics.CapsuleCast(p1, p2, controller.radius, transform.forward, out RaycastHit hit, castDistance) 
                && TagCanJump(hit.collider)) {
            exSpeed += normalMoveSpeed * moveSpeedJumpWallratio * hit.normal;
        }
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
        float castDistance = 2f;
        return Physics.SphereCast(p1, controller.height / 2, Vector3.down, out RaycastHit hit, castDistance) 
                && TagCanJump(hit.collider);
    }

    private void HandleFacement() {
        Vector3 dir = GetMoveDirNormalized();
        if (dir.magnitude > 0f) {
            transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotateSpeed);
        }
    }

    public void ModifyPosition(Vector3 newPosition) {
        controller.enabled = false;
        transform.position = newPosition;
        controller.enabled = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (state == State.SELECT_ITEM && hit.gameObject.CompareTag("ChoosingItem")) {
            GetItem(hit.gameObject);
            // TODO: move to the start position of stage
            ModifyPosition(Vector3.zero);
            state = State.STOP;
        } else if (state == State.GAME && hit.gameObject.TryGetComponent<PlayerFollowObject>(out var playerFollow)) {
            // follow object move
            followObjectMove = hit.gameObject.GetComponent<PlayerFollowObject>().GetDiffPosition();
        }
    }

    private void GetItem(GameObject obj) {
        item = GetTopParentObjectName(obj.transform);
        // Remove the odd name ending
        item = item.Replace("(Clone)", "");
        while (obj.transform.parent) {
            obj = obj.transform.parent.gameObject;
        }
        Destroy(obj);
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
            collider.CompareTag("Wall") ||
            collider.CompareTag("CannonPipe");
    }

    public InputActionMap GetPlayerInputActionMap() {
        return playerInputActionMap;
    }

    public InputActionMap GetPlaceObjectInputActionMap() {
        return placeObjectInputActionMap;
    }
}
