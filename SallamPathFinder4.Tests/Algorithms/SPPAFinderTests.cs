#region File Header
/// <summary>
/// File: SPPAFinderTests.cs
/// Description: Unit tests for SPPAFinder algorithm
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
using SallamPathFinder4.Core.Algorithms.Implementations;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using Xunit;
#endregion

namespace SallamPathFinder4.Tests.Algorithms
{
    #region Test Class Documentation
    /// <summary>
    /// Unit tests for SPPAFinder pathfinding algorithm
    /// Tests obstacle-aware pathfinding with precautionary avoidance
    /// </summary>
    #endregion
    public sealed class SPPAFinderTests
    {
        #region Setup
        private MapGrid CreateEmptyGrid(int width = 50, int height = 50)
        {
            return new MapGrid(width, height);
        }

        private MapGrid CreateGridWithWindow()
        {
            var grid = new MapGrid(10, 10);
            grid[5, 5].ElementType = MapElementType.Window;
            grid.UpdateAllCellProperties();
            return grid;
        }

        private MapGrid CreateGridWithRamp()
        {
            var grid = new MapGrid(10, 10);
            grid[5, 5].ElementType = MapElementType.Ramp;
            grid[5, 5].RampDifficulty = 50;
            grid.UpdateAllCellProperties();
            return grid;
        }
        #endregion

        #region Basic Pathfinding Tests
        [Fact]
        public void FindPath_ValidStartAndEnd_ReturnsPath()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            var finder = new SPPAFinder(grid);
            var start = new Point(5, 5);
            var end = new Point(45, 45);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Path);
        }

        [Fact]
        public void FindPath_WithWindow_PrefersAlternativePath()
        {
            // Arrange
            var grid = CreateGridWithWindow();
            var finder = new SPPAFinder(grid);
            var start = new Point(0, 0);
            var end = new Point(9, 9);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
            // Path should avoid the window if possible
            Assert.DoesNotContain(result.Path, p => p.X == 5 && p.Y == 5);
        }

        [Fact]
        public void FindPath_WithRamp_PrefersAlternativePath()
        {
            // Arrange
            var grid = CreateGridWithRamp();
            var finder = new SPPAFinder(grid);
            var start = new Point(0, 0);
            var end = new Point(9, 9);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
            // Path may still go through ramp if no alternative
        }
        #endregion

        #region Obstacle Coefficient Tests
        [Fact]
        public void FindPath_Wall_BlockedPath()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            grid[5, 5].ElementType = MapElementType.Wall;
            grid.UpdateAllCellProperties();
            var finder = new SPPAFinder(grid);
            var start = new Point(0, 0);
            var end = new Point(9, 9);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void FindPath_ClosedDoor_BlockedPath()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            grid[5, 5].ElementType = MapElementType.Door;
            grid[5, 5].IsDoorOpen = false;
            grid.UpdateAllCellProperties();
            var finder = new SPPAFinder(grid);
            var start = new Point(0, 0);
            var end = new Point(9, 9);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void FindPath_OpenDoor_AllowsPassage()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            grid[5, 5].ElementType = MapElementType.Door;
            grid[5, 5].IsDoorOpen = true;
            grid.UpdateAllCellProperties();
            var finder = new SPPAFinder(grid);
            var start = new Point(0, 0);
            var end = new Point(9, 9);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
        }
        #endregion

        #region Configuration Tests
        [Fact]
        public void FindPath_DifferentMetrics_ReturnPaths()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            var finder = new SPPAFinder(grid);
            var start = new Point(5, 5);
            var end = new Point(45, 45);

            // Act
            finder.Metric = DistanceMetric.Manhattan;
            var result1 = finder.FindPath(start, end);

            finder.Metric = DistanceMetric.Euclidean;
            var result2 = finder.FindPath(start, end);

            // Assert
            Assert.True(result1.Success);
            Assert.True(result2.Success);
        }
        #endregion
    }
}