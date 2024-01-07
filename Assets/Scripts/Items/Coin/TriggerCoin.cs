using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCoin : MonoBehaviour
{
    private Transform parent;
    private AudioManager audioManager;

    void Awake() {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        parent = transform.parent;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") && parent.GetComponent<Coin>().IsRotate()) {
            audioManager.PlaySE("touchMoney");
            parent.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            parent.SetParent(other.transform);
            parent.SetLocalPositionAndRotation(new Vector3(0f, 2.7f, 0f), Quaternion.Euler(0, 0, 0));
            GetComponent<MeshCollider>().enabled = false;
        }
    } 
}
