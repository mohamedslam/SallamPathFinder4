#region File Header
/// <summary>
/// File: ObstacleSettingsService.cs
/// Description: Service for managing dynamic obstacle settings
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Obstacles;
using System.Text.Json;
#endregion

namespace SallamPathFinder4.Services.Obstacle
{
    #region Class Documentation
    /// <summary>
    /// Service for managing dynamic obstacle settings
    /// Saves and loads settings to/from JSON file
    /// Thread-safe with locking mechanism
    /// </summary>
    #endregion
    public sealed class ObstacleSettingsService : IObstacleSettingsService
    {
        #region Constants
        private const string DEFAULT_SETTINGS_FILE = "ObstacleSettings.json";
        #endregion

        #region Private Fields
        private readonly string _settingsPath;
        private ObstacleSettings _currentSettings;
        private readonly object _lockObject = new object();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new obstacle settings service
        /// </summary>
        public ObstacleSettingsService(string settingsFile = DEFAULT_SETTINGS_FILE)
        {
            _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, settingsFile);
            _currentSettings = new ObstacleSettings();
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public ObstacleSettings GetSettings()
        {
            lock (_lockObject)
            {
                return _currentSettings.Clone();
            }
        }

        /// <inheritdoc/>
        public async Task SaveSettingsAsync(ObstacleSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    System.IO.File.WriteAllText(_settingsPath, json);
                    _currentSettings = settings.Clone();
                }
            });
        }

        /// <inheritdoc/>
        public async Task<ObstacleSettings> LoadSettingsAsync()
        {
            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    if (System.IO. File.Exists(_settingsPath))
                    {
                        try
                        {
                            var json = System.IO.File.ReadAllText(_settingsPath);
                            var settings = JsonSerializer.Deserialize<ObstacleSettings>(json);

                            if (settings != null)
                            {
                                _currentSettings = settings;
                                return settings.Clone();
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error loading obstacle settings: {ex.Message}");
                        }
                    }

                    // Return default settings if file doesn't exist or is corrupted
                    return new ObstacleSettings();
                }
            });
        }
        /// <inheritdoc/>
        public void ResetToDefaults()
        {
            lock (_lockObject)
            {
                _currentSettings = new ObstacleSettings();
            }
        }
        #endregion
    }
}