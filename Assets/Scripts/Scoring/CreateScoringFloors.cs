using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateScoringFloors : MonoBehaviour
{
    // Start is called before the first frame update
    public int WinScore;
    void Start()
    {
        CreateFloors(WinScore);
    }
    public void CreateFloors(int WinScore)
    {
        WinScore++;
        for (int i = 0; i < WinScore; i++) {

            Vector3 floorPosition = new(0, 6000, -15 + 30 * ((1.0f / 2 + i) / WinScore));
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            plane.transform.position = floorPosition;
            plane.transform.rotation = Quaternion.identity;
            Color randomColor = new(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            if (plane.TryGetComponent<Renderer>(out var renderer)) {
                renderer.material.color = randomColor;
            }
            plane.transform.localScale = new Vector3(3, 3, 3.0f / WinScore);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
