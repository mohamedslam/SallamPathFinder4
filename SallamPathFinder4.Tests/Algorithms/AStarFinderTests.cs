#region File Header
/// <summary>
/// File: AStarFinderTests.cs
/// Description: Unit tests for AStarFinder algorithm
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
    /// Unit tests for AStarFinder pathfinding algorithm
    /// Tests pathfinding in various map configurations
    /// </summary>
    #endregion
    public sealed class AStarFinderTests
    {
        #region Setup
        private MapGrid CreateEmptyGrid(int width = 50, int height = 50)
        {
            return new MapGrid(width, height);
        }

        private MapGrid CreateGridWithWall()
        {
            var grid = new MapGrid(10, 10);
            // Add wall blocking direct path
            for (int y = 0; y < 10; y++)
            {
                grid[5, y].ElementType = MapElementType.Wall;
            }
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
            var finder = new AStarFinder(grid);
            var start = new Point(5, 5);
            var end = new Point(45, 45);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Path);
            Assert.True(result.PathLength > 0);
        }

        [Fact]
        public void FindPath_SameStartAndEnd_ReturnsPathWithOneNode()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            var finder = new AStarFinder(grid);
            var start = new Point(10, 10);

            // Act
            var result = finder.FindPath(start, start);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1, result.PathLength);
        }

        [Fact]
        public void FindPath_InvalidStart_ReturnsFailure()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            var finder = new AStarFinder(grid);
            var start = new Point(-1, -1);
            var end = new Point(45, 45);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Path);
        }

        [Fact]
        public void FindPath_BlockedPath_ReturnsNoPath()
        {
            // Arrange
            var grid = CreateGridWithWall();
            var finder = new AStarFinder(grid);
            var start = new Point(0, 0);
            var end = new Point(9, 9);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Path);
        }
        #endregion

        #region Algorithm Configuration Tests
        [Fact]
        public void FindPath_ManhattanMetric_ReturnsPath()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            var finder = new AStarFinder(grid);
            finder.Metric = DistanceMetric.Manhattan;
            var start = new Point(5, 5);
            var end = new Point(45, 45);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void FindPath_EuclideanMetric_ReturnsPath()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            var finder = new AStarFinder(grid);
            finder.Metric = DistanceMetric.Euclidean;
            var start = new Point(5, 5);
            var end = new Point(45, 45);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void FindPath_NoDiagonals_ReturnsPath()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            var finder = new AStarFinder(grid);
            finder.AllowDiagonals = false;
            var start = new Point(5, 5);
            var end = new Point(45, 45);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void FindPath_HeavyDiagonals_ReturnsPath()
        {
            // Arrange
            var grid = CreateEmptyGrid();
            var finder = new AStarFinder(grid);
            finder.HeavyDiagonals = true;
            var start = new Point(5, 5);
            var end = new Point(45, 45);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.True(result.Success);
        }
        #endregion

        #region Performance Tests
        [Fact]
        public void FindPath_RespectsSearchLimit()
        {
            // Arrange
            var grid = CreateGridWithWall();
            var finder = new AStarFinder(grid);
            finder.SearchLimit = 100;
            var start = new Point(0, 0);
            var end = new Point(9, 9);

            // Act
            var result = finder.FindPath(start, end);

            // Assert
            Assert.False(result.Success);
            Assert.True(result.NodesExplored <= 100);
        }
        #endregion
    }
}