#region File Header
/// <summary>
/// File: RobotDashboardLogic.cs
/// Description: Business logic for robot dashboard form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Text.RegularExpressions;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Dashboard.frmRobotDashboard.Core
{
    /// <summary>
    /// Business logic for robot dashboard operations
    /// </summary>
    public sealed class RobotDashboardLogic
    {
        #region Constants
        private const int MIN_PORT = 1;
        private const int MAX_PORT = 65535;
        private const string IP_PATTERN = @"^(\d{1,3}\.){3}\d{1,3}$";
        #endregion

        #region Constructor
        public RobotDashboardLogic()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Validates an IP address
        /// </summary>
        public bool IsValidIpAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                return false;

            if (!Regex.IsMatch(ip, IP_PATTERN))
                return false;

            var parts = ip.Split('.');
            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int value))
                    return false;
                if (value < 0 || value > 255)
                    return false;
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

        /// <summary>
        /// Gets battery status text based on level
        /// </summary>
        public string GetBatteryStatus(double level)
        {
            if (level <= 0)
                return "EMPTY - Robot stopped";
            if (level < 10)
                return "CRITICAL - Find charging station immediately";
            if (level < 20)
                return "LOW - Consider recharging soon";
            return "NORMAL - OK";
        }

        /// <summary>
        /// Gets battery color based on level
        /// </summary>
        public Color GetBatteryColor(double level)
        {
            if (level <= 10)
                return Color.FromArgb(231, 76, 60);
            if (level <= 20)
                return Color.FromArgb(241, 196, 15);
            return Color.FromArgb(46, 204, 113);
        }

        /// <summary>
        /// Formats sensor distance for display
        /// </summary>
        public string FormatSensorDistance(double distance)
        {
            if (distance <= 0)
                return "Obstacle!";
            if (distance < 0.5)
                return "Very Close!";
            if (distance < 1.0)
                return "Close";
            return $"{distance:F1} m";
        }
        #endregion
    }
}