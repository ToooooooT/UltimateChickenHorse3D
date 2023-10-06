using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinBenScript : MonoBehaviour
{
    private Animator anim;
    private Transform pos;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        pos = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.SetBool("pointing", !anim.GetBool("pointing"));
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("press w");
            Debug.Log(pos.position.x);
            pos.position = pos.position + new Vector3((float) 0.1, 0, 0);
        }
    }
}
