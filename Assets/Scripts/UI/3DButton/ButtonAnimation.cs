using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    private float pressStartTime;
    private float leaveStartTime;
    private float frequency = 0.25f;
    private float amplitude;
    private Vector3 originPosition;
    private int playerCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        pressStartTime = -1;
        leaveStartTime = -1;
        amplitude = transform.localScale.y;
        originPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePress();
        HandleLeave();
    }

    private void HandlePress() {
        float elapsed = Time.time - pressStartTime;
        
        if (pressStartTime < 0 ||¡@elapsed > 0.5f / (2 * Mathf.PI * frequency)) {
            return;
        }

        float angularFrequency = 2.0f * Mathf.PI * frequency;
        float dy = 1f * -Mathf.Cos(angularFrequency * elapsed) * Time.deltaTime;

        transform.localPosition += new Vector3(0, dy, 0);
    }

    private void HandleLeave() {
        if (leaveStartTime < 0 || transform.localPosition.y >= originPosition.y) {
            leaveStartTime = -1;
            return;
        }

        float elapsed = Time.time - leaveStartTime;
        float angularFrequency = 2.0f * Mathf.PI * frequency;
        float dy = amplitude * Mathf.Cos(angularFrequency * elapsed) * Time.deltaTime;

        transform.localPosition += new Vector3(0, dy, 0);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (playerCount == 0) {
                pressStartTime = Time.time;
            }
            playerCount++;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            playerCount--;
            if (playerCount == 0) {
                pressStartTime = -1;
                leaveStartTime = Time.time;
            }
        }
    }
}
