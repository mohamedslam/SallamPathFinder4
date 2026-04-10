#region File Header
/// <summary>
/// File: IPathAccuracyService.cs
/// Description: Interface for path accuracy monitoring service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Result Classes
    /// <summary>
    /// Path accuracy measurement result
    /// </summary>
    public sealed class PathAccuracyResult
    {
        /// <summary>Overall accuracy percentage (0-100%)</summary>
        public double OverallAccuracy { get; set; }

        /// <summary>Number of deviations from planned path</summary>
        public int DeviationCount { get; set; }

        /// <summary>Points where robot deviated</summary>
        public List<Point> DeviationPoints { get; set; }

        /// <summary>Average deviation distance in cells</summary>
        public double AverageDeviation { get; set; }

        /// <summary>Maximum deviation distance in cells</summary>
        public double MaxDeviation { get; set; }

        /// <summary>Status of path following</summary>
        public PathFollowingStatus Status { get; set; }

        /// <summary>Detailed message about accuracy</summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Path following status
    /// </summary>
    public enum PathFollowingStatus
    {
        /// <summary>Perfect - exactly on path</summary>
        Perfect,

        /// <summary>Good - slight deviations within tolerance</summary>
        Good,

        /// <summary>Warning - deviations exceeding acceptable limits</summary>
        Warning,

        /// <summary>Critical - major deviation, needs replanning</summary>
        Critical,

        /// <summary>Lost - completely off path</summary>
        Lost
    }

    /// <summary>
    /// Real-time tracking data for robot position
    /// </summary>
    public sealed class TrackingData
    {
        public DateTime Timestamp { get; set; }
        public Point PlannedPosition { get; set; }
        public Point ActualPosition { get; set; }
        public double DeviationDistance { get; set; }
        public double AngleError { get; set; }
    }
    #endregion

    #region Interface Documentation
    /// <summary>
    /// Service interface for monitoring path accuracy during robot movement
    /// Tracks deviations, calculates accuracy metrics, and triggers alerts
    /// </summary>
    #endregion
    public interface IPathAccuracyService
    {
        #region Methods
        /// <summary>
        /// Starts monitoring a new path
        /// </summary>
        void StartMonitoring(IReadOnlyList<PathNode> plannedPath, double toleranceCells = 0.5);

        /// <summary>
        /// Stops monitoring the current path
        /// </summary>
        void StopMonitoring();

        /// <summary>
        /// Updates robot position and checks accuracy
        /// </summary>
        Task<PathAccuracyResult> UpdatePositionAsync(Point actualPosition, float actualAngle);

        /// <summary>
        /// Gets the current path accuracy status
        /// </summary>
        PathAccuracyResult GetCurrentAccuracy();

        /// <summary>
        /// Gets the tracking history
        /// </summary>
        IReadOnlyList<TrackingData> GetTrackingHistory();

        /// <summary>
        /// Resets all tracking data
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets the deviation tolerance
        /// </summary>
        void SetTolerance(double toleranceCells);

        /// <summary>
        /// Checks if replanning is needed due to deviation
        /// </summary>
        bool IsReplanningNeeded();
        #endregion

        #region Events
        /// <summary>Event raised when deviation exceeds warning threshold</summary>
        event Action<PathAccuracyResult> DeviationWarning;

        /// <summary>Event raised when replanning is needed</summary>
        event Action<Point> ReplanningNeeded;

        /// <summary>Event raised when robot reaches the end of planned path</summary>
        event Action PathCompleted;
        #endregion
    }
}