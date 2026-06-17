#region File Header
/// <summary>
/// File: TaskSequence.cs
/// Description: Sequence of tasks for a waypoint
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Robot;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Tasks
{
    /// <summary>
    /// Execution mode for task sequence
    /// </summary>
    public enum ExecutionMode
    {
        /// <summary>Execute tasks one after another</summary>
        Sequential,

        /// <summary>Execute tasks simultaneously (if possible)</summary>
        Parallel,

        /// <summary>Execute based on previous task results</summary>
        Conditional
    }

    /// <summary>
    /// Task sequence for a specific waypoint
    /// </summary>
    public class TaskSequence
    {
        #region Properties
        /// <summary>Unique identifier for the sequence</summary>
        public string SequenceId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>The waypoint location where tasks will be executed</summary>
        public Point Waypoint { get; set; }

        /// <summary>List of tasks in this sequence</summary>
        public List<ITask> Tasks { get; set; } = new List<ITask>();

        /// <summary>How to execute the tasks</summary>
        public ExecutionMode Mode { get; set; } = ExecutionMode.Sequential;

        /// <summary>Whether the entire sequence is optional (failure doesn't stop experiment)</summary>
        public bool IsOptional { get; set; } = false;

        /// <summary>Human-readable description</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Delay between tasks in seconds (for sequential mode)</summary>
        public double InterTaskDelay { get; set; } = 0.5;
        #endregion

        #region Constructors
        public TaskSequence()
        {
        }

        public TaskSequence(Point waypoint)
        {
            Waypoint = waypoint;
        }

        public TaskSequence(Point waypoint, List<ITask> tasks)
        {
            Waypoint = waypoint;
            Tasks = tasks ?? new List<ITask>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Executes all tasks in the sequence
        /// </summary>
        /// <param name="robot">The robot executing the tasks</param>
        /// <param name="currentBattery">Current battery level percentage</param>
        /// <returns>List of task results</returns>
        public async Task<List<TaskResult>> ExecuteAsync(RobotDefinition robot, double currentBattery = 100)
        {
            var results = new List<TaskResult>();
            var log = new System.Text.StringBuilder();

            log.AppendLine($"Starting task sequence at ({Waypoint.X},{Waypoint.Y})");
            log.AppendLine($"Mode: {Mode}, Optional: {IsOptional}, Tasks: {Tasks.Count}");

            switch (Mode)
            {
                case ExecutionMode.Sequential:
                    results = await ExecuteSequentialAsync(robot, currentBattery, log);
                    break;

                case ExecutionMode.Parallel:
                    results = await ExecuteParallelAsync(robot, currentBattery, log);
                    break;

                case ExecutionMode.Conditional:
                    results = await ExecuteConditionalAsync(robot, currentBattery, log);
                    break;
            }

            LogExecution(log.ToString());
            return results;
        }

        /// <summary>
        /// Gets total estimated duration for all tasks
        /// </summary>
        public double GetTotalEstimatedDuration(RobotDefinition robot)
        {
            double total = Tasks.Sum(t => t.GetEstimatedTime(robot));

            // Add inter-task delays for sequential mode
            if (Mode == ExecutionMode.Sequential && Tasks.Count > 1)
            {
                total += InterTaskDelay * (Tasks.Count - 1);
            }

            return total;
        }

        /// <summary>
        /// Gets total estimated energy consumption for all tasks
        /// </summary>
        public double GetTotalEstimatedEnergy(RobotDefinition robot)
        {
            if (Mode == ExecutionMode.Parallel)
            {
                // For parallel, take the maximum energy (worst case)
                return Tasks.Max(t => t.GetEstimatedEnergy(robot));
            }

            return Tasks.Sum(t => t.GetEstimatedEnergy(robot));
        }

        /// <summary>
        /// Adds a task to the sequence
        /// </summary>
        public void AddTask(ITask task)
        {
            Tasks.Add(task);
        }

        /// <summary>
        /// Inserts a task at the specified index
        /// </summary>
        public void InsertTask(int index, ITask task)
        {
            Tasks.Insert(index, task);
        }

        /// <summary>
        /// Removes a task from the sequence
        /// </summary>
        public bool RemoveTask(string taskId)
        {
            var task = Tasks.FirstOrDefault(t => t.TaskId == taskId);
            if (task != null)
                return Tasks.Remove(task);
            return false;
        }

        /// <summary>
        /// Removes a task at the specified index
        /// </summary>
        public void RemoveTaskAt(int index)
        {
            if (index >= 0 && index < Tasks.Count)
                Tasks.RemoveAt(index);
        }

        /// <summary>
        /// Clears all tasks from the sequence
        /// </summary>
        public void ClearTasks()
        {
            Tasks.Clear();
        }

        /// <summary>
        /// Checks if all tasks can be executed by the robot
        /// </summary>
        public bool CanExecute(RobotDefinition robot)
        {
            return Tasks.All(t => t.CanExecute(robot));
        }

        /// <summary>
        /// Gets tasks that cannot be executed by the robot
        /// </summary>
        public List<ITask> GetIncompatibleTasks(RobotDefinition robot)
        {
            return Tasks.Where(t => !t.CanExecute(robot)).ToList();
        }

        /// <summary>
        /// Creates a deep copy of the task sequence
        /// </summary>
        public TaskSequence Clone()
        {
            return new TaskSequence
            {
                SequenceId = this.SequenceId,
                Waypoint = this.Waypoint,
                Tasks = this.Tasks.Select(t => t.Clone()).ToList(),
                Mode = this.Mode,
                IsOptional = this.IsOptional,
                Description = this.Description,
                InterTaskDelay = this.InterTaskDelay
            };
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Executes tasks sequentially one after another
        /// </summary>
        private async Task<List<TaskResult>> ExecuteSequentialAsync(RobotDefinition robot, double currentBattery, System.Text.StringBuilder log)
        {
            var results = new List<TaskResult>();

            for (int i = 0; i < Tasks.Count; i++)
            {
                var task = Tasks[i];

                if (!task.CanExecute(robot))
                {
                    var failResult = TaskResult.FailureResult($"Task {task.TaskName} cannot be executed by this robot");
                    results.Add(failResult);
                    log.AppendLine($"Task {i + 1}: {task.TaskName} - INCOMPATIBLE");

                    if (!IsOptional)
                        return results;
                    continue;
                }

                log.AppendLine($"Task {i + 1}: {task.TaskName} - Starting...");

                var result = await task.ExecuteAsync(robot, currentBattery);
                results.Add(result);

                log.AppendLine($"Task {i + 1}: {task.TaskName} - {(result.Success ? "SUCCESS" : "FAILED")} (Duration: {result.ActualDuration:F1}s, Energy: {result.EnergyConsumed:F1}%)");

                if (!result.Success && !IsOptional)
                {
                    log.AppendLine($"Sequence stopped due to task failure");
                    return results;
                }

                // Update battery level for next task
                if (result.EnergyConsumed != 0)
                {
                    currentBattery = Math.Max(0, Math.Min(100, currentBattery - result.EnergyConsumed));
                }

                // Delay between tasks
                if (i < Tasks.Count - 1 && InterTaskDelay > 0)
                {
                    await Task.Delay((int)(InterTaskDelay * 1000));
                }
            }

            log.AppendLine($"Sequence completed: {results.Count(r => r.Success)}/{results.Count} tasks successful");
            return results;
        }

        /// <summary>
        /// Executes tasks in parallel (simultaneously)
        /// </summary>
        private async Task<List<TaskResult>> ExecuteParallelAsync(RobotDefinition robot, double currentBattery, System.Text.StringBuilder log)
        {
            var compatibleTasks = Tasks.Where(t => t.CanExecute(robot)).ToList();
            var incompatibleTasks = Tasks.Where(t => !t.CanExecute(robot)).ToList();

            log.AppendLine($"Compatible tasks: {compatibleTasks.Count}, Incompatible: {incompatibleTasks.Count}");

            var results = new List<TaskResult>();

            // Add failure results for incompatible tasks
            foreach (var task in incompatibleTasks)
            {
                results.Add(TaskResult.FailureResult($"Task {task.TaskName} cannot be executed by this robot"));
            }

            if (!compatibleTasks.Any())
            {
                if (!IsOptional)
                    return results;
                return results;
            }

            // Execute all compatible tasks in parallel
            var taskResults = await Task.WhenAll(compatibleTasks.Select(t => t.ExecuteAsync(robot, currentBattery)));
            results.AddRange(taskResults);

            log.AppendLine($"Parallel execution completed: {results.Count(r => r.Success)}/{results.Count} tasks successful");

            return results;
        }

        /// <summary>
        /// Executes tasks conditionally based on previous results
        /// </summary>
        private async Task<List<TaskResult>> ExecuteConditionalAsync(RobotDefinition robot, double currentBattery, System.Text.StringBuilder log)
        {
            var results = new List<TaskResult>();

            for (int i = 0; i < Tasks.Count; i++)
            {
                var task = Tasks[i];

                // Check if previous task failed and we should skip
                if (i > 0 && results[i - 1] != null && !results[i - 1].Success)
                {
                    log.AppendLine($"Task {i + 1}: {task.TaskName} - SKIPPED (previous task failed)");
                    continue;
                }

                if (!task.CanExecute(robot))
                {
                    var failResult = TaskResult.FailureResult($"Task {task.TaskName} cannot be executed by this robot");
                    results.Add(failResult);
                    log.AppendLine($"Task {i + 1}: {task.TaskName} - INCOMPATIBLE");

                    if (!IsOptional)
                        return results;
                    continue;
                }

                log.AppendLine($"Task {i + 1}: {task.TaskName} - Starting...");

                var result = await task.ExecuteAsync(robot, currentBattery);
                results.Add(result);

                log.AppendLine($"Task {i + 1}: {task.TaskName} - {(result.Success ? "SUCCESS" : "FAILED")}");

                if (!result.Success && !IsOptional)
                {
                    log.AppendLine($"Sequence stopped due to task failure");
                    return results;
                }

                // Update battery
                if (result.EnergyConsumed != 0)
                {
                    currentBattery = Math.Max(0, Math.Min(100, currentBattery - result.EnergyConsumed));
                }
            }

            log.AppendLine($"Conditional sequence completed");
            return results;
        }

        /// <summary>
        /// Logs execution information
        /// </summary>
        private void LogExecution(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[TaskSequence:{SequenceId}] {message}");
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Returns a string representation of the task sequence
        /// </summary>
        public override string ToString()
        {
            return $"Sequence at ({Waypoint.X},{Waypoint.Y}): {Tasks.Count} tasks, Mode: {Mode}";
        }
        #endregion
    }
}