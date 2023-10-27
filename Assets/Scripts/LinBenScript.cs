using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LinBenScript : MonoBehaviour
{
    [Header("Custom Event")]
    public UnityEvent myEvents;
    private Animator anim;
    private Transform pos;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        pos = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.SetBool("pointing", !anim.GetBool("pointing"));
        }
        /*if(Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("press w");
            Debug.Log(pos.position.x);
            pos.position = pos.position + new Vector3((float) 0.1, 0, 0);
        }*/
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
