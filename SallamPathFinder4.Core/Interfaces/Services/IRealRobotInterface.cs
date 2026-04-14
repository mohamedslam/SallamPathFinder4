#region File Header
/// <summary>
/// File: IRealRobotInterface.cs
/// Description: Interface for communication with real robot
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Interface Documentation
    /// <summary>
    /// Interface for communication with real robot
    /// </summary>
    #endregion
    public interface IRealRobotInterface
    {
        #region Connection
        /// <summary>Connects to robot at specified IP and port</summary>
        Task<bool> ConnectAsync(string ip, int port);

        /// <summary>Disconnects from robot</summary>
        void Disconnect();

        /// <summary>Indicates whether connected to robot</summary>
        bool IsConnected { get; }
        #endregion

        #region Send Commands
        /// <summary>Sends a single command to robot</summary>
        Task<bool> SendCommandAsync(RobotCommand command);

        /// <summary>Sends multiple commands to robot</summary>
        Task<int> SendCommandsAsync(List<RobotCommand> commands);

        /// <summary>Stops robot immediately</summary>
        Task StopAsync();

        /// <summary>Emergency stop (immediate halt)</summary>
        Task EmergencyStopAsync();
        #endregion

        #region Request Data
        /// <summary>Requests current telemetry data</summary>
        Task<RobotTelemetry> RequestTelemetryAsync();

        /// <summary>Requests camera frame</summary>
        Task<Image> RequestFrameAsync();

        /// <summary>Requests sensor readings</summary>
        Task<SensorData> RequestSensorsAsync();
        #endregion

        #region Events
        /// <summary>Event raised when telemetry data is received</summary>
        event Action<RobotTelemetry> TelemetryReceived;

        /// <summary>Event raised when command is executed</summary>
        event Action<RobotCommand, bool> CommandExecuted;

        /// <summary>Event raised when camera frame is received</summary>
        event Action<Image> FrameReceived;

        /// <summary>Event raised when error occurs</summary>
        event Action<string> ErrorOccurred;

        /// <summary>Event raised when connection is lost</summary>
        event Action ConnectionLost;

        /// <summary>Event raised when connection is restored</summary>
        event Action ConnectionRestored;
        #endregion
    }
}