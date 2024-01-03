using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyscraperSceneLoader : MonoBehaviour
{
    private string skyboxPath = "Skybox/Cubemaps/FS013/FS013_Night_Cubemap";
    private string bgm = "skyscraperBackground";

    void Start() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxPath);
        RenderSettings.skybox = skyboxMaterial;
        AudioManager audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.PlayBGM(bgm);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
