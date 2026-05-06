#region File Header
/// <summary>
/// File: PSOFinder.cs
/// Description: PSO (Particle Swarm Optimization) pathfinding algorithm
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-21
/// Reference: Kennedy, J., & Eberhart, R. (1995). "Particle Swarm Optimization"
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    #region Class Documentation
    /// <summary>
    /// PSO (Particle Swarm Optimization) algorithm
    /// Swarm-based optimization inspired by bird flocking and fish schooling
    /// Each particle represents a complete path from start to goal
    /// </summary>
    #endregion
    public sealed class PSOFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_POPULATION_SIZE = 50;
        private const int DEFAULT_MAX_ITERATIONS = 100;
        private const double DEFAULT_INERTIA_WEIGHT = 0.7;
        private const double DEFAULT_COGNITIVE_WEIGHT = 1.5;
        private const double DEFAULT_SOCIAL_WEIGHT = 1.5;
        private const double DEFAULT_MAX_VELOCITY = 5.0;
        private const double DEFAULT_INERTIA_DAMPING = 0.99;
        #endregion

        #region Nested Types
        /// <summary>
        /// Particle representing a candidate path
        /// </summary>
        private sealed class Particle
        {
            public List<Point> Position { get; set; }      // Current path
            public List<Point> BestPosition { get; set; }  // Personal best path
            public double BestCost { get; set; }           // Personal best cost
            public double[] Velocity { get; set; }         // Velocity vector
            public double CurrentCost { get; set; }        // Current path cost

            public Particle(int pathLength)
            {
                Position = new List<Point>();
                BestPosition = new List<Point>();
                Velocity = new double[pathLength];
                BestCost = double.MaxValue;
                CurrentCost = double.MaxValue;
            }
        }

        /// <summary>
        /// PSO Configuration
        /// </summary>
        public sealed class PSOConfig
        {
            public int PopulationSize { get; set; } = DEFAULT_POPULATION_SIZE;
            public int MaxIterations { get; set; } = DEFAULT_MAX_ITERATIONS;
            public double InertiaWeight { get; set; } = DEFAULT_INERTIA_WEIGHT;
            public double CognitiveWeight { get; set; } = DEFAULT_COGNITIVE_WEIGHT;
            public double SocialWeight { get; set; } = DEFAULT_SOCIAL_WEIGHT;
            public double MaxVelocity { get; set; } = DEFAULT_MAX_VELOCITY;
            public double InertiaDamping { get; set; } = DEFAULT_INERTIA_DAMPING;
            public int PathSegments { get; set; } = 20;
            public bool UseElitism { get; set; } = true;
        }
        #endregion

        #region Private Fields
        private readonly Random _random;
        private PSOConfig _config;
        private List<Particle> _particles;
        private List<Point> _globalBestPosition;
        private double _globalBestCost;
        private Point _start;
        private Point _goal;
        #endregion

        #region Constructor
        public PSOFinder(MapGrid grid) : base(grid)
        {
            _random = new Random(42);
            _config = new PSOConfig();
            _particles = new List<Particle>();
            _globalBestPosition = new List<Point>();
            _globalBestCost = double.MaxValue;

            SearchLimit = _config.MaxIterations;
            AllowDiagonals = true;
        }
        #endregion

        #region Public Properties
        public PSOConfig Configuration
        {
            get => _config;
            set => _config = value ?? new PSOConfig();
        }

        public double GlobalBestCost => _globalBestCost;
        public int ParticleCount => _particles.Count;
        #endregion

        #region Public Methods - Configuration
        public void SetParameters(int populationSize, int maxIterations, double inertiaWeight = 0.7)
        {
            _config.PopulationSize = Math.Max(10, Math.Min(200, populationSize));
            _config.MaxIterations = Math.Max(20, Math.Min(500, maxIterations));
            _config.InertiaWeight = Math.Max(0.1, Math.Min(1.0, inertiaWeight));
            SearchLimit = _config.MaxIterations;
        }

        public void SetWeights(double inertia, double cognitive, double social)
        {
            _config.InertiaWeight = Math.Max(0.1, Math.Min(1.0, inertia));
            _config.CognitiveWeight = Math.Max(0.1, Math.Min(3.0, cognitive));
            _config.SocialWeight = Math.Max(0.1, Math.Min(3.0, social));
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

                for (int iteration = 0; iteration < _config.MaxIterations && !ShouldStop(); iteration++)
                {
                    EvaluateParticles();
                    UpdateVelocities();
                    UpdatePositions();

                    // Apply inertia damping
                    _config.InertiaWeight *= _config.InertiaDamping;

                    // Debug output every 10 iterations
                    if (iteration % 10 == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"PSO Iteration {iteration}: Best Cost = {_globalBestCost:F2}");
                    }
                }

                stopwatch.Stop();

                if (_globalBestPosition != null && _globalBestPosition.Count > 0)
                {
                    var path = SmoothPath(_globalBestPosition);
                    return new PathResult(path.Select(p => new PathNode(p.X, p.Y)).ToList(),
                        stopwatch.Elapsed.TotalSeconds, _config.PopulationSize * _config.MaxIterations);
                }

                return PathResult.Fail("No path found", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return PathResult.Fail($"PSO error: {ex.Message}", stopwatch.Elapsed.TotalSeconds);
            }
        }
        #endregion

        #region Private Methods - Initialization
        private void InitializePopulation()
        {
            _particles.Clear();
            _globalBestPosition.Clear();
            _globalBestCost = double.MaxValue;

            int pathLength = _config.PathSegments;

            for (int i = 0; i < _config.PopulationSize; i++)
            {
                var particle = new Particle(pathLength);

                // Initialize random path
                particle.Position = GenerateRandomPath();
                particle.CurrentCost = CalculatePathCost(particle.Position);

                // Initialize best position
                particle.BestPosition = new List<Point>(particle.Position);
                particle.BestCost = particle.CurrentCost;

                // Initialize velocity randomly
                for (int j = 0; j < pathLength; j++)
                {
                    particle.Velocity[j] = (_random.NextDouble() - 0.5) * 2 * _config.MaxVelocity;
                }

                _particles.Add(particle);

                // Update global best
                if (particle.CurrentCost < _globalBestCost)
                {
                    _globalBestCost = particle.CurrentCost;
                    _globalBestPosition = new List<Point>(particle.Position);
                }
            }

            System.Diagnostics.Debug.WriteLine($"PSO initialized: {_particles.Count} particles, path length {pathLength}");
        }

        private List<Point> GenerateRandomPath()
        {
            var path = new List<Point>();
            path.Add(_start);

            Point current = _start;
            int pathLength = _config.PathSegments;

            for (int i = 0; i < pathLength - 1; i++)
            {
                // Random direction with bias toward goal
                double t = (double)(i + 1) / pathLength;
                int targetX = (int)(_start.X + t * (_goal.X - _start.X));
                int targetY = (int)(_start.Y + t * (_goal.Y - _start.Y));

                // Add random offset
                int offsetX = (int)((_random.NextDouble() - 0.5) * 10);
                int offsetY = (int)((_random.NextDouble() - 0.5) * 10);

                int newX = Math.Max(0, Math.Min(_grid.Width - 1, targetX + offsetX));
                int newY = Math.Max(0, Math.Min(_grid.Height - 1, targetY + offsetY));

                // Ensure path is valid
                if (!_grid[newX, newY].IsWalkable)
                {
                    newX = targetX;
                    newY = targetY;
                }

                path.Add(new Point(newX, newY));
                // Open node - initial candidate cell
                RaiseDebugEvent(path[path.Count - 2].X, path[path.Count - 2].Y, newX, newY, PathFinderNodeType.Open, 0, 0);
            }

            path.Add(_goal);
            return path;
        }
        #endregion

        #region Private Methods - Evaluation
        private void EvaluateParticles()
        {
            foreach (var particle in _particles)
            {
                particle.CurrentCost = CalculatePathCost(particle.Position);
                // Close node - closed cells in particle's path
                foreach (var point in particle.Position)
                {
                    RaiseDebugEvent(point.X, point.Y, point.X, point.Y, PathFinderNodeType.Close, 0, 0);
                }
                // Update personal best
                if (particle.CurrentCost < particle.BestCost)
                {
                    particle.BestCost = particle.CurrentCost;
                    particle.BestPosition = new List<Point>(particle.Position);
                    // Path nodes - personal best path found
                    foreach (var point in particle.BestPosition)
                    {
                        RaiseDebugEvent(point.X, point.Y, point.X, point.Y, PathFinderNodeType.Path, 0, 0);
                    }
                }

                // Update global best
                if (particle.CurrentCost < _globalBestCost)
                {
                    _globalBestCost = particle.CurrentCost;
                    _globalBestPosition = new List<Point>(particle.Position);
                    // Path nodes - new global best path found
                    foreach (var point in _globalBestPosition)
                    {
                        RaiseDebugEvent(point.X, point.Y, point.X, point.Y, PathFinderNodeType.Path, 0, 0);
                    }
                }
            }
        }

        private double CalculatePathCost(List<Point> path)
        {
            if (path == null || path.Count < 2)
                return double.MaxValue;

            double totalCost = 0;
            int invalidSteps = 0;

            for (int i = 1; i < path.Count; i++)
            {
                Point from = path[i - 1];
                Point to = path[i];

                // Check if point is valid
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

                // Calculate step cost
                double distance = Math.Sqrt(Math.Pow(to.X - from.X, 2) + Math.Pow(to.Y - from.Y, 2));
                double stepCost = distance * cell.SurfaceWeight;

                // Penalize diagonal movement if not allowed
                if (!AllowDiagonals && Math.Abs(to.X - from.X) > 0 && Math.Abs(to.Y - from.Y) > 0)
                {
                    stepCost *= 1.5;
                }

                totalCost += stepCost;
            }

            // Penalize invalid steps heavily
            totalCost += invalidSteps * 1000;

            // Add heuristic for path length
            double directDistance = Math.Sqrt(Math.Pow(_goal.X - _start.X, 2) + Math.Pow(_goal.Y - _start.Y, 2));
            totalCost += Math.Abs(path.Count - directDistance) * 5;

            return totalCost;
        }
        #endregion

        #region Private Methods - Velocity and Position Update
        private void UpdateVelocities()
        {
            int pathLength = _config.PathSegments;

            foreach (var particle in _particles)
            {
                for (int i = 0; i < pathLength; i++)
                {
                    double r1 = _random.NextDouble();
                    double r2 = _random.NextDouble();

                    double cognitive = _config.CognitiveWeight * r1 * (GetPointValue(particle.BestPosition, i) - GetPointValue(particle.Position, i));
                    double social = _config.SocialWeight * r2 * (GetPointValue(_globalBestPosition, i) - GetPointValue(particle.Position, i));

                    particle.Velocity[i] = _config.InertiaWeight * particle.Velocity[i] + cognitive + social;

                    // Clamp velocity
                    particle.Velocity[i] = Math.Max(-_config.MaxVelocity, Math.Min(_config.MaxVelocity, particle.Velocity[i]));
                }
            }
        }

        private double GetPointValue(List<Point> path, int index)
        {
            if (path == null || index >= path.Count)
                return 0;

            // Convert 2D point to 1D value for velocity calculation
            return path[index].X * _grid.Width + path[index].Y;
        }

        private void UpdatePositions()
        {
            int pathLength = _config.PathSegments;

            foreach (var particle in _particles)
            {
                for (int i = 1; i <= pathLength; i++)
                {
                    double currentValue = GetPointValue(particle.Position, i);
                    double newValue = currentValue + particle.Velocity[i - 1];

                    int newX = (int)(newValue / _grid.Width) % _grid.Width;
                    int newY = (int)newValue % _grid.Height;

                    newX = Math.Max(0, Math.Min(_grid.Width - 1, newX));
                    newY = Math.Max(0, Math.Min(_grid.Height - 1, newY));

                    if (i < particle.Position.Count)
                    {
                        particle.Position[i] = new Point(newX, newY);
                        // Current node - particle position update
                        RaiseDebugEvent(particle.Position[i - 1].X, particle.Position[i - 1].Y, newX, newY, PathFinderNodeType.Current, 0, 0);
                    }
                }

                // Ensure path is valid
                particle.Position[0] = _start;
                particle.Position[particle.Position.Count - 1] = _goal;

                // Repair invalid points
                for (int i = 1; i < particle.Position.Count - 1; i++)
                {
                    var point = particle.Position[i];
                    if (!_grid.IsValidCoordinate(point.X, point.Y) || !_grid[point.X, point.Y].IsWalkable)
                    {
                        // Interpolate between neighbors
                        Point prev = particle.Position[i - 1];
                        Point next = particle.Position[i + 1];
                        particle.Position[i] = new Point((prev.X + next.X) / 2, (prev.Y + next.Y) / 2);
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