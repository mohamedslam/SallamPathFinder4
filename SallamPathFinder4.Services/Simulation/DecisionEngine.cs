#region File Header
/// <summary>
/// File: DecisionEngine.cs
/// Description: Core decision engine for obstacle avoidance behavior
/// Integrates with priority system, wait timers, and learning memory
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-01
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path; 

#endregion

namespace SallamPathFinder4.Services.Simulation
{
    /// <summary>
    /// Decision result containing action and metadata
    /// </summary>
    public sealed class DecisionResult
    {
        public AvoidanceBehavior Behavior { get; set; }
        public double WaitTimeSeconds { get; set; }
        public bool NeedsReplan { get; set; }
        public bool ShouldRecordForLearning { get; set; }
        public string Reason { get; set; }
        public DateTime DecisionTime { get; set; }

        public override string ToString()
        {
            return $"{Behavior} - Wait: {WaitTimeSeconds:F1}s - {Reason}";
        }
    } 

    /// <summary>
    /// Core decision engine for obstacle avoidance
    /// </summary>
    public sealed class DecisionEngine : IDisposable
    {
        #region Constants
        private const int MAX_CONCURRENT_OBSTACLES = 10;
        private const double DEFAULT_SAFE_DISTANCE_CM = 30.0;
        private const double DEFAULT_CRITICAL_DISTANCE_CM = 10.0;
        #endregion

        #region Private Fields
        private readonly ConcurrentDictionary<string, ObstacleWaitState> _activeWaits;
        private readonly List<DetectedObstacle> _obstacleMemory;
        private readonly object _memoryLock = new object();
        private bool _isDisposed;
        private CancellationTokenSource _cts;
        private bool _learningEnabled = true;
        private double _safeDistanceCm = DEFAULT_SAFE_DISTANCE_CM;
        private double _criticalDistanceCm = DEFAULT_CRITICAL_DISTANCE_CM;

        // Configurable wait times (can be modified from UI)
        private Dictionary<ObstacleType, double> _waitTimeSettings;
        private Dictionary<ObstacleType, double> _maxWaitTimeSettings;
        #endregion

        #region Constructor
        public DecisionEngine()
        {
            _activeWaits = new ConcurrentDictionary<string, ObstacleWaitState>();
            _obstacleMemory = new List<DetectedObstacle>();
            _cts = new CancellationTokenSource();

            InitializeWaitTimeSettings();
        }
        #endregion

        #region Events
        /// <summary>
        /// Event raised when a decision is made
        /// </summary>
        public event Action<DecisionResult, DetectedObstacle> DecisionMade;

        /// <summary>
        /// Event raised when replanning is needed
        /// </summary>
        public event Action<Point> ReplanningNeeded;

        /// <summary>
        /// Event raised when an obstacle should be recorded for learning
        /// </summary>
        public event Action<DetectedObstacle> RecordForLearning;
        #endregion

        #region Properties
        /// <summary>
        /// Enable or disable learning from obstacles
        /// </summary>
        public bool LearningEnabled
        {
            get => _learningEnabled;
            set => _learningEnabled = value;
        }

        /// <summary>
        /// Safe distance threshold in centimeters
        /// </summary>
        public double SafeDistanceCm
        {
            get => _safeDistanceCm;
            set => _safeDistanceCm = Math.Max(5, Math.Min(100, value));
        }

        /// <summary>
        /// Critical distance threshold in centimeters
        /// </summary>
        public double CriticalDistanceCm
        {
            get => _criticalDistanceCm;
            set => _criticalDistanceCm = Math.Max(1, Math.Min(50, value));
        }

        /// <summary>
        /// Number of obstacles in memory
        /// </summary>
        public int MemoryCount => _obstacleMemory.Count;

        /// <summary>
        /// Number of active wait states
        /// </summary>
        public int ActiveWaitCount => _activeWaits.Count;
        #endregion

