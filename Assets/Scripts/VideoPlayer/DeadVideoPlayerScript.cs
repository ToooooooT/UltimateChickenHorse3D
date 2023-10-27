using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class DeadVideoPlayerScript : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private bool readyBeGhost = false;
    // Start is called before the first frame update
    void Start() {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += VideoFinished;
    }
    private void VideoFinished(VideoPlayer vp) {
        if (vp == videoPlayer) {
            SceneManager.UnloadSceneAsync("GameOver");
        }
    }
    // Update is called once per frame
    void Update() {

    }
}
