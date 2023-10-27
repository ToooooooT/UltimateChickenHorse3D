using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    private Vector2 spawnArea;
    private Vector2 size;
    private GameObject[] playersToGenerate;
    private int numberOfPlayersToGenerate;

    void Start() {
        for (int i = 0; i < numberOfPlayersToGenerate; i++) {
            float randomX = Random.Range(spawnArea.x - size.x / 2, spawnArea.x + size.x / 2);
            float randomZ = Random.Range(spawnArea.y - size.y / 2, spawnArea.y + size.y / 2);
            Vector3 randomPosition = new Vector3(randomX, 0.1f, randomZ);

            Instantiate(playersToGenerate[i], randomPosition, Quaternion.identity);
        }
    }
}
