using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : BaseItem
{
    private float rotateSpeed;
    private float risilientSpeed;
    private enum State { placing, gaming};
    private State state = State.placing; 
    private Vector3 targetUp;
    void Awake()
    {
        state = State.placing;
    }
    // Start is called before the first frame update
    void Start()
    {
        rotateSpeed = 0.03f;
        risilientSpeed = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        targetUp += Time.deltaTime * risilientSpeed * new Vector3(0, 1, 0);
        targetUp = targetUp.normalized;
        transform.parent.up = (Time.deltaTime * rotateSpeed * 10 * (targetUp - Vector3.Dot(targetUp, transform.parent.up) * transform.parent.up) + transform.parent.up).normalized;
    }
    void OnTriggerStay(Collider collision)
    {
        GameObject collideObject = collision.gameObject;
        if (collideObject.CompareTag("Player")) {
            targetUp += (collideObject.transform.position - transform.parent.position) * (collideObject.transform.position - transform.parent.position).magnitude;
        }
    }
    public override void Initialize()
    {
        state = State.gaming;
        targetUp = new Vector3(0, 1, 0);
        transform.parent.localScale = new Vector3(40, 40, 40);
        transform.parent.transform.up = new Vector3(0, 1, 0);
    }

    public override void Reset()
    {
        state = State.placing;
        targetUp = new Vector3(0, 1, 0);
        transform.parent.transform.up = new Vector3(0, 1, 0);
    }
}
