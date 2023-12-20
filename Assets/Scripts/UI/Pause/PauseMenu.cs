using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class PauseMenu : MonoBehaviour
{
    private GameObject gameController;
    private GameObject chooseItemCanvas;
    private List<GameObject> players;
    private Player.State[] playersState;
    private bool[] playersFollowCamera;
    private bool[] playersVirtualCamera;
    private bool[] playersCursor;
    private bool chooseItemCanvasActive;
    private float timeScale;
    private string gameMode;

    private void Awake() {
        gameController = GameObject.Find("GameController");
        players = gameController.GetComponent<StageController>().playerObjects;
        playersState = new Player.State[4];
        playersFollowCamera = new bool[4];
        playersVirtualCamera = new bool[4];
        playersCursor = new bool[4];
        gameMode = PlayerPrefs.GetString("GameMode", "Party");
        chooseItemCanvas = GameObject.Find("ChooseItemCanvas");
    }

    public void Pause() {
        if (!transform.parent.Find("SettingMenu").gameObject.activeSelf) {
            gameObject.SetActive(!gameObject.activeSelf);
            if (gameObject.activeSelf) {
                timeScale = Time.timeScale;
                Time.timeScale = 0f;
            } else {
                Time.timeScale = timeScale;
            }
            if (gameObject.activeSelf) {
                DisablePlayer();
                gameController.GetComponent<PlayerManager>().DisableJoinAction();
            } else {
                EnablePlayer();
                if (gameController.GetComponent<StageController>().partyStage == StageController.PartyStage.CHOOSE_STAGE ||
                gameController.GetComponent<StageController>().createStage == StageController.CreateStage.CHOOSE_STAGE) {
                    gameController.GetComponent<PlayerManager>().EnableJoinAction();
                }
            }
        }
    }

    public void Setting() {
        gameObject.SetActive(false);
        transform.parent.Find("SettingMenu").gameObject.SetActive(true);
    }

    public void Exit() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
        chooseItemCanvasActive = false;
        for (int i = 0; i < playersCursor.Length; ++i) {
            playersCursor[i] = false;
        }
        EnablePlayer();
        chooseItemCanvas.GetComponent<ChooseItemCanvasController>().Reset();
        StageController stageController = gameController.GetComponent<StageController>();
        if (gameMode == "Party") {
            switch (stageController.partyStage) {
            case StageController.PartyStage.CHOOSE_STAGE:
                DestoryPlayer();
                SceneManager.LoadScene("StartUI");
                break;
            default:
                stageController.partyStage = StageController.PartyStage.CHOOSE_STAGE;
                break;
            }
        } else if (gameMode == "Create") {
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

    private void EnablePlayer() {
        for (int i = 0; i < players.Count; ++i) {
            players[i].GetComponent<Player>().Enable(playersState[i]);
            Transform camera = players[i].transform.Find("Camera");
            if (playersFollowCamera[i]) {
                camera.GetComponent<MouseControlFollowCamera>().Enable();
            }
            if (playersVirtualCamera[i]) {
                camera.GetComponent<CameraMovement>().Enable();
            }
            if (!playersCursor[i]) {
                players[i].GetComponent<PlayerCursor>().Disable();
            }
        }
        if (chooseItemCanvasActive) {
            chooseItemCanvas.GetComponent<ChooseItemCanvasController>().SetActiveAllChild(true);
        }
    }

    private void DisablePlayer() {
        for (int i = 0; i < players.Count; ++i) {
            Transform camera = players[i].transform.Find("Camera");
            playersState[i] = players[i].GetComponent<Player>().state;
            playersFollowCamera[i] = camera.GetComponent<MouseControlFollowCamera>().enabled;
            playersVirtualCamera[i] = camera.GetComponent<CameraMovement>().enabled;
            playersCursor[i] = players[i].transform.Find("Canvas").Find("Cursor").gameObject.activeSelf;
            players[i].GetComponent<Player>().Disable(Player.State.STOP);
            players[i].GetComponent<PlayerCursor>().Enable();
            camera.GetComponent<CameraMovement>().Disable();
            camera.GetComponent<MouseControlFollowCamera>().Disable();
        }
        chooseItemCanvasActive = chooseItemCanvas.transform.Find("NextButton").gameObject.activeSelf;
        chooseItemCanvas.GetComponent<ChooseItemCanvasController>().SetActiveAllChild(false);
    }
}
