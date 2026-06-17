#region File Header
/// <summary>
/// File: DeliveryTask.cs
/// Description: Delivery task implementation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Robot;
using System.Drawing;

namespace SallamPathFinder4.Core.Models.Tasks
{
    /// <summary>
    /// Delivery task - robot delivers a package to a location
    /// </summary>
    public class DeliveryTask : TaskBase
    {
        #region Properties
        public override TaskType Type => TaskType.Delivery;

        /// <summary>Package weight in kg</summary>
        public double PackageWeight { get; set; } = 1.0;

        /// <summary>Delivery location (if different from waypoint)</summary>
        public Point? DeliveryLocation { get; set; }

        /// <summary>Recipient name</summary>
        public string Recipient { get; set; } = string.Empty;

        /// <summary>Package ID</summary>
        public string PackageId { get; set; } = Guid.NewGuid().ToString();
        #endregion

        #region Constructor
        public DeliveryTask()
        {
            TaskName = "Delivery";  // ✅ تأكد من تعيين TaskName
            EstimatedDuration = 10.0;
            EstimatedEnergyCost = 3.0;
        }
        #endregion

        #region Public Methods
        public override async Task<TaskResult> ExecuteAsync(RobotDefinition robot, object context = null)
        {
            // Simulate delivery process
            double actualDuration = EstimatedDuration;
            double actualEnergy = EstimatedEnergyCost;

            // Weight affects energy consumption
            actualEnergy *= (1 + PackageWeight / 10.0);

            // Robot type affects duration
            actualDuration = ApplyRobotMultiplier(robot, actualDuration);

            await Task.Delay((int)(actualDuration * 100));

            return TaskResult.SuccessResult(actualDuration, actualEnergy, $"Package {PackageId} delivered to {Recipient}");
        }

        public override bool CanExecute(RobotDefinition robot)
        {
            if (!base.CanExecute(robot)) return false;

            // Only robots with cargo capacity can deliver
            // This would check robot's cargo capacity in a real implementation
            return PackageWeight <= 5.0; // Max 5kg
        }

        public override ITask Clone()
        {
            return new DeliveryTask
            {
                TaskId = this.TaskId,
                TaskName = this.TaskName,
                EstimatedDuration = this.EstimatedDuration,
                EstimatedEnergyCost = this.EstimatedEnergyCost,
                RequiredRobotType = this.RequiredRobotType,
                PackageWeight = this.PackageWeight,
                DeliveryLocation = this.DeliveryLocation,
                Recipient = this.Recipient,
                PackageId = this.PackageId
            };
        }

        public override string GetDescription()
        {
            return $"Deliver package {PackageId} to {Recipient} (Weight: {PackageWeight}kg)";
        }
        #endregion
    }
}