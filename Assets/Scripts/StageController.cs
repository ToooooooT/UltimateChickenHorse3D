using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    private enum Stage {BEFORE_SELECT_ITEM, SELECT_ITEM, PLACE_ITEM, PLAY};

    private string gameMode;
    private Stage stage;
    private GameObject[] playerObjects;
    private GameObject itemGenerator;

    void Start() {
        gameMode = PlayerPrefs.GetString("GameMode", "Party");
        if (gameMode == "Party") {
            stage = Stage.BEFORE_SELECT_ITEM;
        } 
        // other mode initial stage not sure yet

        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        itemGenerator = GameObject.FindGameObjectWithTag("ItemGenerator");
    }

    void Update() {
        switch (stage) {
        case Stage.BEFORE_SELECT_ITEM:
            for (int i = 0; i < playerObjects.Length; ++i) {
                Player player = playerObjects[i].GetComponent<Player>();
                player.state = Player.State.SELECT_ITEM;
                RandomPositionToSelectItem(player);
            }
            stage = Stage.SELECT_ITEM;
            itemGenerator.GetComponent<ItemGenerator>().GenerateItems();
            break;
        case Stage.SELECT_ITEM:
            for (int i = 0; i < playerObjects.Length; ++i) {
                Player player = playerObjects[i].GetComponent<Player>();
                if (player.state != Player.State.STOP) {
                    return;
                }
            }
            stage = Stage.PLACE_ITEM;
            // TODO: do some state tranform of place item stuff 
            break;
        case Stage.PLACE_ITEM:
            for (int i = 0; i < playerObjects.Length; ++i) {
                Player player = playerObjects[i].GetComponent<Player>();
                if (player.HaveItem()) {
                    return;
                }
            }
            stage = Stage.PLAY;
            for (int i = 0; i < playerObjects.Length; ++i) {
                Player player = playerObjects[i].GetComponent<Player>();
                player.state = Player.State.GAME;
            }
            break;
        case Stage.PLAY:
            /* TODO: check all player win or dead, 
                    if true then change to BEFORE_SELECT_ITEM,
                    if finish game change to other scene
            */
            break;
        }
    }

    private void RandomPositionToSelectItem(Player player) {
        Vector3 spawnArea = new(0, 3000, 0);
        Vector3 size = new(20, 0, 20);
        float randomX = Random.Range(spawnArea.x - size.x / 2, spawnArea.x + size.x / 2);
        float randomY = Random.Range(spawnArea.y - size.y / 2, spawnArea.y + size.y / 2);
        float randomZ = Random.Range(spawnArea.z - size.z / 2, spawnArea.z + size.z / 2);
        Vector3 randomPosition = new(randomX, randomY, randomZ);
        player.ModifyPosition(randomPosition);
    }
}
