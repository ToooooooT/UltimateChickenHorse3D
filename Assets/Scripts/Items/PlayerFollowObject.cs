using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowObject : MonoBehaviour
{
    private Vector3 diffPosition;
    private Vector3 lastPosition;
    private Vector3 lastlastPosition;

    void Start() {

    }

    void Update() {
        lastlastPosition = lastPosition;
        lastPosition = transform.position;
        diffPosition = lastPosition - lastlastPosition;
    }

    public Vector3 GetDiffPosition() {
        Vector3 ret = diffPosition;
        ret.y = 0;
        return ret;
    }
}
