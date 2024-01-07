using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyIslandSceneLoader : SkyboxLoaderTemplate
{
    protected override string skyboxPath { get; } = "Skybox/Cubemaps/FS003/FS003_Sunset";
    protected override string bgm { get; } = "LevitateArchipelagoBackground";
}