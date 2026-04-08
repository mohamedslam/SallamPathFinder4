#region File Header
/// <summary>
/// File: IObstacleSettingsService.cs
/// Description: Interface for managing dynamic obstacle settings
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Threading.Tasks;
using SallamPathFinder4.Core.Models.Obstacles;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Interface Documentation
    /// <summary>
    /// Service interface for managing dynamic obstacle settings
    /// Handles loading, saving, and resetting obstacle configurations
    /// </summary>
    #endregion
    public interface IObstacleSettingsService
    {
        #region Methods
        /// <summary>Gets the current obstacle settings</summary>
        ObstacleSettings GetSettings();

        /// <summary>Saves obstacle settings to file</summary>
        Task SaveSettingsAsync(ObstacleSettings settings);

        /// <summary>Loads obstacle settings from file</summary>
        Task<ObstacleSettings> LoadSettingsAsync();

        /// <summary>Resets settings to default values</summary>
        void ResetToDefaults();
        #endregion
    }
}