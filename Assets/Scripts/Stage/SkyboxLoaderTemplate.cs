using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxLoaderTemplate : MonoBehaviour
{
    protected virtual string skyboxPath { get; } = "Skybox/Cubemaps/Classic/FS000_Day_04";
    protected virtual string bgm { get; } = "introBackground";

    protected void Start() {
        LoadSkybox();
        LoadBGM();
    }

    private void LoadSkybox() {
        Material skyboxMaterial = Resources.Load<Material>(skyboxPath);
        RenderSettings.skybox = skyboxMaterial;
    }

    private void LoadBGM() {
        AudioManager audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.PlayBGM(bgm);
    }
}
