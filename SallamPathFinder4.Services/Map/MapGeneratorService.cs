#region File Header
/// <summary>
/// File: MapGeneratorService.cs
/// Description: Service for generating random maps
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Helpers;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Map;
#endregion

namespace SallamPathFinder4.Services.Map
{
    #region Class Documentation
    /// <summary>
    /// Service for generating random maps for experiments
    /// </summary>
    #endregion
    public sealed class MapGeneratorService : IMapGeneratorService
    {
        #region Private Fields
        private readonly Random _random;
        #endregion

        #region Constructor
        public MapGeneratorService()
        {
            _random = new Random();
        }
        #endregion

        #region Public Methods
        public async Task<MapGrid> GenerateRandomMapAsync(MapGenerationOptions options)
        {
            return await Task.Run(() =>
            {
                int seed = options.RandomSeed >= 0 ? options.RandomSeed : Environment.TickCount;
                var random = new Random(seed);

                var grid = new MapGrid(options.Width, options.Height);

                // Clear all cells first
                for (int x = 0; x < options.Width; x++)
                {
                    for (int y = 0; y < options.Height; y++)
                    {
                        grid[x, y].ElementType = MapElementType.Empty;
                        grid[x, y].SurfaceWeight = (byte)random.Next(1, 100);
                    }
                }

                // Add walls
                int wallCount = (options.Width * options.Height * options.WallPercentage) / 100;
                for (int i = 0; i < wallCount; i++)
                {
                    int x = random.Next(1, options.Width - 1);
                    int y = random.Next(1, options.Height - 1);
                    if (grid[x, y].ElementType == MapElementType.Empty)
                    {
                        grid[x, y].ElementType = MapElementType.Wall;
                    }
                }

                // Add start point
                grid[5, 5].ElementType = MapElementType.StartPoint;

                // Add goals
                for (int i = 0; i < options.GoalCount; i++)
                {
                    int x = random.Next(10, options.Width - 10);
                    int y = random.Next(10, options.Height - 10);
                    if (grid[x, y].ElementType == MapElementType.Empty)
                    {
                        grid[x, y].ElementType = MapElementType.GoalPoint;
                    }
                }

                // Add parking points
                for (int i = 0; i < options.ParkingCount; i++)
                {
                    int x = random.Next(10, options.Width - 10);
                    int y = random.Next(10, options.Height - 10);
                    if (grid[x, y].ElementType == MapElementType.Empty)
                    {
                        grid[x, y].ElementType = MapElementType.ParkingPoint;
                    }
                }

                // Add doors
                if (options.AddDoors)
                {
                    int doorCount = Math.Min(10, (options.Width * options.Height) / 200);
                    for (int i = 0; i < doorCount; i++)
                    {
                        int x = random.Next(1, options.Width - 1);
                        int y = random.Next(1, options.Height - 1);
                        if (grid[x, y].ElementType == MapElementType.Empty)
                        {
                            grid[x, y].ElementType = MapElementType.Door;
                            grid[x, y].IsDoorOpen = random.Next(2) == 0;
                        }
                    }
                }

                // Add windows
                if (options.AddWindows)
                {
                    int windowCount = Math.Min(10, (options.Width * options.Height) / 200);
                    for (int i = 0; i < windowCount; i++)
                    {
                        int x = random.Next(1, options.Width - 1);
                        int y = random.Next(1, options.Height - 1);
                        if (grid[x, y].ElementType == MapElementType.Empty)
                        {
                            grid[x, y].ElementType = MapElementType.Window;
                        }
                    }
                }

                // Add ramps
                if (options.AddRamps)
                {
                    int rampCount = Math.Min(5, (options.Width * options.Height) / 400);
                    for (int i = 0; i < rampCount; i++)
                    {
                        int x = random.Next(1, options.Width - 1);
                        int y = random.Next(1, options.Height - 1);
                        if (grid[x, y].ElementType == MapElementType.Empty)
                        {
                            grid[x, y].ElementType = MapElementType.Ramp;
                            grid[x, y].RampDifficulty = (byte)random.Next(10, 51);
                        }
                    }
                }

                grid.UpdateAllCellProperties();

                if (options.EnsureReachability)
                {
                    grid = EnsureReachability(grid, new Point(5, 5));
                }

                return grid;
            });
        }

        public async Task<MapGrid> GenerateEmptyMapAsync(int width, int height)
        {
            return await Task.Run(() =>
            {
                var grid = new MapGrid(width, height);
                grid[5, 5].ElementType = MapElementType.StartPoint;
                grid.UpdateAllCellProperties();
                return grid;
            });
        }

