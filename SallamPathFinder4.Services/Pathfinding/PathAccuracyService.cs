#region File Header
/// <summary>
/// File: PathAccuracyService.cs
/// Description: Monitors robot path accuracy during movement
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Helpers;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Path;
#endregion

namespace SallamPathFinder4.Services.Pathfinding
{
    #region Class Documentation
    /// <summary>
    /// Service for monitoring path accuracy during robot movement
    /// Tracks deviations and triggers events when thresholds are exceeded
    /// </summary>
    #endregion
    public sealed class PathAccuracyService : IPathAccuracyService
    {
        #region Constants
        private const double DEFAULT_TOLERANCE_CELLS = 0.5;
        private const double WARNING_THRESHOLD = 1.0;
        private const double CRITICAL_THRESHOLD = 2.0;
        private const double LOST_THRESHOLD = 3.0;
        #endregion

        #region Private Fields
        private IReadOnlyList<PathNode> _plannedPath;
        private double _toleranceCells;
        private bool _isMonitoring;
        private readonly List<TrackingData> _trackingHistory;
        private int _currentTargetIndex;
        #endregion

        #region Constructor
        public PathAccuracyService()
        {
            _trackingHistory = new List<TrackingData>();
            _toleranceCells = DEFAULT_TOLERANCE_CELLS;
            _isMonitoring = false;
            _currentTargetIndex = 0;
        }
        #endregion

        #region Events
        public event Action<PathAccuracyResult> DeviationWarning;
        public event Action<Point> ReplanningNeeded;
        public event Action PathCompleted;
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public void StartMonitoring(IReadOnlyList<PathNode> plannedPath, double toleranceCells = 0.5)
        {
            if (plannedPath == null || plannedPath.Count == 0)
                throw new ArgumentException("Path cannot be null or empty", nameof(plannedPath));

            _plannedPath = plannedPath;
            _toleranceCells = toleranceCells;
            _isMonitoring = true;
            _currentTargetIndex = 0;
            _trackingHistory.Clear();
        }

        /// <inheritdoc/>
        public void StopMonitoring()
        {
            _isMonitoring = false;
            _plannedPath = null;
            _currentTargetIndex = 0;
        }

        /// <inheritdoc/>
        public async Task<PathAccuracyResult> UpdatePositionAsync(Point actualPosition, float actualAngle)
        {
            return await Task.Run(() =>
            {
                if (!_isMonitoring || _plannedPath == null)
                {
                    return new PathAccuracyResult
                    {
                        Status = PathFollowingStatus.Lost,
                        Message = "Not monitoring any path"
                    };
                }

                if (_currentTargetIndex >= _plannedPath.Count)
                {
                    PathCompleted?.Invoke();
                    return new PathAccuracyResult
                    {
                        Status = PathFollowingStatus.Perfect,
                        Message = "Path completed successfully"
                    };
                }

                var target = _plannedPath[_currentTargetIndex];
                double deviation = MathHelper.EuclideanDistance(actualPosition, new Point(target.X, target.Y));

                // Check if reached current target
                if (deviation <= _toleranceCells)
                {
                    _currentTargetIndex++;
                }

                // Record tracking data
                var trackingData = new TrackingData
                {
                    Timestamp = DateTime.UtcNow,
                    PlannedPosition = new Point(target.X, target.Y),
                    ActualPosition = actualPosition,
                    DeviationDistance = deviation,
                    AngleError = 0 // Would need angle comparison
                };
                _trackingHistory.Add(trackingData);

                // Calculate overall accuracy
                var status = DetermineStatus(deviation);
                var result = new PathAccuracyResult
                {
                    OverallAccuracy = CalculateOverallAccuracy(),
                    DeviationCount = _trackingHistory.Count,
                    DeviationPoints = _trackingHistory.Select(t => t.ActualPosition).ToList(),
                    AverageDeviation = _trackingHistory.Average(t => t.DeviationDistance),
                    MaxDeviation = _trackingHistory.Max(t => t.DeviationDistance),
                    Status = status,
                    Message = GetStatusMessage(status, deviation)
                };

                // Trigger events based on status
                if (status == PathFollowingStatus.Warning || status == PathFollowingStatus.Critical)
                {
                    DeviationWarning?.Invoke(result);
                }

                if (status == PathFollowingStatus.Critical || status == PathFollowingStatus.Lost)
                {
                    ReplanningNeeded?.Invoke(actualPosition);
                }

                if (_currentTargetIndex >= _plannedPath.Count)
                {
                    PathCompleted?.Invoke();
                }

                return result;
            });
        }

