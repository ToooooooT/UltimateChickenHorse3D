using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDetecter : MonoBehaviour
{
    private List<GameObject> items;
    private Collider[] hitColliders;
    private AudioManager audioManager;

    void Start() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        items = GameObject.Find("GameController").GetComponent<StageController>().items;
    }

    void Update() {
        Detect();
    }

    void Detect() {
        hitColliders = Physics.OverlapBox(gameObject.transform.position, Vector3.Scale(transform.parent.localScale, transform.localScale) / 2, Quaternion.identity);
    }

    private GameObject GetTopParentObject(GameObject obj) {
        Transform t = obj.transform;
        while (t.parent) {
            t = t.parent;
        }
        return t.gameObject;
    }

    public void DestroyItems() {
        audioManager.PlaySE("bombExplode");
        foreach (Collider collider in hitColliders) {
            GameObject obj = GetTopParentObject(collider.gameObject);
            if (items.Contains(obj)) {
                Destroy(obj);
            }
        }
    }
}
