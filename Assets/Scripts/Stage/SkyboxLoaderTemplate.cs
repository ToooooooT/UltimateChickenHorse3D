using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxLoaderTemplate : MonoBehaviour
{
    private string skyboxPath = "Skybox/Cubemaps/Classic/FS000_Day_04";

    void Start() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxPath);

        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        
    }
}
