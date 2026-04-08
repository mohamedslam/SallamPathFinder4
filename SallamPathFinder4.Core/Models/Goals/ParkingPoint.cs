#region File Header
/// <summary>
/// File: ParkingPoint.cs
/// Description: Represents a parking/charging point for the robot
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
    /// Represents a parking/charging point where the robot can recharge
    /// Each parking point has a unique number, name, location, and availability state
    /// </summary>
    #endregion
    public sealed class ParkingPoint
    {
        #region Private Fields
        private int _number;
        private string _name;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new parking point
        /// </summary>
        /// <param name="number">Unique parking number (1-based)</param>
        /// <param name="location">Grid coordinates of the parking point</param>
        public ParkingPoint(int number, Point location)
        {
            _number = number;
            _name = $"P{number}";
            Location = location;
            IsAvailable = true;
            IsOccupied = false;
        }
        #endregion

        #region Properties
        /// <summary>Unique parking number (1-based)</summary>
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                _name = $"P{value}";
            }
        }

        /// <summary>Parking name (e.g., "P1", "P2")</summary>
        public string Name => _name;

        /// <summary>Grid coordinates of the parking point</summary>
        public Point Location { get; set; }

        /// <summary>Whether the parking point is available for use</summary>
        public bool IsAvailable { get; set; }

        /// <summary>Whether the parking point is currently occupied by the robot</summary>
        public bool IsOccupied { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Occupies the parking point (robot starts charging)
        /// </summary>
        public void Occupy()
        {
            IsOccupied = true;
            IsAvailable = false;
        }

        /// <summary>
        /// Releases the parking point (robot leaves)
        /// </summary>
        public void Release()
        {
            IsOccupied = false;
            IsAvailable = true;
        }

        /// <summary>
        /// Resets the parking point state
        /// </summary>
        public void Reset()
        {
            IsOccupied = false;
            IsAvailable = true;
        }

        /// <summary>
        /// Creates a deep copy of the parking point
        /// </summary>
        public ParkingPoint Clone()
        {
            return new ParkingPoint(_number, Location)
            {
                IsAvailable = this.IsAvailable,
                IsOccupied = this.IsOccupied
            };
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns string representation of the parking point
        /// </summary>
        public override string ToString()
        {
            return $"{Name} at ({Location.X},{Location.Y})";
        }
        #endregion
    }
}