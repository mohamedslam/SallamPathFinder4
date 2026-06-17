#region File Header
/// <summary>
/// File: AlgorithmParametersRegistry.cs
/// Description: Registry of all configurable parameters for all 12 algorithms
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace SallamPathFinder4.Core.Models.Algorithms
{
    #region Class Documentation
    /// <summary>
    /// Registry of all configurable parameters for all algorithms
    /// Centralized source of truth for algorithm configuration
    /// </summary>
    #endregion
    public static class AlgorithmParametersRegistry
    {
        #region Private Fields
        private static Dictionary<string, List<AlgorithmParameterDefinition>> _parameters;
        private static Dictionary<string, string> _algorithmFormulas;
        private static Dictionary<string, string> _algorithmDescriptions;
        #endregion

        #region Static Constructor
        static AlgorithmParametersRegistry()
        {
            InitializeParameters();
            InitializeFormulas();
            InitializeDescriptions();
        }
        #endregion

        #region Private Methods - Initialization
        private static void InitializeParameters()
        {
            _parameters = new Dictionary<string, List<AlgorithmParameterDefinition>>();

            // ========== Common Parameters (shared across all algorithms) ==========
            var commonParams = new List<AlgorithmParameterDefinition>
            {
                new AlgorithmParameterDefinition
                {
                    Name = "HeuristicWeight",
                    DisplayName = "Heuristic Weight",
                    Type = ParameterType.Integer,
                    DefaultValue = 2,
                    MinValue = 1,
                    MaxValue = 10,
                    Description = "Weight multiplier for heuristic function h(n). Higher values make the search more greedy (faster but less optimal). Lower values make it more thorough (slower but more optimal).",
                    Formula = "f(n) = g(n) + w × h(n)"
                },
                new AlgorithmParameterDefinition
                {
                    Name = "SearchLimit",
                    DisplayName = "Search Limit",
                    Type = ParameterType.Integer,
                    DefaultValue = 50000,
                    MinValue = 1000,
                    MaxValue = 1000000,
                    Description = "Maximum number of nodes to explore before giving up. Increase for complex maps, decrease for faster execution.",
                    Formula = "Stops after exploring N nodes"
                },
                 new AlgorithmParameterDefinition
                {
                    Name = "SequentialMode",
                    DisplayName = "Sequential Mode",
                    Type = ParameterType.Boolean,
                    DefaultValue = false,
                    Description = "Each iteration starts from the previous iteration's endpoint"
                },
                new AlgorithmParameterDefinition
                {
                    Name = "AllowDiagonals",
                    DisplayName = "Allow Diagonal Movement",
                    Type = ParameterType.Boolean,
                    DefaultValue = true,
                    Description = "Allow 8-directional movement (including diagonals). When disabled, only 4-directional movement (up/down/left/right) is allowed.",
                    Formula = "8-dir vs 4-dir movement"
                },
                new AlgorithmParameterDefinition
                {
                    Name = "HeavyDiagonals",
                    DisplayName = "Heavy Diagonals",
                    Type = ParameterType.Boolean,
                    DefaultValue = false,
                    Description = "When enabled, diagonal movement costs 1.414× (√2) instead of 1.0. This makes diagonal paths more expensive and may encourage axis-aligned paths.",
                    Formula = "Cost(diagonal) = Cost(straight) × √2"
                },
                new AlgorithmParameterDefinition
                {
                    Name = "DistanceMetric",
                    DisplayName = "Distance Metric",
                    Type = ParameterType.Enum,
                    DefaultValue = "Manhattan",
                    EnumOptions = new List<string> { "Manhattan", "Euclidean", "MaxDXDY", "DiagonalShortcut", "EuclideanNoSQR" },
                    Description = "Method for calculating heuristic distance to goal.\n- Manhattan: |dx| + |dy| (best for 4-dir movement)\n- Euclidean: √(dx² + dy²) (best for 8-dir movement)\n- MaxDXDY: max(|dx|,|dy|) (Chebyshev)\n- DiagonalShortcut: 2·min(dx,dy) + |dx-dy|\n- EuclideanNoSQR: dx² + dy² (faster, may overestimate)",
                    Formula = "h(n) = distance(start, goal) using selected metric"
                },
                new AlgorithmParameterDefinition
                {
                    Name = "OrderGoalsByDistance",
                    DisplayName = "Order Goals by Distance",
                    Type = ParameterType.Boolean,
                    DefaultValue = false
                },

            };

            // ========== A* (AStar) Parameters ==========
            var astarParams = new List<AlgorithmParameterDefinition>(commonParams);
           
            _parameters["AStar"] = astarParams;
            // ========== SPPA Parameters ==========
            var sppaParams = new List<AlgorithmParameterDefinition>(commonParams);
            sppaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Lambda",
                DisplayName = "Lambda (λ)",
                Type = ParameterType.Double,
                DefaultValue = 2.0,
                MinValue = 0.5,
                MaxValue = 10.0,
                Description = "Weight for obstacle coefficient o(n). Higher values = stronger obstacle avoidance (longer paths). Lower values = shorter paths but may pass near obstacles.",
                Formula = "f(n) = g(n) + h(n) + λ × o(n)"
            });
            sppaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "AlphaS",
                DisplayName = "Alpha_S (αₛ)",
                Type = ParameterType.Double,
                DefaultValue = 1.0,
                MinValue = 0.1,
                MaxValue = 2.0,
                Description = "Weight for static obstacles (walls). Higher values make walls more repulsive.",
                Formula = "o_static(n) = αₛ × IsWall"
            });
            sppaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "AlphaSS",
                DisplayName = "Alpha_SS (αₛₛ)",
                Type = ParameterType.Double,
                DefaultValue = 0.7,
                MinValue = 0.1,
                MaxValue = 2.0,
                Description = "Weight for semi-static obstacles (doors, windows, ramps). Higher values make these obstacles more repulsive.",
                Formula = "o_semi(n) = αₛₛ × (RampDifficulty/100)"
            });
            sppaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "AlphaD",
                DisplayName = "Alpha_D (αᵈ)",
                Type = ParameterType.Double,
                DefaultValue = 0.5,
                MinValue = 0.1,
                MaxValue = 2.0,
                Description = "Weight for dynamic obstacles (people, animals, other robots). Higher values make moving obstacles more repulsive.",
                Formula = "o_dynamic(n) = αᵈ × (1 / distance)"
            }); 
            _parameters["SPPA"] = sppaParams;

            // ========== SPPA-DL Parameters ==========
            var sppaDLParams = new List<AlgorithmParameterDefinition>(sppaParams);
            sppaDLParams.Add(new AlgorithmParameterDefinition
            {
                Name = "LearningRate",
                DisplayName = "Learning Rate (α)",
                Type = ParameterType.Double,
                DefaultValue = 2.0,
                MinValue = 0.5,
                MaxValue = 8.0,
                Description = "Weight for learning memory m(n). Higher values = stronger influence from past obstacle detections.",
                Formula = "f(n) = ... + α × m(n) where m(n) = α × (Frequency / TotalSimulations)"
            });
            sppaDLParams.Add(new AlgorithmParameterDefinition
            {
                Name = "PredictionWeight",
                DisplayName = "Prediction Weight (β)",
                Type = ParameterType.Double,
                DefaultValue = 0.3,
                MinValue = 0.0,
                MaxValue = 1.0,
                Description = "Weight for neural network prediction risk p(n). Higher values = stronger avoidance of predicted obstacle positions.",
                Formula = "f(n) = ... + β × p(n) where p(n) = prediction_confidence × (1 - distance/2)"
            });
            sppaDLParams.Add(new AlgorithmParameterDefinition
            {
                Name = "UseNeuralNetwork",
                DisplayName = "Use Neural Network",
                Type = ParameterType.Boolean,
                DefaultValue = false,
                Description = "Enable neural network prediction for dynamic obstacle movement. When disabled, only basic prediction (linear extrapolation) is used.",
                Formula = "p(n) = 0 when disabled"
            });
            _parameters["SPPA_DL"] = sppaDLParams;

            // ========== ACO Parameters ==========
            var acoParams = new List<AlgorithmParameterDefinition>(commonParams);
            acoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Ants",
                DisplayName = "Number of Ants",
                Type = ParameterType.Integer,
                DefaultValue = 20,
                MinValue = 1,
                MaxValue = 200,
                Description = "Number of artificial ants in the colony. More ants = better solutions but slower execution.",
                Formula = "Each ant explores paths independently"
            });
            acoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Iterations",
                DisplayName = "Max Iterations",
                Type = ParameterType.Integer,
                DefaultValue = 100,
                MinValue = 10,
                MaxValue = 500,
                Description = "Maximum number of iterations (generations). More iterations = better solutions but slower.",
                Formula = "Algorithm runs for N iterations"
            });
            acoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "EvaporationRate",
                DisplayName = "Evaporation Rate (ρ)",
                Type = ParameterType.Double,
                DefaultValue = 0.1,
                MinValue = 0.01,
                MaxValue = 0.99,
                Description = "Rate at which pheromone evaporates. Higher values = faster forgetting (exploration). Lower values = slower forgetting (exploitation).",
                Formula = "τ_ij ← (1 - ρ) × τ_ij + Δτ_ij"
            });
            acoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Alpha",
                DisplayName = "Alpha (α)",
                Type = ParameterType.Double,
                DefaultValue = 1.0,
                MinValue = 0.1,
                MaxValue = 5.0,
                Description = "Pheromone influence factor. Higher values = stronger preference for paths with more pheromone.",
                Formula = "P_ij ∝ [τ_ij]^α × [η_ij]^β"
            });
            acoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Beta",
                DisplayName = "Beta (β)",
                Type = ParameterType.Double,
                DefaultValue = 2.0,
                MinValue = 0.1,
                MaxValue = 5.0,
                Description = "Heuristic influence factor. Higher values = stronger preference for shorter paths.",
                Formula = "P_ij ∝ [τ_ij]^α × [η_ij]^β where η_ij = 1/distance"
            });
            acoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Q0",
                DisplayName = "Exploitation Factor (q₀)",
                Type = ParameterType.Double,
                DefaultValue = 0.9,
                MinValue = 0.5,
                MaxValue = 0.99,
                Description = "Probability of exploitation vs exploration. Higher values = more exploitation (following best path).",
                Formula = "With probability q₀, choose best path; otherwise explore randomly"
            });
            _parameters["ACO"] = acoParams;

            // ========== D* (DStar) Parameters ==========
            var dstarParams = new List<AlgorithmParameterDefinition>(commonParams);
            dstarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "ReplanningRange",
                DisplayName = "Replanning Range",
                Type = ParameterType.Integer,
                DefaultValue = 5,
                MinValue = 1,
                MaxValue = 50,
                Description = "Radius (in cells) within which the algorithm checks for dynamic changes. Smaller = faster but may miss distant changes.",
                Formula = "Only cells within range are re-evaluated"
            });
            dstarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "DynamicReplanning",
                DisplayName = "Dynamic Replanning",
                Type = ParameterType.Boolean,
                DefaultValue = true,
                Description = "Enable automatic replanning when obstacles are detected. When disabled, original path is followed regardless.",
                Formula = "Replans when cost changes > threshold"
            });
            _parameters["DStar"] = dstarParams;

            // ========== KNN Parameters ==========
            var knnParams = new List<AlgorithmParameterDefinition>(commonParams);
            knnParams.Add(new AlgorithmParameterDefinition
            {
                Name = "K",
                DisplayName = "K Neighbors",
                Type = ParameterType.Integer,
                DefaultValue = 3,
                MinValue = 1,
                MaxValue = 20,
                Description = "Number of nearest neighbors to consider. Higher K = smoother path but slower.",
                Formula = "Searches K nearest walkable cells"
            });
            knnParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Radius",
                DisplayName = "Search Radius",
                Type = ParameterType.Integer,
                DefaultValue = 5,
                MinValue = 1,
                MaxValue = 20,
                Description = "Radius (in cells) to search for neighbors. Larger radius = more candidates but slower.",
                Formula = "Only cells within radius are considered"
            });
            _parameters["KNN"] = knnParams;

            // ========== Brute Force Parameters ==========
            var bfParams = new List<AlgorithmParameterDefinition>(commonParams);
            bfParams.Add(new AlgorithmParameterDefinition
            {
                Name = "MaxDepth",
                DisplayName = "Maximum Search Depth",
                Type = ParameterType.Integer,
                DefaultValue = 5000,
                MinValue = 100,
                MaxValue = 50000,
                Description = "Maximum depth (number of steps) to search. Increase for longer paths, decrease for faster execution.",
                Formula = "Stops exploration at depth N"
            });
            bfParams.Add(new AlgorithmParameterDefinition
            {
                Name = "MaxIterations",
                DisplayName = "Maximum Iterations",
                Type = ParameterType.Integer,
                DefaultValue = 100000,
                MinValue = 1000,
                MaxValue = 500000,
                Description = "Maximum number of nodes to explore. Increase for complex maps.",
                Formula = "Stops after exploring N nodes"
            });
            _parameters["BruteForce"] = bfParams;

            // ========== RRT Parameters ==========
            var rrtParams = new List<AlgorithmParameterDefinition>(commonParams);
            rrtParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Iterations",
                DisplayName = "Max Iterations",
                Type = ParameterType.Integer,
                DefaultValue = 5000,
                MinValue = 100,
                MaxValue = 50000,
                Description = "Maximum number of random samples. More samples = better solutions but slower.",
                Formula = "Samples N random points"
            });
            rrtParams.Add(new AlgorithmParameterDefinition
            {
                Name = "StepSize",
                DisplayName = "Step Size",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 5.0,
                MaxValue = 50.0,
                Description = "Maximum step size (in pixels) when expanding the tree. Smaller steps = smoother paths but slower.",
                Formula = "Tree expands by up to S cells per step"
            });
            rrtParams.Add(new AlgorithmParameterDefinition
            {
                Name = "GoalBias",
                DisplayName = "Goal Bias (%)",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 0.0,
                MaxValue = 100.0,
                Description = "Probability of sampling the goal directly. Higher bias = faster goal finding but less exploration.",
                Formula = "With probability B%, sample goal; otherwise sample random point"
            });
            rrtParams.Add(new AlgorithmParameterDefinition
            {
                Name = "SmoothPath",
                DisplayName = "Smooth Path",
                Type = ParameterType.Boolean,
                DefaultValue = true,
                Description = "Apply path smoothing after finding a solution. Removes unnecessary waypoints.",
                Formula = "Removes intermediate points on straight lines"
            });
            rrtParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Bidirectional",
                DisplayName = "Bidirectional Search",
                Type = ParameterType.Boolean,
                DefaultValue = false,
                Description = "Grow trees from both start and goal simultaneously. Faster on open maps.",
                Formula = "Two trees grow toward each other"
            });
            _parameters["RRT"] = rrtParams;

            // ========== PRM Parameters ==========
            var prmParams = new List<AlgorithmParameterDefinition>(commonParams);
            prmParams.Add(new AlgorithmParameterDefinition
            {
                Name = "NumSamples",
                DisplayName = "Number of Samples",
                Type = ParameterType.Integer,
                DefaultValue = 500,
                MinValue = 50,
                MaxValue = 5000,
                Description = "Number of random samples in the roadmap. More samples = better coverage but slower.",
                Formula = "Samples N random free points"
            });
            prmParams.Add(new AlgorithmParameterDefinition
            {
                Name = "ConnectionRadius",
                DisplayName = "Connection Radius",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 1.0,
                MaxValue = 50.0,
                Description = "Maximum distance (in cells) to connect nearby samples. Larger radius = better connectivity but slower.",
                Formula = "Connect samples within R cells"
            });
            prmParams.Add(new AlgorithmParameterDefinition
            {
                Name = "MaxNeighbors",
                DisplayName = "Max Neighbors",
                Type = ParameterType.Integer,
                DefaultValue = 15,
                MinValue = 1,
                MaxValue = 50,
                Description = "Maximum number of neighbors per node. Lower = faster but may miss connections.",
                Formula = "Connect to at most K nearest neighbors"
            });
            prmParams.Add(new AlgorithmParameterDefinition
            {
                Name = "SampleBias",
                DisplayName = "Sample Bias (%)",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 0.0,
                MaxValue = 100.0,
                Description = "Bias toward sampling near obstacles for better coverage of narrow passages.",
                Formula = "With probability B%, sample near obstacles"
            });
            _parameters["PRM"] = prmParams;

            // ========== PSO Parameters ==========
            var psoParams = new List<AlgorithmParameterDefinition>(commonParams);
            psoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "PopulationSize",
                DisplayName = "Population Size",
                Type = ParameterType.Integer,
                DefaultValue = 50,
                MinValue = 10,
                MaxValue = 200,
                Description = "Number of particles in the swarm. More particles = better solutions but slower.",
                Formula = "Swarm size = N particles"
            });
            psoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "MaxIterations",
                DisplayName = "Max Iterations",
                Type = ParameterType.Integer,
                DefaultValue = 100,
                MinValue = 20,
                MaxValue = 500,
                Description = "Maximum number of iterations. More iterations = better convergence but slower.",
                Formula = "Runs for N iterations"
            });
            psoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "InertiaWeight",
                DisplayName = "Inertia Weight (w)",
                Type = ParameterType.Double,
                DefaultValue = 0.7,
                MinValue = 0.1,
                MaxValue = 1.0,
                Description = "Controls momentum of particles. Higher = more exploration, Lower = more exploitation.",
                Formula = "v ← w × v + c₁×r₁×(pBest - x) + c₂×r₂×(gBest - x)"
            });
            psoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "CognitiveWeight",
                DisplayName = "Cognitive Weight (c₁)",
                Type = ParameterType.Double,
                DefaultValue = 1.5,
                MinValue = 0.1,
                MaxValue = 3.0,
                Description = "Weight for particle's personal best position. Higher = stronger attraction to own best.",
                Formula = "c₁ × r₁ × (pBest - x)"
            });
            psoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "SocialWeight",
                DisplayName = "Social Weight (c₂)",
                Type = ParameterType.Double,
                DefaultValue = 1.5,
                MinValue = 0.1,
                MaxValue = 3.0,
                Description = "Weight for global best position. Higher = stronger attraction to swarm's best.",
                Formula = "c₂ × r₂ × (gBest - x)"
            });
            psoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "PathSegments",
                DisplayName = "Path Segments",
                Type = ParameterType.Integer,
                DefaultValue = 20,
                MinValue = 5,
                MaxValue = 50,
                Description = "Number of segments representing a path. Higher = more flexible paths but longer optimization.",
                Formula = "Path represented by N intermediate points"
            });
            psoParams.Add(new AlgorithmParameterDefinition
            {
                Name = "AdaptiveInertia",
                DisplayName = "Adaptive Inertia",
                Type = ParameterType.Boolean,
                DefaultValue = true,
                Description = "Dynamically reduce inertia weight over time for better convergence.",
                Formula = "w ← w × damping factor"
            });
            _parameters["PSO"] = psoParams;

            // ========== GA (Genetic Algorithm) Parameters ==========
            var gaParams = new List<AlgorithmParameterDefinition>(commonParams);
            gaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "PopulationSize",
                DisplayName = "Population Size",
                Type = ParameterType.Integer,
                DefaultValue = 100,
                MinValue = 20,
                MaxValue = 500,
                Description = "Number of chromosomes in the population. More = better solutions but slower.",
                Formula = "Population size = N"
            });
            gaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "MaxGenerations",
                DisplayName = "Max Generations",
                Type = ParameterType.Integer,
                DefaultValue = 200,
                MinValue = 20,
                MaxValue = 1000,
                Description = "Maximum number of generations. More = better convergence but slower.",
                Formula = "Runs for N generations"
            });
            gaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "CrossoverRate",
                DisplayName = "Crossover Rate (%)",
                Type = ParameterType.Double,
                DefaultValue = 80.0,
                MinValue = 0.0,
                MaxValue = 100.0,
                Description = "Probability of crossover between parents. Higher = more genetic diversity.",
                Formula = "With probability P, perform crossover"
            });
            gaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "MutationRate",
                DisplayName = "Mutation Rate (%)",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 0.0,
                MaxValue = 50.0,
                Description = "Probability of mutation per gene. Higher = more exploration.",
                Formula = "With probability P, mutate gene"
            });
            gaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "EliteRatio",
                DisplayName = "Elite Ratio (%)",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 0.0,
                MaxValue = 30.0,
                Description = "Percentage of best individuals preserved unchanged.",
                Formula = "Keep top E% of population"
            });
            gaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "TournamentSize",
                DisplayName = "Tournament Size",
                Type = ParameterType.Integer,
                DefaultValue = 3,
                MinValue = 2,
                MaxValue = 10,
                Description = "Number of individuals competing in tournament selection. Higher = stronger selection pressure.",
                Formula = "Select best from T random individuals"
            });
            gaParams.Add(new AlgorithmParameterDefinition
            {
                Name = "AdaptiveMutation",
                DisplayName = "Adaptive Mutation",
                Type = ParameterType.Boolean,
                DefaultValue = true,
                Description = "Reduce mutation rate over time for better convergence.",
                Formula = "Rate ← rate × decay"
            });
            _parameters["GA"] = gaParams;

            // ========== RRT* Parameters ==========
            var rrtStarParams = new List<AlgorithmParameterDefinition>(commonParams);
            rrtStarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "Iterations",
                DisplayName = "Max Iterations",
                Type = ParameterType.Integer,
                DefaultValue = 5000,
                MinValue = 100,
                MaxValue = 50000,
                Description = "Maximum number of random samples. More samples = asymptotically better solutions.",
                Formula = "Samples N random points (asymptotically optimal)"
            });
            rrtStarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "StepSize",
                DisplayName = "Step Size",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 5.0,
                MaxValue = 50.0,
                Description = "Maximum step size when expanding the tree.",
                Formula = "Tree expands by up to S cells per step"
            });
            rrtStarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "GoalBias",
                DisplayName = "Goal Bias (%)",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 0.0,
                MaxValue = 100.0,
                Description = "Probability of sampling the goal directly.",
                Formula = "With probability B%, sample goal"
            });
            rrtStarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "RewiringRadius",
                DisplayName = "Rewiring Radius",
                Type = ParameterType.Double,
                DefaultValue = 10.0,
                MinValue = 1.0,
                MaxValue = 50.0,
                Description = "Radius within which to rewire nearby nodes. Larger radius = better solutions but slower.",
                Formula = "Rewire nodes within R cells"
            });
            rrtStarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "GoalRadius",
                DisplayName = "Goal Radius",
                Type = ParameterType.Double,
                DefaultValue = 20.0,
                MinValue = 1.0,
                MaxValue = 100.0,
                Description = "Radius at which a node is considered to have reached the goal.",
                Formula = "Success if distance to goal < R"
            });
            rrtStarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "InformedSampling",
                DisplayName = "Informed Sampling",
                Type = ParameterType.Boolean,
                DefaultValue = true,
                Description = "Sample only within ellipse defined by current best solution. Improves convergence.",
                Formula = "Sample within ellipse (start, goal, cost)"
            });
            rrtStarParams.Add(new AlgorithmParameterDefinition
            {
                Name = "SmoothPath",
                DisplayName = "Smooth Path",
                Type = ParameterType.Boolean,
                DefaultValue = true,
                Description = "Apply path smoothing after finding a solution.",
                Formula = "Remove redundant waypoints"
            });
            _parameters["RRTStar"] = rrtStarParams;
        }

        private static void InitializeFormulas()
        {
            _algorithmFormulas = new Dictionary<string, string>
            {
                ["AStar"] = "f(n) = g(n) + h(n)\ng(n): cost from start\nh(n): heuristic to goal",
                ["SPPA"] = "f(n) = g(n) + h(n) + λ·o(n)\no(n) = max(αₛ·static, αₛₛ·semiStatic, αᵈ·dynamic)",
                ["SPPA_DL"] = "f(n) = g(n) + h(n) + λ·o(n) + α·m(n) + β·p(n)\nm(n): learning memory\np(n): prediction risk",
                ["ACO"] = "P_ij = [τ_ij]^α · [η_ij]^β / Σ[τ_il]^α · [η_il]^β\nτ_ij: pheromone\nη_ij: visibility (1/distance)",
                ["DStar"] = "f(n) = g(n) + h(n) + d(n)\nd(n): dynamic obstacle cost",
                ["KNN"] = "d(x,y) = √Σ(x_i - y_i)²\nSelects K nearest neighbors",
                ["BruteForce"] = "min Σ cost(path)\nExhaustive search",
                ["RRT"] = "Sample random points, expand tree, connect to nearest\nv = w·v + c₁·r₁·(pBest - x) + c₂·r₂·(gBest - x)",
                ["PRM"] = "Build roadmap of samples, query with Dijkstra\nLearn obstacles positions",
                ["PSO"] = "v = w·v + c₁·r₁·(pBest - x) + c₂·r₂·(gBest - x)\nParticle Swarm Optimization",
                ["GA"] = "Selection + Crossover + Mutation + Elitism\nGenetic Algorithm",
                ["RRTStar"] = "RRT with rewiring for asymptotic optimality\nSamples more efficiently over time"
            };
        }

        private static void InitializeDescriptions()
        {
            _algorithmDescriptions = new Dictionary<string, string>
            {
                ["AStar"] = "Classic A* algorithm. Finds shortest path using heuristic search. Best for static environments with known maps.",
                ["SPPA"] = "Shortest Path with Precautionary Avoidance. Extends A* with obstacle coefficient o(n) to avoid dangerous areas.",
                ["SPPA_DL"] = "SPPA with Dynamic Learning. Adds learning memory m(n) and neural network prediction p(n) for dynamic environments.",
                ["ACO"] = "Ant Colony Optimization. Uses artificial ants and pheromone trails to find near-optimal paths.",
                ["DStar"] = "Dynamic A*. Designed for dynamic environments with changing obstacles. Efficiently replans only affected parts.",
                ["KNN"] = "K-Nearest Neighbors. Simple heuristic-based local search. Fast but not guaranteed optimal.",
                ["BruteForce"] = "Brute Force Search. Exhaustive search exploring all possibilities. Only suitable for very small maps.",
                ["RRT"] = "Rapidly-exploring Random Tree. Fast path planning for continuous spaces. Not guaranteed optimal.",
                ["PRM"] = "Probabilistic Roadmap. Builds roadmap of samples for multi-query path planning.",
                ["PSO"] = "Particle Swarm Optimization. Swarm-based optimization inspired by bird flocking.",
                ["GA"] = "Genetic Algorithm. Evolutionary algorithm based on natural selection.",
                ["RRTStar"] = "RRT*. Asymptotically optimal version of RRT with rewiring and informed sampling."
            };
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets all parameter definitions for a specific algorithm
        /// </summary>
        public static List<AlgorithmParameterDefinition> GetParameters(string algorithmName)
        {
            string key = algorithmName.Replace("*", "").Replace(" ", "");

            if (_parameters.TryGetValue(key, out var parameters))
            {
                return parameters.Select(p => p.Clone()).ToList();
            }

            return new List<AlgorithmParameterDefinition>();
        }

        /// <summary>
        /// Gets the mathematical formula for an algorithm
        /// </summary>
        public static string GetAlgorithmFormula(string algorithmName)
        {
            string key = algorithmName.Replace("*", "").Replace(" ", "");
            return _algorithmFormulas.GetValueOrDefault(key, "Unknown");
        }

        /// <summary>
        /// Gets the description for an algorithm
        /// </summary>
        public static string GetAlgorithmDescription(string algorithmName)
        {
            string key = algorithmName.Replace("*", "").Replace(" ", "");
            return _algorithmDescriptions.GetValueOrDefault(key, "No description available");
        }

        /// <summary>
        /// Gets all available algorithm names
        /// </summary>
        public static List<string> GetAllAlgorithmNames()
        {
            return new List<string>
            {
                "AStar",
                "SPPA",
                "SPPA_DL",
                "ACO",
                "DStar",
                "KNN",
                "BruteForce",
                "RRT",
                "PRM",
                "PSO",
                "GA",
                "RRTStar"
            };
        }

        /// <summary>
        /// Gets default parameters for an algorithm as a dictionary
        /// </summary>
        public static Dictionary<string, object> GetDefaultParameters(string algorithmName)
        {
            var parameters = GetParameters(algorithmName);
            var defaults = new Dictionary<string, object>();

            foreach (var param in parameters)
            {
                defaults[param.Name] = param.DefaultValue;
            }

            return defaults;
        }
        #endregion
    }
}