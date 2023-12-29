using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [SerializeField] private float countDown;

    void Start() {
        Destroy(gameObject, countDown);
    }

}
