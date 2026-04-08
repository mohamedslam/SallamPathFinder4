#region File Header
/// <summary>
/// File: FileService.cs
/// Description: Service for file operations (save/load maps in binary format)
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Map;
using static System.Runtime.InteropServices.JavaScript.JSType;
#endregion

namespace SallamPathFinder4.Services.File
{
    #region Class Documentation
    /// <summary>
    /// Service for file operations
    /// Saves and loads map data using binary format for efficiency
    /// Supports versioning for backward compatibility
    /// </summary>
    #endregion
    public sealed class FileService : IFileService
    {
        #region Constants
        private const int FILE_VERSION = 2;
        private const int MAGIC_NUMBER = 0x53414C4C; // "SALL" in hex
        #endregion

        #region Private Fields
        private readonly object _lockObject = new object();
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public async Task SaveMapAsync(string filePath, MapData data)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    using var fs = new FileStream(filePath, FileMode.Create);
                    using var writer = new BinaryWriter(fs);

                    // Write header
                    writer.Write(MAGIC_NUMBER);
                    writer.Write(FILE_VERSION);
                    writer.Write(data.GridWidth);
                    writer.Write(data.GridHeight);
                    writer.Write(data.CellSizePixels);
                    writer.Write(data.ScaleCmPerCell);

                    // Write surface weights
                    WriteSurfaceWeights(writer, data);

                    // Write element types
                    WriteElementTypes(writer, data);

                    // Write door states
                    WriteDoorStates(writer, data);

                    // Write ramp difficulties
                    WriteRampDifficulties(writer, data);

                    // Write goals
                    WriteGoals(writer, data);

                    // Write parking points
                    WriteParkingPoints(writer, data);

                    // Write dynamic obstacles
                    WriteDynamicObstacles(writer, data);

                    // Write robot state
                    WriteRobotState(writer, data);

