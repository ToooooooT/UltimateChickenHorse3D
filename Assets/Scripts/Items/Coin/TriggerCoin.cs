using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCoin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            transform.parent.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            transform.parent.SetParent(other.transform);
            transform.parent.SetLocalPositionAndRotation(new Vector3(0f, 2.7f, 0f), Quaternion.Euler(0, 0, 0));
            GetComponent<MeshCollider>().enabled = false;
        }
    } 
}
