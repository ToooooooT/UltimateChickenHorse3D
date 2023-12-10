using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxLoaderTemplate : MonoBehaviour
{
    private string skyboxFolder = "Skybox/Cubemaps/Classic";
    private string skyboxName = "FS000_Day_04";

    void Start() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxFolder + "/" + skyboxName);

        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        
    }
}
