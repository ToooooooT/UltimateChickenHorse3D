using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxLoaderMetro : MonoBehaviour
{
    private string skyboxPath = "Skybox/Panoramics/FS003/FS003_Rainy";

    void Start() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxPath);

        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        
    }
}
