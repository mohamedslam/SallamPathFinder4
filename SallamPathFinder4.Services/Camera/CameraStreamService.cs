#region File Header
/// <summary>
/// File: CameraStreamService.cs
/// Description: Service for receiving camera stream from robot
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Interfaces.Services;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Services.Camera
{
    #region Class Documentation
    /// <summary>
    /// Service for receiving camera stream from robot
    /// Manages continuous frame streaming with configurable frame rate
    /// </summary>
    #endregion
    public sealed class CameraStreamService : ICameraStreamService, IDisposable
    {
        #region Constants
        private const int DEFAULT_FRAME_INTERVAL_MS = 33; // ~30 FPS
        #endregion

        #region Private Fields
        private readonly IRobotCommunicator _communicator;
        private CancellationTokenSource _cts;
        private bool _isStreaming;
        private int _frameIntervalMs;
        private bool _isDisposed;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new camera stream service
        /// </summary>
        public CameraStreamService(IRobotCommunicator communicator)
        {
            _communicator = communicator ?? throw new ArgumentNullException(nameof(communicator));
            _frameIntervalMs = DEFAULT_FRAME_INTERVAL_MS;
            _isStreaming = false;

            // Subscribe to camera frames from communicator
            _communicator.CameraFrameReceived += OnCameraFrameReceived;
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public bool IsStreaming => _isStreaming;
        #endregion

        #region Events
        /// <inheritdoc/>
        public event Action<Image> FrameReceived;
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public async Task StartStreamingAsync()
        {
            if (_isStreaming)
                return;

            _isStreaming = true;
            _cts = new CancellationTokenSource();

            await Task.Run(async () =>
            {
                while (_isStreaming && !_cts.Token.IsCancellationRequested)
                {
                    await _communicator.RequestCameraFrameAsync();
                    await Task.Delay(_frameIntervalMs);
                }
            });
        }

        /// <inheritdoc/>
        public void StopStreaming()
        {
            _isStreaming = false;
            _cts?.Cancel();
        }

        /// <summary>
        /// Sets the frame rate for streaming
        /// </summary>
        public void SetFrameRate(int framesPerSecond)
        {
            _frameIntervalMs = Math.Max(10, 1000 / Math.Max(1, framesPerSecond));
        }
        #endregion

        #region Private Methods
        private void OnCameraFrameReceived(Image frame)
        {
            FrameReceived?.Invoke(frame);
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                StopStreaming();
                _cts?.Dispose();

                if (_communicator != null)
                    _communicator.CameraFrameReceived -= OnCameraFrameReceived;

                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}