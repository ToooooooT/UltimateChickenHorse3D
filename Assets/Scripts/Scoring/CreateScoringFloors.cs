using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScoringFloors : MonoBehaviour
{
    // Start is called before the first frame update
    public int WinScore;
    void Start() {
        CreateFloors(WinScore);
    }

    private void CreateFloors(int WinScore) {
        WinScore++;
        for (int i = 0; i < WinScore; i++) {

            Vector3 floorPosition = new(-3009, 0, 2999 + 30 * ((1.0f / 2 + i) / WinScore));
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            plane.transform.SetPositionAndRotation(floorPosition, Quaternion.identity);
            Color randomColor = new(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            if (plane.TryGetComponent<Renderer>(out var renderer)) {
                renderer.material.color = randomColor;
            }
            plane.transform.localScale = new Vector3(3, 3, 3.0f / WinScore);
            plane.transform.parent = gameObject.transform;
            plane.tag = "Wall";
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
