#region File Header
/// <summary>
/// File: Cell.cs
/// Description: Represents a single cell in the grid map
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
#endregion

namespace SallamPathFinder4.Core.Models.Map
{
    #region Class Documentation
    /// <summary>
    /// Represents a single cell in the grid-based map
    /// Contains properties for navigation, cost calculation, and obstacle handling
    /// Uses struct for better memory efficiency (64 bytes or less recommended)
    /// </summary>
    #endregion
    public sealed class Cell
    {
        #region Constants
        private const byte MIN_SURFACE_WEIGHT = 1;
        private const byte MAX_SURFACE_WEIGHT = 100;
        private const byte DEFAULT_SURFACE_WEIGHT = 1;
        private const byte MIN_RAMP_DIFFICULTY = 0;
        private const byte MAX_RAMP_DIFFICULTY = 100;
        #endregion

        #region Private Fields
        private byte _surfaceWeight;
        private MapElementType _elementType;
        private bool _isDoorOpen;
        private byte _rampDifficulty;
        private DynamicObstacle _occupyingObstacle;
        private bool _isWalkable;
        private double _cost;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new cell at the specified grid coordinates
        /// </summary>
        /// <param name="x">X coordinate (column)</param>
        /// <param name="y">Y coordinate (row)</param>
        public Cell(int x, int y)
        {
            #region Validation
            if (x < 0) throw new ArgumentOutOfRangeException(nameof(x), "X coordinate cannot be negative");
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(y), "Y coordinate cannot be negative");
            #endregion

            X = x;
            Y = y;
            _surfaceWeight = DEFAULT_SURFACE_WEIGHT;
            _elementType = MapElementType.Empty;
            _isDoorOpen = true;
            _rampDifficulty = 0;
            _occupyingObstacle = null;
            _isWalkable = true;
            _cost = DEFAULT_SURFACE_WEIGHT;
        }
        #endregion

        #region Public Properties
        /// <summary>X coordinate (column index) in the grid</summary>
        public int X { get; }

        /// <summary>Y coordinate (row index) in the grid</summary>
        public int Y { get; }

        /// <summary>
        /// Surface weight (1-100) affecting movement cost
        /// Higher values = slower movement, more battery consumption
        /// </summary>
        public byte SurfaceWeight
        {
            get => _surfaceWeight;
            set
            {
                _surfaceWeight = Math.Clamp(value, MIN_SURFACE_WEIGHT, MAX_SURFACE_WEIGHT);
                UpdateCost();
            }
        }

        /// <summary>Type of map element (Wall, Door, Goal, etc.)</summary>
        public MapElementType ElementType
        {
            get => _elementType;
            set
            {
                _elementType = value;
                UpdateWalkabilityAndCost();
            }
        }

        /// <summary>Indicates whether the cell is walkable</summary>
        public bool IsWalkable
        {
            get => _isWalkable;
            set
            {
                _isWalkable = value;
                UpdateCost();
            }
        }

        /// <summary>Movement cost for pathfinding algorithms</summary>
        public double Cost
        {
            get => _cost;
            private set => _cost = value;
        }

        /// <summary>Door open/closed state (only relevant for Door elements)</summary>
        public bool IsDoorOpen
        {
            get => _isDoorOpen;
            set
            {
                _isDoorOpen = value;
                if (_elementType == MapElementType.Door)
                {
                    UpdateWalkabilityAndCost();
                }
            }
        }

        /// <summary>
        /// Ramp difficulty (0-100) affecting speed and battery
        /// Higher values = slower movement, more battery consumption
        /// </summary>
        public byte RampDifficulty
        {
            get => _rampDifficulty;
            set
            {
                _rampDifficulty = Math.Clamp(value, MIN_RAMP_DIFFICULTY, MAX_RAMP_DIFFICULTY);
                if (_elementType == MapElementType.Ramp)
                {
                    UpdateCost();
                }
            }
        }

        /// <summary>Dynamic obstacle currently occupying this cell (null if none)</summary>
        public DynamicObstacle OccupyingObstacle
        {
            get => _occupyingObstacle;
            set
            {
                _occupyingObstacle = value;
                UpdateWalkabilityAndCost();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates cell walkability and cost based on element type
        /// Called when ElementType changes or related properties change
        /// </summary>
        private void UpdateWalkabilityAndCost()
        {
            switch (_elementType)
            {
                case MapElementType.Wall:
                    _isWalkable = false;
                    _cost = double.MaxValue;
                    break;

                case MapElementType.Door:
                    _isWalkable = _isDoorOpen;
                    _cost = _isDoorOpen ? _surfaceWeight : double.MaxValue;
                    break;

                case MapElementType.Window:
                    _isWalkable = true;
                    _cost = _surfaceWeight * 1.5;
                    break;

                case MapElementType.Ramp:
                    _isWalkable = true;
                    double rampFactor = 1.0 + (_rampDifficulty / 100.0);
                    _cost = _surfaceWeight * rampFactor;
                    break;

                default: // Empty, StartPoint, GoalPoint, ParkingPoint
                    _isWalkable = true;
                    _cost = _surfaceWeight;
                    break;
            }

            // Dynamic obstacles override all other settings
            if (_occupyingObstacle != null)
            {
                _isWalkable = false;
                _cost = double.MaxValue;
            }
        }

        /// <summary>
        /// Updates only the cost value (preserves walkability)
        /// Used when SurfaceWeight changes
        /// </summary>
        private void UpdateCost()
        {
            if (!_isWalkable || _occupyingObstacle != null)
            {
                _cost = double.MaxValue;
                return;
            }

            switch (_elementType)
            {
                case MapElementType.Wall:
                    _cost = double.MaxValue;
                    break;
                case MapElementType.Door when !_isDoorOpen:
                    _cost = double.MaxValue;
                    break;
                case MapElementType.Door:
                    _cost = _surfaceWeight;
                    break;
                case MapElementType.Window:
                    _cost = _surfaceWeight * 1.5;
                    break;
                case MapElementType.Ramp:
                    double rampFactor = 1.0 + (_rampDifficulty / 100.0);
                    _cost = _surfaceWeight * rampFactor;
                    break;
                default:
                    _cost = _surfaceWeight;
                    break;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Resets the cell to default empty state
        /// </summary>
        public void Reset()
        {
            _surfaceWeight = DEFAULT_SURFACE_WEIGHT;
            _elementType = MapElementType.Empty;
            _isDoorOpen = true;
            _rampDifficulty = 0;
            _occupyingObstacle = null;
            _isWalkable = true;
            _cost = DEFAULT_SURFACE_WEIGHT;
        }

        /// <summary>
        /// Creates a deep copy of this cell
        /// </summary>
        public Cell Clone()
        {
            return new Cell(X, Y)
            {
                SurfaceWeight = this._surfaceWeight,
                ElementType = this._elementType,
                IsDoorOpen = this._isDoorOpen,
                RampDifficulty = this._rampDifficulty,
                OccupyingObstacle = this._occupyingObstacle
            };
        }
        #endregion

        #region Override Methods
        /// <summary>Returns string representation of the cell</summary>
        public override string ToString()
        {
            return $"Cell({X},{Y}) | Type: {_elementType} | Walkable: {_isWalkable} | Cost: {_cost:F2}";
        }
        #endregion
    }
}