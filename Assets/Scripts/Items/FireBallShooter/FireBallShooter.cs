using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooter : BaseItem
{
    private enum State { shooting, idle, placing };

    [SerializeField] private State state = State.placing;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float countdown;
    [SerializeField] private float countdownTime;

    private GameObject fireBall;
    private StageController stageController;

    private const string FOLDERPATH = "Fireball";

    void Awake() {
        state = State.placing;
        fireBall = Resources.Load<GameObject>(FOLDERPATH + "/Fireball");
        stageController = GameObject.Find("GameController").GetComponent<StageController>();
    }

    void Start() {
        countdownTime = 10;
        rotateSpeed = 40;
    }

    void Update() {
        if (state != State.placing) {
            ShootingMode();
        }
    }

    void ShootingMode() {
        if (state == State.shooting) {
            GenerateFireBall();
            state = State.idle;
            countdown = countdownTime;
        } else {
            countdown -= 10 * Time.deltaTime;
            if (countdown <= 0) {
                state = State.shooting;
            }
        }
        transform.Rotate(transform.up, rotateSpeed * Time.deltaTime);
    }

    public override void Initialize() {
        state = State.shooting;
    }

    public override void Reset() {
        state = State.placing;
    }

    public void GenerateFireBall() {
        GameObject newFireball = Instantiate(fireBall);
        newFireball.transform.position = transform.position + 3.5f * transform.up + 5f * transform.right;
        FireBall FireBallScript = newFireball.GetComponent<FireBall>();
        FireBallScript.velocity = transform.right * 0.5f;
        FireBallScript.parentFireBallShooter = gameObject;
        stageController.items.Add(newFireball);
        Destroy(newFireball, 5f);
    }
}
