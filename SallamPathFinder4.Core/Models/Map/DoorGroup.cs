#region File Header
/// <summary>
/// File: DoorGroup.cs
/// Description: Represents a group of adjacent door cells that act as a single door
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System.Collections.Generic;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Models.Map
{
    /// <summary>
    /// Represents a group of adjacent door cells that open and close together
    /// </summary>
    public sealed class DoorGroup
    {
        #region Constructor
        /// <summary>
        /// Initializes a new door group
        /// </summary>
        /// <param name="cells">List of cell coordinates in this group</param>
        public DoorGroup(List<Point> cells)
        {
            Cells = cells ?? new List<Point>();
            IsOpen = true;
            UpdateIntervalSeconds = 0;
        }
        #endregion

        #region Properties
        /// <summary>
        /// List of cell coordinates in this door group
        /// </summary>
        public List<Point> Cells { get; set; }

        /// <summary>
        /// Indicates whether the door is currently open
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Time interval for next state change in seconds
        /// </summary>
        public double UpdateIntervalSeconds { get; set; }

        /// <summary>
        /// Unique identifier for the door group
        /// </summary>
        public int Id { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a string representation of the door group
        /// </summary>
        public override string ToString()
        {
            return $"DoorGroup {Id}: {Cells.Count} cells, {(IsOpen ? "Open" : "Closed")}";
        }
        #endregion
    }
}