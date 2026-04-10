#region File Header
/// <summary>
/// File: BatteryService.cs
/// Description: Service for battery management with realistic consumption formulas
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Interfaces.Services;
#endregion

namespace SallamPathFinder4.Services.Battery
{
    #region Class Documentation
    /// <summary>
    /// Service for battery management
    /// Calculates consumption based on distance, surface weight, speed, and ramp difficulty
    /// Implements realistic battery behavior for robot simulation
    /// </summary>
    #endregion
    public sealed class BatteryService : IBatteryService
    {
        #region Constants
        private const double DEFAULT_CAPACITY = 100.0;
        private const double DEFAULT_BASE_CONSUMPTION = 1.0;
        private const double MIN_BATTERY_THRESHOLD = 0;
        private const double LOW_BATTERY_WARNING = 20.0;
        private const double CRITICAL_BATTERY_WARNING = 10.0;
        #endregion

        #region Private Fields
        private double _currentCharge;
        private readonly double _capacity;
        private readonly double _baseConsumptionPerCell;
        private readonly object _lockObject = new object();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new battery service with default values
        /// </summary>
        public BatteryService() : this(100.0, 1.0)
        {
        }

        /// <summary>
        /// Initializes a new battery service with specified capacity and consumption rate
        /// </summary>
        public BatteryService(double capacity = 100.0, double baseConsumption = 1.0)
        {
            _capacity = capacity;
            _baseConsumptionPerCell = baseConsumption;
            _currentCharge = capacity;
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public double CurrentCharge
        {
            get
            {
                lock (_lockObject)
                {
                    return _currentCharge;
                }
            }
        }

        /// <inheritdoc/>
        public double Capacity => _capacity;
        #endregion

        #region Events
        /// <inheritdoc/>
        public event Action<double> BatteryChanged;
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public void Consume(double distance, double surfaceWeight, double speed)
        {
            if (distance <= 0) return;

            lock (_lockObject)
            {
                // Calculate consumption factors
                double surfaceFactor = Math.Max(0.1, surfaceWeight / 100.0);
                double speedFactor = Math.Max(0.5, Math.Min(2.0, speed / 10.0));

                // Ramp factor (will be set externally for ramps)
                double rampFactor = 1.0;

                // Base consumption formula
                double consumption = _baseConsumptionPerCell * distance * surfaceFactor * speedFactor * rampFactor;

                // Add penalty for low battery (battery drains faster when low)
                double lowBatteryPenalty = 1.0;
                if (_currentCharge < CRITICAL_BATTERY_WARNING)
                    lowBatteryPenalty = 1.5;
                else if (_currentCharge < LOW_BATTERY_WARNING)
                    lowBatteryPenalty = 1.2;

                consumption *= lowBatteryPenalty;

                // Apply consumption
                _currentCharge = Math.Max(MIN_BATTERY_THRESHOLD, _currentCharge - consumption);

                // Raise event
                BatteryChanged?.Invoke(_currentCharge);
            }
        }

        /// <inheritdoc/>
        public void Recharge()
        {
            lock (_lockObject)
            {
                _currentCharge = _capacity;
                BatteryChanged?.Invoke(_currentCharge);
            }
        }

        public void SetCharge(double charge)
        {
            _currentCharge = Math.Max(0, Math.Min(_capacity, charge));
            BatteryChanged?.Invoke(_currentCharge);
        }

        /// <inheritdoc/>
        public void Recharge(double amount)
        {
            lock (_lockObject)
            {
                _currentCharge = Math.Min(_capacity, _currentCharge + amount);
                BatteryChanged?.Invoke(_currentCharge);
            }
        }

        /// <inheritdoc/>
        public bool HasEnoughCharge(double required)
        {
            lock (_lockObject)
            {
                return _currentCharge >= required;
            }
        }

        /// <inheritdoc/>
        public double GetPercentage()
        {
            lock (_lockObject)
            {
                return (_currentCharge / _capacity) * 100.0;
            }
        }

        /// <summary>
        /// Calculates required battery for a given path
        /// Used to check if robot can complete the path
        /// </summary>
        public double CalculateRequiredBattery(double totalDistance, double averageSurfaceWeight, double averageSpeed)
        {
            double surfaceFactor = averageSurfaceWeight / 100.0;
            double speedFactor = averageSpeed / 10.0;

            return _baseConsumptionPerCell * totalDistance * surfaceFactor * speedFactor;
        }

        /// <summary>
        /// Checks if battery is low (below 20%)
        /// </summary>
        public bool IsLowBattery()
        {
            lock (_lockObject)
            {
                return _currentCharge < LOW_BATTERY_WARNING;
            }
        }

        /// <summary>
        /// Checks if battery is critical (below 10%)
        /// </summary>
        public bool IsCriticalBattery()
        {
            lock (_lockObject)
            {
                return _currentCharge < CRITICAL_BATTERY_WARNING;
            }
        }

        /// <summary>
        /// Checks if battery is empty (0%)
        /// </summary>
        public bool IsEmpty()
        {
            lock (_lockObject)
            {
                return _currentCharge <= MIN_BATTERY_THRESHOLD;
            }
        }
        #endregion
    }
}