using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    private GameObject[] airplanes;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        airplanes = GameObject.FindGameObjectsWithTag("Airplane");
        for (int i = 0; i < airplanes.Length; i++) {
            Airplane airplaneScript = airplanes[i].GetComponent<Airplane>();
            airplanes[i].transform.position = airplanes[i].transform.position + airplaneScript.velocity;
            CheckCollision(airplanes[i]);
            CheckOutOfBound(airplanes[i]);
        }
    }
    void CheckOutOfBound(GameObject airplane)
    {
        if (airplane.transform.position.x < -100 || airplane.transform.position.x > 100 || airplane.transform.position.y < -100 || airplane.transform.position.y > 100 || airplane.transform.position.z < -100 || airplane.transform.position.z > 100) {
            DestroyAirplane(airplane);
        }
    }
    void CheckCollision(GameObject airplane)
    {
        Collider airCollider = airplane.GetComponent<Collider>();
        if (airCollider != null) {
            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(airplane.transform.position, airplane.transform.forward), out hitInfo, 1f)) {
                GameObject collidedObject = hitInfo.collider.gameObject;
                Airplane airplaneScript = airplane.GetComponent<Airplane>();
                if (collidedObject != airplaneScript.parentCarrier) {
                    DestroyAirplane(airplane);
                }
            }
        }
    }
    void DestroyAirplane(GameObject airplane)
    {
        Destroy(airplane);
    }
}
