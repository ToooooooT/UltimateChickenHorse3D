using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private const string IS_WALKING = "IsWalking";
    private const string IS_DEAD = "IsDead";

    [SerializeField] private Player player;

    private Animator animator;
    private bool isAnimationFinished;

    private void Awake() {
        animator = GetComponent<Animator>();
        isAnimationFinished = false;
    }

    void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking());
        animator.SetBool(IS_DEAD, player.IsDeadAnimation());
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead") && !isAnimationFinished &&
        animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            isAnimationFinished = true;
            animator.SetBool(IS_DEAD, false);
            player.state = Player.State.LOSE;
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Dead")) {
            isAnimationFinished = false;
        }
    }

    public void SetDead() {
        if (player.state == Player.State.GAME) {
            isAnimationFinished = false;
            animator.SetBool(IS_DEAD, true);
        }
    }

    public bool IsDead() {
        return animator.GetBool(IS_DEAD);
    }
}
