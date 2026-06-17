#region File Header
/// <summary>
/// File: RobotTelemetry.cs
/// Description: Telemetry data from real robot
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Models.Robot
{
    #region Class - RobotTelemetry
    /// <summary>
    /// Telemetry data received from real robot
    /// </summary>
    public sealed class RobotTelemetry
    {
        #region Constructor
        public RobotTelemetry()
        {
            this.Timestamp = DateTime.UtcNow;
        }
        #endregion

        #region Properties - Position & Movement
        public Point Position { get; set; }
        public float Angle { get; set; }
        public double Speed { get; set; }
        public double LeftWheelSpeed { get; set; }
        public double RightWheelSpeed { get; set; }
        #endregion

        #region Properties - Battery & Power
        public double BatteryPercent { get; set; }
        public double BatteryVoltage { get; set; }
        public double MotorTemp { get; set; }
        public double CpuLoad { get; set; }
        #endregion

        #region Properties - Sensors (Ultrasonic)
        public double FrontDistance { get; set; }
        public double LeftDistance { get; set; }
        public double RightDistance { get; set; }
        public double BackDistance { get; set; }
        #endregion

        #region Properties - IMU
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public float Yaw { get; set; }
        #endregion

        #region Properties - Status
        public string CurrentCommand { get; set; }
        public string State { get; set; }  // Idle, Moving, Turning, Charging, Error
        public DateTime Timestamp { get; set; }
        public double WifiSignal { get; set; }
        public TimeSpan Uptime { get; set; }
        public string ErrorMessage { get; set; }
        #endregion

        #region Derived Properties
        public bool IsBatteryLow => this.BatteryPercent < 20;
        public bool IsBatteryCritical => this.BatteryPercent < 10;
        public bool IsObstacleNear => this.FrontDistance < 15 || this.BackDistance < 15;
        public string StatusColor => this.State switch
        {
            "Error" => "Red",
            "Charging" => "Yellow",
            "Moving" => "Green",
            _ => "Gray"
        };
        #endregion
    }
    #endregion
}