        #region Public Methods - Configuration
        /// <summary>
        /// Sets wait time for a specific obstacle type
        /// </summary>
        public void SetWaitTime(ObstacleType type, double waitTimeSeconds, double maxWaitTimeSeconds)
        {
            _waitTimeSettings[type] = Math.Max(0.5, Math.Min(30, waitTimeSeconds));
            _maxWaitTimeSettings[type] = Math.Max(waitTimeSeconds, Math.Min(60, maxWaitTimeSeconds));
        }

        /// <summary>
        /// Gets wait time for a specific obstacle type
        /// </summary>
        public (double waitTime, double maxWaitTime) GetWaitTime(ObstacleType type)
        {
            return (_waitTimeSettings.GetValueOrDefault(type, 2.0),
                    _maxWaitTimeSettings.GetValueOrDefault(type, 5.0));
        }
        #endregion

        #region Public Methods - Decision Making
        /// <summary>
        /// Makes a decision based on detected obstacles
        /// </summary>
        public async Task<DecisionResult> MakeDecisionAsync(List<DetectedObstacle> obstacles, double robotSpeed, Point robotPosition)
        {
            if (obstacles == null || obstacles.Count == 0)
            {
                // No obstacles - clear any active waits
                ClearExpiredWaits();
                return new DecisionResult
                {
                    Behavior = AvoidanceBehavior.None,
                    WaitTimeSeconds = 0,
                    NeedsReplan = false,
                    ShouldRecordForLearning = false,
                    Reason = "No obstacles detected",
                    DecisionTime = DateTime.UtcNow
                };
            }

            // Sort obstacles by priority and distance
            var prioritizedObstacles = PrioritizeObstacles(obstacles);
            var primaryObstacle = prioritizedObstacles.FirstOrDefault();

            if (primaryObstacle == null)
                return CreateNoneDecision();

            // Check if we're already waiting for this obstacle
            string obstacleId = GetObstacleId(primaryObstacle);

            if (_activeWaits.TryGetValue(obstacleId, out var waitState))
            {
                if (waitState.IsWaiting)
                {
                    return new DecisionResult
                    {
                        Behavior = AvoidanceBehavior.Stop,
                        WaitTimeSeconds = waitState.RemainingWaitTime,
                        NeedsReplan = false,
                        ShouldRecordForLearning = false,
                        Reason = $"Waiting for {primaryObstacle.ObstacleType} ({waitState.RemainingWaitTime:F1}s remaining)",
                        DecisionTime = DateTime.UtcNow
                    };
                }

                if (waitState.ShouldGiveUp)
                {
                    // Waited too long - need to replan
                    _activeWaits.TryRemove(obstacleId, out _);
                    return new DecisionResult
                    {
                        Behavior = AvoidanceBehavior.ReplanPermanent,
                        WaitTimeSeconds = 0,
                        NeedsReplan = true,
                        ShouldRecordForLearning = true,
                        Reason = $"Gave up waiting for {primaryObstacle.ObstacleType} after {waitState.TotalWaitTimeSeconds:F1}s",
                        DecisionTime = DateTime.UtcNow
                    };
                }
            }

            // Make decision based on obstacle properties
            var decision = await EvaluateObstacleAsync(primaryObstacle, robotSpeed, robotPosition);

            // Handle decision
            switch (decision.Behavior)
            {
                case AvoidanceBehavior.Stop:
                    // Start waiting
                    StartWaiting(primaryObstacle);
                    break;

                case AvoidanceBehavior.ReplanTemporary:
                case AvoidanceBehavior.ReplanPermanent:
                    decision.NeedsReplan = true;
                    ReplanningNeeded?.Invoke(primaryObstacle.Location);
                    break;

                case AvoidanceBehavior.EmergencyStop:
                    // Record for learning immediately
                    if (_learningEnabled)
                        RecordForLearning?.Invoke(primaryObstacle);
                    break;
            }

            // Record obstacle for learning if enabled
            if (decision.ShouldRecordForLearning && _learningEnabled)
            {
                RecordObstacle(primaryObstacle);
                RecordForLearning?.Invoke(primaryObstacle);
            }

            DecisionMade?.Invoke(decision, primaryObstacle);

            return decision;
        }

