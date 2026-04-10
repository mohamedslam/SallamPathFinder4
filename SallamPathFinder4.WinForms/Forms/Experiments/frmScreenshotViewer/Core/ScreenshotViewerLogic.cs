#region File Header
/// <summary>
/// File: ScreenshotViewerLogic.cs
/// Description: Business logic for screenshot viewer form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.WinForms.Models;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmScreenshotViewer.Core
{
    /// <summary>
    /// Business logic for finding and managing screenshot paths
    /// </summary>
    public sealed class ScreenshotViewerLogic
    {
        #region Properties
        /// <summary>
        /// Gets the path to the initial screenshot
        /// </summary>
        public string InitialPath { get; private set; }

        /// <summary>
        /// Gets the path to the path screenshot
        /// </summary>
        public string PathPath { get; private set; }

        /// <summary>
        /// Gets the path to the completed screenshot
        /// </summary>
        public string CompletedPath { get; private set; }
        #endregion

        #region Constructor
        public ScreenshotViewerLogic()
        {
            InitialPath = string.Empty;
            PathPath = string.Empty;
            CompletedPath = string.Empty;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Loads all screenshot paths for the given experiment
        /// </summary>
        public void LoadScreenshots(ExperimentResultItem result, string resultsFolderPath)
        {
            string basePath = Path.Combine(resultsFolderPath, "Screenshots", result.Algorithm);

            string initialPath = Path.Combine(basePath,
                $"{result.Algorithm}_{result.Metric}_Initial_Iter{result.Iteration}.png");

            string pathPath = Path.Combine(basePath,
                $"{result.Algorithm}_{result.Metric}_Path_Iter{result.Iteration}.png");

            string completedPath = Path.Combine(basePath,
                $"{result.Algorithm}_{result.Metric}_Completed_Iter{result.Iteration}.png");

            InitialPath = File.Exists(initialPath) ? initialPath : string.Empty;
            PathPath = File.Exists(pathPath) ? pathPath : string.Empty;
            CompletedPath = File.Exists(completedPath) ? completedPath : string.Empty;
        }
        #endregion
    }
}