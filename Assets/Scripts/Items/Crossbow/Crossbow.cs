using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : BaseItem
{
    private enum State { shooting, idle, placing };
    [SerializeField] private State state = State.placing;
    private const string FOLDERPATH = "Arrow";
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float countdown;
    [SerializeField] private float countdownTime;
    [SerializeField] private float changeAimingCountdown;
    [SerializeField] private float changeAimingCountdownTime;
    private GameObject aimingPlayer;
    private AudioManager audioManager;

    void Awake()
    {
        state = State.placing;
    }

    // Start is called before the first frame update
    void Start()
    {
        countdownTime = 30;
        changeAimingCountdown = 0;
        changeAimingCountdownTime = 100;
        rotateSpeed = 5f;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.placing) {
            ShootingMode();
            AimingPlayer();
        }
    }
    void AimingPlayer()
    {
        if (changeAimingCountdown <= 0) {
            ChangeAimingPlayer();
            changeAimingCountdown = changeAimingCountdownTime;
        }
        if (aimingPlayer != null) {
            Slerp();
            changeAimingCountdown -= Time.deltaTime * 40;
        }
    }
    void Slerp()
    {
        Vector3 targetDirection = (aimingPlayer.transform.position - transform.position).normalized;
        transform.forward = transform.forward + Time.deltaTime * rotateSpeed * (targetDirection - transform.forward * Vector3.Dot(targetDirection, transform.forward));
    }
    void ChangeAimingPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> playerList = new(players);
        ShuffleList(playerList);
        foreach (GameObject player in playerList) {
            Player playerScript = player.GetComponent<Player>();
            if(playerScript.GetState() != Player.State.LOSE) {
                aimingPlayer = player;
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
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
    void ShootingMode()
    {
        if (state == State.shooting) {
            GenerateArrow();

            state = State.idle;
            countdown = countdownTime;
        }
        else {
            countdown -= 10 * Time.deltaTime;
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
    }

    public void GenerateArrow()
    {
        audioManager.PlaySE("bowShoot");
        for (int i = transform.childCount-1; i >= 0 ; i--) {
            if (transform.GetChild(i).CompareTag("ArrowPosition")) {
                GameObject newArrow = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/Arrow"));
                newArrow.transform.position = transform.GetChild(i).position;
                newArrow.transform.forward = transform.forward;
                newArrow.transform.localScale = new Vector3(5f, 5f, 5f);
                Arrow ArrowScript = newArrow.GetComponent<Arrow>();
                ArrowScript.velocity = transform.forward * 0.5f;
                ArrowScript.parentCrossbow = gameObject;
                GameObject gameController = GameObject.Find("GameController");
                StageController stageController = gameController.GetComponent<StageController>();
                stageController.items.Add(newArrow);
                Destroy(newArrow, 15f);
                return;
            }
        }
    }
}
