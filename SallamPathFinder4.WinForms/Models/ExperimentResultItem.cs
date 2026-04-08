#region File Header
/// <summary>
/// File: ExperimentResultItem.cs
/// Description: Represents a single experiment result item
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System.Collections.Generic;
using System.Drawing;
#endregion

namespace SallamPathFinder4.WinForms.Models
{
    #region Class Documentation
    /// <summary>
    /// Represents a single experiment result item
    /// Used by frmExperimentDesigner, frmExperimentResults, and frmStatisticsViewer
    /// Following DRY principle - single source of truth
    /// </summary>
    #endregion
    public class ExperimentResultItem
    {
        #region Properties - Basic Info
        public string Algorithm { get; set; }
        public string Metric { get; set; }
        public int Iteration { get; set; }
        #endregion

        #region Properties - Path Metrics
        public int PathLength { get; set; }
        public double ComputationTimeMs { get; set; }
        public bool Success { get; set; }
        public double ReturnPathLength { get; set; }
        #endregion

        #region Properties - Battery Metrics
        public double RemainingBattery { get; set; }
        public double SuccessRate { get; set; }
        #endregion

        #region Properties - Collision and Error Metrics
        public int CollisionCount { get; set; }
        public int InvalidMoveCount { get; set; }
        public double AverageActualSpeed { get; set; }
        #endregion

        #region Properties - Screenshot Paths
        public string InitialScreenshotPath { get; set; }
        public string PathScreenshotPath { get; set; }
        public string CompletedScreenshotPath { get; set; }
        #endregion

        #region Properties - Additional Data
        public string ScreenshotPath { get; set; }  // For backward compatibility
        public List<Point> Path { get; set; }
        public string ErrorMessage { get; set; }
        #endregion
    }
}