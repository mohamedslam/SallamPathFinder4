#region File Header
/// <summary>
/// File: IBatteryService.cs
/// Description: Interface for battery management service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
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

        #region Events
        /// <summary>Event raised when battery level changes</summary>
        event Action<double> BatteryChanged;

   

        #endregion
    }
}