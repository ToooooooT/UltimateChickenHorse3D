using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowObject : MonoBehaviour
{
    // [SerializeField] private float followSpeed;
    private BoxCollider collider_;
    private Vector3 diffPosition;
    private Vector3 lastPosition;
    private Vector3 lastlastPosition;
    // Start is called before the first frame update
    void Start() {
        // followSpeed = 1.0f;   
        // collider_ = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update() {
        lastlastPosition = lastPosition;
        lastPosition = transform.TransformPoint(transform.position);       
        diffPosition = lastPosition - lastlastPosition;
    }

    public Vector3 GetDiffPosition() {
        Vector3 ret = diffPosition;
        ret.y = 0;
        return ret;
    }

    // private bool IsPlayerOnMe() {
    //     Vector3 p1 = transform.position + collider_.center;
    //     float castDistance = .2f;
    //     return Physics.SphereCast(p1, collider_.height / 2, Vector3.up, out RaycastHit hit, castDistance) 
    //             && hit.collider.CompareTag("Player");
    // }

    // private void OnCollisionEnter (Collision collision) {
    //     foreach (ContactPoint contact in collision.contacts) {
    //         if (collision.gameObject.CompareTag("Player")) {
    //             Debug.DrawRay(contact.point, contact.normal, Color.red, 1.0f);
    //             Debug.Log("Collision Normal: " + contact.normal);
    //         }
    //     }
    // }

    // private void OnCollisionStay (Collision collision) {
    //     foreach (ContactPoint contact in collision.contacts) {
    //         if (collision.gameObject.CompareTag("Player")) {
    //             Debug.DrawRay(contact.point, contact.normal, Color.red, 1.0f);
    //             Debug.Log("Collision Normal: " + contact.normal);
    //         }
    //     }
    // }

    // void OnControllerColliderHit(ControllerColliderHit hit) {
    //     if (hit.collider.CompareTag("Player")) {
    //         Vector3 normal = hit.normal;
    //         Vector3 point = hit.point;
    //         Debug.DrawRay(point, normal, Color.red, 1.0f);
    //         Debug.Log("Collision Normal: " + normal);
    //     }
    // }
}
