using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private MouseControlFollowCamera playerCameraScript;
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
    private void Awake()
    {
        colliderFrog = GetComponent<Collider>();
        state = State.PLACING;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gravity = new Vector3(0, -3, 0);
        jumpingSpeed = 20;
        maxEndureTime = 5;
        jumpCooldownTime = 7;
    }

    // Update is called once per frame
    void Update()
    {
        if(ridingPlayer != null && ridingPlayer.GetComponent<Player>().state == Player.State.LOSE) {
            getOff(false);
            Destroy(gameObject);
        }
        if (state == State.RIDING) {
            Eat();
            ShortJump();
            if (IsGrounded() && jumpCooldown <= 0) {
                LongJump();
                if (velocity.y <0) {
                    velocity = new Vector3(0, velocity.y, 0);
                }
                jumpCooldown = jumpCooldownTime;
            }
            else if (IsCeilinged()) {
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
                //animator.SetTrigger("Tongue");
                state = State.EATING;
                kickAnimationClipTime = 0;
                kickOutDatumPoint = ((transform.position + 2.5f * transform.forward) + (transform.position)) / 2;
                kickOutRadius = (((transform.position + 2.5f * transform.forward) - (transform.position)) / 2).magnitude;
                getOff(false);
            }
        }
        else if (state == State.EATING) {
            if (catchedPlayer == ridingPlayer) {
                if(kickAnimationClipTime < 1) {
                    KickOutAnimation();
                    kickAnimationClipTime += 3f * Time.deltaTime;
                }
                else if (DeadAnimation()) {
                    catchedPlayer.GetComponent<Player>().state = Player.State.LOSE;
                    Destroy(gameObject);
                }
            }
            else {
                if (DeadAnimation()) {
                    catchedPlayer.GetComponent<Player>().state = Player.State.LOSE;
                    getOff(false);
                    Destroy(gameObject);
                }
            }
        }
    }
    private void KickOutAnimation()
    {
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
    private bool DeadAnimation()
    {
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
    private void ShortJump()
    {
        if (Input.GetKey(KeyCode.W)) {
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
    }
    private void LongJump()
    {
        if (Input.GetKey(KeyCode.Space)) {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            velocity += 2f * cameraForward.normalized;
            velocity += new Vector3(0, 40, 0);
            transform.forward = cameraForward.normalized;
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!animatorStateInfo.IsName("Jump"))
                animator.SetTrigger("Jump");
        }
    }
    private GameObject PlayerCatched()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //Vector3 playerDirection;
        //TODO set playerDirection
        //float castDistance = float.MaxValue;
        for (int i = 0; i < players.Length; i++) {
            if ((players[i].transform.position - transform.position).magnitude <= 3f && players[i] != ridingPlayer) {
                return players[i];
                /*
                playerDirection = players[i].transform.position - transform.position;
                Physics.SphereCast(transform.position, frogController.height / 2, playerDirection, out RaycastHit hit, castDistance);
                if (hit.collider.gameObject.CompareTag("Player"))
                    return hit.collider.gameObject; 
                */
            }
        }
        return null;
    }
    private void Eat()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 cameraForward = playerCamera.transform.forward;
            cameraForward.y = 0;
            transform.forward = cameraForward.normalized;
            animator.SetTrigger("Tongue");
        }
    }
    private bool IsGrounded()
    {
        Vector3 p1 = transform.position + frogController.center;
        float castDistance = 0.5f;
        return Physics.SphereCast(p1, frogController.radius * 2, Vector3.down, out RaycastHit hit, castDistance)
                && TagCanJump(hit.collider);
    }
    private bool IsCeilinged()
    {
        Vector3 p1 = transform.position + frogController.center;
        float castDistance = 0.5f;
        return Physics.SphereCast(p1, frogController.radius * 2, Vector3.up, out RaycastHit hit, castDistance)
                && TagCanJump(hit.collider);
    }
    private bool TagCanJump(Collider collider)
    {
        return collider.CompareTag("Airplane") ||
            collider.CompareTag("Wall") ||
            collider.CompareTag("CannonPipe");
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (ridingPlayer == null && collider.gameObject.transform.CompareTag("Player")) {
            ridingPlayer = collider.gameObject;
            playerScale = ridingPlayer.transform.localScale;
            endureTime = maxEndureTime;
            ridingPlayer = collider.gameObject;
            ridingPlayer.transform.SetParent(transform);
            ridingPlayer.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            ridingPlayerScript = ridingPlayer.transform.GetComponent<Player>();
            ridingPlayerScript.FrogDisable();
            playerCamera = ridingPlayer.transform.Find("Camera").gameObject;
            /*
            playerCameraScript = playerCamera.GetComponent<MouseControlFollowCamera>();
            playerCameraScript.SetFPS(true);
            playerCameraScript.DisableButton();
            */
            colliderFrog.isTrigger = false;
            frogController = gameObject.AddComponent<CharacterController>();
            frogController.center = new Vector3(0, 0.07f, 0);
            frogController.height = 0.1f;
            frogController.radius = 0.1f;
            ridingPlayer.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            state = State.RIDING;
        }
    }
    public void getOff(bool alreadyGetOff)
    {
        if (alreadyGetOff) return;
        if (ridingPlayer != null) {
            ridingPlayer.transform.SetParent(null);ridingPlayerScript.ModifyPosition(transform.position);
            ridingPlayer.transform.localScale = playerScale;
            Player playerScript = ridingPlayer.GetComponent<Player>();
            if (playerScript != null) {
                playerScript.FrogEnable();
            }
            //ridingPlayer = null;
        }
    }
    public override void Initialize()
    {
        colliderFrog.isTrigger = true;
        state = State.IDLE;
    }
    public override void Reset()
    {
        if(ridingPlayer != null)
            ridingPlayerScript.FrogEnable();
        state = State.IDLE;
    }
}
