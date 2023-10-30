using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    [SerializeField] private int numberOfPlayersToGenerate = 1;
    private const string FOLDERPATH = "Player";
    public State state;

    void Awake() {
        GeneratePlayers();
    }

    void Start() {

    }

    void Update() {
    }

    public void GeneratePlayers() {
        GameObject[] playersToGenerate = LoadPlayerPrefabsInFolder(FOLDERPATH);
        float far = 3500;
        for (int i = 0; i < numberOfPlayersToGenerate; i++) {
            Vector3 farPosition = new(far, far, far);
            Instantiate(playersToGenerate[0], farPosition, Quaternion.identity);
        }
    }

    GameObject[] LoadPlayerPrefabsInFolder(string folderPath) {
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
