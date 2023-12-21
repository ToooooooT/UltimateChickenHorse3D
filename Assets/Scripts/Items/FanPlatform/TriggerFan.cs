using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFan : MonoBehaviour
{
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Player player = other.gameObject.GetComponent<Player>();
            if (player.state == Player.State.GAME) {
                player.state = Player.State.LOSE;
            }
        }
    }
}
