using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private ParticleSystem fireParticleSystem;

    private void Start()
    {
        fireParticleSystem = GetComponent<ParticleSystem>();
    }


    public void StartFire()
    {
        fireParticleSystem.Play();
    }

    public void StopFire()
    {
        fireParticleSystem.Stop();
    }
}
