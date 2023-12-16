using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class PauseMenu : MonoBehaviour
{
    private GameObject gameController;
    private List<GameObject> players;
    private Player.State[] playersState;

    private void Awake() {
        gameController = GameObject.Find("GameController");
        players = gameController.GetComponent<StageController>().playerObjects;
        playersState = new Player.State[4];
    }

    public void Pause() {
        if (!transform.parent.Find("SettingMenu").gameObject.activeSelf) {
            gameObject.SetActive(!gameObject.activeSelf);
            Time.timeScale = (int) Time.timeScale ^ 1;
            if (gameObject.activeSelf) {
                EnablePlayerCursor();
                DisablePlayer();
                gameController.GetComponent<PlayerManager>().DisableJoinAction();
            } else {
                DisablePlayerCursor();
                EnablePlayer();
                if (gameController.GetComponent<StageController>().partyStage == StageController.PartyStage.CHOOSE_STAGE ||
                gameController.GetComponent<StageController>().createStage == StageController.CreateStage.CHOOSE_STAGE)
                gameController.GetComponent<PlayerManager>().EnableJoinAction();
            }
        }
    }

    public void Setting() {
        gameObject.SetActive(false);
        transform.parent.Find("SettingMenu").gameObject.SetActive(true);
    }

    public void Exit() {
        // back to choose stage
        Time.timeScale = 1;
        gameObject.SetActive(false);
        DisablePlayerCursor();
        EnablePlayer();
        StageController stageController = gameController.GetComponent<StageController>();
        if (stageController.gameMode == "Party") {
            switch (stageController.partyStage) {
            case StageController.PartyStage.CHOOSE_STAGE:
                DestoryPlayer();
                SceneManager.LoadScene("StartUI");
                break;
            default:
                stageController.partyStage = StageController.PartyStage.CHOOSE_STAGE;
                break;
            }
        } else if (stageController.gameMode == "Create") {
            switch (stageController.createStage) {
            case StageController.CreateStage.CHOOSE_STAGE:
                DestoryPlayer();
                SceneManager.LoadScene("StartUI");
                break;
            default:
                stageController.createStage = StageController.CreateStage.CHOOSE_STAGE;
                break;
            }
        }
    }

    private void DestoryPlayer() {
        List<GameObject> playerList = gameController.GetComponent<StageController>().playerObjects;
        while (playerList.Count > 0) {
            gameController.GetComponent<PlayerManager>().Unregisterplayer(playerList[0].GetComponent<PlayerInput>());
        }
    }

    private void EnablePlayerCursor() {
        foreach (GameObject player in players) {
            player.GetComponent<PlayerCursor>().Enable();
        }
    }

    private void DisablePlayerCursor() {
        foreach (GameObject player in players) {
            player.GetComponent<PlayerCursor>().Disable();
        }
    }

    private void EnablePlayer() {
        for (int i = 0; i < players.Count; ++i) {
            players[i].GetComponent<Player>().Enable(playersState[i]);
        }
    }

    private void DisablePlayer() {
        for (int i = 0; i < players.Count; ++i) {
            playersState[i] = players[i].GetComponent<Player>().state;
            players[i].GetComponent<Player>().Disable(Player.State.STOP);
        }
    }

}
