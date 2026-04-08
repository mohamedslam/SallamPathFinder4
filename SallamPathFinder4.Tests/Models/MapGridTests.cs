#region File Header
/// <summary>
/// File: MapGridTests.cs
/// Description: Unit tests for MapGrid class
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Map;
using Xunit;
#endregion

namespace SallamPathFinder4.Tests.Models
{
    #region Test Class Documentation
    /// <summary>
    /// Unit tests for MapGrid functionality
    /// Tests grid creation, validation, resizing, and cell operations
    /// </summary>
    #endregion
    public sealed class MapGridTests
    {
        #region Constructor Tests
        [Fact]
        public void Constructor_ValidDimensions_CreatesGrid()
        {
            // Act
            var grid = new MapGrid(50, 50);

            // Assert
            Assert.Equal(50, grid.Width);
            Assert.Equal(50, grid.Height);
        }

        [Fact]
        public void Constructor_InvalidWidth_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MapGrid(5, 50));
        }

        [Fact]
        public void Constructor_InvalidHeight_ThrowsException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new MapGrid(50, 5));
        }

        [Fact]
        public void Constructor_InitializesAllCells()
        {
            // Act
            var grid = new MapGrid(10, 10);

            // Assert
            for (int x = 0; x < 10; x++)
                for (int y = 0; y < 10; y++)
                    Assert.NotNull(grid[x, y]);
        }
        #endregion

        #region Validation Tests
        [Fact]
        public void IsValidCoordinate_ValidCoordinates_ReturnsTrue()
        {
            // Arrange
            var grid = new MapGrid(50, 50);

            // Assert
            Assert.True(grid.IsValidCoordinate(25, 25));
            Assert.True(grid.IsValidCoordinate(0, 0));
            Assert.True(grid.IsValidCoordinate(49, 49));
        }

        [Fact]
        public void IsValidCoordinate_InvalidCoordinates_ReturnsFalse()
        {
            // Arrange
            var grid = new MapGrid(50, 50);

            // Assert
            Assert.False(grid.IsValidCoordinate(-1, 25));
            Assert.False(grid.IsValidCoordinate(25, -1));
            Assert.False(grid.IsValidCoordinate(50, 25));
            Assert.False(grid.IsValidCoordinate(25, 50));
        }
        #endregion

        #region Indexer Tests
        [Fact]
        public void Indexer_GetValidCell_ReturnsCell()
        {
            // Arrange
            var grid = new MapGrid(50, 50);

            // Act
            var cell = grid[10, 10];

            // Assert
            Assert.Equal(10, cell.X);
            Assert.Equal(10, cell.Y);
        }

        [Fact]
        public void Indexer_SetValidCell_UpdatesCell()
        {
            // Arrange
            var grid = new MapGrid(50, 50);
            var newCell = new Cell(20, 20);

            // Act
            grid[20, 20] = newCell;

            // Assert
            Assert.Same(newCell, grid[20, 20]);
        }

        [Fact]
        public void Indexer_InvalidCoordinate_ThrowsException()
        {
            // Arrange
            var grid = new MapGrid(50, 50);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => grid[50, 50]);
        }
        #endregion

        #region Reset Tests
        [Fact]
        public void Reset_ClearsAllCellsToDefault()
        {
            // Arrange
            var grid = new MapGrid(10, 10);
            grid[5, 5].SurfaceWeight = 50;

            // Act
            grid.Reset();

            // Assert
            Assert.Equal(1, grid[5, 5].SurfaceWeight);
        }
        #endregion

        #region Resize Tests
        [Fact]
        public void Resize_LargerGrid_PreservesExistingData()
        {
            // Arrange
            var grid = new MapGrid(10, 10);
            grid[5, 5].SurfaceWeight = 75;

            // Act
            grid.Resize(20, 20);

            // Assert
            Assert.Equal(20, grid.Width);
            Assert.Equal(20, grid.Height);
            Assert.Equal(75, grid[5, 5].SurfaceWeight);
        }

        [Fact]
        public void Resize_SmallerGrid_PreservesDataWithinBounds()
        {
            // Arrange
            var grid = new MapGrid(20, 20);
            grid[5, 5].SurfaceWeight = 75;

            // Act
            grid.Resize(10, 10);

            // Assert
            Assert.Equal(10, grid.Width);
            Assert.Equal(10, grid.Height);
            Assert.Equal(75, grid[5, 5].SurfaceWeight);
        }

        [Fact]
        public void Resize_SameDimensions_DoesNothing()
        {
            // Arrange
            var grid = new MapGrid(20, 20);
            int originalWidth = grid.Width;
            int originalHeight = grid.Height;

            // Act
            grid.Resize(20, 20);

            // Assert
            Assert.Equal(originalWidth, grid.Width);
            Assert.Equal(originalHeight, grid.Height);
        }
        #endregion

        #region Statistics Tests
        [Fact]
        public void GetWalkableCellCount_ReturnsCorrectCount()
        {
            // Arrange
            var grid = new MapGrid(10, 10);

            // Act
            int count = grid.GetWalkableCellCount();

            // Assert
            Assert.Equal(100, count); // All cells walkable by default
        }

        [Fact]
        public void GetAverageSurfaceWeight_ReturnsCorrectAverage()
        {
            // Arrange
            var grid = new MapGrid(10, 10);
            grid[0, 0].SurfaceWeight = 100;
            grid[0, 1].SurfaceWeight = 50;

            // Act
            double avg = grid.GetAverageSurfaceWeight();

            // Assert
            Assert.Equal(1.02, avg, 2); // (98*1 + 100 + 50) / 100 = 1.02
        }
        #endregion
    }
}