        /// <inheritdoc/>
        public PathAccuracyResult GetCurrentAccuracy()
        {
            if (!_isMonitoring || _trackingHistory.Count == 0)
            {
                return new PathAccuracyResult
                {
                    Status = PathFollowingStatus.Lost,
                    Message = "No tracking data available"
                };
            }

            return new PathAccuracyResult
            {
                OverallAccuracy = CalculateOverallAccuracy(),
                DeviationCount = _trackingHistory.Count,
                DeviationPoints = _trackingHistory.Select(t => t.ActualPosition).ToList(),
                AverageDeviation = _trackingHistory.Average(t => t.DeviationDistance),
                MaxDeviation = _trackingHistory.Max(t => t.DeviationDistance),
                Status = DetermineStatus(_trackingHistory.Last().DeviationDistance),
                Message = $"Tracking {_trackingHistory.Count} positions"
            };
        }

        /// <inheritdoc/>
        public IReadOnlyList<TrackingData> GetTrackingHistory()
        {
            return _trackingHistory.AsReadOnly();
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _trackingHistory.Clear();
            _currentTargetIndex = 0;
            _plannedPath = null;
            _isMonitoring = false;
        }

        /// <inheritdoc/>
        public void SetTolerance(double toleranceCells)
        {
            _toleranceCells = Math.Max(0.1, Math.Min(5.0, toleranceCells));
        }

        /// <inheritdoc/>
        public bool IsReplanningNeeded()
        {
            if (!_isMonitoring || _trackingHistory.Count == 0)
                return false;

            var lastDeviation = _trackingHistory.Last().DeviationDistance;
            return lastDeviation >= CRITICAL_THRESHOLD;
        }
        #endregion

        #region Private Methods
        private double CalculateOverallAccuracy()
        {
            if (_trackingHistory.Count == 0) return 100;

            double avgDeviation = _trackingHistory.Average(t => t.DeviationDistance);
            double accuracy = 100 * (1 - Math.Min(1, avgDeviation / LOST_THRESHOLD));
            return Math.Max(0, accuracy);
        }

        private static PathFollowingStatus DetermineStatus(double deviation)
        {
            if (deviation <= DEFAULT_TOLERANCE_CELLS)
                return PathFollowingStatus.Perfect;

            if (deviation <= WARNING_THRESHOLD)
                return PathFollowingStatus.Good;

            if (deviation <= CRITICAL_THRESHOLD)
                return PathFollowingStatus.Warning;

            if (deviation <= LOST_THRESHOLD)
                return PathFollowingStatus.Critical;

            return PathFollowingStatus.Lost;
        }

        private static string GetStatusMessage(PathFollowingStatus status, double deviation)
        {
            return status switch
            {
                PathFollowingStatus.Perfect => $"Perfect path following (deviation: {deviation:F2})",
                PathFollowingStatus.Good => $"Good path following (deviation: {deviation:F2})",
                PathFollowingStatus.Warning => $"Warning: Path deviation detected ({deviation:F2})",
                PathFollowingStatus.Critical => $"Critical: Major deviation ({deviation:F2}) - Replanning needed",
                PathFollowingStatus.Lost => $"Lost: Robot off path ({deviation:F2})",
                _ => "Unknown status"
            };
        }
        #endregion
    }
}