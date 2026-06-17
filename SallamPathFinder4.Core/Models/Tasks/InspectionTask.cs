#region File Header
/// <summary>
/// File: InspectionTask.cs
/// Description: Inspection task for scanning/checking areas
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.Core.Models.Sensors;

namespace SallamPathFinder4.Core.Models.Tasks
{
    /// <summary>
    /// Inspection task types
    /// </summary>
    public enum InspectionType
    {
        Visual,
        Thermal,
        Acoustic,
        Chemical,
        Structural
    }

    /// <summary>
    /// Inspection task - robot inspects a location
    /// </summary>
    public class InspectionTask : TaskBase
    {
        #region Properties
        public override TaskType Type => TaskType.Inspection;

        /// <summary>Type of inspection to perform</summary>
        public InspectionType InspectionType { get; set; } = InspectionType.Visual;

        /// <summary>Required accuracy (0-100)</summary>
        public int RequiredAccuracy { get; set; } = 80;

        /// <summary>Inspection result (successful if accuracy met)</summary>
        public int ActualAccuracy { get; private set; }

        /// <summary>Notes from inspection</summary>
        public string InspectionNotes { get; set; } = string.Empty;
        #endregion

        #region Constructor
        public InspectionTask()
        {
            TaskName = "Inspection";  // ✅ تأكد من تعيين TaskName
            EstimatedDuration = 15.0;
            EstimatedEnergyCost = 5.0;
        }
        #endregion

        #region Public Methods
        public override async Task<TaskResult> ExecuteAsync(RobotDefinition robot, object context = null)
        {
            var random = new Random();

            // Simulate inspection process
            ActualAccuracy = random.Next(50, 101);
            bool success = ActualAccuracy >= RequiredAccuracy;

            double actualDuration = EstimatedDuration;
            double actualEnergy = EstimatedEnergyCost;

            // Robot with camera sensor performs better
            var hasCamera = robot.Sensors.Any(s => s.SensorType == "Camera");
            if (hasCamera && InspectionType == InspectionType.Visual)
            {
                actualDuration *= 0.7;
                success = true;
            }

            actualDuration = ApplyRobotMultiplier(robot, actualDuration);

            await Task.Delay((int)(actualDuration * 100));

            if (success)
            {
                return TaskResult.SuccessResult(actualDuration, actualEnergy,
                    $"{InspectionType} inspection completed with {ActualAccuracy}% accuracy");
            }
            else
            {
                return TaskResult.FailureResult($"{InspectionType} inspection failed: only {ActualAccuracy}% accuracy (required {RequiredAccuracy}%)");
            }
        }

        public override ITask Clone()
        {
            return new InspectionTask
            {
                TaskId = this.TaskId,
                TaskName = this.TaskName,
                EstimatedDuration = this.EstimatedDuration,
                EstimatedEnergyCost = this.EstimatedEnergyCost,
                RequiredRobotType = this.RequiredRobotType,
                InspectionType = this.InspectionType,
                RequiredAccuracy = this.RequiredAccuracy
            };
        }

        public override string GetDescription()
        {
            return $"{InspectionType} inspection (Required accuracy: {RequiredAccuracy}%)";
        }
        #endregion
    }
}