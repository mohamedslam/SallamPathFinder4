#region File Header
/// <summary>
/// File: GAFinder.cs
/// Description: GA (Genetic Algorithm) for pathfinding
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-21
/// Reference: Holland, J. H. (1975). "Adaptation in Natural and Artificial Systems"
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
using System.IO;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    #region Class Documentation
    /// <summary>
    /// GA (Genetic Algorithm) for pathfinding
    /// Operations: Selection, Crossover, Mutation, Elitism
    /// Each chromosome represents a complete path from start to goal
    /// </summary>
    #endregion
    public sealed class GAFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_POPULATION_SIZE = 100;
        private const int DEFAULT_MAX_GENERATIONS = 200;
        private const double DEFAULT_CROSSOVER_RATE = 0.8;
        private const double DEFAULT_MUTATION_RATE = 0.1;
        private const double DEFAULT_ELITE_RATIO = 0.1;
        private const int DEFAULT_TOURNAMENT_SIZE = 3;
        private const int DEFAULT_PATH_SEGMENTS = 20;
        #endregion

        #region Nested Types
        /// <summary>
        /// Chromosome representing a candidate path
        /// </summary>
        public  sealed class Chromosome
        {
            public List<Point> Path { get; set; }
            public double Fitness { get; set; }
            public double Cost { get; set; }

            public Chromosome()
            {
                Path = new List<Point>();
                Fitness = 0;
                Cost = double.MaxValue;
            }

            public Chromosome Clone()
            {
                return new Chromosome
                {
                    Path = new List<Point>(Path),
                    Fitness = Fitness,
                    Cost = Cost
                };
            }
        }

        /// <summary>
        /// GA Configuration
        /// </summary>
        public sealed class GAConfig
        {
            public int PopulationSize { get; set; } = DEFAULT_POPULATION_SIZE;
            public int MaxGenerations { get; set; } = DEFAULT_MAX_GENERATIONS;
            public double CrossoverRate { get; set; } = DEFAULT_CROSSOVER_RATE;
            public double MutationRate { get; set; } = DEFAULT_MUTATION_RATE;
            public double EliteRatio { get; set; } = DEFAULT_ELITE_RATIO;
            public int TournamentSize { get; set; } = DEFAULT_TOURNAMENT_SIZE;
            public int PathSegments { get; set; } = DEFAULT_PATH_SEGMENTS;
            public bool UseAdaptiveMutation { get; set; } = true;
        }
        #endregion

        #region Private Fields
        private readonly Random _random;
        private GAConfig _config;
        private List<Chromosome> _population;
        private Chromosome _bestChromosome;
        private Point _start;
        private Point _goal;
        #endregion

        #region Constructor
        public GAFinder(MapGrid grid) : base(grid)
        {
            _random = new Random(42);
            _config = new GAConfig();
            _population = new List<Chromosome>();
            _bestChromosome = null;

            SearchLimit = _config.MaxGenerations;
            AllowDiagonals = true;
        }
        #endregion

        #region Public Properties
        public GAConfig Configuration
        {
            get => _config;
            set => _config = value ?? new GAConfig();
        }

        public Chromosome BestChromosome => _bestChromosome;
        public int GenerationCount { get; private set; }
        #endregion

        #region Public Methods - Configuration
        public void SetParameters(int populationSize, int maxGenerations, double crossoverRate, double mutationRate)
        {
            _config.PopulationSize = Math.Max(20, Math.Min(500, populationSize));
            _config.MaxGenerations = Math.Max(20, Math.Min(1000, maxGenerations));
            _config.CrossoverRate = Math.Max(0.1, Math.Min(1.0, crossoverRate));
            _config.MutationRate = Math.Max(0.01, Math.Min(0.5, mutationRate));
            SearchLimit = _config.MaxGenerations;
        }

        public void SetEliteRatio(double eliteRatio)
        {
            _config.EliteRatio = Math.Max(0.01, Math.Min(0.3, eliteRatio));
        }
        #endregion

        #region Public Methods - Pathfinding
        public override PathResult FindPath(Point start, Point end)
        {
            // Validation
            if (!_grid.IsValidCoordinate(start.X, start.Y))
                return PathResult.Fail("Start position invalid");
            if (!_grid.IsValidCoordinate(end.X, end.Y))
                return PathResult.Fail("End position invalid");
            if (!_grid[start.X, start.Y].IsWalkable)
                return PathResult.Fail("Start not walkable");
            if (!_grid[end.X, end.Y].IsWalkable)
                return PathResult.Fail("End not walkable");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _start = start;
                _goal = end;

                InitializePopulation();

                for (GenerationCount = 0; GenerationCount < _config.MaxGenerations && !ShouldStop(); GenerationCount++)
                {
                    EvaluateFitness();

                    var newPopulation = new List<Chromosome>();

                    // Elitism: keep best individuals
                    int eliteCount = (int)(_config.PopulationSize * _config.EliteRatio);
                    var elites = _population.OrderByDescending(c => c.Fitness).Take(eliteCount);
                    newPopulation.AddRange(elites.Select(e => e.Clone()));

                    // Create rest of population
                    while (newPopulation.Count < _config.PopulationSize)
                    {
                        Chromosome parent1 = Selection();
                        Chromosome parent2 = Selection();

                        Chromosome offspring = Crossover(parent1, parent2);

                        if (_random.NextDouble() < _config.MutationRate)
                        {
                            Mutate(offspring);
                        }

                        newPopulation.Add(offspring);
                    }

                    _population = newPopulation;

                    // Update adaptive mutation rate
                    if (_config.UseAdaptiveMutation && GenerationCount % 20 == 0)
                    {
                        _config.MutationRate = Math.Max(0.01, _config.MutationRate * 0.95);
                    }

                    // Debug output every 20 generations
                    if (GenerationCount % 20 == 0 && _bestChromosome != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"GA Generation {GenerationCount}: Best Fitness = {_bestChromosome.Fitness:F4}, Cost = {_bestChromosome.Cost:F2}");
                    }
                }

                stopwatch.Stop();

                if (_bestChromosome != null && _bestChromosome.Path.Count > 0)
                {
                    var path = SmoothPath(_bestChromosome.Path);
                    // Final path visualization
                    foreach (var point in path)
                    {
                        RaiseDebugEvent(point.X, point.Y, point.X, point.Y, PathFinderNodeType.Path, 0, 0);
                    }

                    return new PathResult(path.Select(p => new PathNode(p.X, p.Y)).ToList(),
                        stopwatch.Elapsed.TotalSeconds, _config.PopulationSize * _config.MaxGenerations);

                }
               
                return PathResult.Fail("No path found", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return PathResult.Fail($"GA error: {ex.Message}", stopwatch.Elapsed.TotalSeconds);
            }
        }
        #endregion

        #region Private Methods - Initialization
        private void InitializePopulation()
        {
            _population.Clear();
            _bestChromosome = null;

            for (int i = 0; i < _config.PopulationSize; i++)
            {
                var chromosome = new Chromosome();
                chromosome.Path = GenerateRandomPath();
                _population.Add(chromosome);
            }

            System.Diagnostics.Debug.WriteLine($"GA initialized: {_population.Count} chromosomes, path length {_config.PathSegments}");
        }

        private List<Point> GenerateRandomPath()
        {
            var path = new List<Point>();
            path.Add(_start);

            int pathLength = _config.PathSegments;

            for (int i = 0; i < pathLength - 1; i++)
            {
                double t = (double)(i + 1) / pathLength;
                int targetX = (int)(_start.X + t * (_goal.X - _start.X));
                int targetY = (int)(_start.Y + t * (_goal.Y - _start.Y));

                int offsetX = (int)((_random.NextDouble() - 0.5) * 15);
                int offsetY = (int)((_random.NextDouble() - 0.5) * 15);

                int newX = Math.Max(0, Math.Min(_grid.Width - 1, targetX + offsetX));
                int newY = Math.Max(0, Math.Min(_grid.Height - 1, targetY + offsetY));

                if (!_grid[newX, newY].IsWalkable)
                {
                    newX = targetX;
                    newY = targetY;
                }

                path.Add(new Point(newX, newY));
                // Open node - initial chromosome cell
                RaiseDebugEvent(path[path.Count - 2].X, path[path.Count - 2].Y, newX, newY, PathFinderNodeType.Open, 0, 0);
            }

            path.Add(_goal);
            return path;
        }
        #endregion

        #region Private Methods - Fitness Evaluation
        private void EvaluateFitness()
        {
            double totalFitness = 0;

            foreach (var chromosome in _population)
            {
                chromosome.Cost = CalculatePathCost(chromosome.Path);
                chromosome.Fitness = 1.0 / (1.0 + chromosome.Cost);
                // Close node - all cells in this chromosome
                foreach (var point in chromosome.Path)
                {
                    RaiseDebugEvent(point.X, point.Y, point.X, point.Y, PathFinderNodeType.Close, 0, 0);
                }
                totalFitness += chromosome.Fitness;
            }

            // Find best chromosome
            var best = _population.OrderByDescending(c => c.Fitness).First();

            if (_bestChromosome == null || best.Fitness > _bestChromosome.Fitness)
            {
                _bestChromosome = best.Clone();
                // Path nodes - new best chromosome found
                foreach (var point in _bestChromosome.Path)
                {
                    RaiseDebugEvent(point.X, point.Y, point.X, point.Y, PathFinderNodeType.Path, 0, 0);
                }
            }
        }

        private double CalculatePathCost(List<Point> path)
        {
            if (path == null || path.Count < 2)
                return double.MaxValue;

            double totalCost = 0;
            int invalidSteps = 0;
            double totalDistance = 0;

            for (int i = 1; i < path.Count; i++)
            {
                Point from = path[i - 1];
                Point to = path[i];

                if (!_grid.IsValidCoordinate(to.X, to.Y))
                {
                    invalidSteps++;
                    continue;
                }

                var cell = _grid[to.X, to.Y];
                if (!cell.IsWalkable || cell.OccupyingObstacle != null)
                {
                    invalidSteps++;
                    RecordInvalidMove(to);
                    continue;
                }

                double distance = Math.Sqrt(Math.Pow(to.X - from.X, 2) + Math.Pow(to.Y - from.Y, 2));
                totalDistance += distance;

                double stepCost = distance * cell.SurfaceWeight;

                if (!AllowDiagonals && Math.Abs(to.X - from.X) > 0 && Math.Abs(to.Y - from.Y) > 0)
                {
                    stepCost *= 1.5;
                }

                totalCost += stepCost;
            }

            // Heavy penalty for invalid steps
            totalCost += invalidSteps * 10000;

            // Penalize path length
            double directDistance = Math.Sqrt(Math.Pow(_goal.X - _start.X, 2) + Math.Pow(_goal.Y - _start.Y, 2));
            totalCost += Math.Max(0, totalDistance - directDistance) * 2;

            return totalCost;
        }
        #endregion

        #region Private Methods - Genetic Operators
        private Chromosome Selection()
        {
            // Tournament selection
            var tournament = new List<Chromosome>();

            for (int i = 0; i < _config.TournamentSize; i++)
            {
                int index = _random.Next(_population.Count);
                tournament.Add(_population[index]);
            }

            return tournament.OrderByDescending(c => c.Fitness).First().Clone();
        }

        private Chromosome Crossover(Chromosome parent1, Chromosome parent2)
        {
            var offspring = new Chromosome();

            if (_random.NextDouble() < _config.CrossoverRate)
            {
                // Single-point crossover
                int crossoverPoint = _random.Next(1, Math.Min(parent1.Path.Count, parent2.Path.Count) - 1);

                for (int i = 0; i < crossoverPoint; i++)
                {
                    offspring.Path.Add(parent1.Path[i]);
                }

                for (int i = crossoverPoint; i < parent2.Path.Count; i++)
                {
                    offspring.Path.Add(parent2.Path[i]);
                    // Current node - crossover point
                    RaiseDebugEvent(parent1.Path[i - 1].X, parent1.Path[i - 1].Y, parent2.Path[i].X, parent2.Path[i].Y, PathFinderNodeType.Current, 0, 0);
                }
            }
            else
            {
                // No crossover, copy parent1
                offspring.Path = new List<Point>(parent1.Path);
            }

            // Ensure start and goal are correct
            if (offspring.Path.Count > 0)
            {
                offspring.Path[0] = _start;
                offspring.Path[offspring.Path.Count - 1] = _goal;
            }

            return offspring;
        }

        private void Mutate(Chromosome chromosome)
        {
            for (int i = 1; i < chromosome.Path.Count - 1; i++)
            {
                if (_random.NextDouble() < _config.MutationRate)
                {
                    // Mutate point
                    var point = chromosome.Path[i];
                    int offsetX = (int)((_random.NextDouble() - 0.5) * 5);
                    int offsetY = (int)((_random.NextDouble() - 0.5) * 5);

                    int newX = Math.Max(0, Math.Min(_grid.Width - 1, point.X + offsetX));
                    int newY = Math.Max(0, Math.Min(_grid.Height - 1, point.Y + offsetY));

                    if (_grid[newX, newY].IsWalkable)
                    {
                        chromosome.Path[i] = new Point(newX, newY);
                        // Open node - mutated cell
                        RaiseDebugEvent(chromosome.Path[i - 1].X, chromosome.Path[i - 1].Y, newX, newY, PathFinderNodeType.Open, 0, 0);
                    }
                }
            }
        }
        #endregion

        #region Private Methods - Helper Functions
        private List<PathNode> SmoothPath(List<Point> path)
        {
            if (path.Count < 3)
                return path.Select(p => new PathNode(p.X, p.Y)).ToList();

            var smoothed = new List<Point>(path);
            bool improved = true;
            int maxIterations = 50;
            int iteration = 0;

            while (improved && iteration < maxIterations)
            {
                improved = false;
                iteration++;

                for (int i = 0; i < smoothed.Count - 2; i++)
                {
                    for (int j = i + 2; j < smoothed.Count; j++)
                    {
                        if (IsPathClear(smoothed[i], smoothed[j]))
                        {
                            int removeCount = j - i - 1;
                            smoothed.RemoveRange(i + 1, removeCount);
                            improved = true;
                            break;
                        }
                    }
                    if (improved) break;
                }
            }

            return smoothed.Select(p => new PathNode(p.X, p.Y)).ToList();
        }

        private bool IsPathClear(Point from, Point to)
        {
            var points = GetLinePoints(from, to);

            foreach (var point in points)
            {
                if (!_grid.IsValidCoordinate(point.X, point.Y))
                    return false;

                var cell = _grid[point.X, point.Y];
                if (!cell.IsWalkable || cell.OccupyingObstacle != null)
                {
                    RecordInvalidMove(point);
                    return false;
                }
            }

            return true;
        }

        private List<Point> GetLinePoints(Point from, Point to)
        {
            var points = new List<Point>();

            int x0 = from.X;
            int y0 = from.Y;
            int x1 = to.X;
            int y1 = to.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add(new Point(x0, y0));

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return points;
        }
        #endregion
    }
}