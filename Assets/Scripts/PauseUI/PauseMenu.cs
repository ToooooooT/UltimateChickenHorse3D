using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private GameObject gameController;

    private void Awake() {
        gameController = GameObject.Find("GameController");
    }

    public void Pause() {
        gameObject.SetActive(!gameObject.activeSelf);
        Time.timeScale = (int) Time.timeScale ^ 1;
    }

    public void Exit() {
        // back to choose stage
        StageController stageController = gameController.GetComponent<StageController>();
        if (stageController.gameMode == "Party") {
            switch (stageController.partyStage) {
            case StageController.PartyStage.CHOOSE_STAGE:
                SceneManager.LoadScene("StartUI");
                break;
            default:
                stageController.partyStage = StageController.PartyStage.CHOOSE_STAGE;
                //Destory all items and stage
                break;
            }
        } else if (stageController.gameMode == "Create") {
            SceneManager.LoadScene("StartUI");
        }
    }

    public void Resume() {
        gameObject.SetActive(!gameObject.activeSelf);
        Time.timeScale = (int) Time.timeScale ^ 1;
    }
}
