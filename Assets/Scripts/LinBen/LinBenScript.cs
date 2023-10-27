using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinBenScript : MonoBehaviour
{
    [Header("Custom Event")]
    public UnityEvent myEvents;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 targetPosition = other.transform.position;
            targetPosition.y = transform.position.y; // Maintain the same Y level
            transform.LookAt(targetPosition);

            if (myEvents == null) {
                print("myEventTriggerOnEnter was triggered but myEvents was null");
            }
            else {
                print("myEventTriggerOnEnter Activated. Triggering" + myEvents);
                myEvents.Invoke();
            }
            other.GetComponent<Player>().enabled = false;
        }
    }
}
