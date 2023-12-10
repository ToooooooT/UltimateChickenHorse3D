using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : Velocity
{
    
    public GameObject parentCarrier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Forward();
    }
}
