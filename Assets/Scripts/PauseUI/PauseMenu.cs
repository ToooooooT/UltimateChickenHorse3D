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

    private void Awake() {
        gameController = GameObject.Find("GameController");
    }

    public void Pause() {
        if (!transform.parent.Find("SettingMenu").gameObject.activeSelf) {
            gameObject.SetActive(!gameObject.activeSelf);
            Time.timeScale = (int) Time.timeScale ^ 1;
        }
    }

    public void Setting() {
        gameObject.SetActive(false);
        transform.parent.Find("SettingMenu").gameObject.SetActive(true);
    }

    public void Exit() {
        // back to choose stage
        Time.timeScale = 1;
        StageController stageController = gameController.GetComponent<StageController>();
        if (stageController.gameMode == "Party") {
            switch (stageController.partyStage) {
            case StageController.PartyStage.CHOOSE_STAGE:
                DestoryPlayer();
                SceneManager.LoadScene("StartUI");
                break;
            default:
                stageController.partyStage = StageController.PartyStage.CHOOSE_STAGE;
                //Destory all items and stage
                break;
            }
        } else if (stageController.gameMode == "Create") {
            switch (stageController.createStage) {
            case StageController.CreateStage.CHOOSE_STAGE:
                DestoryPlayer();
                SceneManager.LoadScene("StartUI");
                break;
            default:
                stageController.partyStage = StageController.PartyStage.CHOOSE_STAGE;
                //Destory all items and stage
                break;
            }
        }
    }

    public void Resume() {
        if (!transform.parent.Find("SettingMenu").gameObject.activeSelf) {
            gameObject.SetActive(!gameObject.activeSelf);
            Time.timeScale = (int) Time.timeScale ^ 1;
        }
    }

    private void DestoryPlayer() {
        List<GameObject> playerList = gameController.GetComponent<StageController>().playerObjects;
        while (playerList.Count > 0) {
            gameController.GetComponent<PlayerManager>().Unregisterplayer(playerList[0].GetComponent<PlayerInput>());
        }
    }
}
