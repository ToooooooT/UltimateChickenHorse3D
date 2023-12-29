using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : BaseItem
{
    private enum State { shooting, idle ,placing};

    private const string FOLDERPATH = "airplane";
    private static readonly string[] airplaneType = { "Awacs" , "F18" , "F35" , "Hawkeye" , "Seahawk"};

    private State state;
    private GameObject[] airplane;
    private StageController stageController;
    private float countdown;
    private float countdownTime;

    void Awake() {
        state = State.placing;
        airplane = new GameObject[airplaneType.Length];
        for (int i = 0; i < airplaneType.Length; ++i) {
            airplane[i] = Resources.Load<GameObject>(FOLDERPATH + "/" + airplaneType[i]);
        }
        stageController = GameObject.Find("GameController").GetComponent<StageController>();
    }

    void Start() {
        countdownTime = 25;
    }

    void Update() {
        if (state != State.placing) {
            ShootingMode();
        }
    }

    void ShootingMode() {
        if (state == State.shooting) {
            GenerateAirplane();
            state = State.idle;
            countdown = countdownTime;
        } else {
            countdown -= 4 * Time.deltaTime;
            if (countdown <= 0) {
                state = State.shooting;
            }
        }
    }

    public override void Initialize() {
        state = State.shooting;
    }

    public override void Reset() {
        state = State.placing;
    }

    private void GenerateAirplane() {
        int index = Random.Range(0, airplaneType.Length);
        GameObject newAirplane = Instantiate(airplane[index]);
        newAirplane.transform.SetPositionAndRotation(transform.position + 1.6f * transform.up + 2f * transform.forward, transform.rotation);
        if (airplaneType[index] == "Awacs") {
            newAirplane.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
        } else {
            newAirplane.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
        Airplane airplaneScript = newAirplane.GetComponent<Airplane>();
        airplaneScript.velocity = 0.2f * transform.forward;
        airplaneScript.carrier = gameObject;
        stageController.items.Add(newAirplane);
    }
}
