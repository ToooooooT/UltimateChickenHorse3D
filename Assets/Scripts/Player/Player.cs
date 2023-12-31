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
   
    public enum State { MOVE, GAME, SELECT_ITEM, STOP, WIN, DEAD_ANIMATION, LOSE };

    public float jumpSpeedMultiple;
    public float jumpSpeed;
    public bool isPressSpace = false;
    public float verticalVelocity;
    public Vector3 exSpeed;

    [SerializeField] private State state;
    [SerializeField] private float normalMoveSpeed;
    [SerializeField] private float accelerateMoveSpeed;
    [SerializeField] private float moveSpeedJumpWallratio;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float gravityMaxSpeedWithFriction;
    [SerializeField] private float gravityMaxSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float gravityCache;
    [SerializeField] private float buttonPressedWindow;
    [SerializeField] private float resistanceRatio;
    [SerializeField] private float exSpeedThreshold;

    private bool isWalking = false;
    private bool isJumping = false;
    private float buttonPressedTime;
    private float velocity;
    private Vector3 lastExSpeed;
    private CharacterController controller;
    private CinemachineVirtualCamera virtualCamera;
    private InputActionMap playerInputActionMap;
    private InputActionMap placeObjectInputActionMap;
    private string item;
    private Vector3 followObjectMove;
    private GameObject pauseMenu;
    private GameObject chooseItemCanvas;
    private ParticleSystem jumpParticles;
    private ParticleSystem moveParticles;
    private string gameMode;

    private string skillName;
    private float skillCooldown;
    private Data skillData;
    private float castCooldown;
    private GameObject ornament;
    private const string FOLDERPATH = "SkillsItem";

    private void Awake() {
        virtualCamera = transform.Find("Camera").GetComponent<CinemachineVirtualCamera>();
        controller = GetComponent<CharacterController>();
        playerInputActionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        placeObjectInputActionMap = GetComponent<PlayerInput>().actions.FindActionMap("PlaceObject");
        pauseMenu = GameObject.Find("PauseCanvas").transform.Find("PauseMenu").gameObject;
        chooseItemCanvas = GameObject.Find("ChooseItemCanvas").gameObject;
        jumpParticles = transform.Find("PlayerJumpParticles").GetComponent<ParticleSystem>();
        moveParticles = transform.Find("PlayerMovingParticles").GetComponent<ParticleSystem>();
    }

    private void Start() {
        normalMoveSpeed = 5f;
        accelerateMoveSpeed = 7f;
        moveSpeedJumpWallratio = 10f;
        rotateSpeed = 10f;
        velocity = normalMoveSpeed;
        jumpSpeedMultiple = 1f;
        jumpSpeed = 25f;
        gravity = 60;
        gravityCache = 60;
        gravityMaxSpeed = 20f;
        gravityMaxSpeedWithFriction = 5f;
        buttonPressedWindow = .3f;
        item = null;
        exSpeed = Vector3.zero;
        resistanceRatio = 0.7f;
        exSpeedThreshold = 55f;
        gameMode = PlayerPrefs.GetString("GameMode", "Party");
        Enable(State.MOVE);
    }

    private void Update() {
        if (state == State.GAME || state == State.SELECT_ITEM || state == State.MOVE) {
            if (skillData!=null && Input.GetKeyDown(KeyCode.R)) 
                InceptSkill();
            HandleMovement();
            HandleJump();
            HandleFacement(); 
            UseSkill();
        }
    }
    private void UseSkill()
    {
        if (!UsingSkill()) {
            if (ornament != null) {
                ornament.transform.right = (ornament.transform.right + 0.01f * ornament.transform.forward).normalized;
            }
            return;
        }
        switch (skillName) {
            case "JumpHigh":
                Transform jumperTransform = ornament.transform.Find("Jumper");
                Vector3 jumperLocalPosition = jumperTransform.localPosition;
                if (skillData.jumperClip <= 0.2)
                    jumperLocalPosition.y = 2 - skillData.jumperClip * (4 / 0.2f);
                else
                    jumperLocalPosition.y = -2 + (skillData.jumperClip - 0.2f) * (4 / 0.8f);
                jumperTransform.localPosition = jumperLocalPosition;
                skillData.jumperClip = Mathf.Min(Time.deltaTime + skillData.jumperClip, 1);
                break;
            case "DanceInvincible":
                DanceInvincible();
                break;
            case "Shoot":
                Shoot();
                break;
            case "Magnetic":
                Magnetic();
                break;
            case "Hook":
                Hook();
                break;
            case "Tack":
                Tack();
                break;
        }
        castCooldown -= Time.deltaTime;
    }
    private bool UsingSkill()
    {
        if (skillData == null) return false;
        if (skillData.castTime > 0) return true;
        if (skillData.invincible) return true;
        if (skillData.jumpHigh) return true;
        return false;
    }
    public void ChangeSkill(string newSkillName = "")
    {
        if (newSkillName != "")
            skillName = newSkillName;
        skillData = new SkillReader().GetSkill(skillName);
        skillName = skillData.skillName;
        
        Ornament();
        ResetSkill();
    }
    private void ResetSkill()
    {
        //jump
        jumpSpeedMultiple = 1;
        //dance invincible
        SkillEnableMove();
        transform.up = Vector3.up;
    }
    private void Ornament()
    {
        GameObject ornamentPrefab = Resources.Load<GameObject>(FOLDERPATH + "/" + skillName + "/ornament");
        if (ornament != null) {
            Destroy(ornament);
        }
        ornament = Instantiate(ornamentPrefab, transform);
        ornament.transform.localPosition = skillData.ornamentLocalPosition;
        skillData.ornamentLocalScale = ornament.transform.localScale;
    }
    private void InceptSkill()
    {
        switch (skillName) {
            case "JumpHigh":
                skillData.jumpHigh = !skillData.jumpHigh;
                if (skillData.jumpHigh) {
                    jumpSpeedMultiple = 3;
                    ornament.transform.localPosition = skillData.usingPosition;
                    ornament.transform.localScale = skillData.usingScale;
                    ornament.transform.forward = transform.right;
                }
                else {
                    jumpSpeedMultiple = 1;
                    ornament.transform.localPosition = skillData.ornamentLocalPosition;
                    ornament.transform.localScale = skillData.ornamentLocalScale;
                }
                break;
            case "DanceInvincible": 
                skillData.invincible = !skillData.invincible;
                if (skillData.invincible) {
                    SkillDisableMove();
                    ornament.transform.localPosition = skillData.usingPosition;
                    ornament.transform.localScale = skillData.usingScale;
                    PlayerInvisible(gameObject);
                }
                else {
                    SkillEnableMove();
                    transform.up = Vector3.up;
                    ornament.transform.localPosition = skillData.ornamentLocalPosition;
                    ornament.transform.localScale = skillData.ornamentLocalScale;
                    PlayerVisible(gameObject);
                }
                break;
            case "Shoot": 
                break;
            case "Magnetic": 
                break;
            case "Hook": 
                break;
            case "Tack": 
                break;
        }
        castCooldown = skillData.castTime;
    }
    
    private void DanceInvincible()
    {
        //transform.up = new Vector3(0.37f * Mathf.Cos(skillData.dancingAngle), 0.53f, 0.37f * Mathf.Sin(skillData.dancingAngle));
        //skillData.dancingAngle += Time.deltaTime;

    }
    private void Shoot()
    {

    }
    private void Magnetic()
    {

    }
    private void Hook()
    {

    }
    private void Tack()
    {

    }
    private void PlayerInvisible(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.enabled = false;
        }

        foreach (Transform childTransform in obj.transform) {
            if (childTransform.gameObject.name == "ornament(Clone)")
                continue;
            PlayerInvisible(childTransform.gameObject);
        }
    }
    private void PlayerVisible(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.enabled = true;
        }

        foreach (Transform childTransform in obj.transform) {
            PlayerVisible(childTransform.gameObject);
        }
    }
    private void SkillEnableMove()
    {
        InputAction move = playerInputActionMap.FindAction("Move");
        move.Enable();
        InputAction jump = playerInputActionMap.FindAction("Jump");
        jump.started += DoJump;
        jump.canceled += CancelJump;
        InputAction accelerate = playerInputActionMap.FindAction("Accelerate");
        accelerate.started += DoAccelerate;
        accelerate.canceled += NoAccelerate;
    }
    private void SkillDisableMove()
    {
        InputAction move = playerInputActionMap.FindAction("Move");
        move.Disable();
        InputAction jump = playerInputActionMap.FindAction("Jump");
        jump.started -= DoJump;
        jump.canceled -= CancelJump;
        InputAction accelerate = playerInputActionMap.FindAction("Accelerate");
        accelerate.started -= DoAccelerate;
        accelerate.canceled -= NoAccelerate;
    }
    public void Enable(State new_state) {
        state = new_state;
        playerInputActionMap.Enable();
        InputAction move = playerInputActionMap.FindAction("Move");
        move.Enable();
        InputAction jump = playerInputActionMap.FindAction("Jump");
        jump.started += DoJump;
        jump.canceled += CancelJump;
        InputAction accelerate = playerInputActionMap.FindAction("Accelerate");
        accelerate.started += DoAccelerate;
        accelerate.canceled += NoAccelerate;
        InputAction pause = playerInputActionMap.FindAction("Pause");
        pause.started += Pause;
        switch (gameMode) {
        case "Party":
            InputAction giveup = playerInputActionMap.FindAction("GiveUp");
            giveup.performed += GiveUp;
            break;
        case "Create":
            InputAction chooseItemCreate = playerInputActionMap.FindAction("ChooseItemCreate");
            chooseItemCreate.performed += ChooseItemCreate;
            break;
        }
        if (transform.parent != null && transform.parent.gameObject.name == "ToxicFrog") {
            FrogEnable();
        }
    }
    public void Disable(State new_state)
    {
        state = new_state;
        playerInputActionMap.Disable();
        InputAction move = playerInputActionMap.FindAction("Move");
        move.Disable();
        InputAction jump = playerInputActionMap.FindAction("Jump");
        jump.started -= DoJump;
        jump.canceled -= CancelJump;
        InputAction accelerate = playerInputActionMap.FindAction("Accelerate");
        accelerate.started -= DoAccelerate;
        accelerate.canceled -= NoAccelerate;
        InputAction pause = playerInputActionMap.FindAction("Pause");
        pause.started -= Pause;
        switch (gameMode) {
            case "Party":
                InputAction giveup = playerInputActionMap.FindAction("GiveUp");
                giveup.performed -= GiveUp;
                break;
            case "Create":
                InputAction chooseItemCreate = playerInputActionMap.FindAction("ChooseItemCreate");
                chooseItemCreate.performed -= ChooseItemCreate;
                break;
        }
        if (transform.parent != null && transform.parent.TryGetComponent<Frog>(out var frog)) {
            frog.Disable();
        }
    }
    public void FrogEnable() {
        if (transform.parent != null && transform.parent.TryGetComponent<Frog>(out var frog)) {
            InputAction move = playerInputActionMap.FindAction("Move");
            move.Disable();
            InputAction jump = playerInputActionMap.FindAction("Jump");
            jump.Disable();
            InputAction accelerate = playerInputActionMap.FindAction("Accelerate");
            accelerate.Disable();
            gravity = 0;
            frog.Enable();
        }
    }

    public void FrogDisable() {
        if (transform.parent != null && transform.parent.TryGetComponent<Frog>(out var frog)) {
            InputAction move = playerInputActionMap.FindAction("Move");
            move.Enable();
            InputAction jump = playerInputActionMap.FindAction("Jump");
            jump.Enable();
            InputAction accelerate = playerInputActionMap.FindAction("Accelerate");
            accelerate.Enable();
            gravity = gravityCache;
            frog.Disable();
        }
    }

    

    public bool IsWalking() {
        return isWalking;
    }

    public bool IsDeadAnimation() {
        return state == State.DEAD_ANIMATION || state == State.LOSE;
    }

    public void SetDead() {
        if (skillData == null || !skillData.invincible)
            state = State.DEAD_ANIMATION;
    }

    public void SetLose() {
        state = State.LOSE;
    }

    public State GetState() {
        return state;
    }

    public void SetWin() {
        state = State.WIN;
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
            verticalVelocity = jumpSpeedMultiple * jumpSpeed;
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

    private void ChooseItemCreate(InputAction.CallbackContext context) {
        if (state == State.GAME || state == State.STOP) {
            PlayerCursor cursor = GetComponent<PlayerCursor>();
            GameObject camera = transform.Find("Camera").gameObject;
            if (transform.Find("Camera").gameObject.GetComponent<CameraMovement>().enabled) {
                // back to use cursor to choose item
                item = null;
                cursor.Enable();
                camera.GetComponent<MouseControlFollowCamera>().Disable();
                camera.GetComponent<CameraMovement>().Disable();
                camera.GetComponent<CameraMovement>().DestoryTransparentObject();
            } else if (transform.Find("Canvas").Find("Cursor").gameObject.activeSelf) {
                // back to play
                item = null;
                chooseItemCanvas.GetComponent<ChooseItemCanvasController>().Disable();
                cursor.Disable();
                Enable(State.GAME);
                camera.GetComponent<MouseControlFollowCamera>().Enable();
                camera.GetComponent<CameraMovement>().Disable();
            } else {
                // choose item with cursor
                chooseItemCanvas.GetComponent<ChooseItemCanvasController>().Enable();
                Enable(State.STOP);
                cursor.Enable();
                camera.GetComponent<MouseControlFollowCamera>().Disable();
                camera.GetComponent<CameraMovement>().Disable();
            }
        }
    }

    private void Pause(InputAction.CallbackContext context) {
        pauseMenu.GetComponent<PauseMenu>().Pause();
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
            if (skillData != null && skillData.jumpHigh) {
                skillData.jumperClip = 0;
            }
            isJumping = true;
            buttonPressedTime = 0;
            jumpParticles.Play();
            moveParticles.Stop();
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
        } else if ((state == State.GAME || state == State.MOVE ) && hit.gameObject.TryGetComponent<PlayerFollowObject>(out _)) {
            // follow object move
            followObjectMove = hit.gameObject.GetComponent<PlayerFollowObject>().GetDiffPosition(gameObject);
        }
        if (!moveParticles.isPlaying) {
            moveParticles.Play();
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

    public void SetItem(string newItem) {
        item = newItem;
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
