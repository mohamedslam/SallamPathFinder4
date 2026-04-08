#region File Header
/// <summary>
/// File: SimulationLogger.cs
/// Description: Logging service for simulation events
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace SallamPathFinder4.Services.Logging
{
    #region Log Entry Class
    public sealed class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogLevel Level { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }
    #endregion

    #region Class Documentation
    /// <summary>
    /// Service for logging simulation events
    /// </summary>
    #endregion
    public sealed class SimulationLogger : IDisposable
    {
        #region Constants
        private const string LOG_DIRECTORY = "Logs";
        private const int MAX_LOG_FILES = 10;
        private const long MAX_LOG_FILE_SIZE_BYTES = 10 * 1024 * 1024; // 10MB
        #endregion

        #region Private Fields
        private readonly string _logPath;
        private readonly ConcurrentQueue<LogEntry> _logQueue;
        private readonly object _lockObject = new object();
        private bool _isDisposed;
        private Task _processingTask;
        private bool _isRunning;
        #endregion

        #region Constructor
        public SimulationLogger()
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOG_DIRECTORY);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            _logPath = Path.Combine(logDir, $"simulation_{DateTime.Now:yyyyMMdd}.log");
            _logQueue = new ConcurrentQueue<LogEntry>();
            _isRunning = true;
            _processingTask = Task.Run(ProcessLogQueue);
        }
        #endregion

        #region Public Methods
        public void Log(LogLevel level, string category, string message, string details = null)
        {
            var entry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Category = category,
                Message = message,
                Details = details
            };

            _logQueue.Enqueue(entry);
        }

        public void Debug(string category, string message, string details = null)
        {
            Log(LogLevel.Debug, category, message, details);
        }

        public void Info(string category, string message, string details = null)
        {
            Log(LogLevel.Info, category, message, details);
        }

        public void Warning(string category, string message, string details = null)
        {
            Log(LogLevel.Warning, category, message, details);
        }

        public void Error(string category, string message, string details = null)
        {
            Log(LogLevel.Error, category, message, details);
        }

        public void Critical(string category, string message, string details = null)
        {
            Log(LogLevel.Critical, category, message, details);
        }

        public async Task<List<LogEntry>> GetLogsAsync(LogLevel minLevel = LogLevel.Debug, int maxEntries = 1000)
        {
            return await Task.Run(() =>
            {
                var logs = new List<LogEntry>();

                if (System.IO.File.Exists(_logPath))
                {
                    var lines = System.IO.File.ReadAllLines(_logPath);
                    foreach (var line in lines.TakeLast(maxEntries))
                    {
                        var entry = ParseLogLine(line);
                        if (entry != null && entry.Level >= minLevel)
                        {
                            logs.Add(entry);
                        }
                    }
                }

                return logs;
            });
        }

        public async Task ClearLogsAsync()
        {
            await Task.Run(() =>
            {
                if (System.IO.File.Exists(_logPath))
                {
                    System.IO.File.WriteAllText(_logPath, string.Empty);
                }
            });
        }

        public async Task RotateLogsAsync()
        {
            await Task.Run(() =>
            {
                var logDir = Path.GetDirectoryName(_logPath);
                var logFiles = Directory.GetFiles(logDir, "simulation_*.log")
                    .OrderByDescending(f => f)
                    .ToList();

                for (int i = MAX_LOG_FILES; i < logFiles.Count; i++)
                {
                    System.IO.File.Delete(logFiles[i]);
                }
            });
        }
        #endregion

        #region Private Methods
        private async Task ProcessLogQueue()
        {
            while (_isRunning)
            {
                while (_logQueue.TryDequeue(out var entry))
                {
                    await WriteLogEntryAsync(entry);
                }
                await Task.Delay(100);
            }
        }

        private async Task WriteLogEntryAsync(LogEntry entry)
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    var line = $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}|{entry.Level}|{entry.Category}|{entry.Message}|{entry.Details}";
                    System.IO.File.AppendAllText(_logPath, line + Environment.NewLine);

                    var fileInfo = new FileInfo(_logPath);
                    if (fileInfo.Length > MAX_LOG_FILE_SIZE_BYTES)
                    {
                        RotateLogsAsync().Wait();
                    }
                }
            });
        }

        private static LogEntry ParseLogLine(string line)
        {
            var parts = line.Split('|');
            if (parts.Length < 4) return null;

            return new LogEntry
            {
                Timestamp = DateTime.TryParse(parts[0], out var ts) ? ts : DateTime.UtcNow,
                Level = Enum.TryParse<LogLevel>(parts[1], out var level) ? level : LogLevel.Info,
                Category = parts[2],
                Message = parts[3],
                Details = parts.Length > 4 ? parts[4] : null
            };
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isRunning = false;
                _processingTask?.Wait(5000);
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}