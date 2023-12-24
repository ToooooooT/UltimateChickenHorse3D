using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxLoaderGiantTree : MonoBehaviour
{
    private string skyboxPath = "Skybox/Panoramics/FS002/FS002_Day_Sunless";

    void Start() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxPath);

        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        
    }
}
