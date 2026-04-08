#region File Header
/// <summary>
/// File: IMapGeneratorService.cs
/// Description: Interface for random map generation service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Models.Map;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Generation Options
    public sealed class MapGenerationOptions
    {
        public int Width { get; set; } = 100;
        public int Height { get; set; } = 100;
        public int WallPercentage { get; set; } = 10;
        public int GoalCount { get; set; } = 5;
        public int ParkingCount { get; set; } = 2;
        public int DynamicObstacleCount { get; set; } = 3;
        public bool AddDoors { get; set; } = true;
        public bool AddWindows { get; set; } = true;
        public bool AddRamps { get; set; } = true;
        public int RandomSeed { get; set; } = -1;
        public bool EnsureReachability { get; set; } = true;
    }
    #endregion

    #region Interface Documentation
    /// <summary>
    /// Service interface for generating random maps
    /// </summary>
    #endregion
    public interface IMapGeneratorService
    {
        Task<MapGrid> GenerateRandomMapAsync(MapGenerationOptions options);
        Task<MapGrid> GenerateEmptyMapAsync(int width, int height);
        Task<MapGrid> GenerateMazeMapAsync(int width, int height, int randomSeed = -1);
        Task<MapGrid> GenerateRoomMapAsync(int width, int height, int roomCount = 5);
        bool ValidateMapReachability(MapGrid grid, Point start);
    }
}