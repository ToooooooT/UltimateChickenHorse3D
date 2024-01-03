using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Frog : Velocity
{
    private enum State { PLACING, IDLE, RIDING, EATING};
    private State state;
    private float endureTime;
    private float maxEndureTime;
    private GameObject ridingPlayer;
    private GameObject playerCamera;
    private Animator animator;
    private Player ridingPlayerScript;
    private Collider colliderFrog;
    private float jumpingSpeed;
    private Vector3 playerScale;
    private Vector3 gravity;
    private CharacterController frogController;
    private float jumpCooldownTime;
    private float jumpCooldown;
    private GameObject catchedPlayer;
    private float kickAnimationClipTime;
    private Vector3 kickOutDatumPoint;
    private float kickOutRadius;
    private InputActionMap frogPlayerInputActionMap;
    private bool longJumpIsPressed;
    private bool shortJumpIsPressed;

    private void Awake() {
        colliderFrog = GetComponent<Collider>();
        state = State.PLACING;
    }

    void Start() {
        animator = GetComponent<Animator>();
        gravity = new Vector3(0, -3, 0);
        jumpingSpeed = 20;
        maxEndureTime = 25;
        jumpCooldownTime = 7;
        longJumpIsPressed = false;
        shortJumpIsPressed = false;
    }

    void Update() {
        if(ridingPlayer != null && ridingPlayer.GetComponent<Player>().GetState() == Player.State.LOSE) {
            GetOff(false);
            Destroy(gameObject);
        }
        if (state == State.RIDING) {
            if (shortJumpIsPressed) {
                ShortJump();
            }
            if (IsGrounded() && jumpCooldown <= 0) {
                if (longJumpIsPressed) {
                    LongJump();
                }
                if (velocity.y <0) {
                    velocity = new Vector3(0, velocity.y, 0);
                }
                jumpCooldown = jumpCooldownTime;
            } else if (IsCeilinged()) {
                velocity = new Vector3(velocity.x, 0, velocity.z);
            }
            if (!IsGrounded()) {
                Vector3 cameraForward = playerCamera.transform.forward;
                cameraForward.y = 0;
                transform.forward = cameraForward.normalized;
            }
            ridingPlayer.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            frogController.Move(jumpingSpeed * Time.deltaTime * velocity);
            velocity += Time.deltaTime * gravity;
            velocity -= Time.deltaTime * 3 * velocity;
            jumpCooldown -= Time.deltaTime * 10;
            animator.SetTrigger("Idle");
            endureTime -= Time.deltaTime;
            catchedPlayer = PlayerCatched();
            if (catchedPlayer != null) {
                animator.SetTrigger("Tongue");
                state = State.EATING;
            }
            if (endureTime <= 0) {
                catchedPlayer = ridingPlayer;
                state = State.EATING;
                kickAnimationClipTime = 0;
                kickOutDatumPoint = (transform.position + 2.5f * transform.forward + transform.position) / 2;
                kickOutRadius = ((transform.position + 2.5f * transform.forward - transform.position) / 2).magnitude;
                GetOff(false);
            }
        } else if (state == State.EATING) {
            if (catchedPlayer == ridingPlayer) {
                if(kickAnimationClipTime < 1) {
                    KickOutAnimation();
                    kickAnimationClipTime += 3f * Time.deltaTime;
                } else if (DeadAnimation()) {
                    catchedPlayer.GetComponent<Player>().SetDead();
                    Destroy(gameObject);
                }
            } else {
                if (DeadAnimation()) {
                    catchedPlayer.GetComponent<Player>().SetDead();
                    GetOff(false);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void KickOutAnimation() {
        Vector3 frogForward = transform.forward;
        frogForward.y = 0;
        frogForward = frogForward.normalized;
        ridingPlayer.GetComponent<Player>().ModifyPosition(kickOutDatumPoint + kickOutRadius * (-frogForward * Mathf.Cos(kickAnimationClipTime * Mathf.PI) + Vector3.up * Mathf.Sin(kickAnimationClipTime * Mathf.PI)));
        Vector3 modifiedTransform = transform.forward;
        modifiedTransform.y = 0.6f * Mathf.Sin(kickAnimationClipTime * Mathf.PI);
        transform.forward = modifiedTransform;
        if (kickAnimationClipTime >= 0.97) {
            animator.SetTrigger("Tongue");
        }
    }

    private bool DeadAnimation() {
        float clipTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if(clipTime > 0.5) {
            Player catchedPlayerScript = catchedPlayer.GetComponent<Player>();
            catchedPlayerScript.ModifyPosition(transform.position + (3-clipTime) * transform.forward);
        }
        if(clipTime > 0.95) {
            return true;
        }
        return false;
    }

    private void ShortJump() {
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        if (IsGrounded() && jumpCooldown <= 0) {
            velocity += 2 * cameraForward.normalized;
            velocity += new Vector3(0, 4f, 0);
            transform.forward = cameraForward.normalized;
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!animatorStateInfo.IsName("Jump"))
                animator.SetTrigger("Jump");
        }
        else if (!IsGrounded()) {
            velocity += 0.01f * cameraForward.normalized;
        }
    }

    private void LongJump() {
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        velocity += 2f * cameraForward.normalized;
        velocity += new Vector3(0, 40, 0);
        transform.forward = cameraForward.normalized;
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!animatorStateInfo.IsName("Jump")) {
            animator.SetTrigger("Jump");
        }
    }

    private GameObject PlayerCatched() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) {
            if ((players[i].transform.position - transform.position).magnitude <= 3f && players[i] != ridingPlayer) {
                return players[i];
            }
        }
        return null;
    }

    private void Eat(InputAction.CallbackContext context) {
        if (state == State.RIDING) {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            transform.forward = cameraForward.normalized;
            animator.SetTrigger("Tongue");
        }
    }

    private bool IsGrounded() {
        Vector3 p1 = transform.position + frogController.center;
        float castDistance = 0.5f;
        return Physics.SphereCast(p1, frogController.radius * 2, Vector3.down, out RaycastHit hit, castDistance)
                && TagCanJump(hit.collider);
    }

    private bool IsCeilinged() {
        Vector3 p1 = transform.position + frogController.center;
        float castDistance = 0.5f;
        return Physics.SphereCast(p1, frogController.radius * 2, Vector3.up, out RaycastHit hit, castDistance)
                && TagCanJump(hit.collider);
    }

    private bool TagCanJump(Collider collider) {
        return collider.CompareTag("Airplane") ||
            collider.CompareTag("Wall") ||
            collider.CompareTag("CannonPipe");
    }

    private void OnTriggerEnter(Collider collider) {
        if (state == State.IDLE && ridingPlayer == null && collider.gameObject.transform.CompareTag("Player")) {
            ridingPlayer = collider.gameObject;
            playerScale = ridingPlayer.transform.localScale;
            endureTime = maxEndureTime;
            ridingPlayer = collider.gameObject;
            ridingPlayer.transform.SetParent(transform);
            ridingPlayer.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            ridingPlayerScript = ridingPlayer.transform.GetComponent<Player>();
            ridingPlayerScript.FrogEnable();
            playerCamera = ridingPlayer.transform.Find("Camera").gameObject;
            colliderFrog.isTrigger = false;
            frogController = gameObject.AddComponent<CharacterController>();
            frogController.center = new Vector3(0, 0.07f, 0);
            frogController.height = 0.1f;
            frogController.radius = 0.1f;
            ridingPlayer.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            state = State.RIDING;
        }
    }

    public void Enable() {
        frogPlayerInputActionMap = ridingPlayer.GetComponent<PlayerInput>().actions.FindActionMap("Frog");
        frogPlayerInputActionMap.Enable();
        InputAction shortJump = frogPlayerInputActionMap.FindAction("ShortJump");
        shortJump.performed += ctx => shortJumpIsPressed = true;
        shortJump.canceled += ctx => shortJumpIsPressed = false;
        InputAction longJump = frogPlayerInputActionMap.FindAction("LongJump");
        longJump.performed += ctx => longJumpIsPressed = true;
        longJump.canceled += ctx => longJumpIsPressed = false;
        InputAction eat = frogPlayerInputActionMap.FindAction("Eat");
        eat.performed += Eat;
    }

    public void Disable() {
        frogPlayerInputActionMap = ridingPlayer.GetComponent<PlayerInput>().actions.FindActionMap("Frog");
        frogPlayerInputActionMap.Disable();
        InputAction eat = frogPlayerInputActionMap.FindAction("Eat");
        eat.performed -= Eat;
    }


    public void GetOff(bool alreadyGetOff) {
        if (alreadyGetOff) {
            return;
        }
        if (ridingPlayer != null) {
            Player playerScript = ridingPlayer.GetComponent<Player>();
            playerScript?.FrogDisable();
            ridingPlayer.transform.SetParent(null);
            ridingPlayerScript.ModifyPosition(transform.position);
            ridingPlayer.transform.localScale = playerScale;
        }
    }

    public override void Initialize() {
        colliderFrog.isTrigger = true;
        state = State.IDLE;
    }

    public override void Reset() {
        if (ridingPlayer != null) {
            ridingPlayerScript.FrogDisable();
        }
        state = State.IDLE;
    }
}
