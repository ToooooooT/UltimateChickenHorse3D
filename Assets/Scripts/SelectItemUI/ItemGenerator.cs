using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private Vector3 spawnArea;
    [SerializeField] private Vector3 size;
    [SerializeField] private int numberOfObjectsToGenerate;
    private const string FOLDERPATH = "Item";
    private string gameMode;

    void Start() {

    }

    void Update() {

    }

    public void GenerateItems() {
        GameObject[] objectsToGenerate = LoadAllPrefabsInFolder(FOLDERPATH);

        for (int i = 0; i < numberOfObjectsToGenerate; i++) {
            int randomIndex = Random.Range(0, objectsToGenerate.Length);
            GameObject objectToGenerate = objectsToGenerate[randomIndex];

            float randomX = Random.Range(spawnArea.x - size.x / 2, spawnArea.x + size.x / 2);
            float randomY = Random.Range(spawnArea.y - size.y / 2, spawnArea.y + size.y / 2);
            float randomZ = Random.Range(spawnArea.z - size.z / 2, spawnArea.z + size.z / 2);
            Vector3 randomPosition = new(randomX, randomY, randomZ);

            GameObject generatedItem = Instantiate(objectToGenerate, randomPosition, Quaternion.identity);
            generatedItem.tag = "Item";
            // generatedObject.AddComponent<TriggerSelectItem>();
            if (!generatedItem.TryGetComponent<Rigidbody>(out _)) {
                // if no rigidbody, then add
                generatedItem.AddComponent<Rigidbody>();
            }
        }
    }

    GameObject[] LoadAllPrefabsInFolder(string folderPath) {
        List<GameObject> prefabs = new();
        Object[] loadedObjects = Resources.LoadAll(folderPath);

        foreach (Object obj in loadedObjects) {
            if (obj is GameObject) {
                prefabs.Add(obj as GameObject);
            }
        }

        return prefabs.ToArray();
    }
}
