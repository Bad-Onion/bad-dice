using System;
using _Project.Application.Events;
using _Project.Application.States.GameState;
using UnityEngine;
using UnityEngine.Video;

namespace _Project.Presentation.Scripts.Views.UI
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoBackgroundHandler : MonoBehaviour
    {
        [SerializeField] private GameStateEventChannel gameStateEventChannel;
        private VideoPlayer _videoPlayer;

        private void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _videoPlayer.prepareCompleted += OnVideoPrepared;

            _videoPlayer.Prepare();
        }

        private void OnEnable()
        {
            if (gameStateEventChannel != null)
                gameStateEventChannel.OnEventRaised += HandleStateChanged;
        }

        private void OnDisable()
        {
            if (gameStateEventChannel != null)
                gameStateEventChannel.OnEventRaised -= HandleStateChanged;
        }

        private void OnVideoPrepared(VideoPlayer source)
        {
            _videoPlayer.prepareCompleted -= OnVideoPrepared;

            Bus<BootstrapReadyEvent>.Raise(new BootstrapReadyEvent());
        }

        private void HandleStateChanged(Type stateType)
        {
            if (stateType == typeof(MainMenuState))
                _videoPlayer.Play();
            else
                _videoPlayer.Pause();
        }
    }
}