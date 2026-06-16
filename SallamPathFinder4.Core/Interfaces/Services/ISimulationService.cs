#region File Header
/// <summary>
/// File: ISimulationService.cs
/// Description: Interface for robot simulation service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region ObstacleData Class
    /// <summary>
    /// Data structure for obstacle detection events
    /// </summary>
    public sealed class ObstacleData
    {
        public ObstacleType Type { get; set; }
        public Point Location { get; set; }
        public DateTime Timestamp { get; set; }
        public double Distance { get; set; }
    }
    #endregion

    #region Interface Documentation
    /// <summary>
    /// Service interface for robot simulation
    /// Handles robot movement along paths, obstacle detection, and dynamic obstacle updates
    /// </summary>
    #endregion
    public interface ISimulationService
    {
        #region Control Methods
        /// <summary>Starts simulation along the specified path</summary>
        void Start(IReadOnlyList<PathNode> path);

        /// <summary>Pauses the simulation</summary>
        void Pause();

        /// <summary>Stops the simulation and resets position</summary>
        void Stop();

        /// <summary>Moves robot manually using keyboard controls</summary>
        void MoveRobotManually(RobotCommand command, int stepSize = 1, float rotationAngle = 15f);
        #endregion

        #region Properties
        /// <summary>Indicates whether simulation is running</summary>
        bool IsRunning { get; }

        /// <summary>Indicates whether simulation is paused</summary>
        bool IsPaused { get; }

        /// <summary>Current robot position</summary>
        Point CurrentRobotPosition { get; }

        /// <summary>Current robot angle in degrees</summary>
        float CurrentRobotAngle { get; }
        #endregion

        #region Detection Methods
        /// <summary>Sets robot detection parameters for SPPA-DL</summary>
        void SetDetectionParameters(double viewAngleDegrees, int rangeCells, bool enabled);

        void Resume();

        /// <summary>
        /// Sets the goal positions for goal reached detection
        /// </summary>
        void SetGoals(List<Point> goals);

        /// <summary>Gets cells within detection zone for visualization</summary>
        List<Point> GetDetectionZoneCells(Point robotPos, float robotAngle);
        void SetRobotSpeedFromSettings(double speedCmPerSec);
        void SetRobotDimensions(double widthCm, double lengthCm); 
        #endregion

        #region Events
        /// <summary>Event raised when robot moves to a new cell</summary>
        event Action< Point, float,double > RobotMoved;

        /// <summary>Event raised when robot collides with an obstacle</summary>
        event Action<ObstacleData, Point> ObstacleCollision;

        /// <summary>Event raised when obstacle is detected within range</summary>
        event Action<Point, ObstacleType, double> ObstacleDetected;

        /// <summary>Event raised when battery level changes</summary>
        event Action<double> BatteryChanged;
          
        /// <summary>
        /// Event raised when battery reaches zero
        /// </summary>
        event Action BatteryEmpty;

        /// <summary>
        /// Event raised when robot reaches a goal point
        /// </summary>
        event Action<int> GoalReached;  // int = goal index

        // ========== NEW: Obstacle Detection Methods ==========

        /// <summary>
        /// Sets whether learning is enabled
        /// </summary>
        void SetLearningEnabled(bool enabled);

        /// <summary>
        /// Sets wait times for a specific obstacle type
        /// </summary>
        void SetObstacleWaitTime(ObstacleType type, double waitTimeSeconds, double maxWaitTimeSeconds);

        /// <summary>
        /// Sets safe and critical distances for obstacle detection
        /// </summary>
        void SetSafetyDistances(double safeDistanceCm, double criticalDistanceCm);

        /// <summary>
        /// Exports obstacle log to file
        /// </summary>
        Task ExportObstacleLogAsync(string filePath = null);

        /// <summary>
        /// Clears learning memory
        /// </summary>
        void ClearLearningMemory();

        /// <summary>
        /// Clears obstacle log
        /// </summary>
        void ClearObstacleLog();

        /// <summary>
        /// Gets obstacle log statistics
        /// </summary>
        ObstacleLogStatistics GetObstacleLogStatistics();

        #endregion

        // ==========  Get Active Obstacles ==========

        /// <summary>
        /// Gets the list of currently active detected obstacles
        /// </summary>
        IReadOnlyList<DetectedObstacle> GetActiveObstacles();

        /// <summary>
        /// Gets hotspot risk levels for all cells
        /// </summary>
        Dictionary<Point, double> GetHotspotRiskLevels();

        /// <summary>
        /// Gets the current wait state (if robot is waiting for an obstacle)
        /// </summary>
        ObstacleWaitState GetCurrentWaitState();
    }
}