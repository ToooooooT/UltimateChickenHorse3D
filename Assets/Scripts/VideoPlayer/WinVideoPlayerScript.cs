using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class WinVideoPlayerScript : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool readyBeGhost = false;
    // Start is called before the first frame update
    void Start() {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += VideoFinished;
    }
    
    // Update is called once per frame
    void Update() {

    }

    private void VideoFinished(VideoPlayer vp) {
        if (vp == videoPlayer) {
            SceneManager.LoadScene("StartUI");
        }
    }
}
