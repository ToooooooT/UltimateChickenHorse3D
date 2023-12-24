using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tube : BaseItem
{
    private enum State { shooting, idle, placing };
    [SerializeField] private State state = State.placing;
    private const string FOLDERPATH = "BombMan";
    [SerializeField] private float countdown;
    [SerializeField] private float countdownTime;
    void Awake()
    {
        state = State.placing;
    }
    // Start is called before the first frame update
    void Start()
    {
        countdownTime = 1.5f;
    }
    void Update()
    {
        if (state != State.placing) {
            ShootingMode();
        }
    }
    void ShootingMode()
    {
        if (state == State.shooting) {
            GenerateBombMan();
            state = State.idle;
            countdown = countdownTime;
        }
        else {
            countdown -= Time.deltaTime;
            if (countdown <= 0) {
                state = State.shooting;
            }
        }
    }
    public override void Initialize()
    {
        state = State.shooting;
    }

    public override void Reset()
    {
        state = State.placing;
        GameObject[] bombMans = GameObject.FindGameObjectsWithTag("BombMan");
        for (int i = 0; i < bombMans.Length; i++)
            Destroy(bombMans[i]);
    }

    public void GenerateBombMan()
    {
        GameObject newBombMan = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/Mon_00"));
        newBombMan.transform.position = transform.position + 4f * transform.up;
        GameObject gameController = GameObject.Find("GameController");
        StageController stageController = gameController.GetComponent<StageController>();
        stageController.items.Add(newBombMan);
    }
}
