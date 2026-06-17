#region File Header
/// <summary>
/// File: AlgorithmFactory.cs
/// Description: Factory class for creating pathfinding algorithm instances
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Implementations;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Algorithms;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
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
        #region Public Methods

        /// <summary>
        /// Creates a pathfinder instance for the specified algorithm type (with default Manhattan metric)
        /// </summary>
        public IPathFinder Create(AlgorithmType type)
        {
            return Create(type, DistanceMetric.Manhattan);
        }

        /// <summary>
        /// Creates a pathfinder instance for the specified algorithm type with distance metric
        /// </summary>
        /// <summary>
        /// Creates a pathfinder instance for the specified algorithm type with distance metric
        /// </summary>
        public IPathFinder Create(AlgorithmType type, DistanceMetric metric)
        {
            IPathFinder finder;

            switch (type)
            {
                case AlgorithmType.AStar:
                    finder = new AStarFinder(_mapGrid);
                    break;
                case AlgorithmType.SPPA:
                    finder = new SPPAFinder(_mapGrid);
                    break;
                case AlgorithmType.SPPA_DL:
                    finder = new SPPA_DLFinder(_mapGrid, _dynamicObstacles, false, false, 2.0, 0.3);
                    break;
                case AlgorithmType.ACO:
                    finder = new ACOFinder(_mapGrid);
                    break;
                case AlgorithmType.DStar:
                    finder = new DStarFinder(_mapGrid);
                    break;
                case AlgorithmType.KNN:
                    finder = new KNNFinder(_mapGrid);
                    break;
                case AlgorithmType.BruteForce:
                    finder = new BruteForceFinder(_mapGrid);
                    break;
                case AlgorithmType.RRT:
                    finder = new RRTFinder(_mapGrid);
                    break;
                case AlgorithmType.PRM:
                    finder = new PRMFinder(_mapGrid);
                    break;
                case AlgorithmType.PSO:
                    finder = new PSOFinder(_mapGrid);
                    break;
                case AlgorithmType.GA:
                    finder = new GAFinder(_mapGrid);
                    break;
                case AlgorithmType.RRTStar:
                    finder = new RRTStarFinder(_mapGrid);
                    break;
                default:
                    finder = new AStarFinder(_mapGrid);
                    break;
            }

            if (finder != null)
            {
                finder.Metric = metric;
            }

            return finder;
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
            return Create(type);  // الآن تعمل لأن Create(type) موجودة
        }

        /// <summary>
        /// Creates a pathfinder with custom algorithm parameters
        /// </summary>
        public IPathFinder Create(AlgorithmType type, DistanceMetric metric,
            bool allowDiagonals, bool heavyDiagonals, int heuristicWeight, int searchLimit)
        {
            var finder = Create(type, metric);

            if (finder != null)
            {
                finder.AllowDiagonals = allowDiagonals;
                finder.HeavyDiagonals = heavyDiagonals;
                finder.HeuristicWeight = heuristicWeight;
                finder.SearchLimit = searchLimit;
            }

            return finder;
        }

        #endregion

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
                AlgorithmType.RRT => true,
                AlgorithmType.PRM => true,
                AlgorithmType.PSO => true,
                AlgorithmType.GA => true,
                AlgorithmType.RRTStar => true,
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
                AlgorithmType.RRT => "RRT (Rapidly-exploring Random Tree)",
                AlgorithmType.PRM => "PRM (Probabilistic Roadmap)",
                AlgorithmType.PSO => "PSO (Particle Swarm Optimization)",
                AlgorithmType.GA => "GA (Genetic Algorithm)",
                AlgorithmType.RRTStar => "RRT* (RRT-Star)",
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
                AlgorithmType.RRT => "Sample random points, expand tree, connect to nearest",
                AlgorithmType.PRM => "Build roadmap of samples, query with Dijkstra",
                AlgorithmType.PSO => "v = w*v + c1*r1*(pBest - x) + c2*r2*(gBest - x)",
                AlgorithmType.GA => "Selection, Crossover, Mutation, Elitism",
                AlgorithmType.RRTStar => "RRT with rewiring for asymptotic optimality",
                _ => "Unknown"
            };
        }
        #endregion
    }
}