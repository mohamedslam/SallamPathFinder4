#region File Header
/// <summary>
/// File: ColoredPath.cs
/// Description: Represents a path segment with a specific color for visualization
/// Used for multi-goal path visualization with different colors per segment
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
#endregion

namespace SallamPathFinder4.Core.Models.Path
{
    #region Class Documentation
    /// <summary>
    /// Represents a path segment with a specific color
    /// Used for visualizing paths to different goals with distinct colors
    /// Supports both forward and return paths
    /// </summary>
    #endregion
    public sealed class ColoredPath
    {
        #region Constructor
        /// <summary>
        /// Initializes a new colored path
        /// </summary>
        /// <param name="nodes">List of nodes in the path</param>
        /// <param name="color">Color to draw this path segment</param>
        /// <param name="isReturnPath">Indicates if this is a return path to parking</param>
        public ColoredPath(IEnumerable<PathNode> nodes, Color color, bool isReturnPath = false)
        {
            Nodes = nodes?.ToList().AsReadOnly() ?? new List<PathNode>().AsReadOnly();
            Color = color;
            IsReturnPath = isReturnPath;
        }
        #endregion

        #region Properties
        /// <summary>List of nodes in the path (read-only)</summary>
        public IReadOnlyList<PathNode> Nodes { get; }

        /// <summary>Color to draw this path segment</summary>
        public Color Color { get; }

        /// <summary>Indicates if this is a return path to parking</summary>
        public bool IsReturnPath { get; }

        /// <summary>Length of the path in cells</summary>
        public int Length => Nodes.Count;
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates an empty colored path
        /// </summary>
        public static ColoredPath Empty()
        {
            return new ColoredPath(null, Color.Transparent, false);
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns string representation of the colored path
        /// </summary>
        public override string ToString()
        {
            string type = IsReturnPath ? "Return" : "Forward";
            return $"{type} Path: {Length} cells, Color: {Color.Name}";
        }
        #endregion
    }
}