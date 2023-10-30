using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerMoving : MonoBehaviour
{
    // Start is called before the first frame update
    public int winner;
    public int winScore;
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    Vector3 winnerPosition;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ////////////TODO change winner
        if (Input.GetKeyDown(KeyCode.Y)) {
            winner = 1;
        }
        if (Input.GetKeyDown(KeyCode.U)) {
            winner = 2;
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            winner = 3;
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            winner = 4;
        }
        ////////////
        if (winner == 1) {
            if (winnerPosition == new Vector3(0, 0, 0))
                winnerPosition = player1.transform.position;
            player1.transform.position = player1.transform.position + new Vector3(0, 0, 0.05f);
            if (player1.transform.position.z - winnerPosition.z >= 30.0f / (winScore + 1)) {
                player1.transform.position = winnerPosition + new Vector3(0, 0, 30.0f / (winScore + 1));
                winner = 0;
            }
        }
        else if (winner == 2) {
            if (winnerPosition == new Vector3(0, 0, 0))
                winnerPosition = player2.transform.position;
            player2.transform.position = player2.transform.position + new Vector3(0, 0, 0.05f);
            if (player2.transform.position.z - winnerPosition.z >= 30.0f / (winScore + 1)) {
                player2.transform.position = winnerPosition + new Vector3(0, 0, 30.0f / (winScore + 1));
                winner = 0;
            }
        }
        else if (winner == 3) {
            if (winnerPosition == new Vector3(0, 0, 0))
                winnerPosition = player3.transform.position;
            player3.transform.position = player3.transform.position + new Vector3(0, 0, 0.05f);
            if (player3.transform.position.z - winnerPosition.z >= 30.0f / (winScore + 1)) {
                player3.transform.position = winnerPosition + new Vector3(0, 0, 30.0f / (winScore + 1));
                winner = 0;
            }
        }
        else if (winner == 4) {
            if (winnerPosition == new Vector3(0, 0, 0))
                winnerPosition = player4.transform.position;
            player4.transform.position = player4.transform.position + new Vector3(0, 0, 0.05f);
            if (player4.transform.position.z - winnerPosition.z >= 30.0f / (winScore + 1)) {
                player4.transform.position = winnerPosition + new Vector3(0, 0, 30.0f / (winScore + 1));
                winner = 0;
            }
        }
        else {
            winnerPosition = new Vector3(0, 0, 0);
        }
    }
}
