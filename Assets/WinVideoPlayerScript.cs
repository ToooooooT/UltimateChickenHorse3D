using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class WinVideoPlayerScript : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public bool readyBeGhost = false;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += VideoFinished;
    }
    private void VideoFinished(VideoPlayer vp)
    {
        if (vp == videoPlayer)
        {
            SceneManager.LoadScene("StartUI");
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
