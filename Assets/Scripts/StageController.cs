using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StageController : MonoBehaviour
{
    private enum Stage {CHOOSE_STAGE, BEFORE_SELECT_ITEM, SELECT_ITEM, PLACE_ITEM, PLAY, SCOREBOARD};

    private string gameMode;
    private Stage stage;
    private GameObject scoreBoardObject;
    private GameObject LinBenObject;

    public List<GameObject> items;
    public List<GameObject> playerObjects;

    void Start() {
        items = new();
        gameMode = PlayerPrefs.GetString("GameMode", "Party");
        if (gameMode == "Party") {
            stage = Stage.CHOOSE_STAGE;
        } 
        // other mode initial stage not sure yet

        // playerObjects = GameObject.FindGameObjectsWithTag("Player");
        scoreBoardObject = GameObject.FindGameObjectWithTag("ScoreBoard");
        LinBenObject = GameObject.FindGameObjectWithTag("LinBen");
    }

    void Update() {
        switch (stage) {
        case Stage.CHOOSE_STAGE:
            if (Input.GetKey(KeyCode.Y) && playerObjects.Count >= 1) {
                // TODO: change the condition to all player choose the stage
                GetComponent<PlayerManager>().DisableJoinAction();
                stage = Stage.BEFORE_SELECT_ITEM;
            }
            break;
        case Stage.BEFORE_SELECT_ITEM:
            ResetItems();
            PlayerSelectItem();
            AdjustCamera(isFollow: true, isVirtual: false);
            GetComponent<ItemGenerator>().GenerateItems();
            stage = Stage.SELECT_ITEM;
            break;
        case Stage.SELECT_ITEM:
            if (!IsAllPlayersSelectItem()) {
                break;
            }
            ClearChoosingItems();
            AdjustCamera(isFollow: false, isVirtual: true);
            foreach (GameObject player in playerObjects) {
                player.GetComponent<Player>().Disable(Player.State.STOP);
            }
            stage = Stage.PLACE_ITEM;
            break;
        case Stage.PLACE_ITEM:
            if (!IsAllPlayersPlaceItem()) {
                break;
            }
            SetPlayersPlay();
            AdjustCamera(isFollow: true, isVirtual: false);
            MemorizeItemsState();
            stage = Stage.PLAY;
            break;
        case Stage.PLAY:
            CheckPlayersState();
            CheckAllLose();
            CheckWin();
            break;
        case Stage.SCOREBOARD:
            MoveWinner();
            break;
        }
    }

    private void PlayerSelectItem() {
        for (int i = 0; i < playerObjects.Count; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            player.Enable(Player.State.SELECT_ITEM);
            RandomPositionToSelectItem(player);
        }
    }

    private bool IsAllPlayersSelectItem() {
        for (int i = 0; i < playerObjects.Count; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.state != Player.State.STOP) {
                return false;
            }
        }
        return true;
    }

    private bool IsAllPlayersPlaceItem() {
        for (int i = 0; i < playerObjects.Count; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.HaveItem()) {
                return false;
            }
        }
        return true;
    }

    private void SetPlayersPlay() {
        for (int i = 0; i < playerObjects.Count; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            player.Enable(Player.State.GAME);
        }
    }

    private void ClearChoosingItems() {
        GameObject[] choosing_items = GameObject.FindGameObjectsWithTag("ChoosingItem");
        for (int i = 0; i < choosing_items.Length; ++i) {
            Destroy(choosing_items[i]);
        }
    }

    private void RandomPositionToSelectItem(Player player) {
        Vector3 spawnArea = GetComponent<ItemGenerator>().spawnArea;
        spawnArea.y = 3;
        Vector3 size = GetComponent<ItemGenerator>().size;
        float randomX = Random.Range(spawnArea.x - size.x / 2, spawnArea.x + size.x / 2);
        float randomY = Random.Range(spawnArea.y - size.y / 2, spawnArea.y + size.y / 2);
        float randomZ = Random.Range(spawnArea.z - size.z / 2, spawnArea.z + size.z / 2);
        player.ModifyPosition(new Vector3(randomX, randomY, randomZ));
    }

    private void AdjustCamera(bool isFollow, bool isVirtual) {
        for (int i = 0; i < playerObjects.Count; ++i) {
            if (isFollow) {
                playerObjects[i].transform.Find("Camera").GetComponent<MouseControlFollowCamera>().enabled = true;
                playerObjects[i].transform.Find("Camera").GetComponent<MouseControlFollowCamera>().Enable();
            } else {
                playerObjects[i].transform.Find("Camera").GetComponent<MouseControlFollowCamera>().Disable();
                playerObjects[i].transform.Find("Camera").GetComponent<MouseControlFollowCamera>().enabled = false;
            }
            if (isVirtual) {
                playerObjects[i].transform.Find("Camera").GetComponent<CameraMovement>().enabled = true;
                playerObjects[i].transform.Find("Camera").GetComponent<CameraMovement>().Enable();
            } else {
                playerObjects[i].transform.Find("Camera").GetComponent<CameraMovement>().Disable();
                playerObjects[i].transform.Find("Camera").GetComponent<CameraMovement>().enabled = false;
            }
        }
    }

    private void CheckPlayersState() {
        for (int i = 0; i < playerObjects.Count; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.transform.position.y < -50) {
                player.Disable(Player.State.LOSE);
                SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
            } else if (player.state == Player.State.LOSE) {
                SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
            }
        }
    }

    private void CheckWin() {
        WinnerMoving winnerMoving = scoreBoardObject.GetComponent<WinnerMoving>();
        for (int i = 0; i < playerObjects.Count; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            LinBenScript LinBen = LinBenObject.GetComponent<LinBenScript>();
            if (player.state == Player.State.WIN && LinBen.state == LinBenScript.State.FINISH_POINTING) {
                stage = Stage.SCOREBOARD;
                LinBen.state = LinBenScript.State.IDLE;
                winnerMoving.winner = i;
            }
        }
        if (winnerMoving.winner >= 0) {
            for (int i = 0; i < playerObjects.Count; i++) {
                Player player = playerObjects[i].GetComponent<Player>();
                Vector3 position = winnerMoving.GetPlayerCubePosition(i);
                position.y += 1;
                player.ModifyPosition(position);
                player.Enable(Player.State.GAME);
            }
        }
    }

    private void CheckAllLose() {
        for (int i = 0; i < playerObjects.Count; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.state != Player.State.LOSE) {
                return;
            }
        }
        stage = Stage.BEFORE_SELECT_ITEM;
    }

    private void MoveWinner() {
        WinnerMoving winnerMoving = scoreBoardObject.GetComponent<WinnerMoving>();
        // TODO: load video when really win
        // SceneManager.LoadScene("WIN", LoadSceneMode.Additive);
        if (winnerMoving.IsFinishMoving()) {
            stage = Stage.BEFORE_SELECT_ITEM;
        }
    }

    private void MemorizeItemsState() {
        foreach (GameObject item in items) {
            if (item.TryGetComponent<BaseItem>(out var item_base)) {
                item_base.Initialize();
            }
        }
    }

    private void ResetItems() {
        foreach (GameObject item in items) {
            if (item.TryGetComponent<BaseItem>(out var item_base)) {
                item_base.Reset();
            }
        }
    }
}
