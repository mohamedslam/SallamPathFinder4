#region File Header
/// <summary>
/// File: PathNode.cs
/// Description: Represents a single node in a path
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.Core.Models.Path
{
    #region Struct Documentation
    /// <summary>
    /// Represents a single node in a path
    /// Using struct for memory efficiency (value type, no heap allocation)
    /// Immutable design for thread safety
    /// </summary>
    #endregion
    public readonly struct PathNode : IEquatable<PathNode>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new path node at the specified coordinates
        /// </summary>
        /// <param name="x">X coordinate (column)</param>
        /// <param name="y">Y coordinate (row)</param>
        public PathNode(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Properties
        /// <summary>X coordinate (column index)</summary>
        public int X { get; }

        /// <summary>Y coordinate (row index)</summary>
        public int Y { get; }
        #endregion

        #region IEquatable Implementation
        /// <summary>Indicates whether the current node is equal to another node</summary>
        public bool Equals(PathNode other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>Determines whether the specified object is equal to the current node</summary>
        public override bool Equals(object obj)
        {
            return obj is PathNode other && Equals(other);
        }

        /// <summary>Returns the hash code for this node</summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        #endregion

        #region Operators
        /// <summary>Equality operator</summary>
        public static bool operator ==(PathNode left, PathNode right)
        {
            return left.Equals(right);
        }

        /// <summary>Inequality operator</summary>
        public static bool operator !=(PathNode left, PathNode right)
        {
            return !left.Equals(right);
        }
        #endregion

        #region Object Overrides
        /// <summary>Returns string representation of the node</summary>
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
        #endregion
    }
}