#region File Header
/// <summary>
/// File: MapData.cs
/// Description: Data container for map serialization/deserialization
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Models.Map
{
    #region Class Documentation
    /// <summary>
    /// Container for complete map data used in file operations
    /// Supports serialization to binary format
    /// </summary>
    #endregion
    public sealed class MapData
    {
        #region Constructor
        public MapData()
        {
            Goals = new List<GoalData>();
            ParkingPoints = new List<ParkingData>();
            DynamicObstacles = new List<MapObstacleData>();
        }
        #endregion

        #region Properties - Grid
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public int CellSizePixels { get; set; }
        public double ScaleCmPerCell { get; set; }
        #endregion

        #region Properties - Grid Data
        public byte[,] SurfaceWeights { get; set; }
        public MapElementType[,] ElementTypes { get; set; }
        public bool[,] DoorStates { get; set; }
        public byte[,] RampDifficulties { get; set; }
        #endregion

        #region Properties - Game Objects
        public List<GoalData> Goals { get; set; }
        public List<ParkingData> ParkingPoints { get; set; }
        public List<MapObstacleData> DynamicObstacles { get; set; }
        #endregion

        #region Properties - Robot State
        public Point RobotPosition { get; set; }
        public float RobotAngle { get; set; }
        public double BatteryLevel { get; set; }
        #endregion
    }

    #region Supporting Classes
    public sealed class GoalData
    {
        public int Number { get; set; }
        public Point Location { get; set; }
        public int ColorArgb { get; set; }
    }

    public sealed class ParkingData
    {
        public int Number { get; set; }
        public Point Location { get; set; }
    }

    public sealed class MapObstacleData
    {
        public ObstacleType Type { get; set; }
        public Point Location { get; set; }
    }
    #endregion
}