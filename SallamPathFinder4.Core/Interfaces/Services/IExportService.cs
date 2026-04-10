#region File Header
/// <summary>
/// File: IExportService.cs
/// Description: Interface for export service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.Core.Models.Path;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Export Options
    public enum ExportFormat
    {
        CSV,
        JSON,
        HTML,
        PDF
    }

    public sealed class ExportOptions
    {
        public ExportFormat Format { get; set; } = ExportFormat.CSV;
        public bool IncludeHeader { get; set; } = true;
        public string Separator { get; set; } = ",";
        public bool CompressOutput { get; set; } = false;
    }
    #endregion

    #region Interface Documentation
    /// <summary>
    /// Service interface for exporting experiment results
    /// </summary>
    #endregion
    public interface IExportService
    {
        Task ExportExperimentsAsync(
            List<ExperimentData> experiments,
            string filePath,
            ExportOptions options = null);

        Task ExportPathAsync(
            List<PathNode> path,
            string filePath,
            ExportOptions options = null);

        Task ExportComparisonReportAsync(
            object reportData,
            string filePath,
            ExportOptions options = null);

        string ConvertToCsv<T>(List<T> data, bool includeHeader = true);
        string ConvertToJson<T>(List<T> data);
        string ConvertToHtmlTable<T>(List<T> data, string title = "Export Results");
    }
}