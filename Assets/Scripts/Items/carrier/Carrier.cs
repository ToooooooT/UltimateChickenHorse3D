using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrier : MonoBehaviour
{
    private enum State { shooting, idle };
    private State state;
    private const string FOLDERPATH = "airplane";
    public float countdown;
    public float countdownTime;
    private static readonly string[] airplaneType = { "Awacs" , "F18" , "F35" , "Hawkeye" , "Seahawk"};
    // Start is called before the first frame update
    void Start()
    {
        state = State.idle;
        countdownTime = 25;
    }

    // Update is called once per frame
    void Update()
    {

        if (state == State.shooting) {
            GenerateAirplane();
            state = State.idle;
            countdown = countdownTime;
        }
        else {
            countdown -= 0.1f;
            if (countdown <= 0) {
                state = State.shooting;
            }
        }

    }
    public void GenerateAirplane()
    {
        string chooseType = airplaneType[(int)Random.Range(0, airplaneType.Length)];
        GameObject newAirplane = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/" + chooseType));
        newAirplane.transform.position = transform.position + 1.6f * transform.up + 2f * transform.forward;
        newAirplane.transform.rotation = transform.rotation;
        if(chooseType == "Awacs")
            newAirplane.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
        else
            newAirplane.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        Airplane airplaneScript = newAirplane.GetComponent<Airplane>();
        airplaneScript.velocity = 0.2f * transform.forward;
        airplaneScript.parentCarrier = this.gameObject;
    }
}
