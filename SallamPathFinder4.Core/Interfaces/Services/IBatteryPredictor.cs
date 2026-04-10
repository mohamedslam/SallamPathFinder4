#region File Header
/// <summary>
/// File: IBatteryPredictor.cs
/// Description: Interface for battery prediction service
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
    /// Result of battery prediction for a path
    /// </summary>
    public sealed class BatteryPredictionResult
    {
        /// <summary>Whether the robot can complete the path</summary>
        public bool CanComplete { get; set; }

        /// <summary>Estimated battery remaining after completing the path (%)</summary>
        public double EstimatedRemainingBattery { get; set; }

        /// <summary>Estimated battery consumption for the path (%)</summary>
        public double EstimatedConsumption { get; set; }

        /// <summary>Point where battery is expected to run out (if applicable)</summary>
        public Point? ExpectedStopPoint { get; set; }

        /// <summary>Index in path where battery is expected to run out</summary>
        public int ExpectedStopIndex { get; set; }

        /// <summary>Warning message if battery is insufficient</summary>
        public string WarningMessage { get; set; }

        /// <summary>Confidence level of the prediction (0-100%)</summary>
        public double Confidence { get; set; }
    }

    /// <summary>
    /// Request parameters for battery prediction
    /// </summary>
    public sealed class BatteryPredictionRequest
    {
        /// <summary>Current battery level (%)</summary>
        public double CurrentBattery { get; set; }

        /// <summary>Robot speed (cm/s)</summary>
        public double RobotSpeed { get; set; }

        /// <summary>Path to evaluate</summary>
        public IReadOnlyList<PathNode> Path { get; set; }

        /// <summary>Surface weights for each cell in the path</summary>
        public IReadOnlyList<byte> SurfaceWeights { get; set; }

        /// <summary>Ramp difficulties for each cell (0-100)</summary>
        public IReadOnlyList<byte> RampDifficulties { get; set; }
    }
    #endregion

    #region Interface Documentation
    /// <summary>
    /// Service interface for battery consumption prediction
    /// Predicts whether robot can complete a path and where battery will run out
    /// </summary>
    #endregion
    public interface IBatteryPredictor
    {
        #region Methods
        /// <summary>
        /// Predicts battery consumption for a given path
        /// </summary>
        Task<BatteryPredictionResult> PredictAsync(BatteryPredictionRequest request);

        /// <summary>
        /// Finds the nearest parking point reachable with current battery
        /// </summary>
        Task<Point?> FindNearestReachableParkingAsync(Point currentPosition,
            IReadOnlyList<Point> parkingPoints, double currentBattery, double robotSpeed);

        /// <summary>
        /// Calculates the maximum distance the robot can travel with current battery
        /// </summary>
        Task<double> CalculateMaxRangeAsync(double currentBattery, double robotSpeed,
            double averageSurfaceWeight);

        /// <summary>
        /// Gets the recommended action based on battery level
        /// </summary>
        BatteryRecommendation GetRecommendation(double batteryLevel);
        #endregion
    }

    #region Enum
    /// <summary>
    /// Recommended action based on battery level
    /// </summary>
    public enum BatteryRecommendation
    {
        /// <summary>Normal operation - continue mission</summary>
        ContinueMission,

        /// <summary>Complete current goals and return to parking</summary>
        CompleteAndReturn,

        /// <summary>Abandon mission and return to parking immediately</summary>
        ReturnImmediately,

        /// <summary>Stop robot - battery empty</summary>
        StopRobot
    }
    #endregion
}