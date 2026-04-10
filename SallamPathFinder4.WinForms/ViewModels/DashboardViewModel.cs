#region File Header
/// <summary>
/// File: DashboardViewModel.cs
/// Description: ViewModel for robot dashboard form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.WinForms.ViewModels
{
    #region Class Documentation
    /// <summary>
    /// ViewModel for robot dashboard form
    /// Handles robot communication, sensor data, and camera streaming
    /// </summary>
    #endregion
    public sealed class DashboardViewModel : IDisposable
    {
        #region Private Fields
        private readonly IRobotCommunicator _robotCommunicator;
        private readonly ICameraStreamService _cameraStreamService;
        private bool _isDisposed;
        #endregion

        #region Constructor
        public DashboardViewModel(IRobotCommunicator communicator, ICameraStreamService cameraService)
        {
            _robotCommunicator = communicator;
            _cameraStreamService = cameraService;

            if (_robotCommunicator != null)
            {
                _robotCommunicator.SensorDataReceived += OnSensorDataReceived;
                _robotCommunicator.CameraFrameReceived += OnCameraFrameReceived;
            }
        }
        #endregion

        #region Events
        public event Action<Image> CameraFrameReceived;
        public event Action<SensorData> SensorDataReceived;
        public event Action<string> StatusChanged;
        #endregion

        #region Public Methods
        public void SendCommand(RobotCommand command)
        {
            _robotCommunicator?.SendCommandAsync(command);
        }

        public async Task ConnectAsync(string ip, int port)
        {
            if (_robotCommunicator == null)
            {
                StatusChanged?.Invoke("No communicator available");
                return;
            }

            StatusChanged?.Invoke("Connecting...");
            var success = await _robotCommunicator.ConnectAsync(ip, port);
            StatusChanged?.Invoke(success ? "Connected" : "Connection Failed");

            if (success && _cameraStreamService != null)
            {
                await _cameraStreamService.StartStreamingAsync();
            }
        }

        public void Disconnect()
        {
            _robotCommunicator?.Disconnect();
            _cameraStreamService?.StopStreaming();
            StatusChanged?.Invoke("Disconnected");
        }

        public async Task<SensorData> RequestSensorDataAsync()
        {
            if (_robotCommunicator == null) return new SensorData();
            return await _robotCommunicator.RequestSensorDataAsync();
        }
        #endregion

        #region Private Methods
        private void OnSensorDataReceived(SensorData data)
        {
            SensorDataReceived?.Invoke(data);
        }

        private void OnCameraFrameReceived(Image frame)
        {
            CameraFrameReceived?.Invoke(frame);
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_robotCommunicator != null)
                {
                    _robotCommunicator.SensorDataReceived -= OnSensorDataReceived;
                    _robotCommunicator.CameraFrameReceived -= OnCameraFrameReceived;
                }
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}