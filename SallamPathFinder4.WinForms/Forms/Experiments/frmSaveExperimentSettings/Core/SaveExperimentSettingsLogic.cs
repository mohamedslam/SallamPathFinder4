#region File Header
/// <summary>
/// File: SaveExperimentSettingsLogic.cs
/// Description: Business logic for saving experiment settings to JSON file
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using SallamPathFinder4.WinForms.Forms.Shared;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmSaveExperimentSettings.Core
{
    /// <summary>
    /// Business logic for saving experiment settings to JSON configuration files
    /// </summary>
    public sealed class SaveExperimentSettingsLogic
    {
        #region Constants
        private const string SETTINGS_DIRECTORY = "ExperimentSettings";
        private const string FILE_EXTENSION = ".expcfg";
        #endregion

        #region Private Fields
        private readonly ExperimentSettings _settings;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the save logic
        /// </summary>
        /// <param name="settings">Current experiment settings to save</param>
        public SaveExperimentSettingsLogic(ExperimentSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the default file name based on experiment name and current date/time
        /// </summary>
        public string GetDefaultFileName()
        {
            string experimentName = _settings.ExperimentName ?? "Experiment";
            string safeName = new string(experimentName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)).ToArray());
            return $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}";
        }

        /// <summary>
        /// Validates the file name
        /// </summary>
        public bool ValidateFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            // Check for invalid characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return !fileName.Any(c => invalidChars.Contains(c));
        }

        /// <summary>
        /// Saves the settings to a JSON file
        /// </summary>
        public bool SaveToFile(string fileName, string description)
        {
            try
            {
                string settingsDir = GetSettingsDirectory();
                string filePath = Path.Combine(settingsDir, $"{fileName}{FILE_EXTENSION}");

                var saveData = new
                {
                    _settings.ExperimentName,
                    _settings.SelectedAlgorithms,
                    _settings.SelectedMetrics,
                    _settings.GoalCount,
                    _settings.ParkingCount,
                    _settings.StaticObstacles,
                    _settings.DynamicObstacles,
                    _settings.HeuristicWeight,
                    _settings.SearchLimit,
                    _settings.AllowDiagonals,
                    _settings.HeavyDiagonals,
                    _settings.Iterations,
                    _settings.SaveScreenshots,
                    _settings.SaveReplay,
                    _settings.ShowPathOnScreenshots,
                    Description = description,
                    SaveDate = DateTime.Now
                };

                string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the settings directory path, creating it if it doesn't exist
        /// </summary>
        private static string GetSettingsDirectory()
        {
            string settingsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_DIRECTORY);

            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            return settingsDir;
        }
        #endregion
    }
}