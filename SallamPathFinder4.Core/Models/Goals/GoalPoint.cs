#region File Header
/// <summary>
/// File: GoalPoint.cs
/// Description: Represents a goal point that the robot must visit
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Models.Goals
{
    #region Class Documentation
    /// <summary>
    /// Represents a goal point that the robot must visit
    /// Each goal has a unique number, name, location, color, and active state
    /// Supports multiple goals with different colors for visualization
    /// </summary>
    #endregion
    public sealed class GoalPoint
    {
        #region Private Fields
        private int _number;
        private string _name;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new goal point
        /// </summary>
        /// <param name="number">Unique goal number (1-based)</param>
        /// <param name="location">Grid coordinates of the goal</param>
        /// <param name="color">Color for visualization</param>
        public GoalPoint(int number, Point location, Color color)
        {
            _number = number;
            _name = $"G{number}";
            Location = location;
            Color = color;
            IsActive = true;
            IsVisited = false;
        }
        #endregion

        #region Properties
        /// <summary>Unique goal number (1-based)</summary>
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                _name = $"G{value}";
            }
        }

        /// <summary>Goal name (e.g., "G1", "G2")</summary>
        public string Name => _name;

        /// <summary>Grid coordinates of the goal</summary>
        public Point Location { get; set; }

        /// <summary>Color for visualization on the map</summary>
        public Color Color { get; set; }

        /// <summary>Whether the goal is currently active</summary>
        public bool IsActive { get; set; }

        /// <summary>Whether the goal has been visited by the robot</summary>
        public bool IsVisited { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Marks the goal as visited
        /// </summary>
        public void MarkVisited()
        {
            IsVisited = true;
        }

        /// <summary>
        /// Resets the visited state (for new simulation)
        /// </summary>
        public void Reset()
        {
            IsVisited = false;
            IsActive = true;
        }

        /// <summary>
        /// Creates a deep copy of the goal point
        /// </summary>
        public GoalPoint Clone()
        {
            return new GoalPoint(_number, Location, Color)
            {
                IsActive = this.IsActive,
                IsVisited = this.IsVisited
            };
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns string representation of the goal
        /// </summary>
        public override string ToString()
        {
            return $"{Name} at ({Location.X},{Location.Y})";
        }
        #endregion
    }
}