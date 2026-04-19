#region File Header
/// <summary>
/// File: LoadExperimentSettingsLogic.cs
/// Description: Business logic for loading experiment settings from JSON files
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.WinForms.Forms.Shared;
using System.Text.Json;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmLoadExperimentSettings.Core
{
    /// <summary>
    /// Represents a saved settings file with metadata
    /// </summary>
    public sealed class SavedSettingsFile
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime SaveDate { get; set; }
    }

    /// <summary>
    /// Business logic for loading experiment settings from JSON configuration files
    /// </summary>
    public sealed class LoadExperimentSettingsLogic
    {
        #region Constants
        private const string SETTINGS_DIRECTORY = "ExperimentSettings";
        private const string FILE_EXTENSION = ".expcfg";
        #endregion

        #region Private Fields
        private List<SavedSettingsFile> _cachedFiles;
        #endregion

        #region Constructor
        public LoadExperimentSettingsLogic()
        {
            _cachedFiles = new List<SavedSettingsFile>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets all saved settings files
        /// </summary>
        public List<SavedSettingsFile> GetAllSettingsFiles()
        {
            _cachedFiles.Clear();
            string settingsDir = GetSettingsDirectory();

            if (!Directory.Exists(settingsDir))
            {
                return _cachedFiles;
            }

            var files = Directory.GetFiles(settingsDir, $"*{FILE_EXTENSION}");

            foreach (var file in files)
            {
                try
                {
                    string json = File.ReadAllText(file);
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    string name = Path.GetFileNameWithoutExtension(file);
                    string description = data?.ContainsKey("Description") == true ? data["Description"]?.ToString() : "";
                    DateTime saveDate = data?.ContainsKey("SaveDate") == true && DateTime.TryParse(data["SaveDate"]?.ToString(), out var date)
                        ? date
                        : File.GetLastWriteTime(file);

                    _cachedFiles.Add(new SavedSettingsFile
                    {
                        FilePath = file,
                        Name = name,
                        Description = description,
                        SaveDate = saveDate
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading {file}: {ex.Message}");
                }
            }

            _cachedFiles = _cachedFiles.OrderByDescending(f => f.SaveDate).ToList();
            return _cachedFiles;
        }

        /// <summary>
        /// Gets the cached list of settings files
        /// </summary>
        public List<SavedSettingsFile> GetSettingsFiles()
        {
            return _cachedFiles;
        }

        /// <summary>
        /// Gets preview text for a settings file
        /// </summary>
        public string GetPreviewText(SavedSettingsFile file)
        {
            if (file == null) return string.Empty;

            try
            {
                string json = File.ReadAllText(file.FilePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                string preview = $"Name: {file.Name}\n";
                preview += $"Date: {file.SaveDate:yyyy-MM-dd HH:mm:ss}\n";
                preview += $"Description: {file.Description}\n";
                preview += $"\n--- Settings ---\n";
                preview += $"Algorithms: {data?.GetValueOrDefault("SelectedAlgorithms")?.ToString()}\n";
                preview += $"Goal Count: {data?.GetValueOrDefault("GoalCount")}\n";
                preview += $"Parking Count: {data?.GetValueOrDefault("ParkingCount")}\n";
                preview += $"Static Obstacles: {data?.GetValueOrDefault("StaticObstacles")}\n";
                preview += $"Dynamic Obstacles: {data?.GetValueOrDefault("DynamicObstacles")}\n";
                preview += $"Heuristic Weight: {data?.GetValueOrDefault("HeuristicWeight")}\n";
                preview += $"Search Limit: {data?.GetValueOrDefault("SearchLimit")}\n";
                preview += $"Iterations: {data?.GetValueOrDefault("Iterations")}\n";

                return preview;
            }
            catch (Exception ex)
            {
                return $"Error loading preview: {ex.Message}";
            }
        }

        /// <summary>
        /// Loads settings from a file
        /// </summary>
        public ExperimentSettings LoadFromFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                var settings = new ExperimentSettings
                {
                    ExperimentName = data?.GetValueOrDefault("ExperimentName")?.ToString() ?? "Experiment",
                    SelectedAlgorithms = JsonSerializer.Deserialize<List<string>>(data?["SelectedAlgorithms"]?.ToString() ?? "[]"),
                    SelectedMetrics = JsonSerializer.Deserialize<List<string>>(data?["SelectedMetrics"]?.ToString() ?? "[\"Manhattan\"]"),
                    GoalCount = int.Parse(data?["GoalCount"]?.ToString() ?? "5"),
                    ParkingCount = int.Parse(data?["ParkingCount"]?.ToString() ?? "2"),
                    StaticObstacles = int.Parse(data?["StaticObstacles"]?.ToString() ?? "20"),
                    DynamicObstacles = int.Parse(data?["DynamicObstacles"]?.ToString() ?? "5"),
                    HeuristicWeight = int.Parse(data?["HeuristicWeight"]?.ToString() ?? "2"),
                    SearchLimit = int.Parse(data?["SearchLimit"]?.ToString() ?? "20000"),
                    AllowDiagonals = bool.Parse(data?["AllowDiagonals"]?.ToString() ?? "true"),
                    HeavyDiagonals = bool.Parse(data?["HeavyDiagonals"]?.ToString() ?? "false"),
                    Iterations = int.Parse(data?["Iterations"]?.ToString() ?? "5"),
                    SaveScreenshots = bool.Parse(data?["SaveScreenshots"]?.ToString() ?? "true"),
                    SaveReplay = bool.Parse(data?["SaveReplay"]?.ToString() ?? "true"),
                    ShowPathOnScreenshots = bool.Parse(data?["ShowPathOnScreenshots"]?.ToString() ?? "true")
                };

                if (data?.ContainsKey("SavePath") == true && !string.IsNullOrEmpty(data["SavePath"]?.ToString()))
                {
                    settings.SavePath = data["SavePath"]?.ToString();
                }

                return settings;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deletes a settings file
        /// </summary>
        public bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the settings directory path
        /// </summary>
        private static string GetSettingsDirectory()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SETTINGS_DIRECTORY);
        }
        #endregion
    }
}