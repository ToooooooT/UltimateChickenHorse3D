using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperSkyboxLoader : MonoBehaviour
{
    private string skyboxFolder = "Skybox/Cubemaps/Classic";
    private string skyboxName = "FS000_Day_04";

    // Start is called before the first frame update
    void Start() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxFolder + "/" + skyboxName);

        RenderSettings.skybox = skyboxMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
