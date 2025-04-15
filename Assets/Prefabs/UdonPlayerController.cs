
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Video.Components;
using VRC.SDKBase;
using VRC.Udon;

public class UdonPlayerController : UdonSharpBehaviour
{

    VRCUnityVideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VRCUnityVideoPlayer>();
    }

    private float timer = 0;
    private void Update()
    {
        if (Networking.IsOwner(gameObject) && Input.GetKeyDown(KeyCode.Q))
        {
            timer = 0;
            PlayPause(videoPlayer.IsPlaying);
        }

        if (Networking.IsOwner(gameObject) && Input.GetKeyDown(KeyCode.R))
        {
            timer = 0;
            RestartPlayer();
        }
    }

    private void PlayPause(bool pause)
    {
        if(pause)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }

    private void RestartPlayer()
    {
        if(videoPlayer)
        {
            videoPlayer.SetTime(0);
            videoPlayer.Play();
        }
    }
}
