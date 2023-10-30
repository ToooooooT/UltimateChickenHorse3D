using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    private GameObject[] playerObjects;
    void Start() {
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update() {
        for (int i = 0; i < playerObjects.Length; i++) {
            if (playerObjects[i].transform.position.y < -50) {
                playerObjects[i].GetComponent<Player>().ModifyPosition(Vector3.zero);
                SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
                // SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameOver"));
            } else if (playerObjects[i].transform.position.y > 20 && playerObjects[i].transform.position.y < 100) {
                playerObjects[i].GetComponent<Player>().ModifyPosition(Vector3.zero);
                SceneManager.LoadScene("Win", LoadSceneMode.Additive);
                // SceneManager.SetActiveScene(SceneManager.GetSceneByName("Win"));
            } 
        }
    }
}
