using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private float speed = 5f; // Elevator speed
    private float upperLimit = 40f; // Upper limit of the elevator
    private float lowerLimit = 0f; // Lower limit of the elevator

    private int playerCount = 0;

    private void Update() {
        if (playerCount > 0) {
            MoveElevatorUp();
        } else {
            MoveElevatorDown();
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerCount++;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerCount--;
        }
    }


    private void MoveElevatorUp() {
        if (transform.position.y < upperLimit) {
            transform.Translate(speed * Time.deltaTime * Vector3.up);
        }
    }

    private void MoveElevatorDown() {
        if (transform.position.y > lowerLimit) {
            transform.Translate(speed * Time.deltaTime * Vector3.down);
        }
    }
}
