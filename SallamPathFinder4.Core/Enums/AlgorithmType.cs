#region File Header
/// <summary>
/// File: AlgorithmType.cs
/// Description: Defines all supported pathfinding algorithm types
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-21
/// </summary>
#endregion

#region Namespace Imports
using System.ComponentModel;
#endregion

namespace SallamPathFinder4.Core.Enums
{
    #region Enum Documentation
    /// <summary>
    /// Supported pathfinding algorithm types
    /// Used for algorithm selection and factory pattern
    /// Total algorithms: 12
    /// </summary>
    #endregion
    public enum AlgorithmType
    {
        /// <summary>
        /// A* (A-Star) algorithm - Standard informed search
        /// Uses heuristic function: f(n) = g(n) + h(n)
        /// Best for static environments
        /// </summary>
        [Description("A* (A-Star) - f(n) = g(n) + h(n)")]
        AStar = 0,

        /// <summary>
        /// SPPA (Shortest Path with Precautionary Avoidance)
        /// Extended cost function: f(n) = g(n) + h(n) + λ·o(n)
        /// Handles static and semi-static obstacles
        /// Reference: Makarovskikh T., Sallam M. (2024)
        /// </summary>
        [Description("SPPA - f(n) = g(n) + h(n) + λ·o(n)")]
        SPPA = 1,

        /// <summary>
        /// SPPA-DL (SPPA with Dynamic Learning)
        /// Extended cost function: f(n) = g(n) + h(n) + λ·o(n) + α·m(n)
        /// Includes obstacle memory and learning from past simulations
        /// Reference: Makarovskikh T., Sallam M. (2025)
        /// </summary>
        [Description("SPPA-DL - f(n) = g(n) + h(n) + λ·o(n) + α·m(n)")]
        SPPA_DL = 2,

        /// <summary>
        /// ACO (Ant Colony Optimization)
        /// Swarm-based algorithm using pheromone trails
        /// Best for finding near-optimal paths in complex environments
        /// Formula: P_ij = [τ_ij]^α · [η_ij]^β / Σ[τ_il]^α · [η_il]^β
        /// </summary>
        [Description("ACO - P_ij = [τ_ij]^α · [η_ij]^β / Σ[τ_il]^α · [η_il]^β")]
        ACO = 3,

        /// <summary>
        /// D* (Dynamic A*)
        /// Designed for dynamic environments with changing obstacles
        /// Efficiently replans only affected parts of the path
        /// Formula: f(n) = g(n) + h(n) + d(n)
        /// </summary>
        [Description("D* - f(n) = g(n) + h(n) + d(n)")]
        DStar = 4,

        /// <summary>
        /// KNN (K-Nearest Neighbors)
        /// Simple heuristic-based local search
        /// Fast but not guaranteed optimal
        /// Formula: d(x,y) = √Σ(x_i - y_i)²
        /// </summary>
        [Description("KNN - d(x,y) = √Σ(x_i - y_i)²")]
        KNN = 5,

        /// <summary>
        /// Brute Force Search
        /// Exhaustive search exploring all possibilities
        /// Only suitable for very small maps (≤ 50x50)
        /// Formula: min Σ cost(path)
        /// </summary>
        [Description("Brute Force - min Σ cost(path)")]
        BruteForce = 6,

        /// <summary>
        /// RRT (Rapidly-exploring Random Tree)
        /// Fast path planning for continuous spaces
        /// Method: Sample random points, expand tree, connect to nearest
        /// Reference: LaValle, S. M. (1998)
        /// </summary>
        [Description("RRT - Sample random points, expand tree, connect to nearest")]
        RRT = 7,

        /// <summary>
        /// PRM (Probabilistic Roadmap)
        /// Multi-query path planning
        /// Method: Build roadmap of samples, query with Dijkstra
        /// Reference: Kavraki et al. (1996)
        /// </summary>
        [Description("PRM - Build roadmap of samples, query with Dijkstra")]
        PRM = 8,

        /// <summary>
        /// PSO (Particle Swarm Optimization)
        /// Swarm-based optimization inspired by bird flocking
        /// Formula: v = w·v + c1·r1·(pBest - x) + c2·r2·(gBest - x)
        /// Reference: Kennedy & Eberhart (1995)
        /// </summary>
        [Description("PSO - v = w·v + c1·r1·(pBest - x) + c2·r2·(gBest - x)")]
        PSO = 9,

        /// <summary>
        /// GA (Genetic Algorithm)
        /// Evolutionary algorithm based on natural selection
        /// Operations: Selection, Crossover, Mutation, Elitism
        /// Reference: Holland (1975)
        /// </summary>
        [Description("GA - Selection + Crossover + Mutation + Elitism")]
        GA = 10,

        /// <summary>
        /// RRT* (RRT-Star)
        /// Asymptotically optimal version of RRT
        /// Features: Rewiring, Informed sampling
        /// Reference: Karaman & Frazzoli (2011)
        /// </summary>
        [Description("RRT* - RRT with rewiring for asymptotic optimality")]
        RRTStar = 11
    }
}