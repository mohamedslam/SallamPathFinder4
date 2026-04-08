#region File Header
/// <summary>
/// File: IFileService.cs
/// Description: Interface for file operations (save/load maps)
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Threading.Tasks;
using SallamPathFinder4.Core.Models.Map;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Interface Documentation
    /// <summary>
    /// Service interface for file operations
    /// Handles saving and loading map data to/from binary files
    /// </summary>
    #endregion
    public interface IFileService
    {
        #region Methods
        /// <summary>Saves the current map to a file</summary>
        Task SaveMapAsync(string filePath, MapData data);

        /// <summary>Loads a map from a file</summary>
        Task<MapData> LoadMapAsync(string filePath);

        /// <summary>Checks if a file exists</summary>
        bool FileExists(string filePath);
        #endregion
    }
}