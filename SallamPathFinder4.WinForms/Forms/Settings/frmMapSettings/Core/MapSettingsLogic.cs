#region File Header
/// <summary>
/// File: MapSettingsLogic.cs
/// Description: Business logic for map settings form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Settings.frmMapSettings
{
    /// <summary>
    /// Business logic for map settings operations
    /// </summary>
    public sealed class MapSettingsLogic
    {
        #region Constants
        private const int MIN_GRID_SIZE = 10;
        private const int MAX_GRID_SIZE = 500;
        #endregion

        #region Constructor
        public MapSettingsLogic()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Validates grid dimensions
        /// </summary>
        public bool ValidateSettings(int width, int height)
        {
            return width >= MIN_GRID_SIZE && width <= MAX_GRID_SIZE &&
                   height >= MIN_GRID_SIZE && height <= MAX_GRID_SIZE;
        }
        #endregion
    }
}