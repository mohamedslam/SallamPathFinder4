#region File Header
/// <summary>
/// File: AlgorithmFactory.cs
/// Description: Factory class for creating pathfinding algorithm instances
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using System;
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Algorithms.Implementations;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Services.Pathfinding;
#endregion

namespace SallamPathFinder4.Services.Pathfinding
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
        private readonly List<DynamicObstacle> _dynamicObstacles;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new algorithm factory
        /// </summary>
        /// <param name="grid">Map grid for pathfinding</param>
        public AlgorithmFactory(MapGrid grid)
        {
            _mapGrid = grid ?? throw new ArgumentNullException(nameof(grid));
            _dynamicObstacles = new List<DynamicObstacle>();
        }

        /// <summary>
        /// Initializes a new algorithm factory with dynamic obstacles for SPPA-DL
        /// </summary>
        public AlgorithmFactory(MapGrid grid, List<DynamicObstacle> dynamicObstacles)
        {
            _mapGrid = grid ?? throw new ArgumentNullException(nameof(grid));
            _dynamicObstacles = dynamicObstacles ?? new List<DynamicObstacle>();
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
                AlgorithmType.SPPA_DL => new SPPA_DLFinder(_mapGrid, _dynamicObstacles, false, false, 2.0, 0.3),
                AlgorithmType.ACO => new ACOFinder(_mapGrid),
                AlgorithmType.DStar => new DStarFinder(_mapGrid),
                AlgorithmType.KNN => new KNNFinder(_mapGrid),
                AlgorithmType.BruteForce => new BruteForceFinder(_mapGrid),
                _ => new AStarFinder(_mapGrid)
            };
        }

        /// <summary>
        /// Creates a pathfinder with ML options for SPPA-DL
        /// </summary>
        public IPathFinder Create(AlgorithmType type, bool useNeuralNetwork, bool collectTrainingData, double learningRate, double predictionWeight = 0.3)
        {
            if (type == AlgorithmType.SPPA_DL)
            {
                return new SPPA_DLFinder(_mapGrid, _dynamicObstacles, useNeuralNetwork, collectTrainingData, learningRate, predictionWeight);
            }
            return Create(type);
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
        public static bool IsAlgorithmAvailable(AlgorithmType type)
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

        /// <summary>
        /// Gets the mathematical formula for the algorithm
        /// </summary>
        public static string GetAlgorithmFormula(AlgorithmType type)
        {
            return type switch
            {
                AlgorithmType.AStar => "f(n) = g(n) + h(n)",
                AlgorithmType.SPPA => "f(n) = g(n) + h(n) + λ·o(n)",
                AlgorithmType.SPPA_DL => "f(n) = g(n) + h(n) + λ·o(n) + α·m(n) + β·p(n)",
                AlgorithmType.ACO => "P_ij = [τ_ij]^α · [η_ij]^β / Σ[τ_il]^α · [η_il]^β",
                AlgorithmType.DStar => "f(n) = g(n) + h(n) + d(n)",
                AlgorithmType.KNN => "d(x,y) = √Σ(x_i - y_i)²",
                AlgorithmType.BruteForce => "min Σ cost(path)",
                _ => "Unknown"
            };
        }
        #endregion
    }
}