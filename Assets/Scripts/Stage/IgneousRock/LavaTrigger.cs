using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            if (player.GetState() == Player.State.GAME) {
                player.SetDead();
            }
        }
    }
}
