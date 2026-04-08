#region File Header
/// <summary>
/// File: MapElementType.cs
/// Description: Defines all possible map element types (static and semi-static)
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Enums
{
    #region Enum Documentation
    /// <summary>
    /// Types of elements that can exist on the map grid
    /// Classified into:
    /// - Static: Walls (never change)
    /// - Semi-static: Doors, Windows, Ramps (can change state)
    /// - Functional: Start, Goal, Parking (navigation points)
    /// </summary>
    #endregion
    public enum MapElementType
    {
        /// <summary>
        /// Empty cell - normal walkable area
        /// Default state with standard movement cost
        /// </summary>
        Empty = 0,

        /// <summary>
        /// Wall - completely blocked cell
        /// Static obstacle, never walkable
        /// Movement cost: infinite
        /// </summary>
        Wall = 1,

        /// <summary>
        /// Door - semi-static obstacle
        /// Walkable only when open
        /// State can change during simulation
        /// </summary>
        Door = 2,

        /// <summary>
        /// Window - transparent obstacle
        /// Walkable but with higher cost
        /// Represents glass windows or barriers
        /// </summary>
        Window = 3,

        /// <summary>
        /// Ramp - inclined surface
        /// Walkable with speed reduction
        /// Difficulty level (1-100) affects movement cost
        /// </summary>
        Ramp = 4,

        /// <summary>
        /// Start point - robot initial position
        /// Only one per map
        /// </summary>
        StartPoint = 5,

        /// <summary>
        /// Goal point - destination to visit
        /// Multiple goals can exist on same map
        /// Each goal has unique number and color
        /// </summary>
        GoalPoint = 6,

        /// <summary>
        /// Parking point - charging station
        /// Robot returns here after completing goals
        /// Multiple parking points can exist
        /// </summary>
        ParkingPoint = 7
    }
}