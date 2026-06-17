#region File Header
/// <summary>
/// File: frmRobotDashboardLogic.cs
/// Description: Business logic for robot dashboard form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard.Core
{
    #region Class Documentation
    /// <summary>
    /// Business logic for robot dashboard operations
    /// </summary>
    #endregion
    public sealed class RobotDashboardLogic
    {
        #region Constants
        private const int MIN_PORT = 1;
        private const int MAX_PORT = 65535;
        private const string IP_PATTERN = @"^(\d{1,3}\.){3}\d{1,3}$";
        private const double LOW_BATTERY_THRESHOLD = 20.0;
        private const double CRITICAL_BATTERY_THRESHOLD = 10.0;
        private const double TOTAL_BATTERIES = 3.0;
        #endregion

        #region Constructor
        public RobotDashboardLogic()
        {
        }
        #endregion

        #region Public Methods - Validation
        /// <summary>
        /// Validates an IP address
        /// </summary>
        public bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
            {
                return false;
            }

            if (!Regex.IsMatch(ip, IP_PATTERN))
            {
                return false;
            }

            var parts = ip.Split('.');
            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int value))
                {
                    return false;
                }

                if (value < 0 || value > 255)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates a port number
        /// </summary>
        public bool IsValidPort(int port)
        {
            return port >= MIN_PORT && port <= MAX_PORT;
        }
        #endregion

        #region Public Methods - Battery
        /// <summary>
        /// Gets battery status text based on level
        /// </summary>
        public string GetBatteryStatus(double level)
        {
            if (level <= 0)
            {
                return "EMPTY - Robot stopped";
            }

            if (level < CRITICAL_BATTERY_THRESHOLD)
            {
                return "CRITICAL - Find charging station immediately";
            }

            if (level < LOW_BATTERY_THRESHOLD)
            {
                return "LOW - Consider recharging soon";
            }

            return "NORMAL - OK";
        }

        /// <summary>
        /// Gets battery color based on level
        /// </summary>
        public Color GetBatteryColor(double level)
        {
            if (level <= CRITICAL_BATTERY_THRESHOLD)
            {
                return Color.FromArgb(231, 76, 60);
            }

            if (level <= LOW_BATTERY_THRESHOLD)
            {
                return Color.FromArgb(241, 196, 15);
            }

            return Color.FromArgb(46, 204, 113);
        }

        /// <summary>
        /// Formats battery with battery count
        /// </summary>
        public string FormatBatteryWithBatteries(double batteryPercent)
        {
            double batteriesLeft = (batteryPercent / 100.0) * TOTAL_BATTERIES;
            return $"{batteryPercent:F1}% ({batteriesLeft:F1}/{TOTAL_BATTERIES:F0} batteries)";
        }
        #endregion

        #region Public Methods - Sensors
        /// <summary>
        /// Formats sensor distance for display
        /// </summary>
        public string FormatSensorDistance(double distance)
        {
            if (distance <= 0)
            {
                return "Obstacle!";
            }

            if (distance < 0.5)
            {
                return "Very Close!";
            }

            if (distance < 1.0)
            {
                return "Close";
            }

            return $"{distance:F1} m";
        }

        /// <summary>
        /// Gets sensor color based on distance
        /// </summary>
        public Color GetSensorColor(double distance)
        {
            if (distance <= 0.3)
            {
                return Color.FromArgb(231, 76, 60);
            }

            if (distance <= 0.8)
            {
                return Color.FromArgb(241, 196, 15);
            }

            return Color.FromArgb(46, 204, 113);
        }
        #endregion

        #region Public Methods - Telemetry
        /// <summary>
        /// Updates UI with telemetry data
        /// </summary>
        public void UpdateTelemetryDisplay(RobotTelemetry telemetry,
            Label lblBattery, Label lblMotorTemp, Label lblCpuLoad,
            Label lblWifiSignal, Label lblUptime)
        {
            if (telemetry == null)
            {
                return;
            }

            if (lblBattery != null)
            {
                lblBattery.Text = FormatBatteryWithBatteries(telemetry.BatteryPercent);
                lblBattery.ForeColor = GetBatteryColor(telemetry.BatteryPercent);
            }

            if (lblMotorTemp != null)
            {
                lblMotorTemp.Text = $"Motor: {telemetry.MotorTemp:F0}°C";
                lblMotorTemp.ForeColor = telemetry.MotorTemp > 60 ? Color.Red : Color.White;
            }

            if (lblCpuLoad != null)
            {
                lblCpuLoad.Text = $"CPU: {telemetry.CpuLoad:F0}%";
                lblCpuLoad.ForeColor = telemetry.CpuLoad > 80 ? Color.Red : Color.White;
            }

            if (lblWifiSignal != null)
            {
                lblWifiSignal.Text = $"WiFi: {telemetry.WifiSignal:F0} dBm";
                lblWifiSignal.ForeColor = telemetry.WifiSignal < -70 ? Color.Orange : Color.White;
            }

            if (lblUptime != null)
            {
                lblUptime.Text = $"Uptime: {telemetry.Uptime:hh\\:mm\\:ss}";
            }
        }

        /// <summary>
        /// Updates position display
        /// </summary>
        public void UpdatePositionDisplay(Point position, float angle,
            Label lblPosition, Label lblAngle)
        {
            if (lblPosition != null)
            {
                lblPosition.Text = $"Position: ({position.X}, {position.Y})";
            }

            if (lblAngle != null)
            {
                lblAngle.Text = $"Angle: {angle}°";
            }
        }
        #endregion
    }
}