#region File Header
/// <summary>
/// File: ObstacleWaitState.cs
/// Description: Represents a waiting state for an obstacle
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-02
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Models.Obstacles
{
    /// <summary>
    /// Wait state for an active obstacle
    /// </summary>
    public sealed class ObstacleWaitState
    {
        #region Properties
        /// <summary>
        /// Unique identifier for the obstacle
        /// </summary>
        public string ObstacleId { get; set; }

        /// <summary>
        /// Grid location of the obstacle
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// Type of obstacle
        /// </summary>
        public ObstacleType Type { get; set; }

        /// <summary>
        /// Time when waiting started (UTC)
        /// </summary>
        public DateTime WaitStartTime { get; set; }

        /// <summary>
        /// Total wait time in seconds
        /// </summary>
        public double TotalWaitTimeSeconds { get; set; }

        /// <summary>
        /// Maximum wait time before giving up (seconds)
        /// </summary>
        public double MaxWaitTimeSeconds { get; set; }

        /// <summary>
        /// Whether the robot is currently waiting
        /// </summary>
        public bool IsWaiting => (DateTime.UtcNow - WaitStartTime).TotalSeconds < TotalWaitTimeSeconds;

        /// <summary>
        /// Remaining wait time in seconds
        /// </summary>
        public double RemainingWaitTime => Math.Max(0, TotalWaitTimeSeconds - (DateTime.UtcNow - WaitStartTime).TotalSeconds);

        /// <summary>
        /// Whether the robot should give up waiting
        /// </summary>
        public bool ShouldGiveUp => (DateTime.UtcNow - WaitStartTime).TotalSeconds > MaxWaitTimeSeconds;
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts waiting for the obstacle
        /// </summary>
        public void StartWaiting(double waitTimeSeconds, double maxWaitTimeSeconds)
        {
            WaitStartTime = DateTime.UtcNow;
            TotalWaitTimeSeconds = waitTimeSeconds;
            MaxWaitTimeSeconds = maxWaitTimeSeconds;
        }

        /// <summary>
        /// Returns string representation
        /// </summary>
        public override string ToString()
        {
            return $"{Type} at ({Location.X},{Location.Y}) - Waiting: {RemainingWaitTime:F1}s / {TotalWaitTimeSeconds:F1}s";
        }
        #endregion
    }
}