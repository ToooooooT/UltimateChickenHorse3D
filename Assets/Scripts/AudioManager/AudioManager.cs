using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private float volume = 1.0f;
    private float bgmVolume = 1.0f;
    private List<float> seVolume = new List<float>();
    public int sourceCount = 10;
    public float Volume {
        get {
            return volume;
        } set {
            volume = Mathf.Clamp01(value);
        }
    }
    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> seClips = new Dictionary<string, AudioClip>();
    private AudioSource bgmAudio;
    private List<AudioSource> seAudios = new List<AudioSource>();

    void Awake()
    {
        AddSoundSource(sourceCount);
        LoadAllAudioClips();
    }

    private void Update() {
        UpdateVolume();
    }

    public void PlaySE(string name, float volume=1) {
        var clip = seClips[name];
        volume = Mathf.Clamp01(volume);
        if (clip != null) {
            bool flag = false;
            for (int i = 0; i < sourceCount; i++) {
                if (!seAudios[i].isPlaying) {
                    seAudios[i].clip = clip;
                    seAudios[i].volume = volume * this.Volume;
                    seVolume[i] = volume;
                    seAudios[i].Play();
                    flag = true;
                    break;
                }
            }
            if (!flag) {
                seAudios[0].clip = clip;
                seAudios[0].volume = volume * this.Volume;
                seVolume[0] = volume;
                seAudios[0].Play();
            }
            return;
        }
        Debug.LogWarning("Null SE");
    }

    public void PlayBGM(string name, float volume=1) {
        var clip = bgmClips[name];
        volume = Mathf.Clamp01(volume);
        if (clip != null) {
            bgmAudio.clip = clip;
            bgmAudio.loop = true;
            bgmVolume = volume;
            bgmAudio.volume = volume * this.Volume;
            bgmAudio.Play();
            return;
        }
        Debug.LogWarning("Null BGM");
    }

    public void StopBGM() {
        bgmAudio.Stop();
    }

    public void StopSE() {
        for (int i = 0; i < sourceCount; i++) {
            seAudios[i].Stop();
        }
    }

    public void PauseSE(int index) {
        if (index < sourceCount) {
            seAudios[index].Pause();
        }
    }

    public void PauseBGM() {
        bgmAudio.Pause();
    }

    public void ResumeSE() {
        for (int i = 0; i < sourceCount; i++) {
            seAudios[i].UnPause();
        }
    }

    public void ResumeBGM() {
        bgmAudio.UnPause();
    }

    private void LoadAllAudioClips() {
        AudioClip[] introClipsList = Resources.LoadAll<AudioClip>("Audio/intro");
        AudioClip[] itemsClipsList = Resources.LoadAll<AudioClip>("Audio/items");
        AudioClip[] levelsClipsList = Resources.LoadAll<AudioClip>("Audio/levels");
        AudioClip[] playerClipsList = Resources.LoadAll<AudioClip>("Audio/player");

        foreach (AudioClip bgmClip in introClipsList) {
            bgmClips.Add(bgmClip.name, bgmClip);
        }

        foreach (AudioClip bgmClip in levelsClipsList) {
            bgmClips.Add(bgmClip.name, bgmClip);
        }

        foreach (AudioClip seClip in itemsClipsList) {
            seClips.Add(seClip.name, seClip);
        }

        foreach (AudioClip seClip in playerClipsList) {
            seClips.Add(seClip.name, seClip);
        }
    }

    private void AddSoundSource(int n) {
        for (int i = 0; i < n; i++) {
            var audio = this.gameObject.AddComponent<AudioSource>();
            seAudios.Add(audio);
            seVolume.Add(1.0f);
        }
        bgmAudio = this.gameObject.AddComponent<AudioSource>();
    }

    private void UpdateVolume() {
        bgmAudio.volume = bgmVolume * this.Volume;
        for (int i = 0; i < sourceCount; i++) {
            seAudios[i].volume = seVolume[i] * this.Volume;
        }
    }
}
