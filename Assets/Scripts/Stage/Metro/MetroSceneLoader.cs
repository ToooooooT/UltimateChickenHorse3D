using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroSceneLoader : MonoBehaviour
{
    private string skyboxPath = "Skybox/Panoramics/FS003/FS003_Rainy";
    private string bgm = "metroBackground";

    void Start() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxPath);
        RenderSettings.skybox = skyboxMaterial;
        AudioManager audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.PlayBGM(bgm);
    }

    void Update()
    {
        
    }
}
