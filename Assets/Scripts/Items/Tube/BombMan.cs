using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMan : Velocity
{
    private enum State { idle, walking, bombing}
    private State state;
    private const string FOLDERPATH = "BombMan";
    private GameObject aimingPlayer;
    private float rotateSpeed;
    private float changeAimingCountdown;
    private float changeAimingCountdownTime;
    private float bombingCountdown;
    private Animator animator;
    private float Acceleration;
    private bool walking;
    private bool alreadySplit;
    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector3(0, 0, 0);
        state = State.idle;
        changeAimingCountdown = 10;
        changeAimingCountdownTime = 40;
        bombingCountdown = 500;
        rotateSpeed = 3f;
        Acceleration = 181f;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.walking) {
            if (walking) {
                animator.SetBool("walk", true);
                velocity += Time.deltaTime * Acceleration * transform.forward;
            }
            else {
                animator.SetBool("walk", false);
            }
            velocity -= 0.1f * velocity;
            transform.position += Time.deltaTime * velocity;
            AimingPlayer();
        }
        else if (state == State.idle) {
            AimingPlayer();
        }
        if (state == State.bombing) {
            Bombing();
        }
        else {
            bombingCountdown -= Time.deltaTime * 10;
            if (bombingCountdown <= 0) {
                velocity = new Vector3(0, 0, 0);
                animator.SetBool("walk", false);
                walking = false;
                state = State.bombing;
                animator.SetTrigger("attack01");
            }
        }
    }
    void Bombing()
    {
        float clipTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("attack01") && clipTime > 0.48) {
            if(!alreadySplit && transform.localScale.x > 1) {
                alreadySplit = true;
                GenerateSmallBombs(4);
            }
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for(int i = 0; i < players.Length; i++) {
                if((players[i].transform.position - transform.position).magnitude <= transform.localScale.x * 1.25f) {
                    players[i].GetComponent<Player>().state = Player.State.LOSE;
                }
            }
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("attack01") && clipTime > 0.95) {
            Destroy(gameObject);
        }
    }
    void GenerateSmallBombs(int num)
    {
        for(int i = 0; i < num; i++) {
            GameObject newBombMan = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/Mon_00"));
            newBombMan.transform.position = transform.position;
            newBombMan.GetComponent<BombMan>().velocity = 5 * new Vector3(20 * Mathf.Cos(2 * Mathf.PI * (i / num)), 1, 20 * Mathf.Cos(2 * Mathf.PI * (i / num)));
            newBombMan.GetComponent<BombMan>().alreadySplit = true;
            newBombMan.transform.localScale = newBombMan.transform.localScale / 2;
            GameObject gameController = GameObject.Find("GameController");
            StageController stageController = gameController.GetComponent<StageController>();
            stageController.items.Add(newBombMan);
        }
    }
    void AimingPlayer()
    {
        if (aimingPlayer != null) {
            Slerp();
        }
        else {
            if (changeAimingCountdown <= 0) {
                ChangeAimingPlayer();
                changeAimingCountdown = changeAimingCountdownTime;
            }
            changeAimingCountdown -= Time.deltaTime * 10;
        }
    }
    void Slerp()
    {
        Vector3 targetDirection = (aimingPlayer.transform.position - transform.position);
        targetDirection.y = 0;
        targetDirection = targetDirection.normalized;
        transform.forward = (transform.forward + Time.deltaTime * rotateSpeed * (targetDirection - transform.forward * Vector3.Dot(targetDirection, transform.forward)));
    }
    void ChangeAimingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> playerList = new List<GameObject>(players);
        ShuffleList(playerList);
        foreach (GameObject player in playerList) {
            Player playerScript = player.GetComponent<Player>();
            if (playerScript.state != Player.State.LOSE) {
                aimingPlayer = player;
                Vector3 targetDirection = (aimingPlayer.transform.position - transform.position);
                targetDirection.y = 0;
                targetDirection = targetDirection.normalized;
                transform.forward = targetDirection;
                walking = false;
                animator.SetTrigger("damage");
                velocity = new Vector3(0, 0, 0);
                walking = true;
                state = State.walking;
                return;
            }
        }
    }
    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (state == State.walking && collision.gameObject.transform.CompareTag("Player")) {
            velocity = new Vector3(0, 0, 0);
            animator.SetBool("walk", false);
            walking = false;
            state = State.bombing;
            animator.SetTrigger("attack01");
        }
    }
}
