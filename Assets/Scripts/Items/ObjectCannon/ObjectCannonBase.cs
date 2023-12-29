using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCannonBase : BaseItem
{
    private ObjectCannon cannon;

    void Awake() {
        cannon = transform.Find("rotater").GetComponent<ObjectCannon>();
    }

    public override void Initialize() {
        cannon.Initialize();
    }

    public override void Reset() {
        cannon.Reset();
    }
}
