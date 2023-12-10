using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkin : MonoBehaviour
{
    private string hatFolderPath = "Skin/Hat";
    private GameObject[] hatsToGenerate;

    // Start is called before the first frame update
    void Start()
    {
        hatsToGenerate = LoadAllPrefabsInFolder(hatFolderPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Transform player = other.transform;
            SetRandomHat(player);
        }
    }

    

    private void SetRandomHat(Transform player) {
        Transform playerHead = player.Find("PlayerVisual/Head");
        Transform hatTransform = playerHead.Find("Hat");
        if (hatTransform != null) {
            Destroy(hatTransform.gameObject);
        }
        int randomIndex = Random.Range(0, hatsToGenerate.Length);
        GameObject hat = Instantiate(hatsToGenerate[randomIndex], Vector3.zero, Quaternion.identity);
        hat.transform.parent = playerHead;
        Debug.Log("Hat attached to player: " + player.name);
        hat.name = "Hat";
        hat.transform.localPosition = new Vector3(0, 0.38f, 0);
        hat.transform.localRotation = Quaternion.identity;
        hat.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
    }

    private GameObject[] LoadAllPrefabsInFolder(string folderPath) {
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
