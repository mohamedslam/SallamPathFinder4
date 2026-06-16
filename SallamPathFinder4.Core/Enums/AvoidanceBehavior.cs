#region File Header
/// <summary>
/// File: AvoidanceBehavior.cs
/// Description: Defines robot behavior when obstacle is detected
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-06-01
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Enums
{
    /// <summary>
    /// Behavior types for obstacle avoidance
    /// </summary>
    public enum AvoidanceBehavior
    {
        /// <summary>No action needed</summary>
        None = 0,

        /// <summary>Reduce robot speed</summary>
        SlowDown = 1,

        /// <summary>Stop and wait for obstacle to clear</summary>
        Stop = 2,

        /// <summary>Stop immediately (emergency)</summary>
        EmergencyStop = 3,

        /// <summary>Avoid obstacle then return to original path</summary>
        ReplanTemporary = 4,

        /// <summary>Calculate completely new path</summary>
        ReplanPermanent = 5
    }
}