using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    public GameObject[] playersToGenerate;
    [SerializeField] private Vector3 spawnArea;
    [SerializeField] private Vector3 size;
    [SerializeField] private int numberOfPlayersToGenerate = 1;

    void Start() {
        for (int i = 0; i < numberOfPlayersToGenerate; i++) {
            float randomX = Random.Range(spawnArea.x - size.x / 2, spawnArea.x + size.x / 2);
            float randomZ = Random.Range(spawnArea.z - size.z / 2, spawnArea.z + size.z / 2);
            Vector3 randomPosition = new Vector3(randomX, spawnArea.y, randomZ);

            Instantiate(playersToGenerate[i], randomPosition, Quaternion.identity);
        }
    }
}
