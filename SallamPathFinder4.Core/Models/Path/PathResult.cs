#region File Header
/// <summary>
/// File: PathResult.cs
/// Description: Result container for pathfinding operations
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace SallamPathFinder4.Core.Models.Path
{
    #region Class Documentation
    /// <summary>
    /// Contains the result of a pathfinding operation
    /// Immutable design for thread safety and predictability
    /// </summary>
    #endregion
    public sealed class PathResult
    {
        #region Constructor
        /// <summary>
        /// Initializes a successful path result
        /// </summary>
        /// <param name="path">The found path (list of nodes)</param>
        /// <param name="computationTimeSeconds">Time taken to compute the path</param>
        /// <param name="nodesExplored">Number of nodes explored during search</param>
        public PathResult(IReadOnlyList<PathNode> path, double computationTimeSeconds, int nodesExplored)
            : this(path, computationTimeSeconds, nodesExplored, null)
        {
        }

        /// <summary>
        /// Initializes a path result with optional error message
        /// </summary>
        /// <param name="path">The found path (null if no path exists)</param>
        /// <param name="computationTimeSeconds">Time taken to compute the path</param>
        /// <param name="nodesExplored">Number of nodes explored during search</param>
        /// <param name="errorMessage">Error message if pathfinding failed (null if successful)</param>
        public PathResult(IReadOnlyList<PathNode> path, double computationTimeSeconds, int nodesExplored, string errorMessage)
        {
            Path = path?.ToList().AsReadOnly();
            ComputationTimeSeconds = computationTimeSeconds;
            NodesExplored = nodesExplored;
            ErrorMessage = errorMessage;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The found path as a read-only list
        /// Null if no path exists or an error occurred
        /// </summary>
        public IReadOnlyList<PathNode> Path { get; }

        /// <summary>
        /// Time taken to compute the path in seconds
        /// </summary>
        public double ComputationTimeSeconds { get; }

        /// <summary>
        /// Number of nodes explored during the search
        /// Useful for algorithm performance comparison
        /// </summary>
        public int NodesExplored { get; }

        /// <summary>
        /// Error message if pathfinding failed
        /// Null if successful
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Indicates whether a valid path was found
        /// </summary>
        public bool Success => Path != null && Path.Count > 0 && string.IsNullOrEmpty(ErrorMessage);

        /// <summary>
        /// Total length of the path in cells
        /// Returns 0 if no path exists
        /// </summary>
        public int PathLength => Path?.Count ?? 0;
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a failed path result with the specified error message
        /// </summary>
        public static PathResult Fail(string errorMessage, double computationTimeSeconds = 0)
        {
            return new PathResult(null, computationTimeSeconds, 0, errorMessage);
        }

        /// <summary>
        /// Creates an empty path result (no path found, no error)
        /// </summary>
        public static PathResult Empty(double computationTimeSeconds = 0)
        {
            return new PathResult(null, computationTimeSeconds, 0, "No path found");
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns string representation of the path result
        /// </summary>
        public override string ToString()
        {
            if (Success)
            {
                return $"Path found: {PathLength} cells, {ComputationTimeSeconds * 1000:F2}ms, {NodesExplored} nodes explored";
            }

            return $"Path failed: {ErrorMessage ?? "Unknown error"}";
        }
        #endregion
    }
}