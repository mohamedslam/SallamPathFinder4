#region File Header
/// <summary>
/// File: HeuristicCalculatorTests.cs
/// Description: Unit tests for HeuristicCalculator class
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
using SallamPathFinder4.Core.Algorithms.Heuristics;
using SallamPathFinder4.Core.Enums;
using Xunit;
#endregion

namespace SallamPathFinder4.Tests.Algorithms
{
    #region Test Class Documentation
    /// <summary>
    /// Unit tests for HeuristicCalculator
    /// Tests all distance metrics with various scenarios
    /// </summary>
    #endregion
    public sealed class HeuristicCalculatorTests
    {
        #region Test Constants
        private static readonly Point PointA = new Point(0, 0);
        private static readonly Point PointB = new Point(3, 4);
        private static readonly Point PointC = new Point(5, 5);
        private static readonly Point PointD = new Point(10, 0);
        #endregion

        #region Manhattan Tests
        [Fact]
        public void Calculate_ManhattanMetric_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointB, DistanceMetric.Manhattan);

            // Assert
            Assert.Equal(7, result); // |3-0| + |4-0| = 7
        }

        [Fact]
        public void Calculate_ManhattanMetric_WithWeight_ReturnsWeightedValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointB, DistanceMetric.Manhattan, weight: 2);

            // Assert
            Assert.Equal(14, result); // 7 * 2 = 14
        }

        [Fact]
        public void Calculate_ManhattanMetric_SamePoint_ReturnsZero()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointA, DistanceMetric.Manhattan);

            // Assert
            Assert.Equal(0, result);
        }
        #endregion

        #region Euclidean Tests
        [Fact]
        public void Calculate_EuclideanMetric_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointB, DistanceMetric.Euclidean);

            // Assert
            Assert.Equal(5, result); // √(3² + 4²) = 5
        }

        [Fact]
        public void Calculate_EuclideanMetric_HorizontalLine_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointD, DistanceMetric.Euclidean);

            // Assert
            Assert.Equal(10, result); // √(10² + 0²) = 10
        }
        #endregion

        #region MaxDXDY Tests
        [Fact]
        public void Calculate_MaxDXDYMetric_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointB, DistanceMetric.MaxDXDY);

            // Assert
            Assert.Equal(4, result); // max(3, 4) = 4
        }

        [Fact]
        public void Calculate_MaxDXDYMetric_Square_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointC, DistanceMetric.MaxDXDY);

            // Assert
            Assert.Equal(5, result); // max(5, 5) = 5
        }
        #endregion

        #region DiagonalShortcut Tests
        [Fact]
        public void Calculate_DiagonalShortcut_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointB, DistanceMetric.DiagonalShortcut);

            // Assert
            // min(3,4)=3 → 3*2=6, |3-4|=1 → total 7
            Assert.Equal(7, result);
        }

        [Fact]
        public void Calculate_DiagonalShortcut_Square_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointC, DistanceMetric.DiagonalShortcut);

            // Assert
            // min(5,5)=5 → 5*2=10, |5-5|=0 → total 10
            Assert.Equal(10, result);
        }
        #endregion

        #region EuclideanNoSQR Tests
        [Fact]
        public void Calculate_EuclideanNoSQR_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Calculate(PointA, PointB, DistanceMetric.EuclideanNoSQR);

            // Assert
            Assert.Equal(25, result); // 3² + 4² = 25
        }
        #endregion

        #region Individual Method Tests
        [Fact]
        public void Manhattan_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Manhattan(PointA, PointB);

            // Assert
            Assert.Equal(7, result);
        }

        [Fact]
        public void Euclidean_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Euclidean(PointA, PointB);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public void Chebyshev_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Chebyshev(PointA, PointB);

            // Assert
            Assert.Equal(4, result);
        }

        [Fact]
        public void Octile_ReturnsCorrectValue()
        {
            // Act
            int result = HeuristicCalculator.Octile(PointA, PointB);

            // Assert
            Assert.Equal(7, result);
        }
        #endregion

        #region Real Distance Conversion Tests
        [Fact]
        public void ToRealDistance_ConvertsCorrectly()
        {
            // Act
            double result = HeuristicCalculator.ToRealDistance(10, 5.0);

            // Assert
            Assert.Equal(50, result);
        }

        [Fact]
        public void ToGridDistance_ConvertsCorrectly()
        {
            // Act
            int result = HeuristicCalculator.ToGridDistance(47, 10.0);

            // Assert
            Assert.Equal(5, result); // Ceiling(47/10) = 5
        }
        #endregion
    }
}