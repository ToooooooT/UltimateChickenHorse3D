using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandController : MonoBehaviour
{
    [SerializeField] private float angle;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;

    private List<GameObject> islandObjects = new List<GameObject>();
    private List<Vector3> islandDirections = new List<Vector3>();
    private int islandObjectCount = 0;
    private float changeDirectionInterval;

    void Start() {
        angle = 45f;
        speed = 1f;
        rotationSpeed = 0.2f;
        minX = transform.position.x - 10;
        maxX = transform.position.x + 90;
        minZ = transform.position.z - 30;
        maxZ = transform.position.x + 30;
        changeDirectionInterval = 0.5f;
        FindIslandObjects();
        initIsland();
        InvokeRepeating("ChangeDirection", 0f, changeDirectionInterval);
    }

    void Update() {
        MoveIslandObjects();
    }

    private void FindIslandObjects() {
        Transform parentTransform = transform.parent;
        for (int i = 0; i < parentTransform.childCount; i++) {
            Transform childTransform = parentTransform.GetChild(i);
            GameObject childObject = childTransform.gameObject;

            if (childObject.name.StartsWith("island_")) {
                islandObjects.Add(childObject);
                islandObjectCount++;
            }
        }
    }

    private void initIsland() {
        for (int i = 0; i < islandObjectCount; i++) {
            islandDirections.Add(new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized);
            islandObjects[i].transform.forward = islandDirections[i];
        }
    }

    private void SetActiveIsland(bool active) {
        foreach (GameObject island in islandObjects) {
            island.SetActive(active);
        }
    }

    private void MoveIslandObjects() {
        for (int i = 0; i < islandObjectCount; i++) {
            GameObject island = islandObjects[i];
            Vector3 dir = islandDirections[i];
            if (island.activeSelf) {
                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                //Quaternion targetRotation = Quaternion.LookRotation(new Vector3(-1, 0, 0), Vector3.up);
                Quaternion smoothRotation = Quaternion.Slerp(island.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                Vector3 movement = island.transform.forward * speed * Time.deltaTime;

                island.transform.forward = smoothRotation * Vector3.forward;
                if (island.transform.position.x > maxX || island.transform.position.x < minX) {
                    islandDirections[i] = new Vector3(-dir.x, dir.y, dir.z);
                }
                if (island.transform.position.z > maxZ || island.transform.position.z < minZ) {
                    islandDirections[i] = new Vector3(dir.x, dir.y, -dir.z);
                }
                island.transform.position += movement;
            }
        }
    }

    void ChangeDirection() {
        for (int i = 0; i < islandObjectCount; i++) {
            if (islandObjects[i].activeSelf) {
                Vector3 forward = islandObjects[i].transform.forward;
                Vector2 dir = new(forward.x, forward.z);
                Vector2 newDir = RotateVector(dir, Random.Range(-angle, angle));
                islandDirections[i] = new Vector3(newDir.x, 0, newDir.y);
            }
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
