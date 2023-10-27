using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public GameObject[] objectsToGenerate;
    public Vector3 spawnArea;
    public Vector3 size;
    public int numberOfObjectsToGenerate;

    void Start()
    {
        for (int i = 0; i < numberOfObjectsToGenerate; i++)
        {
            int randomIndex = Random.Range(0, objectsToGenerate.Length);
            GameObject objectToGenerate = objectsToGenerate[randomIndex];

            float randomX = Random.Range(spawnArea.x - size.x / 2, spawnArea.x + size.x / 2);
            float randomY = Random.Range(spawnArea.y - size.y / 2, spawnArea.y + size.y / 2);
            float randomZ = Random.Range(spawnArea.z - size.z / 2, spawnArea.z + size.z / 2);
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

            GameObject generatedObject = Instantiate(objectToGenerate, randomPosition, Quaternion.identity);
            generatedObject.AddComponent<TriggerSelectItem>();

            Rigidbody rb = generatedObject.GetComponent<Rigidbody>();
            if (rb == null) {
                // if no rigidbody, then add
                rb = generatedObject.AddComponent<Rigidbody>();
            }
        }
    }
}
