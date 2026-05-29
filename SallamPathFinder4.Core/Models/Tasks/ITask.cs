#region File Header
/// <summary>
/// File: ITask.cs
/// Description: Interface for robot tasks at waypoints
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Robot;

namespace SallamPathFinder4.Core.Models.Tasks
{
    /// <summary>
    /// Types of tasks
    /// </summary>
    public enum TaskType
    {
        Delivery,
        Inspection,
        Charging,
        Patrol,
        Communication,
        Mapping,
        Custom
    }

    /// <summary>
    /// Task execution result
    /// </summary>
    public class TaskResult
    {
        public bool Success { get; set; }
        public double ActualDuration { get; set; }      // seconds
        public double EnergyConsumed { get; set; }      // percentage
        public string Message { get; set; }
        public object Data { get; set; }

        public static TaskResult SuccessResult(double duration, double energy, string message = "")
        {
            return new TaskResult { Success = true, ActualDuration = duration, EnergyConsumed = energy, Message = message };
        }

        public static TaskResult FailureResult(string message)
        {
            return new TaskResult { Success = false, Message = message };
        }
    }

    /// <summary>
    /// Main task interface
    /// </summary>
    public interface ITask
    {
        string TaskId { get; set; }
        string TaskName { get; set; }
        TaskType Type { get; }
        double EstimatedDuration { get; set; }      // seconds
        double EstimatedEnergyCost { get; set; }    // percentage
        RobotType RequiredRobotType { get; set; }

        Task<TaskResult> ExecuteAsync(RobotDefinition robot, object context = null);
        bool CanExecute(RobotDefinition robot);
        double GetEstimatedTime(RobotDefinition robot);
        double GetEstimatedEnergy(RobotDefinition robot);
        ITask Clone();
        string GetDescription();
    }
}