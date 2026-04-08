#region File Header
/// <summary>
/// File: ObstacleMemoryTests.cs
/// Description: Unit tests for ObstacleMemory class
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Linq;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
using Xunit;
#endregion

namespace SallamPathFinder4.Tests.Models
{
    #region Test Class Documentation
    /// <summary>
    /// Unit tests for ObstacleMemory functionality
    /// Tests recording, querying, and persistence of obstacle data
    /// </summary>
    #endregion
    public sealed class ObstacleMemoryTests
    {
        #region Recording Tests
        [Fact]
        public void RecordDetection_NewLocation_CreatesRecord()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");

            // Act
            memory.RecordDetection(10, 10, ObstacleType.Adult);

            // Assert
            Assert.Equal(1, memory.TotalDetections);
            Assert.True(memory.HasMemory(10, 10));
            Assert.Equal(1, memory.GetFrequency(10, 10));
        }

        [Fact]
        public void RecordDetection_ExistingLocation_IncrementsFrequency()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");

            // Act
            memory.RecordDetection(10, 10, ObstacleType.Adult);
            memory.RecordDetection(10, 10, ObstacleType.Adult);

            // Assert
            Assert.Equal(2, memory.TotalDetections);
            Assert.Equal(2, memory.GetFrequency(10, 10));
        }

        [Fact]
        public void RecordDetection_MultipleTypes_TracksDominantType()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");

            // Act
            memory.RecordDetection(10, 10, ObstacleType.Adult);
            memory.RecordDetection(10, 10, ObstacleType.Adult);
            memory.RecordDetection(10, 10, ObstacleType.Child);

            // Assert
            Assert.Equal(ObstacleType.Adult, memory.GetDominantType(10, 10));
        }
        #endregion

        #region Query Tests
        [Fact]
        public void GetObstacleCoefficient_ReturnsCorrectValue()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");
            memory.RecordDetection(10, 10, ObstacleType.Adult);
            memory.IncrementSimulation();

            // Act
            double coeff = memory.GetObstacleCoefficient(10, 10, 2.0);

            // Assert
            Assert.Equal(2.0, coeff); // 2.0 * (1/1) = 2.0
        }

        [Fact]
        public void GetObstacleCoefficient_NoMemory_ReturnsZero()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");

            // Act
            double coeff = memory.GetObstacleCoefficient(99, 99);

            // Assert
            Assert.Equal(0, coeff);
        }

        [Fact]
        public void GetMemoryCells_ReturnsAllRecordedLocations()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");
            memory.RecordDetection(10, 10, ObstacleType.Adult);
            memory.RecordDetection(20, 20, ObstacleType.Child);

            // Act
            var cells = memory.GetMemoryCells();

            // Assert
            Assert.Equal(2, cells.Count);
            Assert.Contains(new System.Drawing.Point(10, 10), cells);
            Assert.Contains(new System.Drawing.Point(20, 20), cells);
        }
        #endregion

        #region Simulation Counter Tests
        [Fact]
        public void IncrementSimulation_IncreasesCounter()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");

            // Act
            memory.IncrementSimulation();
            memory.IncrementSimulation();

            // Assert
            Assert.Equal(2, memory.TotalSimulations);
        }

        [Fact]
        public void GetObstacleCoefficient_UsesSimulationCount()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");
            memory.RecordDetection(10, 10, ObstacleType.Adult);
            memory.IncrementSimulation();
            memory.IncrementSimulation();

            // Act
            double coeff = memory.GetObstacleCoefficient(10, 10, 2.0);

            // Assert
            Assert.Equal(1.0, coeff); // 2.0 * (1/2) = 1.0
        }
        #endregion

        #region Clear Tests
        [Fact]
        public void Clear_RemovesAllRecords()
        {
            // Arrange
            var memory = new ObstacleMemory("test_memory.json");
            memory.RecordDetection(10, 10, ObstacleType.Adult);
            memory.IncrementSimulation();

            // Act
            memory.Clear();

            // Assert
            Assert.Equal(0, memory.TotalDetections);
            Assert.Equal(0, memory.TotalSimulations);
            Assert.False(memory.HasMemory(10, 10));
        }
        #endregion
    }
}