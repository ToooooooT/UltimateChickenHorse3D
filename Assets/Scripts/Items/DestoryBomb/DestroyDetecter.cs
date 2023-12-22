using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDetecter : MonoBehaviour
{
    private List<GameObject> items;
    private Collider[] hitColliders;

    void Start() {
        items = GameObject.Find("GameController").GetComponent<StageController>().items;   
    }

    void Update() {
        Detect();
    }

    void Detect() {
        hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
    }

    private GameObject GetTopParentObject(GameObject obj) {
        Transform t = obj.transform;
        while (t.parent) {
            t = t.parent;
        }
        return t.gameObject;
    }

    public void DestroyItems() {
        foreach (Collider collider in hitColliders) {
            GameObject obj = GetTopParentObject(collider.gameObject);
            if (items.Contains(obj)) {
                Destroy(obj);
            }
        }
    }
}
