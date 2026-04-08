#region File Header
/// <summary>
/// File: RobotCommand.cs
/// Description: Commands that can be sent to the robot
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Models.Robot
{
    #region Enum Documentation
    /// <summary>
    /// Commands that can be sent to the robot for manual control
    /// Used for both simulation and physical robot communication
    /// </summary>
    #endregion
    public enum RobotCommand
    {
        /// <summary>Move robot forward</summary>
        Forward = 0,

        /// <summary>Move robot backward</summary>
        Backward = 1,

        /// <summary>Turn robot left (rotate counter-clockwise)</summary>
        TurnLeft = 2,

        /// <summary>Turn robot right (rotate clockwise)</summary>
        TurnRight = 3,

        /// <summary>Stop all movement immediately</summary>
        Stop = 4,

        /// <summary>Start camera streaming</summary>
        StartCamera = 5,

        /// <summary>Stop camera streaming</summary>
        StopCamera = 6,

        /// <summary>Request current sensor data</summary>
        GetSensorData = 7,

        /// <summary>Request current battery level</summary>
        GetBatteryLevel = 8
    }
}