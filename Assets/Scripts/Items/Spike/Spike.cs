using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    void Start() {
        
    }

    void Update() {
        
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            if (player.state == Player.State.GAME) {
                other.transform.Find("PlayerVisual").GetComponent<PlayerAnimator>().SetDead();
            }
        }
    }
}
