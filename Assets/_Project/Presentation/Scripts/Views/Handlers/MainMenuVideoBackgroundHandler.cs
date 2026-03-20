using System;
using _Project.Application.Events.Core;
using _Project.Application.Events.EventChannels;
using _Project.Application.Events.Load;
using _Project.Application.States.GameState;
using UnityEngine;
using UnityEngine.Video;

namespace _Project.Presentation.Scripts.Views.Handlers
{
    [RequireComponent(typeof(VideoPlayer))]
    public class MainMenuVideoBackgroundHandler : MonoBehaviour
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
            gameStateEventChannel.OnEventRaised += HandleStateChanged;
        }

        private void OnDisable()
        {
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