                    // Save dynamic obstacles
                    writer.Write(data.DynamicObstacles?.Count ?? 0);
                    if (data.DynamicObstacles != null)
                    {
                        foreach (var obstacle in data.DynamicObstacles)
                        {
                            writer.Write((int)obstacle.Type);
                            writer.Write(obstacle.Location.X);
                            writer.Write(obstacle.Location.Y);
                        }
                    }
                }
            });


        }

        /// <inheritdoc/>
        public async Task<MapData> LoadMapAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            return await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    using var fs = new FileStream(filePath, FileMode.Open);
                    using var reader = new BinaryReader(fs);

                    // Read and validate header
                    int magic = reader.ReadInt32();
                    if (magic != MAGIC_NUMBER)
                        throw new InvalidDataException("Invalid file format: wrong magic number");

                    int version = reader.ReadInt32();
                    if (version != FILE_VERSION)
                        throw new InvalidOperationException($"Unsupported file version: {version}. Expected: {FILE_VERSION}");

                    var data = new MapData
                    {
                        GridWidth = reader.ReadInt32(),
                        GridHeight = reader.ReadInt32(),
                        CellSizePixels = reader.ReadInt32(),
                        ScaleCmPerCell = reader.ReadDouble()
                    };

                    // Read surface weights
                    ReadSurfaceWeights(reader, data);

                    // Read element types
                    ReadElementTypes(reader, data);

                    // Read door states
                    ReadDoorStates(reader, data);

                    // Read ramp difficulties
                    ReadRampDifficulties(reader, data);

                    // Read goals
                    ReadGoals(reader, data);

                    // Read parking points
                    ReadParkingPoints(reader, data);

                    // Read dynamic obstacles
                    ReadDynamicObstacles(reader, data);

                    // Read robot state
                    ReadRobotState(reader, data);

                    // Read dynamic obstacles
                    int obstacleCount = reader.ReadInt32();
                    data.DynamicObstacles = new List<MapObstacleData>();
                    for (int i = 0; i < obstacleCount; i++)
                    {
                        data.DynamicObstacles.Add(new MapObstacleData
                        {
                            Type = (ObstacleType)reader.ReadInt32(),
                            Location = new Point(reader.ReadInt32(), reader.ReadInt32())
                        });
                    }

                    return data;
                }
         
            });
        }

        /// <inheritdoc/>
        public bool FileExists(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }
        #endregion

        #region Private Write Methods
        private static void WriteSurfaceWeights(BinaryWriter writer, MapData data)
        {
            if (data.SurfaceWeights != null)
            {
                for (int y = 0; y < data.GridHeight; y++)
                    for (int x = 0; x < data.GridWidth; x++)
                        writer.Write(data.SurfaceWeights[x, y]);
            }
            else
            {
                for (int i = 0; i < data.GridWidth * data.GridHeight; i++)
                    writer.Write((byte)1);
            }
        }

        private static void WriteElementTypes(BinaryWriter writer, MapData data)
        {
            if (data.ElementTypes != null)
            {
                for (int y = 0; y < data.GridHeight; y++)
                    for (int x = 0; x < data.GridWidth; x++)
                        writer.Write((int)data.ElementTypes[x, y]);
            }
            else
            {
                for (int i = 0; i < data.GridWidth * data.GridHeight; i++)
                    writer.Write((int)MapElementType.Empty);
            }
        }

        private static void WriteDoorStates(BinaryWriter writer, MapData data)
        {
            if (data.DoorStates != null)
            {
                for (int y = 0; y < data.GridHeight; y++)
                    for (int x = 0; x < data.GridWidth; x++)
                        writer.Write(data.DoorStates[x, y]);
            }
            else
            {
                for (int i = 0; i < data.GridWidth * data.GridHeight; i++)
                    writer.Write(true);
            }
        }

        private static void WriteRampDifficulties(BinaryWriter writer, MapData data)
        {
            if (data.RampDifficulties != null)
            {
                for (int y = 0; y < data.GridHeight; y++)
                    for (int x = 0; x < data.GridWidth; x++)
                        writer.Write(data.RampDifficulties[x, y]);
            }
            else
            {
                for (int i = 0; i < data.GridWidth * data.GridHeight; i++)
                    writer.Write((byte)0);
            }
        }

        private static void WriteGoals(BinaryWriter writer, MapData data)
        {
            writer.Write(data.Goals?.Count ?? 0);
            if (data.Goals != null)
            {
                foreach (var goal in data.Goals)
                {
                    writer.Write(goal.Number);
                    writer.Write(goal.Location.X);
                    writer.Write(goal.Location.Y);
                    writer.Write(goal.ColorArgb);
                }
            }
        }

        private static void WriteParkingPoints(BinaryWriter writer, MapData data)
        {
            writer.Write(data.ParkingPoints?.Count ?? 0);
            if (data.ParkingPoints != null)
            {
                foreach (var parking in data.ParkingPoints)
                {
                    writer.Write(parking.Number);
                    writer.Write(parking.Location.X);
                    writer.Write(parking.Location.Y);
                }
            }
        }

        private static void WriteDynamicObstacles(BinaryWriter writer, MapData data)
        {
            writer.Write(data.DynamicObstacles?.Count ?? 0);
            if (data.DynamicObstacles != null)
            {
                foreach (var obstacle in data.DynamicObstacles)
                {
                    writer.Write((int)obstacle.Type);
                    writer.Write(obstacle.Location.X);
                    writer.Write(obstacle.Location.Y);
                }
            }
        }

        private static void WriteRobotState(BinaryWriter writer, MapData data)
        {
            writer.Write(data.RobotPosition.X);
            writer.Write(data.RobotPosition.Y);
            writer.Write(data.RobotAngle);
            writer.Write(data.BatteryLevel);
        }
        #endregion

        #region Private Read Methods
        private static void ReadSurfaceWeights(BinaryReader reader, MapData data)
        {
            data.SurfaceWeights = new byte[data.GridWidth, data.GridHeight];
            for (int y = 0; y < data.GridHeight; y++)
                for (int x = 0; x < data.GridWidth; x++)
                    data.SurfaceWeights[x, y] = reader.ReadByte();
        }

        private static void ReadElementTypes(BinaryReader reader, MapData data)
        {
            data.ElementTypes = new MapElementType[data.GridWidth, data.GridHeight];
            for (int y = 0; y < data.GridHeight; y++)
                for (int x = 0; x < data.GridWidth; x++)
                    data.ElementTypes[x, y] = (MapElementType)reader.ReadInt32();
        }

        private static void ReadDoorStates(BinaryReader reader, MapData data)
        {
            data.DoorStates = new bool[data.GridWidth, data.GridHeight];
            for (int y = 0; y < data.GridHeight; y++)
                for (int x = 0; x < data.GridWidth; x++)
                    data.DoorStates[x, y] = reader.ReadBoolean();
        }

        private static void ReadRampDifficulties(BinaryReader reader, MapData data)
        {
            data.RampDifficulties = new byte[data.GridWidth, data.GridHeight];
            for (int y = 0; y < data.GridHeight; y++)
                for (int x = 0; x < data.GridWidth; x++)
                    data.RampDifficulties[x, y] = reader.ReadByte();
        }

        private static void ReadGoals(BinaryReader reader, MapData data)
        {
            int goalCount = reader.ReadInt32();
            data.Goals = new List<GoalData>();
            for (int i = 0; i < goalCount; i++)
            {
                data.Goals.Add(new GoalData
                {
                    Number = reader.ReadInt32(),
                    Location = new Point(reader.ReadInt32(), reader.ReadInt32()),
                    ColorArgb = reader.ReadInt32()
                });
            }
        }

        private static void ReadParkingPoints(BinaryReader reader, MapData data)
        {
            int parkingCount = reader.ReadInt32();
            data.ParkingPoints = new List<ParkingData>();
            for (int i = 0; i < parkingCount; i++)
            {
                data.ParkingPoints.Add(new ParkingData
                {
                    Number = reader.ReadInt32(),
                    Location = new Point(reader.ReadInt32(), reader.ReadInt32())
                });
            }
        }

        private static void ReadDynamicObstacles(BinaryReader reader, MapData data)
        {
            int obstacleCount = reader.ReadInt32();
            data.DynamicObstacles = new List<MapObstacleData>();
            for (int i = 0; i < obstacleCount; i++)
            {
                data.DynamicObstacles.Add(new MapObstacleData
                {
                    Type = (ObstacleType)reader.ReadInt32(),
                    Location = new Point(reader.ReadInt32(), reader.ReadInt32())
                });
            }
        }

        private static void ReadRobotState(BinaryReader reader, MapData data)
        {
            data.RobotPosition = new Point(reader.ReadInt32(), reader.ReadInt32());
            data.RobotAngle = reader.ReadSingle();
            data.BatteryLevel = reader.ReadDouble();
        }
        #endregion
    }
}