using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity : BaseItem
{
    public Vector3 velocity;

    void Start() {

    }

    protected void Forward() {
        transform.forward = velocity;
    }
}
