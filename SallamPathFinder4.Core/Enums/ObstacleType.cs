#region File Header
/// <summary>
/// File: ObstacleType.cs
/// Description: Defines dynamic obstacle types with their movement properties
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Enums
{
    #region Enum Documentation
    /// <summary>
    /// Types of dynamic obstacles that can move in the environment
    /// Each type has unique movement speed, randomness, radius, and weight
    /// Used for realistic obstacle behavior simulation
    /// </summary>
    #endregion
    public enum ObstacleType
    {
        /// <summary>
        /// Adult person walking
        /// Speed: moderate (1.5-2.5 cells/s)
        /// Behavior: semi-predictable, avoids robot
        /// </summary>
        Adult = 0,

        /// <summary>
        /// Child running unpredictably
        /// Speed: fast (2-3 cells/s)
        /// Behavior: highly random, may suddenly change direction
        /// Priority: high (safety critical)
        /// </summary>
        Child = 1,

        /// <summary>
        /// Animal (dog, cat, etc.)
        /// Speed: variable (1-2 cells/s)
        /// Behavior: erratic, may run away or approach
        /// </summary>
        Animal = 2,

        /// <summary>
        /// Another robot
        /// Speed: predictable (0.8-1.5 cells/s)
        /// Behavior: cooperative, follows predictable patterns
        /// Can potentially communicate with this robot
        /// </summary>
        OtherRobot = 3,

        /// <summary>
        /// Moving equipment (carts, trolleys, etc.)
        /// Speed: slow (0.3-0.7 cells/s)
        /// Behavior: linear movement, predictable
        /// Size: larger than other obstacles
        /// </summary>
        Equipment = 4
    }
}