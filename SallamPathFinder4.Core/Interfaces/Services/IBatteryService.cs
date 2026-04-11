#region File Header
/// <summary>
/// File: IBatteryService.cs
/// Description: Interface for battery management service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Interface Documentation
    /// <summary>
    /// Service interface for battery management
    /// Handles battery consumption, recharging, and change notifications
    /// </summary>
    #endregion
    public interface IBatteryService
    {
        #region Properties
        /// <summary>Current battery level (0-100 percent)</summary>
        double CurrentCharge { get; }

        /// <summary>Maximum battery capacity</summary>
        double Capacity { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Consumes battery based on distance, surface weight, and speed
        /// </summary>
        /// <param name="distance">Distance traveled in centimeters</param>
        /// <param name="surfaceWeight">Surface weight factor (1-100)</param>
        /// <param name="speed">Robot speed in cm/s</param>
        void Consume(double distance, double surfaceWeight, double speed);

        /// <summary>
        /// Recharges battery to full capacity
        /// </summary>
        void Recharge();

        /// <summary>
        /// Recharges battery by the specified amount
        /// </summary>
        /// <param name="amount">Amount to recharge (0-100)</param>
        void Recharge(double amount); 
        void SetCharge(double charge);

        /// <summary>
        /// Checks if the battery has enough charge for the required amount
        /// </summary>
        bool HasEnoughCharge(double required);

        /// <summary>
        /// Gets current battery percentage
        /// </summary>
        double GetPercentage();
        #endregion

        #region Methods - Battery Calculation for Charging

        /// <summary>
        /// Calculates the battery percentage needed to reach a target point
        /// </summary>
        /// <param name="distanceInCells">Distance from current position to target in cells</param>
        /// <param name="averageSurfaceWeight">Average surface weight along the path (1-100)</param>
        /// <param name="robotSpeed">Robot speed in cm/s</param>
        /// <returns>Battery percentage needed (0-100)</returns>
        double CalculateBatteryNeededForDistance(double distanceInCells, double averageSurfaceWeight, double robotSpeed);

        /// <summary>
        /// Checks if current battery can reach the nearest parking point
        /// </summary>
        /// <param name="distanceToParkingCells">Distance to nearest parking in cells</param>
        /// <param name="averageSurfaceWeight">Average surface weight along the path</param>
        /// <param name="robotSpeed">Robot speed in cm/s</param>
        /// <param name="safetyMarginPercent">Safety margin percentage (5-20%)</param>
        /// <returns>True if battery is sufficient to reach parking with safety margin</returns>
        bool CanReachParking(double distanceToParkingCells, double averageSurfaceWeight, double robotSpeed, double safetyMarginPercent);

        /// <summary>
        /// Sets battery charge to full (100%)
        /// </summary>
        void SetFullCharge();

        /// <summary>
        /// Formats battery level as percentage and battery count
        /// </summary>
        string FormatBatteryWithBatteries(double batteryPercent, double totalBatteries = 3.0);

        #endregion

        #region Events
        /// <summary>Event raised when battery level changes</summary>
        event Action<double> BatteryChanged;

   

        #endregion
    }
}