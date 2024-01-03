using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoController : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float angle;
    [SerializeField] private float speed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;

    private GameObject[] tornados;
    private float countDown;

    void Start() {
        tornados = new GameObject[2];
        tornados[0] = transform.Find("Tornado").gameObject;
        tornados[1] = transform.Find("Tornado1").gameObject;
        countDown = -5;
        duration = 30;
        angle = 2f;
        speed = 1f;
        minX = 30;
        maxX = 90;
        minZ = -60;
        maxZ = 25;
    }

    void Update() {
        if (countDown < 0.0f) {
            SetActiveTornados(false);
        }
        if (countDown < -1.0f) {
            GenerateTornados();
        }
        countDown -= Time.deltaTime;
        MoveTornados();
    }

    private void SetActiveTornados(bool active) {
        foreach (GameObject tornado in tornados) {
            tornado.SetActive(active);
        }
    }

    private void MoveTornados() {
        foreach (GameObject tornado in tornados) {
            if (tornado.activeSelf) {
                Vector3 forward = tornado.transform.forward;
                Vector2 dir = new(forward.x, forward.z);
                dir = RotateVector(dir, Random.Range(-angle, angle));
                tornado.transform.forward = new(dir.x, 0, dir.y);
                Vector3 position = tornado.transform.localPosition;
                position.x += dir.x * speed * Time.deltaTime;
                position.x = Mathf.Clamp(position.x, minX, maxX);
                position.z += dir.y * speed * Time.deltaTime;
                position.z = Mathf.Clamp(position.z, minZ, maxZ);
                tornado.transform.localPosition = position;
            }
        }
    }

    private void GenerateTornados() {
        foreach (GameObject tornado in tornados) {
            tornado.SetActive(true);
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            float y = tornado.transform.localPosition.y;
            tornado.transform.localPosition = new Vector3(randomX, y, randomZ);
            countDown = duration;
        }
    }

    private Vector2 RotateVector(Vector2 vector, float angleInDegrees) {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleInRadians);
        float sin = Mathf.Sin(angleInRadians);

        float x = vector.x * cos - vector.y * sin;
        float y = vector.x * sin + vector.y * cos;

        return new Vector2(x, y);
    }
}
