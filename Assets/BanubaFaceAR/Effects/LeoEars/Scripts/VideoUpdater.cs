using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace BNB
{
    public class VideoUpdater : MonoBehaviour
    {
        VideoPlayer[] _videoPlayers;

        private void Awake()
        {
            _videoPlayers = GetComponentsInChildren<VideoPlayer>();
            foreach (var video in _videoPlayers) {
                video.sendFrameReadyEvents = true;
            }
        }

        void OnFrameReady(VideoPlayer source, long frameIdx)
        {
            source.frameReady -= OnFrameReady;
            source.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        private void OnEnable()
        {
            foreach (var video in _videoPlayers) {
                video.gameObject.GetComponent<MeshRenderer>().enabled = false;
                video.frameReady += OnFrameReady;
                video.Play();
            }
        }
    }
}
