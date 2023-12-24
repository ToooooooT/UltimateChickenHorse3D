using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperSkyboxLoader : MonoBehaviour
{
    private string skyboxPath = "Skybox/Cubemaps/FS013/FS013_Night_Cubemap";

    // Start is called before the first frame update
    void Start() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxPath);

        RenderSettings.skybox = skyboxMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
