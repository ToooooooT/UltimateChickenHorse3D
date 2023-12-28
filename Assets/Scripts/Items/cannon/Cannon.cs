using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : BaseItem
{
    private enum State { shooting, idle , placing};

    [SerializeField] private State state = State.placing;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float countdown;
    [SerializeField] private float countdownTime;

    private GameObject bomb;
    private Transform[] cannons;
    private ParticleSystem[] smokes;
    private StageController stageController;
    private int cannonNums;

    private const string FOLDERPATH = "cannon bomb";

    void Awake() {
        state = State.placing;
        bomb = Resources.Load<GameObject>(FOLDERPATH + "/Round_shot");
        cannonNums = transform.childCount;
        cannons = new Transform[cannonNums];
        smokes = new ParticleSystem[cannonNums];
        for (int i = 0; i < cannonNums; ++i) {
            cannons[i] = transform.GetChild(i);
            smokes[i] = cannons[i].Find("Small_cannon").Find("Smoke").GetComponent<ParticleSystem>();
        }
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
            Generatebomb();
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

    private void Generatebomb() {
        for (int i = 0; i < cannonNums; i++) {
            GameObject newBomb = Instantiate(bomb);
            Transform newBombTransform = newBomb.transform;
            newBombTransform.position = cannons[i].position + 1.6f * cannons[i].up + 2f * cannons[i].forward;
            newBombTransform.localScale = new Vector3(10f, 10f, 10f);
            CannonBomb cannonBomb = newBomb.GetComponent<CannonBomb>();
            cannonBomb.velocity = cannons[i].forward * 0.5f;
            cannonBomb.cannon = gameObject;
            stageController.items.Add(newBomb);
            smokes[i].Play();
            Destroy(newBomb, 5f);
        }
    }
}
