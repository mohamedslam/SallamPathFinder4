#region File Header
/// <summary>
/// File: ChargingTask.cs
/// Description: Battery charging task
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-15
/// </summary>
#endregion

using SallamPathFinder4.Core.Models.Robot;

namespace SallamPathFinder4.Core.Models.Tasks
{
    /// <summary>
    /// Charging task - robot recharges its battery
    /// </summary>
    public class ChargingTask : TaskBase
    {
        #region Properties
        public override TaskType Type => TaskType.Charging;

        /// <summary>Target battery level after charging (0-100)</summary>
        public double TargetBatteryLevel { get; set; } = 100;

        /// <summary>Current battery level before charging</summary>
        public double CurrentBatteryLevel { get; set; }

        /// <summary>Charging rate (% per second)</summary>
        public double ChargingRate { get; set; } = 5.0;

        /// <summary>Charging station efficiency (0-1)</summary>
        public double ChargingEfficiency { get; set; } = 0.95;
        #endregion

        #region Constructor
        public ChargingTask()
        {
            TaskName = "Charging";  
            EstimatedDuration = 20.0;
            EstimatedEnergyCost = -10.0;  // Negative = gains energy
        }
        #endregion

        #region Public Methods
        public override async Task<TaskResult> ExecuteAsync(RobotDefinition robot, object context = null)
        {
            if (context is double currentBattery)
                CurrentBatteryLevel = currentBattery;

            double batteryNeeded = TargetBatteryLevel - CurrentBatteryLevel;
            if (batteryNeeded <= 0)
            {
                return await Task.FromResult(TaskResult.SuccessResult(0, 0, "Battery already at target level"));
            }

            double actualDuration = batteryNeeded / (ChargingRate * ChargingEfficiency);
            double energyGained = batteryNeeded;

            actualDuration = ApplyRobotMultiplier(robot, actualDuration);

            await Task.Delay((int)(actualDuration * 100));

            return TaskResult.SuccessResult(actualDuration, -energyGained,
                $"Charged from {CurrentBatteryLevel:F0}% to {TargetBatteryLevel:F0}% in {actualDuration:F1}s");
        }

        public override bool CanExecute(RobotDefinition robot)
        {
            return true;  // All robots can charge
        }

        public override ITask Clone()
        {
            return new ChargingTask
            {
                TaskId = this.TaskId,
                TaskName = this.TaskName,
                EstimatedDuration = this.EstimatedDuration,
                EstimatedEnergyCost = this.EstimatedEnergyCost,
                RequiredRobotType = this.RequiredRobotType,
                TargetBatteryLevel = this.TargetBatteryLevel,
                ChargingRate = this.ChargingRate,
                ChargingEfficiency = this.ChargingEfficiency
            };
        }

        public override string GetDescription()
        {
            return $"Charge battery to {TargetBatteryLevel:F0}% (Rate: {ChargingRate:F1}%/s)";
        }
        #endregion
    }
}