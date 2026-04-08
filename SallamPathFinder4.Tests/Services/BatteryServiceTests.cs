#region File Header
/// <summary>
/// File: BatteryServiceTests.cs
/// Description: Unit tests for BatteryService class
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Services.Battery;
using Xunit;
#endregion

namespace SallamPathFinder4.Tests.Services
{
    #region Test Class Documentation
    /// <summary>
    /// Unit tests for BatteryService functionality
    /// Tests consumption, recharging, and battery level calculations
    /// </summary>
    #endregion
    public sealed class BatteryServiceTests
    {
        #region Initialization Tests
        [Fact]
        public void Constructor_DefaultValues_InitializesCorrectly()
        {
            // Act
            var battery = new BatteryService();

            // Assert
            Assert.Equal(100, battery.CurrentCharge);
            Assert.Equal(100, battery.Capacity);
            Assert.Equal(100, battery.GetPercentage());
        }

        [Fact]
        public void Constructor_CustomCapacity_InitializesCorrectly()
        {
            // Act
            var battery = new BatteryService(200, 1.5);

            // Assert
            Assert.Equal(200, battery.CurrentCharge);
            Assert.Equal(200, battery.Capacity);
        }
        #endregion

        #region Consumption Tests
        [Fact]
        public void Consume_ValidDistance_ReducesBattery()
        {
            // Arrange
            var battery = new BatteryService();

            // Act
            battery.Consume(10, 50, 10);

            // Assert
            Assert.True(battery.CurrentCharge < 100);
        }

        [Fact]
        public void Consume_ZeroDistance_NoChange()
        {
            // Arrange
            var battery = new BatteryService();

            // Act
            battery.Consume(0, 50, 10);

            // Assert
            Assert.Equal(100, battery.CurrentCharge);
        }

        [Fact]
        public void Consume_NegativeDistance_NoChange()
        {
            // Arrange
            var battery = new BatteryService();

            // Act
            battery.Consume(-5, 50, 10);

            // Assert
            Assert.Equal(100, battery.CurrentCharge);
        }

        [Fact]
        public void Consume_ExceedsCapacity_BatteryNotNegative()
        {
            // Arrange
            var battery = new BatteryService();

            // Act
            battery.Consume(1000, 100, 20);

            // Assert
            Assert.Equal(0, battery.CurrentCharge);
        }
        #endregion

        #region Recharge Tests
        [Fact]
        public void Recharge_FullRecharge_RestoresToFull()
        {
            // Arrange
            var battery = new BatteryService();
            battery.Consume(50, 50, 10);

            // Act
            battery.Recharge();

            // Assert
            Assert.Equal(100, battery.CurrentCharge);
        }

        [Fact]
        public void Recharge_PartialRecharge_IncreasesBattery()
        {
            // Arrange
            var battery = new BatteryService();
            battery.Consume(50, 50, 10);
            double beforeRecharge = battery.CurrentCharge;

            // Act
            battery.Recharge(20);

            // Assert
            Assert.Equal(beforeRecharge + 20, battery.CurrentCharge);
        }

        [Fact]
        public void Recharge_ExceedsCapacity_CapsAtMaximum()
        {
            // Arrange
            var battery = new BatteryService();
            battery.Consume(10, 50, 10);

            // Act
            battery.Recharge(200);

            // Assert
            Assert.Equal(100, battery.CurrentCharge);
        }
        #endregion

        #region Status Tests
        [Fact]
        public void HasEnoughCharge_EnoughCharge_ReturnsTrue()
        {
            // Arrange
            var battery = new BatteryService();

            // Act
            bool result = battery.HasEnoughCharge(50);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasEnoughCharge_NotEnoughCharge_ReturnsFalse()
        {
            // Arrange
            var battery = new BatteryService();
            battery.Consume(80, 50, 10);

            // Act
            bool result = battery.HasEnoughCharge(30);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsLowBattery_Below20_ReturnsTrue()
        {
            // Arrange
            var battery = new BatteryService();
            battery.Consume(90, 50, 10);

            // Act
            bool result = battery.IsLowBattery();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsCriticalBattery_Below10_ReturnsTrue()
        {
            // Arrange
            var battery = new BatteryService();
            battery.Consume(95, 50, 10);

            // Act
            bool result = battery.IsCriticalBattery();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEmpty_ZeroBattery_ReturnsTrue()
        {
            // Arrange
            var battery = new BatteryService();
            battery.Consume(1000, 100, 20);

            // Act
            bool result = battery.IsEmpty();

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Event Tests
        [Fact]
        public void Consume_RaisesBatteryChangedEvent()
        {
            // Arrange
            var battery = new BatteryService();
            bool eventRaised = false;

            battery.BatteryChanged += (level) => eventRaised = true;

            // Act
            battery.Consume(10, 50, 10);

            // Assert
            Assert.True(eventRaised);
        }

        [Fact]
        public void Recharge_RaisesBatteryChangedEvent()
        {
            // Arrange
            var battery = new BatteryService();
            battery.Consume(50, 50, 10);
            bool eventRaised = false;

            battery.BatteryChanged += (level) => eventRaised = true;

            // Act
            battery.Recharge();

            // Assert
            Assert.True(eventRaised);
        }
        #endregion
    }
}