        public async Task<MapGrid> GenerateMazeMapAsync(int width, int height, int randomSeed = -1)
        {
            return await Task.Run(() =>
            {
                int seed = randomSeed >= 0 ? randomSeed : Environment.TickCount;
                var random = new Random(seed);

                var grid = new MapGrid(width, height);

                // Fill with walls first
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        grid[x, y].ElementType = MapElementType.Wall;
                    }
                }

                // Simple DFS maze generation
                var stack = new Stack<Point>();
                var start = new Point(1, 1);
                grid[start.X, start.Y].ElementType = MapElementType.Empty;
                stack.Push(start);

                while (stack.Count > 0)
                {
                    var current = stack.Peek();
                    var neighbors = GetUnvisitedNeighbors(grid, current, width, height);

                    if (neighbors.Count > 0)
                    {
                        var next = neighbors[random.Next(neighbors.Count)];
                        int wallX = (current.X + next.X) / 2;
                        int wallY = (current.Y + next.Y) / 2;

                        grid[wallX, wallY].ElementType = MapElementType.Empty;
                        grid[next.X, next.Y].ElementType = MapElementType.Empty;

                        stack.Push(next);
                    }
                    else
                    {
                        stack.Pop();
                    }
                }

                grid[5, 5].ElementType = MapElementType.StartPoint;
                grid[width - 6, height - 6].ElementType = MapElementType.GoalPoint;
                grid.UpdateAllCellProperties();

                return grid;
            });
        }

        public async Task<MapGrid> GenerateRoomMapAsync(int width, int height, int roomCount = 5)
        {
            return await Task.Run(() =>
            {
                var grid = new MapGrid(width, height);

                // Fill with empty
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        grid[x, y].ElementType = MapElementType.Empty;
                    }
                }

                // Add rooms
                var rooms = new List<Rectangle>();
                for (int r = 0; r < roomCount; r++)
                {
                    int roomW = _random.Next(8, 20);
                    int roomH = _random.Next(8, 20);
                    int roomX = _random.Next(2, width - roomW - 2);
                    int roomY = _random.Next(2, height - roomH - 2);

                    var room = new Rectangle(roomX, roomY, roomW, roomH);
                    rooms.Add(room);

                    // Draw room walls
                    for (int x = roomX; x < roomX + roomW; x++)
                    {
                        grid[x, roomY].ElementType = MapElementType.Wall;
                        grid[x, roomY + roomH - 1].ElementType = MapElementType.Wall;
                    }
                    for (int y = roomY; y < roomY + roomH; y++)
                    {
                        grid[roomX, y].ElementType = MapElementType.Wall;
                        grid[roomX + roomW - 1, y].ElementType = MapElementType.Wall;
                    }

                    // Add door
                    int doorX = roomX + roomW / 2;
                    int doorY = roomY + roomH - 1;
                    if (doorY < height - 1)
                    {
                        grid[doorX, doorY].ElementType = MapElementType.Door;
                        grid[doorX, doorY].IsDoorOpen = true;
                    }
                }

                grid[5, 5].ElementType = MapElementType.StartPoint;
                grid[width - 10, height - 10].ElementType = MapElementType.GoalPoint;
                grid.UpdateAllCellProperties();

                return grid;
            });
        }

        public bool ValidateMapReachability(MapGrid grid, Point start)
        {
            var visited = new HashSet<(int, int)>();
            var queue = new Queue<Point>();
            queue.Enqueue(start);
            visited.Add((start.X, start.Y));

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = current.X + dx;
                        int ny = current.Y + dy;

                        if (grid.IsValidCoordinate(nx, ny) &&
                            grid[nx, ny].IsWalkable &&
                            !visited.Contains((nx, ny)))
                        {
                            visited.Add((nx, ny));
                            queue.Enqueue(new Point(nx, ny));
                        }
                    }
                }
            }

            // Check if all goals and parking are reachable
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var cell = grid[x, y];
                    if ((cell.ElementType == MapElementType.GoalPoint ||
                         cell.ElementType == MapElementType.ParkingPoint) &&
                        !visited.Contains((x, y)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region Private Methods
        private static List<Point> GetUnvisitedNeighbors(MapGrid grid, Point current, int width, int height)
        {
            var neighbors = new List<Point>();
            var directions = new (int dx, int dy)[] { (2, 0), (-2, 0), (0, 2), (0, -2) };

            foreach (var dir in directions)
            {
                int nx = current.X + dir.dx;
                int ny = current.Y + dir.dy;

                if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1)
                {
                    if (grid[nx, ny].ElementType == MapElementType.Wall)
                    {
                        neighbors.Add(new Point(nx, ny));
                    }
                }
            }

            return neighbors;
        }

        private static MapGrid EnsureReachability(MapGrid grid, Point start)
        {
            // Simple implementation - just return grid
            // Full implementation would add paths to unreachable areas
            return grid;
        }
        #endregion
    }
}