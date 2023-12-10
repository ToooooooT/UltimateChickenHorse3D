using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PavilionCountPlayers : MonoBehaviour
{
    private Countdown countdown;
    private int id;
    void Start() {
        countdown = GetComponentInParent<Countdown>();
        id = transform.GetSiblingIndex();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            countdown.playersInPavilion[id]++;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            countdown.playersInPavilion[id]--;
        }
    }
}
