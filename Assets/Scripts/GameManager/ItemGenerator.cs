using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public Vector3 spawnArea { get; private set; }
    public Vector3 size { get; private set; }
    [SerializeField] private int numberOfObjectsToGenerate;
    private const string FOLDERPATH = "Item";
    private const string ITEMTAG = "ChoosingItem";

    void Start() {
        spawnArea = new(3000, 10, 3000);
        size = new(70, 0, 70);
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
            Vector3 randomPosition = new(randomX, 0.5f, randomZ);

            // Generate random rotation around each axis
            randomX = Random.Range(0f, 360f);
            randomY = Random.Range(0f, 360f);
            randomZ = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);

            GameObject generatedItem = Instantiate(objectToGenerate, randomPosition, randomRotation);
            SetTag(generatedItem.transform);
            DisableIsTrigger(generatedItem.transform);
            AddRigidBody(generatedItem.transform);
            AddCapsuleCollider(generatedItem.transform);
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

    private void SetTag(Transform parent) {
        parent.gameObject.tag = ITEMTAG;
        foreach (Transform child in parent) {
            SetTag(child);
        }
    }

    private void AddRigidBody(Transform parent) {
        // Do we need to add rigidbody on all child object???
        if (!parent.gameObject.TryGetComponent<Rigidbody>(out _)) {
            parent.gameObject.AddComponent<Rigidbody>();
        }
    }

    private void AddCapsuleCollider(Transform parent) {
        // Do we need to add capsule on all child object???
        if (!parent.gameObject.TryGetComponent<BoxCollider>(out _) &&
            !parent.gameObject.TryGetComponent<CapsuleCollider>(out _) &&
            !parent.gameObject.TryGetComponent<MeshCollider>(out _) && 
            !parent.gameObject.TryGetComponent<SphereCollider>(out _)) {
            parent.gameObject.AddComponent<CapsuleCollider>();
        }
    }

    private void DisableIsTrigger(Transform parent) {
        if (parent.gameObject.TryGetComponent<Collider>(out var collider)) {
            collider.isTrigger = false;
        }
        foreach (Transform child in parent) {
            SetTag(child);
        }
    }

}
