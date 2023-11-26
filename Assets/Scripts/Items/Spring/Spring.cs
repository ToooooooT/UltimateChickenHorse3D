using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : BaseItem
{
    private Transform topTransform;
    private SpringTop topScript;
    private Transform springTransform;
    private Vector3 origin_position;
    private Quaternion origin_rotation;
    private bool triggerVibration;
    public bool isTriggerVibration;
    private float dampingRatio;
    private Vector3 initialTopPosition;
    private Vector3 initialSpringScale;
    private float frequency;
    private float amplitude;
    private float startTime;

    // Start is called before the first frame update
    void Start() {
        Transform baseTransform = transform.Find("Base");
        topTransform = transform.Find("Top");
        topScript = topTransform.GetComponent<SpringTop>();
        springTransform = transform.Find("SpringSelf");
        initialTopPosition = topTransform.localPosition;
        amplitude = initialTopPosition.y / 2 - baseTransform.localPosition.y;
        initialSpringScale = springTransform.localScale;
        triggerVibration = false;
        isTriggerVibration = false;
        dampingRatio = 0.1f;
        frequency = 5;
        startTime = -1;
    }

    // Update is called once per frame
    void Update() {
        HandleStep();
        Vibration();
    }

    private void HandleStep() {
        if (topScript.TouchPlayer()) {
            triggerVibration = true;
            isTriggerVibration = true;
        }
    }

    private void Vibration() {
        if (!triggerVibration) {
            isTriggerVibration = false;
        }

        if (triggerVibration) {
            startTime = Time.time;
            triggerVibration = false;
        } else if (startTime == -1) {
            return;
        }
        
        float elapsed = Time.time - startTime;
        float angularFrequency = 2.0f * Mathf.PI * frequency;
        float dampedAmplitude = amplitude * Mathf.Exp(-dampingRatio * angularFrequency * elapsed) * Mathf.Sin(angularFrequency * elapsed);

        topTransform.localPosition = initialTopPosition + new Vector3(0f, dampedAmplitude, 0f);
        springTransform.localPosition = topTransform.localPosition / 2;
        springTransform.localScale = new Vector3(initialSpringScale.x, springTransform.localPosition.y, initialSpringScale.z);

        if (Mathf.Exp(-dampingRatio * angularFrequency * elapsed) < 0.0001) {
            startTime = -1;
            return;
        }
    }

    public override void Initialize() {
        origin_position = transform.position;
        origin_rotation = transform.rotation;
    }

    public override void Reset() {
        transform.SetPositionAndRotation(origin_position, origin_rotation);
    }
}
