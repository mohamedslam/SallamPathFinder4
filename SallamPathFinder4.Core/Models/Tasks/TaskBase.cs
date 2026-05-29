#region File Header
/// <summary>
/// File: TaskBase.cs
/// Description: Abstract base class for all tasks
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Core.Models.Sensors;

namespace SallamPathFinder4.Core.Models.Tasks
{
    /// <summary>
    /// Abstract base class for task implementations
    /// </summary>
    public abstract class TaskBase : ITask
    {
        #region Properties
        /// <summary>
        /// Unique identifier for the task
        /// </summary>
        public string TaskId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Display name of the task
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Type of the task
        /// </summary>
        public abstract TaskType Type { get; }

        /// <summary>
        /// Estimated duration in seconds
        /// </summary>
        public double EstimatedDuration { get; set; } = 5.0;

        /// <summary>
        /// Estimated energy cost in percentage points
        /// </summary>
        public double EstimatedEnergyCost { get; set; } = 2.0;

        /// <summary>
        /// Robot type required to execute this task
        /// </summary>
        public RobotType RequiredRobotType { get; set; } = RobotType.Wheeled;
        #endregion

        #region Constructors
        protected TaskBase()
        {
            // Default values set in property initializers
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Executes the task asynchronously
        /// </summary>
        /// <param name="robot">The robot executing the task</param>
        /// <param name="context">Optional context data (e.g., current battery level)</param>
        /// <returns>Task result containing success status, duration, and energy consumed</returns>
        public abstract Task<TaskResult> ExecuteAsync(RobotDefinition robot, object context = null);

        /// <summary>
        /// Creates a deep copy of the task
        /// </summary>
        public abstract ITask Clone();

        /// <summary>
        /// Gets a human-readable description of the task
        /// </summary>
        public abstract string GetDescription();
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Checks if the robot can execute this task
        /// </summary>
        /// <param name="robot">The robot to check</param>
        /// <returns>True if the robot can execute the task</returns>
        public virtual bool CanExecute(RobotDefinition robot)
        {
            if (robot == null) return false;
            return robot.RobotType == RequiredRobotType;
        }

        /// <summary>
        /// Gets the estimated time for this task considering robot capabilities
        /// </summary>
        /// <param name="robot">The robot executing the task</param>
        /// <returns>Estimated duration in seconds</returns>
        public virtual double GetEstimatedTime(RobotDefinition robot)
        {
            double time = EstimatedDuration;

            // Apply robot-specific modifiers
            if (robot != null)
            {
                time = ApplyRobotMultiplier(robot, time);
            }

            return time;
        }

        /// <summary>
        /// Gets the estimated energy cost for this task considering robot capabilities
        /// </summary>
        /// <param name="robot">The robot executing the task</param>
        /// <returns>Estimated energy cost in percentage points</returns>
        public virtual double GetEstimatedEnergy(RobotDefinition robot)
        {
            double energy = EstimatedEnergyCost;

            // Apply robot-specific modifiers
            if (robot != null)
            {
                energy = ApplyRobotMultiplier(robot, energy);
            }

            return energy;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Applies robot type multiplier to a base value
        /// </summary>
        /// <param name="robot">The robot</param>
        /// <param name="baseValue">The base value to multiply</param>
        /// <returns>The multiplied value</returns>
        protected double ApplyRobotMultiplier(RobotDefinition robot, double baseValue)
        {
            if (robot == null) return baseValue;

            // Apply robot type multiplier based on capabilities
            double multiplier = robot.RobotType switch
            {
                RobotType.Wheeled => 1.0,
                RobotType.Tracked => 1.2,      // Slower but more stable
                RobotType.Flying => 0.8,        // Faster
                RobotType.Humanoid => 1.5,      // Slower
                RobotType.Omnidirectional => 1.0,
                RobotType.Custom => 1.0,
                _ => 1.0
            };

            // Additional modifiers based on sensors
            if (robot.Sensors != null)
            {
                // Robots with better sensors perform tasks faster
                bool hasCamera = robot.Sensors.Any(s => s.SensorType == "Camera");
                bool hasLidar = robot.Sensors.Any(s => s.SensorType == "Lidar");

                if (hasLidar)
                    multiplier *= 0.9;
                if (hasCamera)
                    multiplier *= 0.95;
            }

            return baseValue * multiplier;
        }

        /// <summary>
        /// Logs task execution for debugging
        /// </summary>
        protected void LogExecution(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[Task:{TaskName}] {message}");
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Returns a string representation of the task
        /// </summary>
        public override string ToString()
        {
            return $"{TaskName} (ID: {TaskId.Substring(0, 8)}...)";
        }
        #endregion
    }
}