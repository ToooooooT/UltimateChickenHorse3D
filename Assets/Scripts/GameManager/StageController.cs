// using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Runtime.Versioning;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StageController : MonoBehaviour
{
    public enum PartyStage {CHOOSE_STAGE, BEFORE_SELECT_ITEM, SELECT_ITEM, PLACE_ITEM, PLAY, SCOREBOARD};
    public enum CreateStage {CHOOSE_STAGE, PLAY};

    public PartyStage partyStage;
    public CreateStage createStage;
    public List<GameObject> items;
    public List<GameObject> playerObjects;

    private string gameMode;
    private GameObject scoreBoardObject;
    private GameObject LinBenObject;
    private GameObject selectStageController;
    private GameObject selectStageMenu;
    private GameObject pauseMenu;
    private string stageName;
    private GameObject stage;
    private bool isFirstChooseStage;
    private int createItemCounter;
    private List<int> winnerIndexs;

    private Vector3[] selectItemPositions;

    void Start() {
        items = new();
        winnerIndexs = new();
        selectItemPositions = new Vector3[4];
        selectItemPositions[0] = new Vector3(3000 - 38, 1, 3000 - 38);
        selectItemPositions[1] = new Vector3(3000 + 38, 1, 3000 + 38);
        selectItemPositions[2] = new Vector3(3000 + 38, 1, 3000 - 38);
        selectItemPositions[3] = new Vector3(3000 - 38, 1, 3000 + 38);
        // remember to change to party
        gameMode = PlayerPrefs.GetString("GameMode", "Party");
        if (gameMode == "Party") {
            partyStage = PartyStage.CHOOSE_STAGE;
        } else if (gameMode == "Create") {
            createStage = CreateStage.CHOOSE_STAGE;
            createItemCounter = 0;
        }
        // other mode initial stage not sure yet

        scoreBoardObject = GameObject.FindGameObjectWithTag("ScoreBoard");
        selectStageMenu = GameObject.Find("SelectStageMenu").gameObject;
        selectStageController = GameObject.Find("SelectStageMenu").gameObject.
                                    transform.Find("StagesController").gameObject;
        pauseMenu = GameObject.Find("PauseCanvas").gameObject;
        isFirstChooseStage = true;
        stage = null;
    }

    void Update() {
        switch (gameMode) {
        case "Party":
            PartyMode();
            break;
        case "Create":
            CreateMode();
            break;
        }
    }

    private void CreateMode() {
        switch (createStage) {
        case CreateStage.CHOOSE_STAGE:
            FirstChooseStage();
            if (CheckAllPlayerSelectStage()) {
                foreach (GameObject player in playerObjects) {
                    Player p = player.GetComponent<Player>();
                    p.Enable(Player.State.GAME); 
                    AdjustCamera(isFollow: true, isVirtual: false, player);
                    p.ModifyPosition(Vector3.zero);
                }
            }   
            break;
        case CreateStage.PLAY:
            foreach (GameObject player in playerObjects) {
                Player p = player.GetComponent<Player>();
                switch (p.state) {
                case Player.State.LOSE:
                    p.Enable(Player.State.GAME);
                    AdjustCamera(isFollow: true, isVirtual: false, player);
                    p.ModifyPosition(Vector3.zero);
                    // reset items
                    Transform rocket = player.transform.Find("Rocket");
                    rocket?.GetComponent<Rocket>().Reset();
                    Transform coin = player.transform.Find("Coin");
                    coin?.GetComponent<Coin>().Reset();
                    break;
                case Player.State.WIN:
                    LinBenScript LinBen = LinBenObject.GetComponent<LinBenScript>();
                    if (LinBen.state == LinBenScript.State.FINISH_POINTING) {
                        LinBen.state = LinBenScript.State.IDLE;
                        p.Enable(Player.State.GAME);
                        AdjustCamera(isFollow: true, isVirtual: false, player);
                        p.ModifyPosition(Vector3.zero);
                        // destroy items
                        if (player.transform.Find("Rocket") != null) {
                            Destroy(player.transform.Find("Rocket").gameObject);
                        }
                        if (player.transform.Find("Coin") != null) {
                            Destroy(player.transform.Find("Coin").gameObject);
                        }
                    }
                    break;
                case Player.State.STOP:
                    if (!pauseMenu.transform.Find("PauseMenu").gameObject.activeSelf && 
                            !pauseMenu.transform.Find("SettingMenu").gameObject.activeSelf) {
                        if (!p.HaveItem() && 
                                !player.transform.GetComponent<PlayerInput>().actions.FindActionMap("Cursor").enabled) {
                            AdjustCamera(isFollow: false, isVirtual: false, player);
                            player.GetComponent<PlayerCursor>().Enable();
                        } else if (p.HaveItem() && 
                                player.transform.GetComponent<PlayerInput>().actions.FindActionMap("Cursor").enabled) {
                            AdjustCamera(isFollow: false, isVirtual: true, player);
                            player.GetComponent<PlayerCursor>().Disable();
                        }
                    }
                    break;
                case Player.State.GAME:
                    // check lose 
                    if (player.transform.position.y < -50) {
                        player.GetComponent<Player>().SetDead();
                    }
                    break;
                }
            }
            for (; createItemCounter < items.Count; ++createItemCounter) {
                if (items[createItemCounter].TryGetComponent<BaseItem>(out var item_base)) {
                    item_base.Initialize();
                }
            }
            break;
        }
    }

    private void PartyMode() {
        switch (partyStage) {
        case PartyStage.CHOOSE_STAGE:
            FirstChooseStage();
            CheckAllPlayerSelectStage();   
            break;
        case PartyStage.BEFORE_SELECT_ITEM:
            ResetItemsInStage();
            PlayerSelectItem();
            GetComponent<ItemGenerator>().GenerateItems();
            partyStage = PartyStage.SELECT_ITEM;
            break;
        case PartyStage.SELECT_ITEM:
            if (!IsAllPlayersSelectItem()) {
                break;
            }
            ClearChoosingItems();
            foreach (GameObject player in playerObjects) {
                AdjustCamera(isFollow: false, isVirtual: true, player);
            }
            partyStage = PartyStage.PLACE_ITEM;
            break;
        case PartyStage.PLACE_ITEM:
            if (!IsAllPlayersPlaceItem()) {
                break;
            }
            SetPlayersPlay();
            MemorizeItemsStateInStage();
            partyStage = PartyStage.PLAY;
            break;
        case PartyStage.PLAY:
            CheckPlayersState();
            CheckAllLose();
            CheckWin();
            break;
        case PartyStage.SCOREBOARD:
            MoveWinner();
            break;
        }
    }

    private void FirstChooseStage() {
        if (isFirstChooseStage) {
            DestroyItemsAndStages();
            GetComponent<PlayerManager>().EnableJoinAction();
            foreach (GameObject player in playerObjects) {
                Player p = player.GetComponent<Player>();
                AdjustCamera(isFollow: true, isVirtual: false, player);
                p.Enable(Player.State.MOVE);
                p.ModifyPosition(0.5f * Vector3.up);
            }
            isFirstChooseStage = false;
            selectStageMenu.SetActive(true);
        }
    }

    private bool CheckAllPlayerSelectStage() {
        if (selectStageController.GetComponent<JumpToStage>().flag) {
            selectStageController.GetComponent<JumpToStage>().flag = false;
            stageName = selectStageController.GetComponent<JumpToStage>().GetChoosenStageName();
            stage = Instantiate(Resources.Load<GameObject>("Stages/" + stageName));
            LinBenObject = GameObject.FindGameObjectWithTag("LinBen");
            GetComponent<PlayerManager>().DisableJoinAction();
            partyStage = PartyStage.BEFORE_SELECT_ITEM;
            createStage = CreateStage.PLAY;
            isFirstChooseStage = true;
            return true;
        }
        return false;
    }

    private void PlayerSelectItem() {
        for (int i = 0; i < playerObjects.Count; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            player.Enable(Player.State.SELECT_ITEM);
            player.ModifyPosition(selectItemPositions[i]);
            AdjustCamera(isFollow: true, isVirtual: false, playerObjects[i]);
        }
    }

    private bool IsAllPlayersSelectItem() {
        bool ret = true;
        for (int i = 0; i < playerObjects.Count; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            ret &= player.HaveItem();
            if (player.HaveItem() && player.state != Player.State.STOP) {
                player.ModifyPosition(Vector3.zero);
                player.Disable(Player.State.STOP);
            }
        }
        return ret;
    }

    private bool IsAllPlayersPlaceItem() {
        bool ret = true;
        for (int i = 0; i < playerObjects.Count; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            ret &= !player.HaveItem();
            if (!player.HaveItem() && playerObjects[i].transform.Find("Camera").GetComponent<CameraMovement>().enabled) {
                playerObjects[i].transform.Find("Camera").GetComponent<CameraMovement>().Disable();
            }
        }
        return ret;
    }

    private void SetPlayersPlay() {
        for (int i = 0; i < playerObjects.Count; ++i) {
            Player player = playerObjects[i].GetComponent<Player>();
            player.Enable(Player.State.GAME);
            AdjustCamera(isFollow: true, isVirtual: false, playerObjects[i]);
        }
    }

    private void ClearChoosingItems() {
        GameObject[] choosing_items = GameObject.FindGameObjectsWithTag("ChoosingItem");
        for (int i = 0; i < choosing_items.Length; ++i) {
            Destroy(choosing_items[i]);
        }
    }

    private void AdjustCamera(bool isFollow, bool isVirtual, GameObject player) {
        Transform camera = player.transform.Find("Camera");
        if (isFollow) {
            camera.GetComponent<MouseControlFollowCamera>().Enable();
        } else {
            camera.GetComponent<MouseControlFollowCamera>().Disable();
        }
        if (isVirtual) {
            camera.GetComponent<CameraMovement>().Enable();
        } else {
            camera.GetComponent<CameraMovement>().Disable();
        }
    }

    private void CheckPlayersState() {
        for (int i = 0; i < playerObjects.Count; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            if (player.transform.position.y < -50) {
                player.SetDead();
                // SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
            } else if (player.state == Player.State.LOSE) {
                // SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
            }
        }
    }

    private void CheckWin() {
        WinnerMoving winnerMoving = scoreBoardObject.GetComponent<WinnerMoving>();
        for (int i = 0; i < playerObjects.Count; i++) {
            Player player = playerObjects[i].GetComponent<Player>();
            LinBenScript LinBen = LinBenObject.GetComponent<LinBenScript>();
            if (player.state == Player.State.WIN && LinBen.state == LinBenScript.State.FINISH_POINTING) {
                partyStage = PartyStage.SCOREBOARD;
                LinBen.state = LinBenScript.State.IDLE;
                winnerMoving.winner = i;
                winnerIndexs.Add(i);
            }
        }
        if (winnerMoving.winner >= 0) {
            for (int i = 0; i < playerObjects.Count; i++) {
                Player player = playerObjects[i].GetComponent<Player>();
                Vector3 position = winnerMoving.GetPlayerCubePosition(i);
                position.y += 1;
                player.ModifyPosition(position);
                player.Enable(Player.State.MOVE);
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
        partyStage = PartyStage.BEFORE_SELECT_ITEM;
    }

    private void MoveWinner() {
        WinnerMoving winnerMoving = scoreBoardObject.GetComponent<WinnerMoving>();
        // TODO: load video when really win
        // SceneManager.LoadScene("WIN", LoadSceneMode.Additive);
        if (winnerMoving.IsFinishMoving()) {
            partyStage = PartyStage.BEFORE_SELECT_ITEM;
            // if winner take the coin, the coin should be destroy
            foreach (int index in winnerIndexs) {
                Transform t = playerObjects[index].transform.Find("Coin");
                if (t != null) {
                    Destroy(playerObjects[index].transform.Find("Coin").gameObject);
                }
            }
        }
    }

    private void MemorizeItemsStateInStage() {
        List<GameObject> nullItems = new();       
        foreach (GameObject item in items) {
            if (item == null) {
                nullItems.Add(item);
            } else if (item.TryGetComponent<BaseItem>(out var item_base)) {
                item_base.Initialize();
            }
        }
        foreach (GameObject nullItem in nullItems) {
            items.Remove(nullItem);
        }
    }

    private void ResetItemsInStage() {
        List<GameObject> nullItems = new();       
        foreach (GameObject item in items) {
            if (item == null) {
                nullItems.Add(item);
            } else if (item.TryGetComponent<BaseItem>(out var item_base)) {
                item_base.Reset();
            }
        }
        foreach (GameObject nullItem in nullItems) {
            items.Remove(nullItem);
        }
    }

    private void DestroyItemsAndStages() {
        while (items.Count > 0) {
            Destroy(items[0]);
            items.Remove(items[0]);
        }
        if (stage != null) {
            Destroy(stage);
            stage = null;
        }
    }
}
