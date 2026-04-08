#region File Header
/// <summary>
/// File: SimulationServiceTests.cs
/// Description: Unit tests for SimulationService class
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Collections.Generic;
using System.Drawing;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Services.Simulation;
using Xunit;
#endregion

namespace SallamPathFinder4.Tests.Services
{
    #region Test Class Documentation
    /// <summary>
    /// Unit tests for SimulationService functionality
    /// Tests robot movement, path following, and obstacle detection
    /// </summary>
    #endregion
    public sealed class SimulationServiceTests
    {
        #region Setup
        private MapGrid CreateTestGrid(int width = 50, int height = 50)
        {
            return new MapGrid(width, height);
        }

        private List<PathNode> CreateTestPath()
        {
            return new List<PathNode>
            {
                new PathNode(5, 5),
                new PathNode(6, 5),
                new PathNode(7, 5),
                new PathNode(8, 5),
                new PathNode(9, 5)
            };
        }
        #endregion

        #region Initialization Tests
        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();

            // Act
            var simulation = new SimulationService(grid, obstacles);

            // Assert
            Assert.False(simulation.IsRunning);
            Assert.False(simulation.IsPaused);
        }
        #endregion

        #region Start Tests
        [Fact]
        public void Start_ValidPath_BeginsSimulation()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);
            var path = CreateTestPath();

            // Act
            simulation.Start(path);

            // Assert
            Assert.True(simulation.IsRunning);
        }

        [Fact]
        public void Start_NullPath_ThrowsException()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => simulation.Start(null));
        }

        [Fact]
        public void Start_EmptyPath_ThrowsException()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);
            var emptyPath = new List<PathNode>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => simulation.Start(emptyPath));
        }
        #endregion

        #region Control Tests
        [Fact]
        public void Pause_StopsMovement()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);
            var path = CreateTestPath();

            simulation.Start(path);
            simulation.Pause();

            // Assert
            Assert.True(simulation.IsPaused);
            Assert.True(simulation.IsRunning);
        }

        [Fact]
        public void Stop_EndsSimulation()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);
            var path = CreateTestPath();

            simulation.Start(path);
            simulation.Stop();

            // Assert
            Assert.False(simulation.IsRunning);
            Assert.False(simulation.IsPaused);
        }
        #endregion

        #region Position Tests
        [Fact]
        public void CurrentRobotPosition_ReturnsCorrectPosition()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);
            var path = CreateTestPath();

            // Act
            simulation.Start(path);
            var position = simulation.CurrentRobotPosition;

            // Assert
            Assert.Equal(5, position.X);
            Assert.Equal(5, position.Y);
        }
        #endregion

        #region Detection Tests
        [Fact]
        public void SetDetectionParameters_ConfiguresCorrectly()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);

            // Act
            simulation.SetDetectionParameters(120, 3, true);
            var zoneCells = simulation.GetDetectionZoneCells(new Point(10, 10), 0);

            // Assert
            Assert.NotNull(zoneCells);
        }

        [Fact]
        public void GetDetectionZoneCells_ReturnsCellsWithinRange()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);
            simulation.SetDetectionParameters(360, 2, true);

            // Act
            var zoneCells = simulation.GetDetectionZoneCells(new Point(10, 10), 0);

            // Assert
            Assert.True(zoneCells.Count > 0);
        }

        [Fact]
        public void GetDetectionZoneCells_DetectionDisabled_ReturnsEmpty()
        {
            // Arrange
            var grid = CreateTestGrid();
            var obstacles = new List<DynamicObstacle>();
            var simulation = new SimulationService(grid, obstacles);
            simulation.SetDetectionParameters(360, 2, false);

            // Act
            var zoneCells = simulation.GetDetectionZoneCells(new Point(10, 10), 0);

            // Assert
            Assert.Empty(zoneCells);
        }
        #endregion
    }
}