#region File Header
/// <summary>
/// File: IExperimentService.cs
/// Description: Interface for experiment logging and management
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Collections.Generic;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Models.Experiments;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Interface Documentation
    /// <summary>
    /// Service interface for experiment logging and management
    /// Handles saving experiment results and exporting data
    /// </summary>
    #endregion
    public interface IExperimentService
    {
        #region Methods
        /// <summary>Logs a completed experiment to CSV</summary>
        Task LogExperimentAsync(ExperimentData data);

        /// <summary>Gets all logged experiments</summary>
        List<ExperimentData> GetAllExperiments();

        /// <summary>Exports all experiments to a CSV file</summary>
        Task ExportAllAsync(string outputPath);

        /// <summary>Exports selected experiments to a CSV file</summary>
        Task ExportSelectedAsync(List<string> experimentIds, string outputPath);

        /// <summary>
        /// Deletes an experiment by ID
        /// </summary>
        Task<bool> DeleteExperimentAsync(string experimentId);

        /// <summary>
        /// Deletes multiple experiments by IDs
        /// </summary>
        Task<int> DeleteExperimentsAsync(List<string> experimentIds);

        #endregion
    }
}