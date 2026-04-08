#region File Header
/// <summary>
/// File: BatteryPredictor.cs
/// Description: Predicts battery consumption and determines reachable points
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Helpers;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Path;
#endregion

namespace SallamPathFinder4.Services.Battery
{
    #region Class Documentation
    /// <summary>
    /// Service for predicting battery consumption and finding reachable points
    /// Uses path characteristics to estimate battery usage
    /// </summary>
    #endregion
    public sealed class BatteryPredictor : IBatteryPredictor
    {
        #region Constants
        private const double BASE_CONSUMPTION_PER_CELL = 1.0;
        private const double CONFIDENCE_FACTOR = 0.9;
        private const double LOW_BATTERY_THRESHOLD = 20.0;
        private const double CRITICAL_BATTERY_THRESHOLD = 10.0;
        #endregion

        #region Private Fields
        private readonly Random _random = new Random();
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public async Task<BatteryPredictionResult> PredictAsync(BatteryPredictionRequest request)
        {
            if (request?.Path == null || request.Path.Count == 0)
            {
                return new BatteryPredictionResult
                {
                    CanComplete = false,
                    EstimatedRemainingBattery = 0,
                    EstimatedConsumption = 0,
                    ExpectedStopPoint = null,
                    ExpectedStopIndex = -1,
                    WarningMessage = "Invalid path for prediction",
                    Confidence = 0
                };
            }

            return await Task.Run(() =>
            {
                double totalConsumption = 0;
                double cumulativeConsumption = 0;
                Point? stopPoint = null;
                int stopIndex = -1;

                for (int i = 0; i < request.Path.Count; i++)
                {
                    double surfaceFactor = (request.SurfaceWeights != null && i < request.SurfaceWeights.Count)
                        ? request.SurfaceWeights[i] / 100.0
                        : 0.5;

                    double rampFactor = (request.RampDifficulties != null && i < request.RampDifficulties.Count)
                        ? 1.0 + (request.RampDifficulties[i] / 100.0)
                        : 1.0;

                    double speedFactor = Math.Max(0.5, Math.Min(2.0, request.RobotSpeed / 10.0));

                    double stepConsumption = BASE_CONSUMPTION_PER_CELL * surfaceFactor * rampFactor * speedFactor;
                    cumulativeConsumption += stepConsumption;

                    if (stopPoint == null && cumulativeConsumption > request.CurrentBattery)
                    {
                        stopPoint = new Point(request.Path[i].X, request.Path[i].Y);
                        stopIndex = i;
                    }

                    totalConsumption += stepConsumption;
                }

                double estimatedRemaining = request.CurrentBattery - totalConsumption;
                bool canComplete = estimatedRemaining >= 0;

                return new BatteryPredictionResult
                {
                    CanComplete = canComplete,
                    EstimatedRemainingBattery = Math.Max(0, estimatedRemaining),
                    EstimatedConsumption = totalConsumption,
                    ExpectedStopPoint = canComplete ? null : stopPoint,
                    ExpectedStopIndex = canComplete ? -1 : stopIndex,
                    WarningMessage = GetWarningMessage(canComplete, estimatedRemaining),
                    Confidence = CONFIDENCE_FACTOR * 100
                };
            });
        }

        /// <inheritdoc/>
        public async Task<Point?> FindNearestReachableParkingAsync(Point currentPosition,
            IReadOnlyList<Point> parkingPoints, double currentBattery, double robotSpeed)
        {
            if (parkingPoints == null || parkingPoints.Count == 0)
            {
                return null;
            }

            var result = await Task.Run(() =>
            {
                Point? nearestParking = null;
                double minDistance = double.MaxValue;

                foreach (var parking in parkingPoints)
                {
                    int distance = MathHelper.ManhattanDistance(currentPosition, parking);
                    double estimatedConsumption = distance * (BASE_CONSUMPTION_PER_CELL * (robotSpeed / 10.0));

                    if (estimatedConsumption <= currentBattery && distance < minDistance)
                    {
                        minDistance = distance;
                        nearestParking = parking;
                    }
                }

                return nearestParking;
            });

            return result;
        }

        /// <inheritdoc/>
        public async Task<double> CalculateMaxRangeAsync(double currentBattery, double robotSpeed,
            double averageSurfaceWeight)
        {
            return await Task.Run(() =>
            {
                double surfaceFactor = averageSurfaceWeight / 100.0;
                double speedFactor = Math.Max(0.5, Math.Min(2.0, robotSpeed / 10.0));
                double consumptionPerCell = BASE_CONSUMPTION_PER_CELL * surfaceFactor * speedFactor;

                if (consumptionPerCell <= 0) return 0.0;

                return currentBattery / consumptionPerCell;
            });
        }

        /// <inheritdoc/>
        public BatteryRecommendation GetRecommendation(double batteryLevel)
        {
            if (batteryLevel <= 0)
                return BatteryRecommendation.StopRobot;

            if (batteryLevel < CRITICAL_BATTERY_THRESHOLD)
                return BatteryRecommendation.ReturnImmediately;

            if (batteryLevel < LOW_BATTERY_THRESHOLD)
                return BatteryRecommendation.CompleteAndReturn;

            return BatteryRecommendation.ContinueMission;
        }
        #endregion

        #region Private Methods
        private static string GetWarningMessage(bool canComplete, double remainingBattery)
        {
            if (canComplete)
            {
                if (remainingBattery < CRITICAL_BATTERY_THRESHOLD)
                    return "WARNING: Very low battery after completion!";
                if (remainingBattery < LOW_BATTERY_THRESHOLD)
                    return "Caution: Low battery after completion";
                return "OK: Sufficient battery for entire path";
            }

            return "WARNING: Insufficient battery to complete path!";
        }
        #endregion
    }
}