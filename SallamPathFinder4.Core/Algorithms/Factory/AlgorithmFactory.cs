#region File Header
/// <summary>
/// File: AlgorithmFactory.cs
/// Description: Factory class for creating pathfinding algorithm instances
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Algorithms.Implementations;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Models.Map;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Factory
{
    #region Class Documentation
    /// <summary>
    /// Factory class for creating pathfinding algorithm instances
    /// Supports A*, SPPA, SPPA-DL, ACO, D*, KNN, and Brute Force
    /// Follows Factory Design Pattern
    /// </summary>
    #endregion
    public sealed class AlgorithmFactory
    {
        #region Private Fields
        private readonly MapGrid _mapGrid;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new algorithm factory
        /// </summary>
        /// <param name="grid">Map grid for pathfinding</param>
        public AlgorithmFactory(MapGrid grid)
        {
            _mapGrid = grid ?? throw new ArgumentNullException(nameof(grid));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a pathfinder instance for the specified algorithm type
        /// </summary>
        public IPathFinder Create(AlgorithmType type)
        {
            return type switch
            {
                AlgorithmType.AStar => new AStarFinder(_mapGrid),
                AlgorithmType.SPPA => new SPPAFinder(_mapGrid),
                //AlgorithmType.SPPA_DL => new SPPA_DLFinder(_mapGrid),
                AlgorithmType.ACO => new ACOFinder(_mapGrid),
                AlgorithmType.DStar => new DStarFinder(_mapGrid),
                AlgorithmType.KNN => new KNNFinder(_mapGrid),
                AlgorithmType.BruteForce => new BruteForceFinder(_mapGrid),
                _ => new AStarFinder(_mapGrid)
            };
        }

        /// <summary>
        /// Creates a pathfinder with custom algorithm parameters
        /// </summary>
        public IPathFinder Create(AlgorithmType type, DistanceMetric metric,
            bool allowDiagonals, bool heavyDiagonals, int heuristicWeight, int searchLimit)
        {
            var finder = Create(type);

            finder.Metric = metric;
            finder.AllowDiagonals = allowDiagonals;
            finder.HeavyDiagonals = heavyDiagonals;
            finder.HeuristicWeight = heuristicWeight;
            finder.SearchLimit = searchLimit;

            return finder;
        }

        /// <summary>
        /// Checks if the specified algorithm is available
        /// </summary>
        public bool IsAlgorithmAvailable(AlgorithmType type)
        {
            return type switch
            {
                AlgorithmType.AStar => true,
                AlgorithmType.SPPA => true,
                AlgorithmType.SPPA_DL => true,
                AlgorithmType.ACO => true,
                AlgorithmType.DStar => true,
                AlgorithmType.KNN => true,
                AlgorithmType.BruteForce => true,
                _ => false
            };
        }

        /// <summary>
        /// Gets the display name of the algorithm
        /// </summary>
        public static string GetAlgorithmDisplayName(AlgorithmType type)
        {
            return type switch
            {
                AlgorithmType.AStar => "A* (A-Star)",
                AlgorithmType.SPPA => "SPPA",
                AlgorithmType.SPPA_DL => "SPPA-DL",
                AlgorithmType.ACO => "Ant Colony Optimization",
                AlgorithmType.DStar => "D* (Dynamic A*)",
                AlgorithmType.KNN => "K-Nearest Neighbors",
                AlgorithmType.BruteForce => "Brute Force",
                _ => "Unknown"
            };
        }
        #endregion
    }
}