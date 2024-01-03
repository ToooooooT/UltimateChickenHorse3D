using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DroneController : MonoBehaviour
{
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float speed;

    void Start() {
        minX = -25;   
        maxX = 105;
        speed = 3.0f;
    }

    void Update() {
        Move();
    }

    private void Move() {
        Vector3 position = transform.localPosition;
        position.x += speed * Time.deltaTime;
        if (position.x > maxX || position.x < minX) {
            speed *= -1;
            Vector3 rotation = transform.eulerAngles;
            rotation.y = (rotation.y + 180) % 360;
            transform.eulerAngles = rotation;
        }
        position.x = Mathf.Clamp(position.x, minX, maxX);
        transform.localPosition = position;
    }
}