        /// <summary>
        /// Gets the current waiting obstacles (for UI display)
        /// </summary>
        public List<ObstacleWaitState> GetActiveWaits()
        {
            return _activeWaits.Values.ToList();
        }

        /// <summary>
        /// Clears all wait states
        /// </summary>
        public void ClearAllWaits()
        {
            _activeWaits.Clear();
        }

        /// <summary>
        /// Exports obstacle memory to CSV format
        /// </summary>
        public string ExportObstacleMemoryToCsv()
        {
            lock (_memoryLock)
            {
                var lines = new List<string>();
                lines.Add("Timestamp,LocationX,LocationY,ObstacleType,Priority,DistanceCm,WasAvoided,LearningRecorded");

                foreach (var obstacle in _obstacleMemory)
                {
                    var priority = ObstaclePriorityHelper.GetPriorityValue(obstacle.ObstacleType);
                    lines.Add($"{obstacle.FirstDetectionTime:yyyy-MM-dd HH:mm:ss},{obstacle.Location.X},{obstacle.Location.Y}," +
                              $"{obstacle.ObstacleType},{priority},{obstacle.DistanceCm:F1},true,true");
                }

                return string.Join(Environment.NewLine, lines);
            }
        }

        /// <summary>
        /// Clears obstacle memory
        /// </summary>
        public void ClearObstacleMemory()
        {
            lock (_memoryLock)
            {
                _obstacleMemory.Clear();
            }
        }

        /// <summary>
        /// Gets obstacle memory statistics
        /// </summary>
        public (int total, Dictionary<ObstacleType, int> byType) GetMemoryStatistics()
        {
            lock (_memoryLock)
            {
                var byType = _obstacleMemory
                    .GroupBy(o => o.ObstacleType)
                    .ToDictionary(g => g.Key, g => g.Count());

                return (_obstacleMemory.Count, byType);
            }
        }
        #endregion

        #region Private Methods - Decision Logic
        /// <summary>
        /// Prioritizes obstacles by priority level and distance
        /// </summary>
        private List<DetectedObstacle> PrioritizeObstacles(List<DetectedObstacle> obstacles)
        {
            return obstacles
                .OrderByDescending(o => ObstaclePriorityHelper.GetPriorityValue(o.ObstacleType))
                .ThenBy(o => o.DistanceCm)
                .Take(MAX_CONCURRENT_OBSTACLES)
                .ToList();
        }

        /// <summary>
        /// Evaluates a single obstacle and returns decision
        /// </summary>
        private async Task<DecisionResult> EvaluateObstacleAsync(DetectedObstacle obstacle, double robotSpeed, Point robotPosition)
        {
            await Task.CompletedTask;  // For async compatibility

            double distance = obstacle.DistanceCm;
            var priority = ObstaclePriorityHelper.GetPriority(obstacle.ObstacleType);
            int priorityValue = (int)priority;

            // Emergency: very close
            if (distance < _criticalDistanceCm)
            {
                return new DecisionResult
                {
                    Behavior = AvoidanceBehavior.EmergencyStop,
                    WaitTimeSeconds = 0,
                    NeedsReplan = true,
                    ShouldRecordForLearning = true,
                    Reason = $"EMERGENCY: {obstacle.ObstacleType} at {distance:F0}cm!",
                    DecisionTime = DateTime.UtcNow
                };
            }

            // High priority obstacles at medium distance
            if (priorityValue >= 70 && distance < _safeDistanceCm)
            {
                var (waitTime, maxWaitTime) = GetWaitTime(obstacle.ObstacleType);
                return new DecisionResult
                {
                    Behavior = AvoidanceBehavior.Stop,
                    WaitTimeSeconds = waitTime,
                    NeedsReplan = false,
                    ShouldRecordForLearning = true,
                    Reason = $"{obstacle.ObstacleType} detected at {distance:F0}cm - waiting {waitTime}s",
                    DecisionTime = DateTime.UtcNow
                };
            }

            // Medium priority obstacles at safe distance
            if (priorityValue >= 50 && distance < _safeDistanceCm + 20)
            {
                return new DecisionResult
                {
                    Behavior = AvoidanceBehavior.SlowDown,
                    WaitTimeSeconds = 0,
                    NeedsReplan = false,
                    ShouldRecordForLearning = true,
                    Reason = $"Slowing down for {obstacle.ObstacleType} at {distance:F0}cm",
                    DecisionTime = DateTime.UtcNow
                };
            }

            // Low priority obstacles - may need replan
            if (priorityValue < 50 && distance < _safeDistanceCm)
            {
                return new DecisionResult
                {
                    Behavior = AvoidanceBehavior.ReplanTemporary,
                    WaitTimeSeconds = 0,
                    NeedsReplan = true,
                    ShouldRecordForLearning = true,
                    Reason = $"Replanning to avoid {obstacle.ObstacleType}",
                    DecisionTime = DateTime.UtcNow
                };
            }

            // Default - no action needed
            return new DecisionResult
            {
                Behavior = AvoidanceBehavior.None,
                WaitTimeSeconds = 0,
                NeedsReplan = false,
                ShouldRecordForLearning = false,
                Reason = $"Obstacle at safe distance ({distance:F0}cm)",
                DecisionTime = DateTime.UtcNow
            };
        }

