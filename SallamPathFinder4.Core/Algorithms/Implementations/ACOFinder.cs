#region File Header
/// <summary>
/// File: ACOFinder.cs
/// Description: Ant Colony Optimization (ACO) pathfinding algorithm
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Algorithms.Base;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Algorithms.Implementations
{
    public sealed class ACOFinder : BasePathFinder
    {
        #region Constants
        private const int DEFAULT_ANTS = 20;
        private const double DEFAULT_EVAPORATION = 0.1;
        private const double DEFAULT_ALPHA = 1.0;
        private const double DEFAULT_BETA = 2.0;
        private const double DEFAULT_Q0 = 0.9;
        private const double INITIAL_PHEROMONE = 0.01;
        private const int DEFAULT_ITERATIONS = 100;
        #endregion

        #region Nested Types
        private sealed class Ant
        {
            public List<Point> Path { get; set; }
            public double PathLength { get; set; }
            public bool Completed { get; set; }

            public Ant()
            {
                Path = new List<Point>();
                PathLength = 0;
                Completed = false;
            }
        }
        #endregion

        #region Private Fields
        private int _numberOfAnts = DEFAULT_ANTS;
        private double _evaporationRate = DEFAULT_EVAPORATION;
        private double _alpha = DEFAULT_ALPHA;
        private double _beta = DEFAULT_BETA;
        private double _q0 = DEFAULT_Q0;
        private double _initialPheromone = INITIAL_PHEROMONE;
        private int _maxIterations = DEFAULT_ITERATIONS;
        private double[,,] _pheromones;
        private readonly Random _random = new Random();
        #endregion

        #region Constructor
        public ACOFinder(MapGrid grid) : base(grid)
        {
            _numberOfAnts = DEFAULT_ANTS;
            _evaporationRate = DEFAULT_EVAPORATION;
            _alpha = DEFAULT_ALPHA;
            _beta = DEFAULT_BETA;
            _q0 = DEFAULT_Q0;
            _maxIterations = DEFAULT_ITERATIONS;
        }
        #endregion

        #region Public Methods
        public void SetParameters(int ants, double evaporation, double alpha, double beta, int iterations)
        {
            _numberOfAnts = Math.Max(1, Math.Min(200, ants));
            _evaporationRate = Math.Max(0.01, Math.Min(0.99, evaporation));
            _alpha = Math.Max(0.1, Math.Min(5.0, alpha));
            _beta = Math.Max(0.1, Math.Min(5.0, beta));
            _maxIterations = Math.Max(10, Math.Min(500, iterations));
        }

        public override PathResult FindPath(Point start, Point end)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            if (!_grid.IsValidCoordinate(start.X, start.Y))
                return PathResult.Fail("Start position invalid");
            if (!_grid.IsValidCoordinate(end.X, end.Y))
                return PathResult.Fail("End position invalid");
            if (!_grid[start.X, start.Y].IsWalkable)
                return PathResult.Fail("Start not walkable");
            if (!_grid[end.X, end.Y].IsWalkable)
                return PathResult.Fail("End not walkable");

            try
            {
                int width = _grid.Width;
                int height = _grid.Height;

                _pheromones = new double[width, height, 4];
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        for (int d = 0; d < 4; d++)
                            _pheromones[x, y, d] = _initialPheromone;

                int[] dx = { 0, 1, 0, -1 };
                int[] dy = { -1, 0, 1, 0 };

                Ant globalBestAnt = null;

                for (int iter = 0; iter < _maxIterations && !ShouldStop(); iter++)
                {
                    var ants = new List<Ant>();

                    for (int a = 0; a < _numberOfAnts; a++)
                    {
                        var ant = new Ant();
                        ant.Path.Add(start);
                        Point current = start;

                        while (!(current.X == end.X && current.Y == end.Y) && ant.Path.Count < SearchLimit)
                        {
                            var possibleMoves = new List<int>();
                            for (int d = 0; d < 4; d++)
                            {
                                int nx = current.X + dx[d];
                                int ny = current.Y + dy[d];
                                if (_grid.IsValidCoordinate(nx, ny))
                                {
                                    var cell = _grid[nx, ny];
                                    if (cell.IsWalkable)
                                    {
                                        if (!ant.Path.Any(p => p.X == nx && p.Y == ny))
                                            possibleMoves.Add(d);
                                    }
                                    else
                                    {
                                        RecordInvalidMove(new Point(nx, ny));
                                    }
                                }
                            }

                            if (possibleMoves.Count == 0) break;

                            int nextDir = SelectNextMove(current, end, possibleMoves, dx, dy);
                            current = new Point(current.X + dx[nextDir], current.Y + dy[nextDir]);
                            ant.Path.Add(current);
                            ant.PathLength += CalculateStepCost(ant.Path[ant.Path.Count - 2], current);
                        }

                        if (current.X == end.X && current.Y == end.Y)
                        {
                            ant.Completed = true;
                            ants.Add(ant);

                            if (globalBestAnt == null || ant.PathLength < globalBestAnt.PathLength)
                                globalBestAnt = ant;
                        }
                    }

                    for (int x = 0; x < width; x++)
                        for (int y = 0; y < height; y++)
                            for (int d = 0; d < 4; d++)
                                _pheromones[x, y, d] *= (1 - _evaporationRate);

                    foreach (var ant in ants.Where(a => a.Completed))
                    {
                        double deposit = 1.0 / ant.PathLength;
                        for (int i = 0; i < ant.Path.Count - 1; i++)
                        {
                            Point from = ant.Path[i];
                            Point to = ant.Path[i + 1];
                            int dir = GetDirection(from, to);
                            if (dir >= 0)
                                _pheromones[from.X, from.Y, dir] += deposit;
                        }
                    }
                }

                stopwatch.Stop();

                if (globalBestAnt != null && globalBestAnt.Path.Count > 0)
                {
                    var pathNodes = globalBestAnt.Path.Select(p => new PathNode(p.X, p.Y)).ToList();
                    return new PathResult(pathNodes, stopwatch.Elapsed.TotalSeconds, _numberOfAnts * _maxIterations);
                }

                return PathResult.Fail("No path found", stopwatch.Elapsed.TotalSeconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return PathResult.Fail(ex.Message, stopwatch.Elapsed.TotalSeconds);
            }
        }
        #endregion

        #region Private Methods
        private int SelectNextMove(Point current, Point end, List<int> possibleMoves, int[] dx, int[] dy)
        {
            if (_random.NextDouble() < _q0)
            {
                double bestValue = -1;
                int bestDir = possibleMoves[0];
                foreach (int d in possibleMoves)
                {
                    int nx = current.X + dx[d];
                    int ny = current.Y + dy[d];
                    double heuristic = 1.0 / (Math.Abs(nx - end.X) + Math.Abs(ny - end.Y) + 0.1);
                    double value = Math.Pow(_pheromones[current.X, current.Y, d], _alpha) * Math.Pow(heuristic, _beta);
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestDir = d;
                    }
                }
                return bestDir;
            }
            else
            {
                double total = 0;
                var probs = new double[4];
                foreach (int d in possibleMoves)
                {
                    int nx = current.X + dx[d];
                    int ny = current.Y + dy[d];
                    double heuristic = 1.0 / (Math.Abs(nx - end.X) + Math.Abs(ny - end.Y) + 0.1);
                    probs[d] = Math.Pow(_pheromones[current.X, current.Y, d], _alpha) * Math.Pow(heuristic, _beta);
                    total += probs[d];
                }

                double rand = _random.NextDouble() * total;
                double cumulative = 0;
                foreach (int d in possibleMoves)
                {
                    cumulative += probs[d];
                    if (rand <= cumulative)
                        return d;
                }
                return possibleMoves[0];
            }
        }

        private int GetDirection(Point from, Point to)
        {
            if (to.X > from.X) return 1;
            if (to.X < from.X) return 3;
            if (to.Y > from.Y) return 2;
            if (to.Y < from.Y) return 0;
            return -1;
        }
        #endregion
    }
}