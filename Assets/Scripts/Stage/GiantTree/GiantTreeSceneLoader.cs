using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantTreeSceneLoader : MonoBehaviour
{
    private string skyboxPath = "Skybox/Panoramics/FS002/FS002_Day_Sunless";
    private string bgm = "giantTreeBackground";

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
