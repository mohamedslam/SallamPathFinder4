#region File Header
/// <summary>
/// File: ExperimentResultsLogic.cs
/// Description: Business logic for experiment results form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SallamPathFinder4.WinForms.Models;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentResults.Core
{
    /// <summary>
    /// Business logic for exporting and managing experiment results
    /// </summary>
    public sealed class ExperimentResultsLogic
    {
        #region Constructor
        public ExperimentResultsLogic()
        {
        }
        #endregion

        #region Public Methods - Export
        /// <summary>
        /// Exports results to CSV file
        /// </summary>
        public void ExportToCsv(List<ExperimentResultItem> results, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("Algorithm,Metric,Iteration,PathLength,TimeMs,BatteryRemaining,Collisions,Errors,AvgSpeed,Success,ScreenshotPath");

            foreach (var r in results)
            {
                writer.WriteLine($"{r.Algorithm},{r.Metric},{r.Iteration},{r.PathLength},{r.ComputationTimeMs:F2}," +
                    $"{r.RemainingBattery:F1},{r.CollisionCount},{r.InvalidMoveCount}," +
                    $"{r.AverageActualSpeed:F1},{r.Success},{r.ScreenshotPath}");
            }
        }

        /// <summary>
        /// Exports results to HTML file
        /// </summary>
        public void ExportToHtml(List<ExperimentResultItem> results, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("<html><head><title>Experiment Results</title><style>");
            writer.WriteLine("body { font-family: Arial; margin: 20px; }");
            writer.WriteLine("table { border-collapse: collapse; width: 100%; }");
            writer.WriteLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            writer.WriteLine("th { background-color: #4CAF50; color: white; }");
            writer.WriteLine(".success { color: green; } .failure { color: red; }");
            writer.WriteLine("</style></head><body>");
            writer.WriteLine("<h1>Experiment Results</h1>");
            writer.WriteLine($"<p>Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            writer.WriteLine($"<p>Total: {results.Count} | Success: {results.Count(r => r.Success)}</p>");
            writer.WriteLine("<table><tr><th>#</th><th>Algorithm</th><th>Metric</th><th>Length</th><th>Time(ms)</th><th>Success</th></tr>");

            int id = 1;
            foreach (var r in results)
            {
                string cssClass = r.Success ? "success" : "failure";
                writer.WriteLine($"<tr class='{cssClass}'><td>{id++}</td><td>{r.Algorithm}</td>");
                writer.WriteLine($"<td>{r.Metric}</td><td>{r.PathLength}</td>");
                writer.WriteLine($"<td>{r.ComputationTimeMs:F2}</td><td>{(r.Success ? "✓" : "✗")}</td></tr>");
            }
            writer.WriteLine("</table></body></html>");
        }
        #endregion
    }
}