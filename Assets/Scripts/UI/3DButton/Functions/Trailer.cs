using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class Trailer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RenderTexture renderTx;

    // Start is called before the first frame update
    void Start()
    {
        ClearOutRenderTexture(renderTx);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (!videoPlayer.isPlaying) {
                PlayTrailer();
            } else {
                StopTrailer();
            }
        }
    }

    private void PlayTrailer() {
        if (videoPlayer != null) {
            // Play the video
            videoPlayer.Play();
        }
    }

    private void StopTrailer() {
        if (videoPlayer != null) {
            // Play the video
            videoPlayer.Stop();
            ClearOutRenderTexture(renderTx);
        }
    }

    private void ClearOutRenderTexture(RenderTexture renderTexture) {
        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = rt;
    }
}
