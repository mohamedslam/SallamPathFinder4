#region File Header
/// <summary>
/// File: IRobotCommunicator.cs
/// Description: Interface for communication with physical robot via WiFi
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Robot;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Interface Documentation
    /// <summary>
    /// Service interface for communication with physical robot via WiFi
    /// Handles command sending, sensor data retrieval, and camera streaming
    /// </summary>
    #endregion
    public interface IRobotCommunicator
    {
        #region Connection Methods
        /// <summary>Connects to the robot at the specified IP and port</summary>
        Task<bool> ConnectAsync(string ip, int port);

        /// <summary>Disconnects from the robot</summary>
        void Disconnect();

        /// <summary>Checks if connected to robot</summary>
        bool IsConnected { get; }
        #endregion

        #region Command Methods
        /// <summary>Sends a command to the robot</summary>
        Task SendCommandAsync(RobotCommand command);

        /// <summary>Requests sensor data from the robot</summary>
        Task<SensorData> RequestSensorDataAsync();

        /// <summary>Requests a camera frame from the robot</summary>
        Task<Image> RequestCameraFrameAsync();
        #endregion

        #region Events
        /// <summary>Event raised when sensor data is received</summary>
        event Action<SensorData> SensorDataReceived;

        /// <summary>Event raised when a camera frame is received</summary>
        event Action<Image> CameraFrameReceived;

        /// <summary>Event raised when an obstacle is detected</summary>
        event Action<ObstacleData> ObstacleDetected;
        #endregion
    }
}