using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : Velocity
{
    public GameObject carrier;

    void Start() {

    }

    void Update() {
        Forward();
        Move();
        CheckCollision();
    }

    private void Move() {
        transform.position = transform.position + Time.deltaTime * 40 * velocity;
    }

    private void CheckCollision() {
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out RaycastHit hitInfo, 1f)) {
            GameObject collidedObject = hitInfo.collider.gameObject;
            if (collidedObject != carrier && collidedObject.CompareTag("Wall")) {
                Destroy(gameObject);
            }
        }
    }
}
