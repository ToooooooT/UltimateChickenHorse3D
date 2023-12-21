using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollowObject : MonoBehaviour
{
    private Vector3 lastPosition;
    private Vector3 lastlastPosition;

    void Start() {

    }

    void Update() {
        lastlastPosition = lastPosition;
        lastPosition = transform.position;
    }

    public virtual Vector3 GetDiffPosition(GameObject gameObject) {
        Vector3 ret = lastPosition - lastlastPosition;
        ret.y = 0;
        return ret;
    }
}
