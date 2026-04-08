#region File Header
/// <summary>
/// File: RobotProfile.cs
/// Description: Represents a complete robot profile with settings and metadata
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Models.Robot
{
    #region Class Documentation
    /// <summary>
    /// Complete robot profile including settings, state, and metadata
    /// Used for multi-robot management
    /// </summary>
    #endregion
    public sealed class RobotProfile
    {
        #region Constructor
        public RobotProfile()
        {
            Id = Guid.NewGuid().ToString().Substring(0, 8);
            Name = "New Robot";
            Settings = new RobotSettings();
            CreatedAt = DateTime.UtcNow;
            LastUsed = DateTime.UtcNow;
            IsFavorite = false;
            Description = string.Empty;
            Color = Color.FromArgb(52, 73, 94);
            TotalDistanceTraveledCm = 0;
            TotalSimulationsRun = 0;
            SuccessRate = 0;
        }
        #endregion

        #region Properties - Identification
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }
        #endregion

        #region Properties - Settings
        public RobotSettings Settings { get; set; }
        #endregion

        #region Properties - Metadata
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsed { get; set; }
        public bool IsFavorite { get; set; }
        #endregion

        #region Properties - Statistics
        public double TotalDistanceTraveledCm { get; set; }
        public int TotalSimulationsRun { get; set; }
        public double SuccessRate { get; set; }
        #endregion

        #region Public Methods
        public RobotProfile Clone()
        {
            return new RobotProfile
            {
                Id = this.Id,
                Name = this.Name,
                Description = this.Description,
                Color = this.Color,
                Settings = this.Settings.Clone(),
                CreatedAt = this.CreatedAt,
                LastUsed = this.LastUsed,
                IsFavorite = this.IsFavorite,
                TotalDistanceTraveledCm = this.TotalDistanceTraveledCm,
                TotalSimulationsRun = this.TotalSimulationsRun,
                SuccessRate = this.SuccessRate
            };
        }

        public void UpdateStatistics(double distance, bool success)
        {
            TotalDistanceTraveledCm += distance;
            TotalSimulationsRun++;

            if (TotalSimulationsRun > 0)
            {
                double currentSuccessCount = (SuccessRate / 100) * (TotalSimulationsRun - 1);
                double newSuccessCount = currentSuccessCount + (success ? 1 : 0);
                SuccessRate = (newSuccessCount / TotalSimulationsRun) * 100;
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Id}) - Last used: {LastUsed:yyyy-MM-dd}";
        }
        #endregion
    }
}