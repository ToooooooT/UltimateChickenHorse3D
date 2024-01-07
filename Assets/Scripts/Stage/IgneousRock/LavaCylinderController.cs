using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaCylinderController : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float speed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float maxScaleTime;

    private GameObject[] lavas;

    private const float minPositionY = -18.5f;
    private const float maxPositionY = 5f;

    private float maxScaleCountdown;

    void Start() {
        lavas = new GameObject[2];
        lavas[0] = transform.Find("Lava").gameObject;
        lavas[1] = transform.Find("Lava1").gameObject;
        minX = 30;
        maxX = 90;
        minZ = -60;
        maxZ = 25;
        minScale = 0.1f;
        maxScale = 25;
        speed = 40;
        maxScaleTime = 5f;
    }

    void Update() {
        EnlargeLava();
    }

    private void EnlargeLava() {
        foreach (GameObject lava in lavas) {
            Vector3 scale = lava.transform.localScale;
            if (scale.y == maxScale) {
                maxScaleCountdown -= Time.deltaTime;
                if (maxScaleCountdown < 0) {
                    maxScaleCountdown = maxScaleTime;
                    speed *= -1;
                    scale.y += speed * Time.deltaTime;
                    scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
                    lava.transform.localScale = scale;
                    return;
                }
            } else if (scale.y == minScale) {
                speed *= -1;
                MoveLava();
            }
            scale = lava.transform.localScale;
            scale.y += speed * Time.deltaTime;
            scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
            lava.transform.localScale = scale;
        }
    }

    private void MoveLava() {
        foreach (GameObject lava in lavas) {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            lava.transform.localPosition = new Vector3(randomX, 0, randomZ);
        }
    }
}
