#region File Header
/// <summary>
/// File: ExportService.cs
/// Description: Service for exporting experiment results
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.Core.Models.Path;
using System.Text;
using System.Text.Json;
#endregion

namespace SallamPathFinder4.Services.Export
{
    #region Class Documentation
    /// <summary>
    /// Service for exporting experiment results in various formats
    /// </summary>
    #endregion
    public sealed class ExportService : IExportService
    {
        #region Public Methods
        public async Task ExportExperimentsAsync(List<ExperimentData> experiments, string filePath, ExportOptions options = null)
        {
            options ??= new ExportOptions();

            await Task.Run(() =>
            {
                string content = options.Format switch
                {
                    ExportFormat.CSV => ConvertToCsv(experiments, options.IncludeHeader),
                    ExportFormat.JSON => ConvertToJson(experiments),
                    ExportFormat.HTML => ConvertToHtmlTable(experiments, "Experiment Results"),
                    _ => ConvertToCsv(experiments, options.IncludeHeader)
                };

                System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
            });
        }

        public async Task ExportPathAsync(List<PathNode> path, string filePath, ExportOptions options = null)
        {
            options ??= new ExportOptions();

            await Task.Run(() =>
            {
                var pathData = path.Select((p, i) => new { Index = i, X = p.X, Y = p.Y }).ToList();
                string content = options.Format switch
                {
                    ExportFormat.CSV => ConvertToCsv(pathData, options.IncludeHeader),
                    ExportFormat.JSON => ConvertToJson(pathData),
                    _ => ConvertToCsv(pathData, options.IncludeHeader)
                };

                System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
            });
        }

        public async Task ExportComparisonReportAsync(object reportData, string filePath, ExportOptions options = null)
        {
            options ??= new ExportOptions();

            await Task.Run(() =>
            {
                string json = JsonSerializer.Serialize(reportData, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, json, Encoding.UTF8);
            });
        }

        public string ConvertToCsv<T>(List<T> data, bool includeHeader = true)
        {
            var sb = new StringBuilder();
            var properties = typeof(T).GetProperties();

            if (includeHeader)
            {
                sb.AppendLine(string.Join(",", properties.Select(p => p.Name)));
            }

            foreach (var item in data)
            {
                var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                sb.AppendLine(string.Join(",", values.Select(v => v.Contains(",") ? $"\"{v}\"" : v)));
            }

            return sb.ToString();
        }

        public string ConvertToJson<T>(List<T> data)
        {
            return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        }

        public string ConvertToHtmlTable<T>(List<T> data, string title = "Export Results")
        {
            var sb = new StringBuilder();
            var properties = typeof(T).GetProperties();

            sb.AppendLine($"<html><head><title>{title}</title></head><body>");
            sb.AppendLine($"<h1>{title}</h1>");
            sb.AppendLine("<table border='1' cellpadding='5' cellspacing='0'>");
            sb.AppendLine("<thead><tr>");

            foreach (var prop in properties)
            {
                sb.AppendLine($"<th>{prop.Name}</th>");
            }

            sb.AppendLine("</tr></thead><tbody>");

            foreach (var item in data)
            {
                sb.AppendLine("<tr>");
                foreach (var prop in properties)
                {
                    sb.AppendLine($"<td>{prop.GetValue(item)}</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody></table></body></html>");
            return sb.ToString();
        }
        #endregion
    }
}