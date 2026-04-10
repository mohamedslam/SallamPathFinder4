#region File Header
/// <summary>
/// File: ObstacleSettingsViewModel.cs
/// Description: ViewModel for obstacle settings form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Obstacles;
#endregion

namespace SallamPathFinder4.WinForms.ViewModels
{
    #region Class Documentation
    /// <summary>
    /// ViewModel for obstacle settings form
    /// Manages obstacle type settings, movement behavior, and spawning parameters
    /// </summary>
    #endregion
    public sealed class ObstacleSettingsViewModel
    {
        #region Private Fields
        private readonly IObstacleSettingsService _settingsService;
        private ObstacleSettings _currentSettings;
        #endregion

        #region Constructor
        public ObstacleSettingsViewModel(IObstacleSettingsService settingsService)
        {
            _settingsService = settingsService;
            LoadSettings();
        }
        #endregion

        #region Properties
        public ObstacleSettings CurrentSettings => _currentSettings;
        #endregion

        #region Public Methods
        public async void LoadSettings()
        {
            _currentSettings = await _settingsService.LoadSettingsAsync();
        }

        public ObstacleSettings GetSettings()
        {
            return _currentSettings?.Clone() ?? new ObstacleSettings();
        }

        public async void SaveSettings(ObstacleSettings settings)
        {
            _currentSettings = settings;
            await _settingsService.SaveSettingsAsync(settings);
        }

        public void ResetToDefaults()
        {
            _currentSettings = new ObstacleSettings();
            _settingsService.ResetToDefaults();
        }
        #endregion
    }
}