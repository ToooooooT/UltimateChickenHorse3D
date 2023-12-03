using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : BaseItem
{
    [SerializeField] private float fanRadius = 3.0f;
    private Transform wind;
    private Transform fanCenter;
    private Vector3 origin_position;
    private Quaternion origin_rotation;
    private Vector3 originalWindDirection = Vector3.forward;
    private Vector3 windDirection;
    private float windSpeed;
    private float windDistance;
    [SerializeField] private float speedRatio;

    void Awake() {
        fanCenter = transform.Find("FanCenter");
        wind = transform.Find("FanCenter/CFXR4 Wind Trails");
    }

    void Start() {
        windDirection = transform.rotation * originalWindDirection;
        windSpeed = wind.GetComponent<ParticleSystem>().main.startSpeed.constant;
        windDistance = windSpeed * wind.GetComponent<ParticleSystem>().main.startLifetime.constant;
        speedRatio = 0.04f;
    }

    void Update() {
        UpdateWindDirection();
        Blow();
    }

    private void UpdateWindDirection() {
        windDirection = transform.rotation * originalWindDirection;
    }

    private void Blow() {
        if (!wind.gameObject.activeSelf) {
            return;
        }

        if (Physics.SphereCast(fanCenter.transform.position, fanRadius, windDirection, out RaycastHit hitInfo, windDistance)) {
            GameObject collidedObject = hitInfo.collider.gameObject;
            if (collidedObject.CompareTag("Player")) {
                Player player = collidedObject.GetComponent<Player>();
                player.exSpeed += windSpeed * speedRatio * windDirection;
            }
        }
    }


    public override void Initialize() {
        origin_position = transform.position;
        origin_rotation = transform.rotation;
        wind.gameObject.SetActive(true);
    }

    public override void Reset() {
        transform.SetPositionAndRotation(origin_position, origin_rotation);
        wind.gameObject.SetActive(true);
    }
}
