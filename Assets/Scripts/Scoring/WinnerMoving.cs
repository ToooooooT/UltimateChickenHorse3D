using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerMoving : MonoBehaviour
{
    public int winner;
    public int winScore;
    private bool finishMoving;
    [SerializeField] private GameObject[] playersCube;
    private Vector3 winnerPosition;

    void Start() {
        finishMoving = false;
        winner = -1;
    }

    void Update() {
        if (winner >= 0 && winner <= 3) {
            finishMoving = false;
            if (winnerPosition == new Vector3(0, 0, 0))
                winnerPosition = playersCube[winner].transform.position;
            playersCube[winner].transform.position = playersCube[winner].transform.position + new Vector3(0, 0, 0.005f);
            if (playersCube[winner].transform.position.z - winnerPosition.z >= 30.0f / (winScore + 1)) {
                playersCube[winner].transform.position = winnerPosition + new Vector3(0, 0, 30.0f / (winScore + 1));
                winner = -1;
                finishMoving = true;
            }
        } else {
            winnerPosition = new Vector3(0, 0, 0);
        }
    }

    public Vector3 GetPlayerCubePosition(int index) {
        return playersCube[index].transform.position;
    }

    public bool IsFinishMoving() {
        return finishMoving;
    }
}