        private void StartWaiting(DetectedObstacle obstacle)
        {
            string obstacleId = GetObstacleId(obstacle);
            var (waitTime, maxWaitTime) = GetWaitTime(obstacle.ObstacleType);

            var waitState = new ObstacleWaitState
            {
                ObstacleId = obstacleId,
                Location = obstacle.Location,
                Type = obstacle.ObstacleType
            };
            waitState.StartWaiting(waitTime, maxWaitTime);

            _activeWaits.AddOrUpdate(obstacleId, waitState, (key, old) => waitState);
        }

        private void ClearExpiredWaits()
        {
            var expired = _activeWaits
                .Where(kvp => !kvp.Value.IsWaiting && kvp.Value.ShouldGiveUp)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expired)
            {
                _activeWaits.TryRemove(key, out _);
            }
        }

        private void RecordObstacle(DetectedObstacle obstacle)
        {
            lock (_memoryLock)
            {
                // Keep only last 1000 obstacles
                while (_obstacleMemory.Count > 1000)
                {
                    _obstacleMemory.RemoveAt(0);
                }

                _obstacleMemory.Add(obstacle);
            }
        }

        private string GetObstacleId(DetectedObstacle obstacle)
        {
            return $"{obstacle.Location.X}_{obstacle.Location.Y}_{obstacle.ObstacleType}";
        }

        private DecisionResult CreateNoneDecision()
        {
            return new DecisionResult
            {
                Behavior = AvoidanceBehavior.None,
                WaitTimeSeconds = 0,
                NeedsReplan = false,
                ShouldRecordForLearning = false,
                Reason = "No action needed",
                DecisionTime = DateTime.UtcNow
            };
        }
        #endregion

        #region Private Methods - Initialization
        private void InitializeWaitTimeSettings()
        {
            _waitTimeSettings = new Dictionary<ObstacleType, double>
            {
                { ObstacleType.Child, 5.0 },
                { ObstacleType.Adult, 3.0 },
                { ObstacleType.Animal, 2.0 },
                { ObstacleType.OtherRobot, 4.0 },
                { ObstacleType.Equipment, 1.0 }
            };

            _maxWaitTimeSettings = new Dictionary<ObstacleType, double>
            {
                { ObstacleType.Child, 10.0 },
                { ObstacleType.Adult, 8.0 },
                { ObstacleType.Animal, 5.0 },
                { ObstacleType.OtherRobot, 8.0 },
                { ObstacleType.Equipment, 3.0 }
            };
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _activeWaits?.Clear();
                _obstacleMemory?.Clear();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}