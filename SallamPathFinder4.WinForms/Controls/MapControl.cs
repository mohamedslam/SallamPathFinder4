#region File Header
/// <summary>
/// File: MapControl.cs
/// Description: Professional map control for grid-based pathfinding visualization
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-18
/// </summary>
#endregion

#region Namespace Imports
using Newtonsoft.Json.Linq;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
using SallamPathFinder4.WinForms.Helpers;
using System.Drawing.Drawing2D;
using System.Text.Json;
#endregion

namespace SallamPathFinder4.WinForms.Controls
{
    public sealed partial class MapControl : Panel
    {
        #region Enums
        public enum DrawMode
        {
            None,
            SetWeight,
            SetElement,
            Erase,
            SetDynamicObstacle,
            SetParkingPoint
        }
        #endregion

        #region Constants 
        private const int MIN_CELL_SIZE = 4;
        private const int DEFAULT_CELL_SIZE = 30;
        private const float MIN_ZOOM = 0.5f;
        private const float MAX_ZOOM = 3.0f;
        private const float ZOOM_STEP = 0.1f;
        private const double SQRT2 = 1.4142135623730951;
        private const int NORMAL_PATH_THICKNESS = 4;
        private const int RETURN_PATH_THICKNESS = 2;
        private const int CHARGING_PATH_THICKNESS = 4;
        private const string ROBOTS_FOLDER = "Robots";
        #endregion

        #region Private Fields - Special Cells
        private Point _startPoint;
        private HashSet<Point> _collisionCells;
        private HashSet<Point> _scannedCells;
        private HashSet<Point> _invalidPathCells;
        private Dictionary<(int, int), int> _startPoints;
        private int _nextStartIndex;
        // Active obstacles from simulation
        private List<DetectedObstacle> _activeObstacles;
        private Dictionary<Point, double> _hotspotRiskLevels;
        #endregion

        #region Private Fields - Core
        private MapGrid _mapGrid;
        private List<GoalPoint> _goals;
        private List<ParkingPoint> _parkingPoints;
        private List<ColoredPath> _coloredPaths;
        private int _cellSize;
        private DrawMode _currentDrawMode;
        private byte _currentWeight;
        private MapElementType _currentElement;
        private bool _showGrid;
        private bool _showCoordinates;
        private float _zoomLevel;
        private PointF _viewOffset;
        private Point? _highlightedCell;
        private bool _showOrderNumbers = false;
        // Legend visibility and position
        private bool _showLegend = true;
        private Point _legendPosition = new Point(10, 10);  // Top-left corner
                                                            // Wait countdown display
        private bool _isWaiting = false;
        private double _remainingWaitSeconds = 0;
        private Point _waitLocation;
        private ObstacleType _waitingForObstacle;
        private System.Windows.Forms.Timer _countdownTimer;
        // Legend dragging
        private bool _isDraggingLegend = false;
        private Point _legendDragStart;
        #endregion

        #region Private Fields - Robot (Legacy - For backward compatibility)
        private Point _robotPosition;
        private float _robotAngle = 0;
        private bool _showRobot = true;
        private double _robotSpeed = 10.0;
        private bool _isMovingForward = false;
        private bool _isMovingBackward = false;
        private int _robotWidthCm = 30;
        private int _robotLengthCm = 30;
        private int _robotHeightCm = 40;
        private double _scaleCmPerCell = 10.0;
        #endregion

        #region Private Fields - Robot Management
        private List<RobotDefinition> _availableRobots;
        private RobotDefinition _selectedRobot;
        private RobotDefinition _currentRobot;
        private string _robotsDirectoryPath;
        private bool _useCustomRobot = false;
        private bool _showSensorFOV = true;
        private bool _showSensorLabels = true;
        #endregion

        #region Private Fields - Obstacles
        private List<DynamicObstacle> _dynamicObstacles;
        private ObstacleType _currentObstacleType;
        #endregion

        //#region Private Fields - Detection Zone
        //private bool _showDetectionZone;
        //private Color _detectionZoneColor;
        //private List<Point> _detectionZoneCells;
        //private int _detectionRangeCells;
        //#endregion

        #region Private Fields - Interaction
        private bool _isPanning;
        private Point _lastMousePosition;
        private Random _random;
        #endregion

        #region Private Fields - Start Point
        private Point _robotStartPoint;
        #endregion

        #region Private Fields - Robot Friction
        private double _currentSpeed = 0;
        private System.Windows.Forms.Timer _frictionTimer;
        private bool _isMoving = false;
        #endregion

        #region Private Fields - GIF Recording
        private GifRecorder _gifRecorder;
        private bool _isRecording = false;
        #endregion

        #region Private Fields - Search Visualization
        private HashSet<Point> _openCells = new HashSet<Point>();
        private HashSet<Point> _closedCells = new HashSet<Point>();
        private Point? _currentCell = null;
        private List<Point> _pathCells = new List<Point>();
        private Dictionary<Point, Point> _cellParents = new Dictionary<Point, Point>();
        private Dictionary<Point, int> _cellOrder = new Dictionary<Point, int>();
        private int _currentOrder = 0;
        private int _arrowDrawCounter = 0;
        private const int ARROW_DRAW_INTERVAL = 10;
        #endregion

        #region Constructor
        public MapControl()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            BackColor = Color.White;
            AutoScroll = false;

            InitializeRobotManagement();

            _goals = new List<GoalPoint>();
            _parkingPoints = new List<ParkingPoint>();
            _coloredPaths = new List<ColoredPath>();
            _dynamicObstacles = new List<DynamicObstacle>();
            //_detectionZoneCells = new List<Point>();
            _random = new Random();

            _cellSize = DEFAULT_CELL_SIZE;
            _showGrid = true;
            _showCoordinates = false;
            _zoomLevel = 1.0f;
            _viewOffset = PointF.Empty;
            _showRobot = true;
            _robotPosition = new Point(10, 10);
            _robotAngle = 0;
            _currentDrawMode = DrawMode.None;
            _currentWeight = 1;
            _currentElement = MapElementType.Wall;
            _currentObstacleType = ObstacleType.Adult;
            //_showDetectionZone = true;
            //_detectionZoneColor = Color.FromArgb(80, 52, 152, 219);
            //_detectionRangeCells = 2;

            this.MouseWheel += MapControl_MouseWheel;

            _collisionCells = new HashSet<Point>();
            _scannedCells = new HashSet<Point>();
            _invalidPathCells = new HashSet<Point>();
            _startPoint = new Point(10, 10);
            _startPoints = new Dictionary<(int, int), int>();
            _nextStartIndex = 0;
            //  Initialize obstacle lists
            _activeObstacles = new List<DetectedObstacle>();
            _hotspotRiskLevels = new Dictionary<Point, double>();
        }

        private void InitializeComponent()
        {
            this.Name = "MapControl";
            this.Size = new Size(400, 400);
        }
        #endregion

        #region Robot Management Initialization
        /// <summary>
        /// Initializes the robot management system
        /// Creates robots directory if not exists and loads all saved robots
        /// </summary>
        private void InitializeRobotManagement()
        {
            _availableRobots = new List<RobotDefinition>();
            _robotsDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ROBOTS_FOLDER);

            // Create robots directory if it doesn't exist
            if (!Directory.Exists(_robotsDirectoryPath))
            {
                Directory.CreateDirectory(_robotsDirectoryPath);
            }

            // Load all saved robots
            LoadAllRobots();

            // If no robots exist, create a default robot from legacy settings
            if (_availableRobots.Count == 0)
            {
                CreateDefaultRobotFromLegacy();
            }

            // Select the first robot as default
            _selectedRobot = _availableRobots.FirstOrDefault();
        }

        /// <summary>
        /// Creates a default robot from the legacy settings for backward compatibility
        /// </summary>
        private void CreateDefaultRobotFromLegacy()
        {
            var defaultRobot = new RobotDefinition
            {
                RobotId = Guid.NewGuid().ToString(),
                RobotName = "Default Robot",
                RobotType = RobotType.Wheeled,
                CreatedAt = DateTime.Now,
                Description = "Default robot created from legacy settings",
                Appearance = new RobotAppearance
                {
                    ShapeType = RobotShapeType.RoundedRect,
                    Width = _robotWidthCm,
                    Height = _robotHeightCm,
                    Length = _robotLengthCm,
                    Color = "#3498db",
                    Icon = "🤖",
                    ShowWheels = true,
                    ShowDirectionArrow = true,
                    ShowSensorPoints = true
                },
                Kinematics = new RobotKinematics
                {
                    MaxForwardSpeed = 1.5,
                    MaxReverseSpeed = 0.8,
                    MaxLateralSpeed = 0.5,
                    MaxTurnRate = 90,
                    MinTurnRadius = 30,
                    TurnAcceleration = 45,
                    LinearAcceleration = 0.5,
                    LinearDeceleration = 1.0,
                    MaxSlopeAngle = 30,
                    MaxStepHeight = 5,
                    MaxGapWidth = 10,
                    Wheelbase = 40,
                    TrackWidth = 35
                }
            };

            _availableRobots.Add(defaultRobot);
            SaveRobot(defaultRobot);
        }
        #endregion

        #region Robot Management - Public Methods
        /// <summary>
        /// Gets the list of available robot names for UI display
        /// </summary>
        /// <returns>List of robot names</returns>
        public List<string> GetRobotNames()
        {
            return _availableRobots?.Select(r => r.RobotName).ToList() ?? new List<string>();
        }

        /// <summary>
        /// Gets the list of available robots
        /// </summary>
        public List<RobotDefinition> GetAvailableRobots() => _availableRobots;

        /// <summary>
        /// Gets the currently selected robot
        /// </summary>
        public RobotDefinition GetSelectedRobot() => _selectedRobot;

        /// <summary>
        /// Sets whether to use custom robot drawing or legacy drawing
        /// </summary>
        /// <param name="use">True to use custom robot, false to use legacy</param>
        public void SetUseCustomRobot(bool use)
        {
            _useCustomRobot = use;
            Invalidate();
        }

        /// <summary>
        /// Gets whether custom robot drawing is enabled
        /// </summary>
        public bool IsUsingCustomRobot() => _useCustomRobot;

        /// <summary>
        /// Selects a robot by its ID
        /// </summary>
        /// <param name="robotId">The robot ID to select</param>
        /// <returns>True if found and selected, false otherwise</returns>
        public bool SelectRobot(string robotId)
        {
            var robot = _availableRobots?.FirstOrDefault(r => r.RobotId == robotId);
            if (robot != null)
            {
                _selectedRobot = robot;

                // Update legacy properties for backward compatibility
                UpdateLegacyPropertiesFromRobot(robot);

                Invalidate();
                OnRobotSelected?.Invoke(robot);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Selects a robot by its name
        /// </summary>
        /// <param name="robotName">The robot name to select</param>
        /// <returns>True if found and selected, false otherwise</returns>
        public bool SelectRobotByName(string robotName)
        {
            var robot = _availableRobots?.FirstOrDefault(r => r.RobotName == robotName);
            if (robot != null)
            {
                return SelectRobot(robot.RobotId);
            }
            return false;
        }

        /// <summary>
        /// Adds a new custom robot to the collection
        /// </summary>
        /// <param name="robot">The robot definition to add</param>
        public void AddCustomRobot(RobotDefinition robot)
        {
            if (robot == null) throw new ArgumentNullException(nameof(robot));

            if (_availableRobots == null)
                _availableRobots = new List<RobotDefinition>();

            // Ensure robot has an ID
            if (string.IsNullOrEmpty(robot.RobotId))
                robot.RobotId = Guid.NewGuid().ToString();

            _availableRobots.Add(robot);
            SaveRobot(robot);

            // Auto-select if this is the first robot
            if (_selectedRobot == null)
                SelectRobot(robot.RobotId);

            OnRobotAdded?.Invoke(robot);
            Invalidate();
        }

        /// <summary>
        /// Deletes a robot by its ID
        /// </summary>
        /// <param name="robotId">The robot ID to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public bool DeleteRobot(string robotId)
        {
            if (_availableRobots == null || _availableRobots.Count <= 1)
            {
                // Cannot delete the last robot
                System.Diagnostics.Debug.WriteLine("Cannot delete the last robot");
                return false;
            }

            var robot = _availableRobots.FirstOrDefault(r => r.RobotId == robotId);
            if (robot == null) return false;

            // Delete the file
            string filePath = Path.Combine(_robotsDirectoryPath, $"{robot.RobotId}.json");
            if (File.Exists(filePath))
                File.Delete(filePath);

            // Remove from list
            _availableRobots.Remove(robot);

            // If the deleted robot was selected, select another one
            if (_selectedRobot?.RobotId == robotId)
            {
                _selectedRobot = _availableRobots.FirstOrDefault();
                if (_selectedRobot != null)
                    UpdateLegacyPropertiesFromRobot(_selectedRobot);
            }

            OnRobotDeleted?.Invoke(robot);
            Invalidate();
            return true;
        }

        /// <summary>
        /// Saves the currently selected robot
        /// </summary>
        public void SaveCurrentRobot()
        {
            if (_selectedRobot != null)
                SaveRobot(_selectedRobot);
        }

        /// <summary>
        /// Updates the legacy robot properties from a RobotDefinition
        /// </summary>
        /// <param name="robot">The robot definition to sync from</param>
        private void UpdateLegacyPropertiesFromRobot(RobotDefinition robot)
        {
            if (robot?.Appearance != null)
            {
                _robotWidthCm = (int)robot.Appearance.Width;
                _robotLengthCm = (int)robot.Appearance.Length;
                _robotHeightCm = (int)robot.Appearance.Height;
            }

            if (robot?.Kinematics != null)
            {
                _robotSpeed = robot.Kinematics.MaxForwardSpeed * 10; // Convert to cm/s approximation
            }
        }

        /// <summary>
        /// Creates a new empty robot definition for user customization
        /// </summary>
        /// <returns>A new robot definition with default values</returns>
        public RobotDefinition CreateNewRobotTemplate()
        {
            return new RobotDefinition
            {
                RobotId = Guid.NewGuid().ToString(),
                RobotName = "New Robot",
                RobotType = RobotType.Wheeled,
                CreatedAt = DateTime.Now,
                Description = string.Empty,
                Appearance = new RobotAppearance
                {
                    ShapeType = RobotShapeType.RoundedRect,
                    Width = _robotWidthCm,
                    Height = _robotHeightCm,
                    Length = _robotLengthCm,
                    Color = "#3498db",
                    Icon = "🤖",
                    ShowWheels = true,
                    ShowDirectionArrow = true,
                    ShowSensorPoints = true
                },
                Kinematics = new RobotKinematics
                {
                    MaxForwardSpeed = 1.5,
                    MaxReverseSpeed = 0.8,
                    MaxLateralSpeed = 0.5,
                    MaxTurnRate = 90,
                    MinTurnRadius = 30,
                    TurnAcceleration = 45,
                    LinearAcceleration = 0.5,
                    LinearDeceleration = 1.0,
                    MaxSlopeAngle = 30,
                    MaxStepHeight = 5,
                    MaxGapWidth = 10,
                    Wheelbase = 40,
                    TrackWidth = 35
                }
            };
        }
        #endregion

        #region Robot Management - Private Methods
        /// <summary>
        /// Loads all robots from the robots directory
        /// </summary>
        private void LoadAllRobots()
        {
            if (!Directory.Exists(_robotsDirectoryPath)) return;

            var robotFiles = Directory.GetFiles(_robotsDirectoryPath, "*.json");
            foreach (var filePath in robotFiles)
            {
                try
                {
                    var robot = LoadRobotFromFile(filePath);
                    if (robot != null)
                        _availableRobots.Add(robot);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load robot from {filePath}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Loads a single robot from a JSON file
        /// </summary>
        /// <param name="filePath">Path to the JSON file</param>
        /// <returns>RobotDefinition or null if load fails</returns>
        private RobotDefinition LoadRobotFromFile(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var robot = new RobotDefinition
                {
                    RobotId = root.GetProperty("RobotId").GetString(),
                    RobotName = root.GetProperty("RobotName").GetString(),
                    CreatedAt = root.GetProperty("CreatedAt").GetDateTime(),
                    Description = root.GetProperty("Description").GetString() ?? string.Empty
                };

                // Parse RobotType
                if (root.TryGetProperty("RobotType", out var robotTypeElement))
                {
                    if (robotTypeElement.ValueKind == JsonValueKind.String)
                        robot.RobotType = Enum.Parse<RobotType>(robotTypeElement.GetString());
                    else if (robotTypeElement.ValueKind == JsonValueKind.Number)
                        robot.RobotType = (RobotType)robotTypeElement.GetInt32();
                }

                // Parse Appearance
                if (root.TryGetProperty("Appearance", out var appearance))
                {
                    if (appearance.TryGetProperty("ShapeType", out var shapeTypeElement))
                    {
                        if (shapeTypeElement.ValueKind == JsonValueKind.String)
                            robot.Appearance.ShapeType = Enum.Parse<RobotShapeType>(shapeTypeElement.GetString());
                        else if (shapeTypeElement.ValueKind == JsonValueKind.Number)
                            robot.Appearance.ShapeType = (RobotShapeType)shapeTypeElement.GetInt32();
                    }

                    robot.Appearance.Width = appearance.GetProperty("Width").GetDouble();
                    robot.Appearance.Height = appearance.GetProperty("Height").GetDouble();
                    robot.Appearance.Length = appearance.GetProperty("Length").GetDouble();
                    robot.Appearance.Color = appearance.GetProperty("Color").GetString();
                    robot.Appearance.Icon = appearance.GetProperty("Icon").GetString();
                    robot.Appearance.ShowWheels = appearance.GetProperty("ShowWheels").GetBoolean();
                    robot.Appearance.ShowDirectionArrow = appearance.GetProperty("ShowDirectionArrow").GetBoolean();
                    robot.Appearance.ShowSensorPoints = appearance.GetProperty("ShowSensorPoints").GetBoolean();
                }

                // Parse Kinematics
                if (root.TryGetProperty("Kinematics", out var kinematics))
                {
                    robot.Kinematics.MaxForwardSpeed = kinematics.GetProperty("MaxForwardSpeed").GetDouble();
                    robot.Kinematics.MaxReverseSpeed = kinematics.GetProperty("MaxReverseSpeed").GetDouble();
                    robot.Kinematics.MaxLateralSpeed = kinematics.GetProperty("MaxLateralSpeed").GetDouble();
                    robot.Kinematics.MaxTurnRate = kinematics.GetProperty("MaxTurnRate").GetDouble();
                    robot.Kinematics.MinTurnRadius = kinematics.GetProperty("MinTurnRadius").GetDouble();
                    robot.Kinematics.TurnAcceleration = kinematics.GetProperty("TurnAcceleration").GetDouble();
                    robot.Kinematics.LinearAcceleration = kinematics.GetProperty("LinearAcceleration").GetDouble();
                    robot.Kinematics.LinearDeceleration = kinematics.GetProperty("LinearDeceleration").GetDouble();
                    robot.Kinematics.MaxSlopeAngle = kinematics.GetProperty("MaxSlopeAngle").GetDouble();
                    robot.Kinematics.MaxStepHeight = kinematics.GetProperty("MaxStepHeight").GetDouble();
                    robot.Kinematics.MaxGapWidth = kinematics.GetProperty("MaxGapWidth").GetDouble();
                    robot.Kinematics.Wheelbase = kinematics.GetProperty("Wheelbase").GetDouble();
                    robot.Kinematics.TrackWidth = kinematics.GetProperty("TrackWidth").GetDouble();
                }

                // Parse Sensors
                if (root.TryGetProperty("Sensors", out var sensorsArray))
                {
                    foreach (var sensorElement in sensorsArray.EnumerateArray())
                    {
                        var sensor = new SimpleSensor
                        {
                            SensorId = sensorElement.GetProperty("SensorId").GetString(),
                            SensorName = sensorElement.GetProperty("SensorName").GetString(),
                            SensorType = sensorElement.GetProperty("SensorType").GetString(),
                            PositionX = sensorElement.GetProperty("PositionX").GetInt32(),
                            PositionY = sensorElement.GetProperty("PositionY").GetInt32(),
                            MountAngle = sensorElement.GetProperty("MountAngle").GetDouble(),
                            DisplayColor = sensorElement.GetProperty("DisplayColor").GetString()
                        };
                        robot.Sensors.Add(sensor);
                    }
                }

                return robot;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading robot: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Saves a robot to a JSON file
        /// </summary>
        /// <param name="robot">The robot to save</param>
        private void SaveRobot(RobotDefinition robot)
        {
            try
            {
                string filePath = Path.Combine(_robotsDirectoryPath, $"{robot.RobotId}.json");
                robot.SaveToFile(filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving robot: {ex.Message}");
            }
        }
        #endregion

        #region Robot Drawing Methods
        /// <summary>
        /// Main robot drawing entry point - dispatches to custom or legacy drawing
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawRobot(Graphics g)
        {
            System.Diagnostics.Debug.WriteLine($"DrawRobot: _showRobot={_showRobot}, _useCustomRobot={_useCustomRobot}, _selectedRobot={(_selectedRobot != null)}, _robotPosition=({_robotPosition.X},{_robotPosition.Y})");

            if (!_showRobot)
            {
                System.Diagnostics.Debug.WriteLine("  - EXIT: _showRobot is false");
                return;
            }

            if (_mapGrid == null)
            {
                System.Diagnostics.Debug.WriteLine("  - EXIT: _mapGrid is null");
                return;
            }

            if (!_mapGrid.IsValidCoordinate(_robotPosition.X, _robotPosition.Y))
            {
                System.Diagnostics.Debug.WriteLine($"  - EXIT: invalid robot position ({_robotPosition.X},{_robotPosition.Y})");
                return;
            }

            if (!_showRobot || _mapGrid == null || !_mapGrid.IsValidCoordinate(_robotPosition.X, _robotPosition.Y)) return;

            if (_useCustomRobot && _selectedRobot != null)
                DrawCustomRobot(g);
            else
                DrawLegacyRobot(g);
        }

        /// <summary>
        /// Draws the robot using the custom RobotDefinition
        /// Supports multiple shapes: Rectangle, Square, Circle, RoundedRect, Triangle, Hexagon
        /// The robot body is rotated to face the movement direction
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawCustomRobot(Graphics g)
        {
            if (_selectedRobot == null) return;

            Rectangle robotRect = GetCellRect(_robotPosition.X, _robotPosition.Y);
            if (robotRect.Width <= 0 || robotRect.Height <= 0) return;

            // Save current graphics state before transformations
            var state = g.Save();

            // Move origin to robot center
            int centerX = robotRect.X + robotRect.Width / 2;
            int centerY = robotRect.Y + robotRect.Height / 2;
            g.TranslateTransform(centerX, centerY);

            // Apply rotation to face movement direction
            g.RotateTransform(_robotAngle+90f);

            // Calculate dimensions based on robot definition
            int cellSizePx = robotRect.Width;
            double cellSizeCm = _scaleCmPerCell;
            if (cellSizeCm <= 0) cellSizeCm = 10.0;

            double widthRatio = Math.Max(0.3, Math.Min(0.9, _selectedRobot.Appearance.Width / cellSizeCm));
            double lengthRatio = Math.Max(0.3, Math.Min(0.9, _selectedRobot.Appearance.Length / cellSizeCm));

            //int robotWidthPx = (int)(widthRatio * cellSizePx);
            //int robotLengthPx = (int)(lengthRatio * cellSizePx);
            //robotWidthPx = Math.Min(robotWidthPx, cellSizePx - 2);
            //robotLengthPx = Math.Min(robotLengthPx, cellSizePx - 2);
            double robotWidthCm = _selectedRobot.Appearance.Width;
            double robotLengthCm = _selectedRobot.Appearance.Length;
            double robotHeightCm = _selectedRobot.Appearance.Height;
            // Convert cm to pixels using the current scale (cm per cell)
            
            double scaleFactor = 1.0;  // Optional fine-tuning scale factor (default = 1.0 for exact size) Change to 0.95 or 1.05 as needed 
            int robotWidthPx = (int)(robotWidthCm / cellSizeCm * cellSizePx * scaleFactor);
            int robotLengthPx = (int)(robotLengthCm / cellSizeCm * cellSizePx * scaleFactor);
            // Clamp to reasonable bounds (minimum 4px, maximum cell size * 2)
            robotWidthPx = Math.Max(4, Math.Min(robotWidthPx, cellSizePx * 2));
            robotLengthPx = Math.Max(4, Math.Min(robotLengthPx, cellSizePx * 2));

            if (robotWidthPx < 4) robotWidthPx = 4;
            if (robotLengthPx < 4) robotLengthPx = 4;

            // Get body color from robot definition
            Color bodyColor;
            try
            {
                bodyColor = ColorTranslator.FromHtml(_selectedRobot.Appearance.Color);
            }
            catch
            {
                bodyColor = Color.FromArgb(52, 152, 219);
            }

            // 1. Draw robot body based on shape type
            switch (_selectedRobot.Appearance.ShapeType)
            {
                case RobotShapeType.Circle:
                    DrawRobotCircle(g, robotWidthPx, robotLengthPx, bodyColor);
                    break;
                case RobotShapeType.Triangle:
                    DrawRobotTriangle(g, robotWidthPx, robotLengthPx, bodyColor);
                    break;
                case RobotShapeType.Hexagon:
                    DrawRobotHexagon(g, robotWidthPx, robotLengthPx, bodyColor);
                    break;
                case RobotShapeType.Square:
                    DrawRobotSquare(g, robotWidthPx, robotLengthPx, bodyColor);
                    break;
                case RobotShapeType.RoundedRect:
                    DrawRobotRoundedRect(g, robotWidthPx, robotLengthPx, bodyColor);
                    break;
                case RobotShapeType.Rectangle:
                default:
                    DrawRobotRectangle(g, robotWidthPx, robotLengthPx, bodyColor);
                    break;
            }

            // 2. Draw robot-specific components (wheels, tracks, propellers)
            switch (_selectedRobot.RobotType)
            {
                case RobotType.Flying:
                    DrawDroneComponents(g, robotWidthPx, robotLengthPx);
                    break;
                case RobotType.Tracked:
                    DrawTrackComponents(g, robotWidthPx, robotLengthPx);
                    break;
                case RobotType.Wheeled:
                    DrawWheelComponents(g, robotWidthPx, robotLengthPx);
                    break;
                case RobotType.Omnidirectional:
                    DrawOmniComponents(g, robotWidthPx, robotLengthPx);
                    break;
            }

            // 3. Draw direction arrow if enabled (shows robot's front)
            if (_selectedRobot.Appearance.ShowDirectionArrow)
            {
                DrawDirectionArrowOnRobot(g, robotLengthPx);
            }

            // 4. Draw sensors as points if enabled
            if (_selectedRobot.Appearance.ShowSensorPoints && _selectedRobot.Sensors.Count > 0)
            {
                DrawRobotSensors(g, robotWidthPx, robotLengthPx);
            }
            // ✅ Draw sensor FOV cones (drawn AFTER sensors so they appear on top)
            if (_showSensorFOV && _selectedRobot.Sensors.Count > 0)
            {
                DrawAllSensorsFOV(g, robotWidthPx, robotLengthPx);
            }

            // 5. Draw sensor field of view (FOV) if enabled
            if (_showSensorFOV && _selectedRobot.Sensors.Count > 0)
            {
                DrawAllSensorsFOV(g, robotWidthPx, robotLengthPx);
            }

            // Restore original graphics state (undo translation and rotation)
            g.Restore(state);
        }
        #region Robot Components Drawing

        /// <summary>
        /// Draws propellers and arms for flying robot (Drone)
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Robot width in pixels</param>
        /// <param name="length">Robot length in pixels</param>
        private void DrawDroneComponents(Graphics g, int width, int length)
        {
            int armLength = width / 2;
            int motorSize = Math.Max(4, width / 6);
            int propellerLength = motorSize * 2;

            using (var armPen = new Pen(Color.FromArgb(100, 100, 100), 2))
            using (var motorBrush = new SolidBrush(Color.FromArgb(80, 80, 80)))
            using (var propellerBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
            {
                // Cross arms
                g.DrawLine(armPen, -armLength, -armLength, armLength, armLength);
                g.DrawLine(armPen, -armLength, armLength, armLength, -armLength);

                // Four propeller positions
                Point[] motorPositions = new Point[]
                {
            new Point(-armLength, -armLength),  // Top-left
            new Point(armLength, -armLength),   // Top-right
            new Point(-armLength, armLength),   // Bottom-left
            new Point(armLength, armLength)     // Bottom-right
                };

                foreach (var pos in motorPositions)
                {
                    // Motor
                    g.FillEllipse(motorBrush, pos.X - motorSize / 2, pos.Y - motorSize / 2, motorSize, motorSize);
                    g.DrawEllipse(Pens.Black, pos.X - motorSize / 2, pos.Y - motorSize / 2, motorSize, motorSize);

                    // Propeller (rotates with robot)
                    var state = g.Save();
                    g.TranslateTransform(pos.X, pos.Y);
                    g.RotateTransform(_robotAngle);
                    g.FillRectangle(propellerBrush, -propellerLength / 2, -motorSize / 3, propellerLength, motorSize / 2);
                    g.Restore(state);
                }
            }
        }


        /// <summary>
        /// Draws tracks for tracked robot
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Robot width in pixels</param>
        /// <param name="length">Robot length in pixels</param>
        private void DrawTrackComponents(Graphics g, int width, int length)
        {
            int trackWidth = Math.Max(3, width / 8);
            int trackHeight = length;

            using (var trackBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
            using (var wheelBrush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                // Left track
                g.FillRectangle(trackBrush, -width / 2 - trackWidth, -trackHeight / 2, trackWidth, trackHeight);
                // Right track
                g.FillRectangle(trackBrush, width / 2, -trackHeight / 2, trackWidth, trackHeight);

                // Track wheels (rollers)
                for (int i = -trackHeight / 2 + 5; i < trackHeight / 2 - 5; i += 8)
                {
                    g.FillEllipse(wheelBrush, -width / 2 - trackWidth / 2, i - 2, trackWidth, 4);
                    g.FillEllipse(wheelBrush, width / 2 - trackWidth / 2, i - 2, trackWidth, 4);
                }
            }
        }

        /// <summary>
        /// Draws wheels for wheeled robot
        /// Wheels are placed at the rear of the robot
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Robot width in pixels</param>
        /// <param name="length">Robot length in pixels</param>
        private void DrawWheelComponents(Graphics g, int width, int length)
        {
            int wheelWidth = Math.Max(4, width / 6);
            int wheelHeight = Math.Max(6, length / 5);
            int wheelOffset = width / 3;

            using (var wheelBrush = new SolidBrush(Color.FromArgb(44, 62, 80)))
            using (var axleBrush = new SolidBrush(Color.FromArgb(150, 150, 150)))
            {
                // Rear wheels
                g.FillEllipse(wheelBrush, -wheelOffset - wheelWidth / 2, length / 2 - wheelHeight / 2, wheelWidth, wheelHeight);
                g.FillEllipse(wheelBrush, wheelOffset - wheelWidth / 2, length / 2 - wheelHeight / 2, wheelWidth, wheelHeight);

                // Wheel axles
                g.FillEllipse(axleBrush, -wheelOffset - wheelWidth / 4, length / 2 - wheelHeight / 4, wheelWidth / 2, wheelHeight / 2);
                g.FillEllipse(axleBrush, wheelOffset - wheelWidth / 4, length / 2 - wheelHeight / 4, wheelWidth / 2, wheelHeight / 2);

                // Front wheels for longer robots
                if (length > width)
                {
                    g.FillEllipse(wheelBrush, -wheelOffset - wheelWidth / 2, -length / 2 + wheelHeight / 2, wheelWidth, wheelHeight);
                    g.FillEllipse(wheelBrush, wheelOffset - wheelWidth / 2, -length / 2 + wheelHeight / 2, wheelWidth, wheelHeight);

                    g.FillEllipse(axleBrush, -wheelOffset - wheelWidth / 4, -length / 2 + wheelHeight / 4, wheelWidth / 2, wheelHeight / 2);
                    g.FillEllipse(axleBrush, wheelOffset - wheelWidth / 4, -length / 2 + wheelHeight / 4, wheelWidth / 2, wheelHeight / 2);
                }
            }
        }


        /// <summary>
        /// Draws mecanum wheels for omnidirectional robot
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Robot width in pixels</param>
        /// <param name="length">Robot length in pixels</param>
        private void DrawOmniComponents(Graphics g, int width, int length)
        {
            int wheelSize = Math.Max(4, width / 5);
            int offset = width / 3;

            using (var wheelBrush = new SolidBrush(Color.FromArgb(44, 62, 80)))
            using (var rollerBrush = new SolidBrush(Color.FromArgb(100, 100, 100)))
            {
                // Four mecanum wheels
                Point[] wheelPositions = new Point[]
                {
            new Point(-offset, -offset),  // Front-left
            new Point(offset, -offset),   // Front-right
            new Point(-offset, offset),   // Rear-left
            new Point(offset, offset)     // Rear-right
                };

                foreach (var pos in wheelPositions)
                {
                    g.FillEllipse(wheelBrush, pos.X - wheelSize / 2, pos.Y - wheelSize / 2, wheelSize, wheelSize);

                    // Rollers at 45 degrees
                    var state = g.Save();
                    g.TranslateTransform(pos.X, pos.Y);
                    g.RotateTransform(45);
                    g.FillRectangle(rollerBrush, -wheelSize / 2, -wheelSize / 6, wheelSize, wheelSize / 3);
                    g.Restore(state);
                }
            }
        }

        #endregion

        #region Sensor FOV Drawing

        /// <summary>
        /// Draws field of view for all sensors exactly as designed in Robot Designer
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="robotWidthPx">Robot width in pixels</param>
        /// <param name="robotHeightPx">Robot height in pixels</param>
        private void DrawAllSensorsFOV(Graphics g, int robotWidthPx, int robotHeightPx)
        {
            if (_selectedRobot?.Sensors == null) return;

            foreach (var sensor in _selectedRobot.Sensors)
            {
                if (!sensor.IsEnabled) continue;

                // Calculate sensor position on robot (relative to center)
                int sensorX = (int)(sensor.PositionX / 100.0 * robotWidthPx);
                int sensorY = (int)(sensor.PositionY / 100.0 * robotHeightPx);

                // Draw FOV for this specific sensor
                DrawSingleSensorFOV(g, sensor, sensorX, sensorY);
            }
        }

        /// <summary>
        /// Draws field of view for a single sensor with color based on sensor type
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="sensor">The sensor to draw</param>
        /// <param name="robotWidthPx">Robot width in pixels</param>
        /// <param name="robotHeightPx">Robot height in pixels</param>
        private void DrawSingleSensorFOV(Graphics g, SimpleSensor sensor, int sensorX, int sensorY)
        {
            // Calculate range in pixels based on MaxRange (cm to pixels conversion)
            // Assuming 10cm per cell, and cell size in pixels
            double rangeCm = sensor.MaxRange;
            double rangePx = (rangeCm / 10.0) * _cellSize * _zoomLevel;
            rangePx = Math.Max(15, Math.Min(200, rangePx));  // Limit for visualization

            // Calculate FOV cone
            double halfFOVRad = (sensor.FieldOfView / 2.0) * Math.PI / 180.0;
            double directionRad = (sensor.MountAngle) * Math.PI / 180.0;

            // Calculate cone points
            PointF[] conePoints = new PointF[3];
            conePoints[0] = new PointF(sensorX, sensorY);  // Sensor position

            // Left edge of FOV
            conePoints[1] = new PointF(
                sensorX + (float)(rangePx * Math.Cos(directionRad - halfFOVRad)),
                sensorY + (float)(rangePx * Math.Sin(directionRad - halfFOVRad))
            );

            // Right edge of FOV
            conePoints[2] = new PointF(
                sensorX + (float)(rangePx * Math.Cos(directionRad + halfFOVRad)),
                sensorY + (float)(rangePx * Math.Sin(directionRad + halfFOVRad))
            );

            // Get colors based on sensor type
            Color fillColor = GetSensorFOVColor(sensor.SensorType, 60);
            Color borderColor = GetSensorFOVColor(sensor.SensorType, 180);

            // Draw FOV cone
            using (var fillBrush = new SolidBrush(fillColor))
            using (var borderPen = new Pen(borderColor, 1.5f))
            {
                g.FillPolygon(fillBrush, conePoints);
                g.DrawPolygon(borderPen, conePoints);
            }

            // Draw range arc for better visualization
            float startAngle = (float)((sensor.MountAngle - sensor.FieldOfView / 2) * 180 / Math.PI);
            float sweepAngle = (float)sensor.FieldOfView;

            using (var arcPen = new Pen(borderColor, 1))
            {
                g.DrawArc(arcPen,
                    sensorX - (float)rangePx, sensorY - (float)rangePx,
                    (float)rangePx * 2, (float)rangePx * 2,
                    startAngle, sweepAngle);
            }
        }

        /// <summary>
        /// Gets FOV color for a sensor type with specified alpha
        /// </summary>
        private Color GetSensorFOVColor(string sensorType, int alpha)
        {
            return sensorType switch
            {
                "Ultrasonic" => Color.FromArgb(alpha, 46, 204, 113),   // Green
                "Infrared" => Color.FromArgb(alpha, 231, 76, 60),      // Red
                "Lidar" => Color.FromArgb(alpha, 44, 62, 80),          // Dark Gray
                "Camera" => Color.FromArgb(alpha, 155, 89, 182),       // Purple
                "Proximity" => Color.FromArgb(alpha, 241, 196, 15),    // Yellow
                "GPS" => Color.FromArgb(alpha, 22, 160, 133),          // Turquoise
                "IMU" => Color.FromArgb(alpha, 52, 152, 219),          // Blue
                "Temperature" => Color.FromArgb(alpha, 230, 126, 34),  // Orange
                "Pressure" => Color.FromArgb(alpha, 52, 73, 94),       // Dark Blue
                "Humidity" => Color.FromArgb(alpha, 26, 188, 156),     // Teal
                _ => Color.FromArgb(alpha, 52, 152, 219)               // Default Blue
            };
        }

        /// <summary>
        /// Gets short name for sensor type for display on map
        /// </summary>
        private string GetSensorShortName(string sensorType)
        {
            return sensorType switch
            {
                "Ultrasonic" => "US",
                "Infrared" => "IR",
                "Lidar" => "LD",
                "Camera" => "CAM",
                "Proximity" => "PRX",
                "GPS" => "GPS",
                "IMU" => "IMU",
                "Temperature" => "TMP",
                "Pressure" => "PRS",
                "Humidity" => "HMD",
                _ => "SNR"
            };
        }

        /// <summary>
        /// Gets the display color for a sensor based on its type
        /// </summary>
        /// <param name="sensorType">Type of sensor (Ultrasonic, Infrared, etc.)</param>
        /// <returns>Color for the sensor's FOV visualization</returns>
        private Color GetSensorColor(string sensorType)
        {
            return sensorType switch
            {
                "Ultrasonic" => Color.FromArgb(60, 46, 204, 113),   // Green
                "Infrared" => Color.FromArgb(60, 231, 76, 60),      // Red
                "Lidar" => Color.FromArgb(60, 44, 62, 80),          // Dark Gray
                "Camera" => Color.FromArgb(60, 155, 89, 182),       // Purple
                "Proximity" => Color.FromArgb(60, 241, 196, 15),    // Yellow
                "GPS" => Color.FromArgb(60, 22, 160, 133),          // Turquoise
                "IMU" => Color.FromArgb(60, 52, 152, 219),          // Blue
                "Temperature" => Color.FromArgb(60, 230, 126, 34),  // Orange
                "Pressure" => Color.FromArgb(60, 52, 73, 94),       // Dark Blue
                "Humidity" => Color.FromArgb(60, 26, 188, 156),     // Teal
                _ => Color.FromArgb(60, 52, 152, 219)               // Default Blue
            };
        }

        /// <summary>
        /// Gets the border color for a sensor based on its type
        /// </summary>
        private Color GetSensorBorderColor(string sensorType)
        {
            return sensorType switch
            {
                "Ultrasonic" => Color.FromArgb(150, 46, 204, 113),
                "Infrared" => Color.FromArgb(150, 231, 76, 60),
                "Lidar" => Color.FromArgb(150, 44, 62, 80),
                "Camera" => Color.FromArgb(150, 155, 89, 182),
                "Proximity" => Color.FromArgb(150, 241, 196, 15),
                "GPS" => Color.FromArgb(150, 22, 160, 133),
                "IMU" => Color.FromArgb(150, 52, 152, 219),
                "Temperature" => Color.FromArgb(150, 230, 126, 34),
                "Pressure" => Color.FromArgb(150, 52, 73, 94),
                "Humidity" => Color.FromArgb(150, 26, 188, 156),
                _ => Color.FromArgb(150, 52, 152, 219)
            };
        }

        #endregion

        /// <summary>
        /// Draws a rectangular robot body centered at origin
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Body width in pixels</param>
        /// <param name="length">Body length in pixels</param>
        /// <param name="color">Body color</param>
        private void DrawRobotRectangle(Graphics g, int width, int length, Color color)
        {
            RectangleF bodyRect = new RectangleF(-width / 2, -length / 2, width, length);
            using (var bodyBrush = new SolidBrush(color))
                g.FillRectangle(bodyBrush, bodyRect);
            using (var borderPen = new Pen(Color.White, 1.5f))
                g.DrawRectangle(borderPen, bodyRect.X, bodyRect.Y, bodyRect.Width, bodyRect.Height);
        }
        /// <summary>
        /// Draws a square robot body centered at origin
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Body width in pixels</param>
        /// <param name="length">Body length in pixels</param>
        /// <param name="color">Body color</param>
        private void DrawRobotSquare(Graphics g, int width, int length, Color color)
        {
            int size = Math.Min(width, length);
            RectangleF bodyRect = new RectangleF(-size / 2, -size / 2, size, size);
            using (var bodyBrush = new SolidBrush(color))
                g.FillRectangle(bodyBrush, bodyRect);
            using (var borderPen = new Pen(Color.White, 1.5f))
                g.DrawRectangle(borderPen, bodyRect.X, bodyRect.Y, bodyRect.Width, bodyRect.Height);
        }

        /// <summary>
        /// Draws a rounded rectangle robot body centered at origin
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Body width in pixels</param>
        /// <param name="length">Body length in pixels</param>
        /// <param name="color">Body color</param>
        private void DrawRobotRoundedRect(Graphics g, int width, int length, Color color)
        {
            RectangleF bodyRect = new RectangleF(-width / 2, -length / 2, width, length);
            int radius = Math.Min(width, length) / 4;

            using (var bodyBrush = new SolidBrush(color))
            using (var path = new GraphicsPath())
            {
                path.AddArc(bodyRect.X, bodyRect.Y, radius * 2, radius * 2, 180, 90);
                path.AddArc(bodyRect.Right - radius * 2, bodyRect.Y, radius * 2, radius * 2, 270, 90);
                path.AddArc(bodyRect.Right - radius * 2, bodyRect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(bodyRect.X, bodyRect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();

                g.FillPath(bodyBrush, path);

                using (var borderPen = new Pen(Color.White, 1.5f))
                    g.DrawPath(borderPen, path);
            }
        }


        /// <summary>
        /// Draws a circular robot body centered at origin
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Body width in pixels</param>
        /// <param name="length">Body length in pixels</param>
        /// <param name="color">Body color</param>
        private void DrawRobotCircle(Graphics g, int width, int length, Color color)
        {
            int radius = Math.Min(width, length) / 2;
            using (var bodyBrush = new SolidBrush(color))
                g.FillEllipse(bodyBrush, -radius, -radius, radius * 2, radius * 2);
            using (var borderPen = new Pen(Color.White, 1.5f))
                g.DrawEllipse(borderPen, -radius, -radius, radius * 2, radius * 2);
        }

        /// <summary>
        /// Draws a triangular robot body centered at origin
        /// The triangle points upward (front of robot)
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Base width in pixels</param>
        /// <param name="length">Height from base to tip in pixels</param>
        /// <param name="color">Body color</param>
        private void DrawRobotTriangle(Graphics g, int width, int length, Color color)
        {
            PointF[] triangle = new PointF[]
            {
        new PointF(0, -length / 2),           // Tip (front)
        new PointF(-width / 2, length / 2),   // Bottom-left
        new PointF(width / 2, length / 2)     // Bottom-right
            };

            using (var bodyBrush = new SolidBrush(color))
                g.FillPolygon(bodyBrush, triangle);
            using (var borderPen = new Pen(Color.White, 1.5f))
                g.DrawPolygon(borderPen, triangle);
        }

        /// <summary>
        /// Draws a hexagonal robot body centered at origin
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="width">Body width in pixels</param>
        /// <param name="length">Body length in pixels</param>
        /// <param name="color">Body color</param>
        private void DrawRobotHexagon(Graphics g, int width, int length, Color color)
        {
            double angleStep = Math.PI * 2 / 6;
            PointF[] hexagon = new PointF[6];

            for (int i = 0; i < 6; i++)
            {
                double angle = i * angleStep - Math.PI / 2;
                float x = (float)(Math.Cos(angle) * width / 2);
                float y = (float)(Math.Sin(angle) * length / 2);
                hexagon[i] = new PointF(x, y);
            }

            using (var bodyBrush = new SolidBrush(color))
                g.FillPolygon(bodyBrush, hexagon);
            using (var borderPen = new Pen(Color.White, 1.5f))
                g.DrawPolygon(borderPen, hexagon);
        }
        /// <summary>
        /// Draws direction arrow showing robot's front direction
        /// The arrow is drawn at the front tip of the robot
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="robotLength">Robot length in pixels</param>
        private void DrawDirectionArrowOnRobot(Graphics g, int robotLength)
        {
            using (var arrowBrush = new SolidBrush(Color.FromArgb(46, 204, 113)))
            {
                PointF[] arrowPoints = new PointF[]
                {
            new PointF(0, -robotLength / 2 - 3),      // Arrow tip (front)
            new PointF(-5, -robotLength / 2 + 8),     // Arrow left base
            new PointF(5, -robotLength / 2 + 8)       // Arrow right base
                };
                g.FillPolygon(arrowBrush, arrowPoints);
            }
        }

        /// <summary>
        /// Draws sensor points on the robot body
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="robotWidth">Robot width in pixels</param>
        /// <param name="robotLength">Robot length in pixels</param>
        private void DrawRobotSensors(Graphics g, int robotWidth, int robotLength)
        {
            if (_selectedRobot?.Sensors == null) return;

            foreach (var sensor in _selectedRobot.Sensors)
            {
                if (!sensor.IsEnabled) continue;

                Color sensorColor;
                try { sensorColor = ColorTranslator.FromHtml(sensor.DisplayColor); }
                catch { sensorColor = Color.FromArgb(52, 152, 219); }

                // ✅ Convert position from -50..50 to -width/2 .. width/2
                int sensorX = (int)(sensor.PositionX / 100.0 * robotWidth);
                int sensorY = (int)(sensor.PositionY / 100.0 * robotLength);

                using (var sensorBrush = new SolidBrush(sensorColor))
                using (var sensorPen = new Pen(Color.White, 1))
                {
                    g.FillEllipse(sensorBrush, sensorX - 4, sensorY - 4, 8, 8);
                    g.DrawEllipse(sensorPen, sensorX - 4, sensorY - 4, 8, 8);
                }
            }
        }

        /// <summary>
        /// Draws the robot using legacy method for backward compatibility
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawLegacyRobot(Graphics g)
        {
            Rectangle robotRect = GetCellRect(_robotPosition.X, _robotPosition.Y);
            if (robotRect.Width <= 0 || robotRect.Height <= 0) return;

            var state = g.Save();

            int centerX = robotRect.X + robotRect.Width / 2;
            int centerY = robotRect.Y + robotRect.Height / 2;
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(_robotAngle);

            int cellSizePx = robotRect.Width;
            double cellSizeCm = _scaleCmPerCell;
            if (cellSizeCm <= 0) cellSizeCm = 10.0;

            double widthRatio = Math.Max(0.3, Math.Min(0.9, _robotWidthCm / cellSizeCm));
            double lengthRatio = Math.Max(0.3, Math.Min(0.9, _robotLengthCm / cellSizeCm));

            int robotWidthPx = (int)(widthRatio * cellSizePx);
            int robotLengthPx = (int)(lengthRatio * cellSizePx);

            robotWidthPx = Math.Min(robotWidthPx, cellSizePx - 2);
            robotLengthPx = Math.Min(robotLengthPx, cellSizePx - 2);

            if (robotWidthPx < 4) robotWidthPx = 4;
            if (robotLengthPx < 4) robotLengthPx = 4;

            // Body
            RectangleF bodyRect = new RectangleF(-robotWidthPx / 2, -robotLengthPx / 2, robotWidthPx, robotLengthPx);

            using (var bodyBrush = new SolidBrush(Color.FromArgb(220, 41, 128, 185)))
            {
                g.FillRectangle(bodyBrush, bodyRect);
            }

            using (var borderPen = new Pen(Color.White, 1.5f))
            {
                g.DrawRectangle(borderPen, bodyRect.X, bodyRect.Y, bodyRect.Width, bodyRect.Height);
            }

            // Head triangle
            int headSize = Math.Max(4, Math.Min(robotWidthPx, robotLengthPx) / 3);

            PointF[] head = new PointF[]
            {
                new PointF(robotWidthPx / 2 + headSize / 3, 0),
                new PointF(robotWidthPx / 2 - headSize / 2, -headSize / 2),
                new PointF(robotWidthPx / 2 - headSize / 2, headSize / 2)
            };

            using (var headBrush = new SolidBrush(Color.FromArgb(220, 231, 76, 60)))
            {
                g.FillPolygon(headBrush, head);
            }

            // Wheels
            if (cellSizePx >= 20)
            {
                int wheelRadius = Math.Max(2, Math.Min(robotWidthPx, robotLengthPx) / 5);

                using (var wheelBrush = new SolidBrush(Color.FromArgb(220, 60, 60, 60)))
                using (var wheelPen = new Pen(Color.FromArgb(200, 150, 150, 150), 1))
                {
                    float topWheelY = -robotLengthPx / 2 - wheelRadius + 2;
                    float bottomWheelY = robotLengthPx / 2 - wheelRadius - 2;

                    g.FillEllipse(wheelBrush, -wheelRadius, topWheelY, wheelRadius * 2, wheelRadius * 2);
                    g.DrawEllipse(wheelPen, -wheelRadius, topWheelY, wheelRadius * 2, wheelRadius * 2);

                    g.FillEllipse(wheelBrush, -wheelRadius, bottomWheelY, wheelRadius * 2, wheelRadius * 2);
                    g.DrawEllipse(wheelPen, -wheelRadius, bottomWheelY, wheelRadius * 2, wheelRadius * 2);
                }
            }

            // Eye
            int eyeSize = Math.Max(2, headSize / 5);
            using (var eyeBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(eyeBrush, robotWidthPx / 2 - headSize / 4, -eyeSize / 2, eyeSize, eyeSize);
            }

            g.Restore(state);
        }
        #endregion

        #region Public Properties - Core
        public bool ShowOrderNumbers
        {
            get => _showOrderNumbers;
            set { _showOrderNumbers = value; Invalidate(); }
        }

        public double ScaleCmPerCell
        {
            get => _scaleCmPerCell;
            set
            {
                _scaleCmPerCell = Math.Max(1.0, value);
                Invalidate();
            }
        }

        public MapGrid MapGrid
        {
            get => _mapGrid;
            set { _mapGrid = value; Invalidate(); }
        }

        public List<GoalPoint> Goals
        {
            get => _goals;
            set { _goals = value ?? new List<GoalPoint>(); Invalidate(); }
        }

        public List<ParkingPoint> ParkingPoints
        {
            get => _parkingPoints;
            set { _parkingPoints = value ?? new List<ParkingPoint>(); Invalidate(); }
        }

        public int CellSize
        {
            get => _cellSize;
            set { _cellSize = Math.Max(MIN_CELL_SIZE, value); Invalidate(); }
        }

        public float ZoomLevel
        {
            get => _zoomLevel;
            set { _zoomLevel = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, value)); Invalidate(); }
        }

        public DrawMode CurrentDrawMode
        {
            get => _currentDrawMode;
            set => _currentDrawMode = value;
        }

        public byte CurrentWeight
        {
            get => _currentWeight;
            set => _currentWeight = value;
        }

        public MapElementType CurrentElement
        {
            get => _currentElement;
            set => _currentElement = value;
        }
        public bool ShowGrid
        {
            get => _showGrid;
            set { _showGrid = value; Invalidate(); }
        }
        public bool ShowCoordinates
        {
            get => _showCoordinates;
            set { _showCoordinates = value; Invalidate(); }
        }

        /// <summary>
        /// Gets or sets whether to show the obstacle legend on the map
        /// </summary>
        public bool ShowLegend
        {
            get => _showLegend;
            set
            {
                _showLegend = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the legend position on the map
        /// </summary>
        public Point LegendPosition
        {
            get => _legendPosition;
            set
            {
                _legendPosition = value;
                Invalidate();
            }
        }
        #endregion


        #region Public Properties - Robot
        public Point RobotPosition
        {
            get => _robotPosition;
            set { _robotPosition = value; Invalidate(); }
        }

        public float RobotAngle
        {
            get => _robotAngle;
            set { _robotAngle = value; Invalidate(); }
        }

        public bool ShowRobot
        {
            get => _showRobot;
            set { _showRobot = value; Invalidate(); }
        }

        public double RobotSpeed
        {
            get => _robotSpeed;
            set
            {
                _robotSpeed = value;
                Invalidate();
            }
        }
        public bool ShowSensorFOV
        {
            get => _showSensorFOV;
            set
            {
                _showSensorFOV = value;
                Invalidate();
            }
        }
        #endregion

        #region Public Properties - Obstacles
        public List<DynamicObstacle> DynamicObstacles
        {
            get => _dynamicObstacles;
            set { _dynamicObstacles = value ?? new List<DynamicObstacle>(); Invalidate(); }
        }

        public ObstacleType CurrentObstacleType
        {
            get => _currentObstacleType;
            set => _currentObstacleType = value;
        }
        #endregion

        #region Public Properties - Detection Zone
        //public bool ShowDetectionZone
        //{
        //    get => _showDetectionZone;
        //    set { _showDetectionZone = value; Invalidate(); }
        //}

        //public Color DetectionZoneColor
        //{
        //    get => _detectionZoneColor;
        //    set { _detectionZoneColor = value; Invalidate(); }
        //}

        public bool ShowSensorLabels
        {
            get => _showSensorLabels;
            set
            {
                _showSensorLabels = value;
                Invalidate();
            }
        }
        #endregion

        #region Public Properties - Special Cells
        public Point StartPoint
        {
            get => _startPoint;
            set { _startPoint = value; Invalidate(); }
        }

        public HashSet<Point> CollisionCells
        {
            get => _collisionCells;
            set { _collisionCells = value ?? new HashSet<Point>(); Invalidate(); }
        }

        public HashSet<Point> ScannedCells
        {
            get => _scannedCells;
            set { _scannedCells = value ?? new HashSet<Point>(); Invalidate(); }
        }

        public HashSet<Point> InvalidPathCells
        {
            get => _invalidPathCells;
            set { _invalidPathCells = value ?? new HashSet<Point>(); Invalidate(); }
        }

        /// <summary>
        /// Updates the list of active obstacles from simulation
        /// </summary>
        public void UpdateActiveObstacles(List<DetectedObstacle> obstacles)
        {
            _activeObstacles = obstacles ?? new List<DetectedObstacle>();
            Invalidate();
        }

        /// <summary>
        /// Updates hotspot risk levels for display
        /// </summary>
        public void UpdateHotspotRiskLevels(Dictionary<Point, double> riskLevels)
        {
            _hotspotRiskLevels = riskLevels ?? new Dictionary<Point, double>();
            Invalidate();
        }
        #endregion

        #region Public Properties - Start Point
        public Point RobotStartPoint
        {
            get => _robotStartPoint;
            set
            {
                _robotStartPoint = value;
                SetCurrentStartPoint(value);
                Invalidate();
            }
        }

        public bool HasCustomStartPoint => _robotStartPoint != Point.Empty;
        #endregion

        #region Public Properties - View
        public PointF ViewOffset => _viewOffset;
        #endregion

        #region Public Methods - Start Points
        public void SetCurrentStartPoint(Point location)
        {
            ResetStartPoints();
            AddStartPoint(location);
            _robotStartPoint = location;
            this.RobotPosition = location;
            System.Diagnostics.Debug.WriteLine($"[MapControl] Start point set to ({location.X},{location.Y})");
        }

        public void ResetStartPoints()
        {
            _startPoints?.Clear();
            _nextStartIndex = 0;
            _robotStartPoint = Point.Empty;
        }

        public void AddStartPoint(Point location)
        {
            if (_startPoints == null)
                _startPoints = new Dictionary<(int, int), int>();

            if (!_startPoints.ContainsKey((location.X, location.Y)))
            {
                _startPoints[(location.X, location.Y)] = _nextStartIndex;
                _nextStartIndex++;
                Invalidate();
            }
        }
        #endregion

        #region Public Methods - Path Drawing
        public void ClearPaths()
        {
            _coloredPaths.Clear();
            Invalidate();
        }

        public void DrawPath(List<PathNode> path, Color color)
        {
            _coloredPaths.Clear();
            if (path != null && path.Count > 0)
            {
                _coloredPaths.Add(new ColoredPath(path, color, false));
            }
            Invalidate();
        }

        public void DrawColoredPaths(List<ColoredPath> paths)
        {
            _coloredPaths = paths ?? new List<ColoredPath>();
            Invalidate();
        }
        #endregion

        #region Public Methods - Goals and Parking
        public void AddGoalAt(Point cell, Color color)
        {
            System.Diagnostics.Debug.WriteLine($"AddGoalAt: cell=({cell.X},{cell.Y}), current goals count={_goals.Count}");
            if (_mapGrid == null || !_mapGrid.IsValidCoordinate(cell.X, cell.Y))
            {
                System.Diagnostics.Debug.WriteLine("AddGoalAt: Invalid cell or null grid");
                return;
            }
            int newNumber = _goals.Count + 1;
            var newGoal = new GoalPoint(newNumber, cell, color);
            _goals.Add(newGoal);
            System.Diagnostics.Debug.WriteLine($"AddGoalAt: Goal added, new count={_goals.Count}");

            _mapGrid[cell.X, cell.Y].ElementType = MapElementType.GoalPoint;
            _mapGrid.UpdateCellProperties(cell.X, cell.Y);
            Invalidate();
        }

        public void AddParkingAt(Point cell)
        {
            if (_mapGrid == null || !_mapGrid.IsValidCoordinate(cell.X, cell.Y)) return;

            int newNumber = _parkingPoints.Count + 1;
            _parkingPoints.Add(new ParkingPoint(newNumber, cell));
            _mapGrid[cell.X, cell.Y].ElementType = MapElementType.ParkingPoint;
            _mapGrid.UpdateCellProperties(cell.X, cell.Y);
            Invalidate();
        }

        public void RemoveGoalAt(Point cell)
        {
            var goal = _goals.FirstOrDefault(g => g.Location.X == cell.X && g.Location.Y == cell.Y);
            if (goal != null)
            {
                _goals.Remove(goal);
                if (_mapGrid != null)
                {
                    _mapGrid[cell.X, cell.Y].ElementType = MapElementType.Empty;
                    _mapGrid.UpdateCellProperties(cell.X, cell.Y);
                }
                Invalidate();
            }
        }

        public void RemoveParkingAt(Point cell)
        {
            var parking = _parkingPoints.FirstOrDefault(p => p.Location.X == cell.X && p.Location.Y == cell.Y);
            if (parking != null)
            {
                _parkingPoints.Remove(parking);
                if (_mapGrid != null)
                {
                    _mapGrid[cell.X, cell.Y].ElementType = MapElementType.Empty;
                    _mapGrid.UpdateCellProperties(cell.X, cell.Y);
                }
                Invalidate();
            }
        }

        public void ClearGoals()
        {
            foreach (var goal in _goals)
            {
                if (_mapGrid != null)
                {
                    _mapGrid[goal.Location.X, goal.Location.Y].ElementType = MapElementType.Empty;
                    _mapGrid.UpdateCellProperties(goal.Location.X, goal.Location.Y);
                }
            }
            _goals.Clear();
            Invalidate();
        }

        public void ClearParkingPoints()
        {
            foreach (var parking in _parkingPoints)
            {
                if (_mapGrid != null)
                {
                    _mapGrid[parking.Location.X, parking.Location.Y].ElementType = MapElementType.Empty;
                    _mapGrid.UpdateCellProperties(parking.Location.X, parking.Location.Y);
                }
            }
            _parkingPoints.Clear();
            Invalidate();
        }
        #endregion

        #region Public Methods - Robot
        public void UpdateRobot(double speed, Point position, float angle)
        {
            _robotSpeed = speed;
            _robotPosition = position;
            _robotAngle = angle;
            System.Diagnostics.Debug.WriteLine($"[MapControl] UpdateRobot: Speed={_robotSpeed} cm/s, Position=({position.X},{position.Y})");
            Invalidate();
        }

        public void MoveRobotManually(Keys key, double currentSpeed = 10.0)
        {
            if (_mapGrid == null || !_mapGrid.IsValidCoordinate(_robotPosition.X, _robotPosition.Y)) return;

            Point newPosition = _robotPosition;
            float newAngle = _robotAngle;
            int stepSize = 1;
            float tankTurnAngle = 22.5f;
            float pivotTurnAngle = 45f;
            double moveDistance = currentSpeed / 50.0;
            moveDistance = Math.Max(0.2, Math.Min(1.5, moveDistance));

            switch (key)
            {
                case Keys.W:
                    newPosition = new Point(
                        _robotPosition.X + (int)(stepSize * Math.Cos(_robotAngle * Math.PI / 180)),
                        _robotPosition.Y + (int)(stepSize * Math.Sin(_robotAngle * Math.PI / 180)));
                    System.Diagnostics.Debug.WriteLine($"[Robot] Moving FORWARD at angle {_robotAngle}°");
                    break;

                case Keys.S:
                    newPosition = new Point(
                        _robotPosition.X - (int)(stepSize * Math.Cos(_robotAngle * Math.PI / 180)),
                        _robotPosition.Y - (int)(stepSize * Math.Sin(_robotAngle * Math.PI / 180)));
                    System.Diagnostics.Debug.WriteLine($"[Robot] Moving BACKWARD at angle {_robotAngle}°");
                    break;

                case Keys.A:
                    newAngle = _robotAngle - tankTurnAngle;
                    newPosition = new Point(
                        _robotPosition.X + (int)(0.5 * Math.Cos((_robotAngle - 15) * Math.PI / 180)),
                        _robotPosition.Y + (int)(0.5 * Math.Sin((_robotAngle - 15) * Math.PI / 180)));
                    System.Diagnostics.Debug.WriteLine($"[Robot] TANK TURN LEFT: angle {_robotAngle}° -> {newAngle}°");
                    break;

                case Keys.D:
                    newAngle = _robotAngle + tankTurnAngle;
                    newPosition = new Point(
                        _robotPosition.X + (int)(0.5 * Math.Cos((_robotAngle + 15) * Math.PI / 180)),
                        _robotPosition.Y + (int)(0.5 * Math.Sin((_robotAngle + 15) * Math.PI / 180)));
                    System.Diagnostics.Debug.WriteLine($"[Robot] TANK TURN RIGHT: angle {_robotAngle}° -> {newAngle}°");
                    break;

                case Keys.Q:
                    newAngle = _robotAngle - pivotTurnAngle;
                    newPosition = _robotPosition;
                    System.Diagnostics.Debug.WriteLine($"[Robot] PIVOT LEFT: angle {_robotAngle}° -> {newAngle}°");
                    break;

                case Keys.E:
                    newAngle = _robotAngle + pivotTurnAngle;
                    newPosition = _robotPosition;
                    System.Diagnostics.Debug.WriteLine($"[Robot] PIVOT RIGHT: angle {_robotAngle}° -> {newAngle}°");
                    break;

                case Keys.R:
                    newPosition = new Point(
                        _robotPosition.X + (int)(moveDistance * Math.Cos((_robotAngle + 90) * Math.PI / 180)),
                        _robotPosition.Y + (int)(moveDistance * Math.Sin((_robotAngle + 90) * Math.PI / 180)));
                    System.Diagnostics.Debug.WriteLine($"[Robot] STRAFE RIGHT");
                    break;

                case Keys.F:
                    newPosition = new Point(
                        _robotPosition.X + (int)(moveDistance * Math.Cos((_robotAngle - 90) * Math.PI / 180)),
                        _robotPosition.Y + (int)(moveDistance * Math.Sin((_robotAngle - 90) * Math.PI / 180)));
                    System.Diagnostics.Debug.WriteLine($"[Robot] STRAFE LEFT");
                    break;
            }

            if (_mapGrid.IsValidCoordinate(newPosition.X, newPosition.Y))
            {
                var cell = _mapGrid[newPosition.X, newPosition.Y];
                if (cell.IsWalkable && cell.OccupyingObstacle == null)
                {
                    _robotPosition = newPosition;
                    _robotAngle = newAngle;

                    if (_robotAngle < 0) _robotAngle += 360;
                    if (_robotAngle >= 360) _robotAngle -= 360;

                    OnRobotManuallyMoved?.Invoke(_robotPosition, _robotAngle, currentSpeed);
                    Invalidate();
                }
            }
        }

        public void RotateRobot(float deltaAngle)
        {
            _robotAngle += deltaAngle;
            if (_robotAngle < 0) _robotAngle += 360;
            if (_robotAngle >= 360) _robotAngle -= 360;
            System.Diagnostics.Debug.WriteLine($"[RotateRobot] New Angle={_robotAngle}");
            OnRobotManuallyMoved?.Invoke(_robotPosition, _robotAngle, _robotSpeed);
            Invalidate();
        }

        public void SetRobotDimensions(int widthCm, int lengthCm, int heightCm)
        {
            _robotWidthCm = Math.Max(10, Math.Min(200, widthCm));
            _robotLengthCm = Math.Max(10, Math.Min(200, lengthCm));
            _robotHeightCm = Math.Max(5, Math.Min(150, heightCm));
            Invalidate();
        }
        #endregion

        #region Public Methods - Detection Zone
        //public void UpdateDetectionZone(List<Point> cells)
        //{
        //    _detectionZoneCells = cells ?? new List<Point>();
        //    Invalidate();
        //}

        //public void ClearDetectionZone()
        //{
        //    _detectionZoneCells.Clear();
        //    Invalidate();
        //}

        //public void SetDetectionRange(int range)
        //{
        //    _detectionRangeCells = Math.Max(1, Math.Min(10, range));
        //    Invalidate();
        //}
        #endregion

        #region Public Methods - Coordinate Conversion
        public Point GetGridCellAtPoint(Point clientPoint)
        {
            float scaledCellSize = _cellSize * _zoomLevel;
            if (scaledCellSize <= 0) return new Point(-1, -1);

            int x = (int)((clientPoint.X - _viewOffset.X) / scaledCellSize);
            int y = (int)((clientPoint.Y - _viewOffset.Y) / scaledCellSize);
            return new Point(x, y);
        }

        private Rectangle GetCellRect(int x, int y)
        {
            float scaledCellSize = Math.Max(1, _cellSize * _zoomLevel);
            return new Rectangle(
                (int)(x * scaledCellSize + _viewOffset.X),
                (int)(y * scaledCellSize + _viewOffset.Y),
                (int)scaledCellSize,
                (int)scaledCellSize);
        }
        #endregion

        #region Public Methods - Recording
        public void StartRecording(GifRecorder recorder)
        {
            _gifRecorder = recorder;
            _isRecording = true;
        }

        public void StopRecording()
        {
            _isRecording = false;
            _gifRecorder = null;
        }
        #endregion

        #region Public Methods - Search Visualization
        public void ClearSearchCells()
        {
            _openCells?.Clear();
            _closedCells?.Clear();
            _currentCell = null;
            _pathCells?.Clear();
            _cellParents?.Clear();
            _currentOrder = 0;
            Invalidate();
        }

        public void UpdateSearchCell(int fromX, int fromY, int x, int y, PathFinderNodeType type, int totalCost, int cost)
        {
            var cell = new Point(x, y);
            var parentCell = new Point(fromX, fromY);
            var rect = GetCellRect(cell.X, cell.Y);
            rect.Inflate(2, 2);

            switch (type)
            {
                case PathFinderNodeType.Open:
                    _openCells.Add(cell);
                    _cellParents[cell] = parentCell;
                    break;
                case PathFinderNodeType.Close:
                    _openCells.Remove(cell);
                    _closedCells.Add(cell);
                    if (!_cellParents.ContainsKey(cell))
                        _cellParents[cell] = parentCell;
                    break;
                case PathFinderNodeType.Current:
                    if (_currentCell.HasValue)
                    {
                        var oldRect = GetCellRect(_currentCell.Value.X, _currentCell.Value.Y);
                        oldRect.Inflate(2, 2);
                        Invalidate(oldRect);
                    }
                    _currentCell = cell;
                    if (!_cellParents.ContainsKey(cell))
                        _cellParents[cell] = parentCell;
                    break;
                case PathFinderNodeType.Path:
                    _pathCells.Add(cell);
                    if (!_cellParents.ContainsKey(cell))
                        _cellParents[cell] = parentCell;
                    break;
            }

            _arrowDrawCounter++;
            Invalidate(rect);

            if (_isRecording && _gifRecorder != null)
            {
                _gifRecorder.CaptureFrame();
            }
        }
        #endregion

        #region Events
        public event EventHandler ViewChanged;
        public event Action<Point, float, double> OnRobotManuallyMoved;
        public event Action<RobotDefinition> OnRobotSelected;
        public event Action<RobotDefinition> OnRobotAdded;
        public event Action<RobotDefinition> OnRobotDeleted;
        #endregion

        #region Protected Methods - Paint
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_mapGrid == null)
            {
                base.OnPaint(e);
                return;
            }

            Graphics g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            float scaledCellSize = _cellSize * _zoomLevel;
            int visibleWidth = this.ClientSize.Width;
            int visibleHeight = this.ClientSize.Height;

            int startX = Math.Max(0, (int)(-_viewOffset.X / scaledCellSize));
            int startY = Math.Max(0, (int)(-_viewOffset.Y / scaledCellSize));
            int endX = Math.Min(_mapGrid.Width - 1, (int)((visibleWidth - _viewOffset.X) / scaledCellSize) + 2);
            int endY = Math.Min(_mapGrid.Height - 1, (int)((visibleHeight - _viewOffset.Y) / scaledCellSize) + 2);

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    Cell cell = _mapGrid[x, y];
                    Rectangle rect = GetCellRect(x, y);

                    if (rect.Width <= 0 || rect.Height <= 0) continue;

                    DrawCellBackground(g, cell, rect);
                    DrawElement(g, cell, rect);
                    DrawGoalOrParking(g, x, y, rect);
                    DrawPaths(g, x, y, rect);
                    DrawDynamicObstacles(g, x, y, rect);
                   // DrawDetectionZone(g, x, y, rect);
                    DrawGridLines(g, rect);
                    DrawSpecialCells(g, x, y, rect);
                    DrawPathArrows(g);

                    if (_showCoordinates)
                    {
                        DrawCoordinates(g, rect, x, y);
                    }
                }
            }

            DrawPathLines(g);
            DrawRobot(g);
            DrawSearchCells(g);
            //Draw legend on top of everything
            DrawLegend(g);
            //Draw wait countdown
            DrawWaitCountdown(g);
            base.OnPaint(e);
        }

        private void DrawCoordinates(Graphics g, Rectangle rect, int x, int y)
        {
            if (!_showCoordinates) return;

            string text = $"({x},{y})";
            using (var font = new Font("Consolas", Math.Max(6, rect.Height / 4), FontStyle.Regular))
            using (var brush = new SolidBrush(Color.FromArgb(150, 50, 50, 50)))
            {
                SizeF textSize = g.MeasureString(text, font);
                float textX = rect.X + (rect.Width - textSize.Width) / 2;
                float textY = rect.Y + (rect.Height - textSize.Height) / 2;
                g.DrawString(text, font, brush, textX, textY);
            }
        }

        private void DrawCellBackground(Graphics g, Cell cell, Rectangle rect)
        {
            int intensity = 255 - (int)((cell.SurfaceWeight / 100.0) * 255);
            intensity = Math.Max(0, Math.Min(255, intensity));

            using var brush = new SolidBrush(Color.FromArgb(255, intensity, intensity, intensity));
            g.FillRectangle(brush, rect);
        }

        private void DrawElement(Graphics g, Cell cell, Rectangle rect)
        {
            switch (cell.ElementType)
            {
                case MapElementType.Wall:
                    using (var brush = new SolidBrush(Color.FromArgb(60, 50, 50)))
                    {
                        g.FillRectangle(brush, rect);
                    }
                    break;
                case MapElementType.Window:
                    using (var brush = new SolidBrush(Color.FromArgb(100, 100, 200)))
                    {
                        g.FillRectangle(brush, rect);
                    }
                    using (var pen = new Pen(Color.FromArgb(150, 150, 255), 1))
                    {
                        g.DrawLine(pen, rect.X, rect.Y, rect.Right, rect.Bottom);
                        g.DrawLine(pen, rect.Right, rect.Y, rect.X, rect.Bottom);
                    }
                    break;

                case MapElementType.Ramp:
                    using (var brush = new LinearGradientBrush(rect, Color.LightGray, Color.DarkGray, LinearGradientMode.ForwardDiagonal))
                    {
                        Point[] pts = { new Point(rect.X, rect.Bottom), new Point(rect.Right, rect.Bottom), new Point(rect.X, rect.Y) };
                        g.FillPolygon(brush, pts);
                    }
                    break;

                case MapElementType.StartPoint:
                    using (var brush = new SolidBrush(Color.FromArgb(150, 46, 204, 113)))
                    {
                        g.FillRectangle(brush, rect);
                    }
                    break;

                case MapElementType.Door:
                    Color doorColor = cell.IsDoorOpen
                        ? Color.FromArgb(150, 160, 100)
                        : Color.FromArgb(180, 139, 69, 19);

                    using (var brush = new SolidBrush(doorColor))
                    {
                        g.FillRectangle(brush, rect);
                    }

                    using (var pen = new Pen(cell.IsDoorOpen ? Color.DarkGreen : Color.DarkRed, 2))
                    {
                        int midX = rect.X + rect.Width / 2;
                        g.DrawLine(pen, midX, rect.Y, midX, rect.Y + rect.Height);
                    }

                    if (!cell.IsDoorOpen)
                    {
                        using (var font = new Font("Segoe UI", rect.Height / 3, FontStyle.Bold))
                        using (var textBrush = new SolidBrush(Color.White))
                        {
                            string text = "🔒";
                            SizeF textSize = g.MeasureString(text, font);
                            float textX = rect.X + (rect.Width - textSize.Width) / 2;
                            float textY = rect.Y + (rect.Height - textSize.Height) / 2;
                            g.DrawString(text, font, textBrush, textX, textY);
                        }
                    }
                    break;
            }
        }

        private void DrawGoalOrParking(Graphics g, int x, int y, Rectangle rect)
        {
            var cell = _mapGrid[x, y];

            if (cell.ElementType == MapElementType.GoalPoint)
            {
                var goal = _goals.FirstOrDefault(gp => gp.Location.X == x && gp.Location.Y == y);
                if (goal != null)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(180, goal.Color)))
                    {
                        g.FillRectangle(brush, rect);
                    }

                    using (var pen = new Pen(goal.Color, 2))
                    {
                        g.DrawRectangle(pen, rect);
                    }

                    string text = $"G{goal.Number}";
                    using (var font = new Font("Segoe UI", Math.Max(8, rect.Height / 3), FontStyle.Bold))
                    {
                        SizeF textSize = g.MeasureString(text, font);
                        PointF textPoint = new PointF(rect.X + (rect.Width - textSize.Width) / 2, rect.Y + (rect.Height - textSize.Height) / 2);

                        using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                        {
                            g.DrawString(text, font, shadowBrush, textPoint.X + 1, textPoint.Y + 1);
                        }

                        using (var textBrush = new SolidBrush(Color.White))
                        {
                            g.DrawString(text, font, textBrush, textPoint);
                        }
                    }
                }
            }
            else if (cell.ElementType == MapElementType.ParkingPoint)
            {
                var parking = _parkingPoints.FirstOrDefault(p => p.Location.X == x && p.Location.Y == y);
                if (parking != null)
                {
                    using (var brush = new SolidBrush(Color.FromArgb(180, 46, 204, 113)))
                    {
                        g.FillRectangle(brush, rect);
                    }

                    using (var pen = new Pen(Color.FromArgb(46, 204, 113), 2))
                    {
                        g.DrawRectangle(pen, rect);
                    }

                    string text = $"P{parking.Number}";
                    using (var font = new Font("Segoe UI", Math.Max(8, rect.Height / 3), FontStyle.Bold))
                    {
                        SizeF textSize = g.MeasureString(text, font);
                        PointF textPoint = new PointF(rect.X + (rect.Width - textSize.Width) / 2, rect.Y + (rect.Height - textSize.Height) / 2);

                        using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                        {
                            g.DrawString(text, font, shadowBrush, textPoint.X + 1, textPoint.Y + 1);
                        }

                        using (var textBrush = new SolidBrush(Color.White))
                        {
                            g.DrawString(text, font, textBrush, textPoint);
                        }
                    }
                }
            }
        }

        private void DrawPaths(Graphics g, int x, int y, Rectangle rect)
        {
            if (_coloredPaths == null) return;

            foreach (var coloredPath in _coloredPaths)
            {
                if (coloredPath == null || coloredPath.Nodes == null) continue;

                if (coloredPath.Type == PathType.Return || coloredPath.Type == PathType.Charging)
                {
                    continue;
                }

                foreach (var node in coloredPath.Nodes)
                {
                    if (node.X == x && node.Y == y)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(120, coloredPath.Color)))
                        {
                            g.FillRectangle(brush, rect);
                        }
                        return;
                    }
                }
            }
        }

        private void DrawPathLines(Graphics g)
        {
            if (_coloredPaths == null) return;

            foreach (var coloredPath in _coloredPaths)
            {
                if (coloredPath == null || coloredPath.Nodes == null || coloredPath.Nodes.Count < 2) continue;

                using (var pen = new Pen(coloredPath.Color, coloredPath.Thickness))
                {
                    if (coloredPath.IsDashed)
                    {
                        pen.DashStyle = DashStyle.Dash;
                    }

                    for (int i = 1; i < coloredPath.Nodes.Count; i++)
                    {
                        var from = coloredPath.Nodes[i - 1];
                        var to = coloredPath.Nodes[i];
                        var rectFrom = GetCellRect(from.X, from.Y);
                        var rectTo = GetCellRect(to.X, to.Y);
                        var fromCenter = new Point(rectFrom.X + rectFrom.Width / 2, rectFrom.Y + rectFrom.Height / 2);
                        var toCenter = new Point(rectTo.X + rectTo.Width / 2, rectTo.Y + rectTo.Height / 2);
                        g.DrawLine(pen, fromCenter, toCenter);
                    }
                }
            }
        }

        private void DrawDynamicObstacles(Graphics g, int x, int y, Rectangle rect)
        {
            foreach (var obs in _dynamicObstacles)
            {
                if (obs.Location.X == x && obs.Location.Y == y)
                {
                    DrawDynamicObstacleIcon(g, obs, rect);
                }
            }
        }

        private void DrawDynamicObstacleIcon(Graphics g, DynamicObstacle obs, Rectangle rect)
        {
            Color backColor;
            string icon;

            switch (obs.Type)
            {
                case ObstacleType.Adult:
                    backColor = Color.FromArgb(200, 231, 76, 60);
                    icon = "👤";
                    break;
                case ObstacleType.Child:
                    backColor = Color.FromArgb(200, 241, 196, 15);
                    icon = "🧒";
                    break;
                case ObstacleType.Animal:
                    backColor = Color.FromArgb(200, 139, 69, 19);
                    icon = "🐕";
                    break;
                case ObstacleType.OtherRobot:
                    backColor = Color.FromArgb(200, 52, 152, 219);
                    icon = "🤖";
                    break;
                case ObstacleType.Equipment:
                    backColor = Color.FromArgb(200, 127, 140, 141);
                    icon = "🔧";
                    break;
                default:
                    backColor = Color.FromArgb(200, 192, 57, 43);
                    icon = "⚠️";
                    break;
            }

            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, rect);
            }

            using (var font = new Font("Segoe UI Emoji", rect.Height / 2, FontStyle.Regular))
            {
                SizeF iconSize = g.MeasureString(icon, font);
                float ix = rect.X + (rect.Width - iconSize.Width) / 2;
                float iy = rect.Y + (rect.Height - iconSize.Height) / 2;

                using (var textBrush = new SolidBrush(Color.White))
                {
                    g.DrawString(icon, font, textBrush, ix, iy);
                }
            }

            using (var pen = new Pen(Color.FromArgb(100, 0, 0, 0), 1))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        //private void DrawDetectionZone(Graphics g, int x, int y, Rectangle rect)
        //{
        //    if (_showDetectionZone && _detectionZoneCells != null)
        //    {
        //        foreach (var cellPos in _detectionZoneCells)
        //        {
        //            if (cellPos.X == x && cellPos.Y == y)
        //            {
        //                using (var brush = new SolidBrush(_detectionZoneColor))
        //                {
        //                    g.FillRectangle(brush, rect);
        //                }
        //                break;
        //            }
        //        }
        //    }
        //}

        private void DrawGridLines(Graphics g, Rectangle rect)
        {
            if (!_showGrid) return;

            using (var pen = new Pen(Color.FromArgb(120, 120, 120), 1))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawSpecialCells(Graphics g, int x, int y, Rectangle rect)
        {
            Point cell = new Point(x, y);

            // Draw start points
            if (_startPoints != null && _startPoints.ContainsKey((x, y)))
            {
                int startIndex = _startPoints[(x, y)];
                string startText = $"S{startIndex}";

                using (var brush = new SolidBrush(Color.FromArgb(180, 46, 204, 113)))
                {
                    g.FillRectangle(brush, rect);
                }
                using (var pen = new Pen(Color.FromArgb(46, 204, 113), 2))
                {
                    g.DrawRectangle(pen, rect);
                }
                DrawCellText(g, rect, startText, Color.Yellow);
                return;
            }

            // Draw collision cells
            if (_collisionCells != null && _collisionCells.Contains(cell))
            {
                using (var brush = new SolidBrush(Color.FromArgb(200, 231, 76, 60)))
                {
                    g.FillRectangle(brush, rect);
                }
                DrawCellText(g, rect, "C", Color.White);
                return;
            }

            // Draw invalid path cells
            if (_invalidPathCells != null && _invalidPathCells.Contains(cell))
            {
                using (var brush = new SolidBrush(Color.FromArgb(200, 139, 0, 0)))
                {
                    g.FillRectangle(brush, rect);
                }
                DrawCellText(g, rect, "X", Color.Yellow);
                return;
            }

            // Draw scanned cells
            if (_scannedCells != null && _scannedCells.Contains(cell))
            {
                using (var brush = new SolidBrush(Color.FromArgb(200, 241, 196, 15)))
                {
                    g.FillRectangle(brush, rect);
                }
                DrawCellText(g, rect, "PC", Color.Black);
                return;
            }

            // ========== NEW: Draw detected obstacles ==========
            if (_activeObstacles != null)
            {
                var obstacle = _activeObstacles.FirstOrDefault(o => o.Location.X == x && o.Location.Y == y);
                if (obstacle != null)
                {
                    DrawDetectedObstacle(g, x, y, rect, obstacle);
                    return;
                }
            }

            // ========== NEW: Draw hotspot indicators ==========
            if (_hotspotRiskLevels != null && _hotspotRiskLevels.TryGetValue(cell, out double riskLevel))
            {
                DrawHotspotIndicator(g, rect, riskLevel);
            }
        }

        private void DrawCellText(Graphics g, Rectangle rect, string text, Color textColor)
        {
            using (var font = new Font("Segoe UI", Math.Max(8, rect.Height / 3), FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(text, font);
                PointF textPoint = new PointF(
                    rect.X + (rect.Width - textSize.Width) / 2,
                    rect.Y + (rect.Height - textSize.Height) / 2);

                using (var shadowBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 0)))
                {
                    g.DrawString(text, font, shadowBrush, textPoint.X + 1, textPoint.Y + 1);
                }

                using (var brush = new SolidBrush(textColor))
                {
                    g.DrawString(text, font, brush, textPoint);
                }
            }
        }

        private void DrawSearchCells(Graphics g)
        {
            if (_mapGrid == null) return;

            float scaledCellSize = _cellSize * _zoomLevel;

            foreach (var cell in _openCells)
            {
                if (!_mapGrid.IsValidCoordinate(cell.X, cell.Y)) continue;
                if (IsImportantCell(cell)) continue;

                var rect = GetCellRect(cell.X, cell.Y);

                using (var brush = new SolidBrush(Color.FromArgb(200, 100, 255, 100)))
                {
                    g.FillRectangle(brush, rect);
                }

                if (_cellParents.ContainsKey(cell))
                {
                    DrawDirectionArrow(g, rect, _cellParents[cell], cell);
                }
            }

            foreach (var cell in _closedCells)
            {
                if (!_mapGrid.IsValidCoordinate(cell.X, cell.Y)) continue;
                if (IsImportantCell(cell)) continue;

                var rect = GetCellRect(cell.X, cell.Y);

                using (var brush = new SolidBrush(Color.FromArgb(200, 255, 100, 100)))
                {
                    g.FillRectangle(brush, rect);
                }

                if (_cellParents.ContainsKey(cell))
                {
                    DrawDirectionArrow(g, rect, _cellParents[cell], cell);
                }
            }

            if (_currentCell.HasValue && _mapGrid.IsValidCoordinate(_currentCell.Value.X, _currentCell.Value.Y))
            {
                var rect = GetCellRect(_currentCell.Value.X, _currentCell.Value.Y);

                if (IsImportantCell(_currentCell.Value))
                {
                    using (var brush = new SolidBrush(Color.FromArgb(150, 100, 100, 255)))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }
                else
                {
                    using (var brush = new SolidBrush(Color.FromArgb(220, 100, 100, 255)))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }

                if (_cellParents.ContainsKey(_currentCell.Value))
                {
                    DrawDirectionArrow(g, rect, _cellParents[_currentCell.Value], _currentCell.Value);
                }
            }

            foreach (var cell in _pathCells)
            {
                if (!_mapGrid.IsValidCoordinate(cell.X, cell.Y)) continue;
                if (IsImportantCell(cell)) continue;

                var rect = GetCellRect(cell.X, cell.Y);

                using (var brush = new SolidBrush(Color.FromArgb(200, 255, 215, 0)))
                {
                    g.FillRectangle(brush, rect);
                }
            }
        }

        private bool IsImportantCell(Point cell)
        {
            return IsGoalCell(cell) || IsParkingCell(cell) || IsStartPointCell(cell);
        }

        private bool IsStartPointCell(Point cell)
        {
            return _startPoint.X == cell.X && _startPoint.Y == cell.Y;
        }

        private bool IsParkingCell(Point cell)
        {
            if (_parkingPoints == null) return false;
            return _parkingPoints.Any(p => p.Location.X == cell.X && p.Location.Y == cell.Y);
        }

        private bool IsGoalCell(Point cell)
        {
            if (_goals == null) return false;
            return _goals.Any(g => g.Location.X == cell.X && g.Location.Y == cell.Y);
        }

        private void DrawDirectionArrow(Graphics g, Rectangle rect, Point from, Point to)
        {
            if (from.X == to.X && from.Y == to.Y) return;

            int centerX = rect.X + rect.Width / 2;
            int centerY = rect.Y + rect.Height / 2;

            int dx = to.X - from.X;
            int dy = to.Y - from.Y;

            if (dx > 0) dx = 1;
            if (dx < 0) dx = -1;
            if (dy > 0) dy = 1;
            if (dy < 0) dy = -1;

            int arrowSize = Math.Min(rect.Width, rect.Height) / 6;
            int arrowHeadSize = arrowSize / 2;

            if (arrowSize < 3) arrowSize = 3;
            if (arrowHeadSize < 2) arrowHeadSize = 2;

            Point start = new Point(centerX, centerY);
            Point end = new Point(centerX + dx * arrowSize, centerY + dy * arrowSize);

            using (var pen = new Pen(Color.Black, 1.5f))
            {
                g.DrawLine(pen, start, end);

                if (dx == 1)
                {
                    g.DrawLine(pen, end, new Point(end.X - arrowHeadSize, end.Y - arrowHeadSize));
                    g.DrawLine(pen, end, new Point(end.X - arrowHeadSize, end.Y + arrowHeadSize));
                }
                else if (dx == -1)
                {
                    g.DrawLine(pen, end, new Point(end.X + arrowHeadSize, end.Y - arrowHeadSize));
                    g.DrawLine(pen, end, new Point(end.X + arrowHeadSize, end.Y + arrowHeadSize));
                }
                else if (dy == 1)
                {
                    g.DrawLine(pen, end, new Point(end.X - arrowHeadSize, end.Y - arrowHeadSize));
                    g.DrawLine(pen, end, new Point(end.X + arrowHeadSize, end.Y - arrowHeadSize));
                }
                else if (dy == -1)
                {
                    g.DrawLine(pen, end, new Point(end.X - arrowHeadSize, end.Y + arrowHeadSize));
                    g.DrawLine(pen, end, new Point(end.X + arrowHeadSize, end.Y + arrowHeadSize));
                }
            }
        }

        private void DrawPathArrows(Graphics g)
        {
            if (_pathCells == null || _pathCells.Count < 2) return;

            for (int i = 0; i < _pathCells.Count - 1; i++)
            {
                var current = _pathCells[i];
                var next = _pathCells[i + 1];

                if (!_mapGrid.IsValidCoordinate(current.X, current.Y)) continue;
                if (!_mapGrid.IsValidCoordinate(next.X, next.Y)) continue;

                var rect = GetCellRect(current.X, current.Y);
                DrawPathArrow(g, rect, current, next);
            }
        }

        private void DrawPathArrow(Graphics g, Rectangle rect, Point current, Point next)
        {
            if (current.X == next.X && current.Y == next.Y) return;

            int centerX = rect.X + rect.Width / 2;
            int centerY = rect.Y + rect.Height / 2;

            int dx = next.X - current.X;
            int dy = next.Y - current.Y;

            int arrowSize = Math.Min(rect.Width, rect.Height) / 3;
            int arrowHeadSize = arrowSize / 2;

            Point start = new Point(centerX, centerY);
            Point end = new Point(centerX + dx * arrowSize, centerY + dy * arrowSize);

            using (var pen = new Pen(Color.DarkOrange, 2))
            {
                g.DrawLine(pen, start, end);

                if (dx == 1)
                {
                    g.DrawLine(pen, end, new Point(end.X - arrowHeadSize, end.Y - arrowHeadSize));
                    g.DrawLine(pen, end, new Point(end.X - arrowHeadSize, end.Y + arrowHeadSize));
                }
                else if (dx == -1)
                {
                    g.DrawLine(pen, end, new Point(end.X + arrowHeadSize, end.Y - arrowHeadSize));
                    g.DrawLine(pen, end, new Point(end.X + arrowHeadSize, end.Y + arrowHeadSize));
                }
                else if (dy == 1)
                {
                    g.DrawLine(pen, end, new Point(end.X - arrowHeadSize, end.Y - arrowHeadSize));
                    g.DrawLine(pen, end, new Point(end.X + arrowHeadSize, end.Y - arrowHeadSize));
                }
                else if (dy == -1)
                {
                    g.DrawLine(pen, end, new Point(end.X - arrowHeadSize, end.Y + arrowHeadSize));
                    g.DrawLine(pen, end, new Point(end.X + arrowHeadSize, end.Y + arrowHeadSize));
                }
            }
        }
        #endregion

        #region Protected Methods - Mouse Events
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Check if clicking on legend
            Rectangle legendRect = new Rectangle(_legendPosition.X, _legendPosition.Y, 180, 220);
            if (_showLegend && legendRect.Contains(e.Location))
            {
                StartDragLegend(e.Location);
                return;
            }

            if (e.Button == MouseButtons.Middle)
            {
                _isPanning = true;
                _lastMousePosition = e.Location;
                this.Cursor = Cursors.SizeAll;
            }
            else
            {
                ProcessDrawing(e);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_isDraggingLegend)
            {
                StopDragLegend();
                return;
            }
            if (e.Button == MouseButtons.Middle)
            {
                _isPanning = false;
                this.Cursor = Cursors.Default;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDraggingLegend)
            {
                DragLegend(e.Location);
                return;
            }
            if (_isPanning)
            {
                int dx = e.X - _lastMousePosition.X;
                int dy = e.Y - _lastMousePosition.Y;
                _viewOffset = new PointF(_viewOffset.X + dx, _viewOffset.Y + dy);
                _lastMousePosition = e.Location;
                Invalidate();
                ViewChanged?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                ProcessDrawing(e);
            }

            base.OnMouseMove(e);
        }

        private void ProcessDrawing(MouseEventArgs e)
        {
            if (_mapGrid == null) return;

            Point cell = GetGridCellAtPoint(e.Location);
            if (!_mapGrid.IsValidCoordinate(cell.X, cell.Y)) return;

            float scaledCellSize = _cellSize * _zoomLevel;
            if (scaledCellSize <= 0) return;

            Rectangle rect = new Rectangle(
                (int)(cell.X * scaledCellSize + _viewOffset.X),
                (int)(cell.Y * scaledCellSize + _viewOffset.Y),
                (int)scaledCellSize,
                (int)scaledCellSize);

            if (_currentDrawMode == DrawMode.SetWeight)
            {
                _mapGrid[cell.X, cell.Y].SurfaceWeight = _currentWeight;
                _mapGrid.UpdateCellProperties(cell.X, cell.Y);
                Invalidate(rect);
            }
            else if (_currentDrawMode == DrawMode.SetElement)
            {
                _mapGrid[cell.X, cell.Y].ElementType = _currentElement;
                _mapGrid.UpdateCellProperties(cell.X, cell.Y);
                Invalidate(rect);
            }
            else if (_currentDrawMode == DrawMode.Erase || e.Button == MouseButtons.Right)
            {
                if (_goals.Any(g => g.Location.X == cell.X && g.Location.Y == cell.Y))
                    RemoveGoalAt(cell);
                else if (_parkingPoints.Any(p => p.Location.X == cell.X && p.Location.Y == cell.Y))
                    RemoveParkingAt(cell);
                else
                {
                    _mapGrid[cell.X, cell.Y].ElementType = MapElementType.Empty;
                    _mapGrid[cell.X, cell.Y].SurfaceWeight = 1;
                    _mapGrid[cell.X, cell.Y].IsDoorOpen = true;
                    _mapGrid[cell.X, cell.Y].RampDifficulty = 0;
                    _mapGrid.UpdateCellProperties(cell.X, cell.Y);
                    Invalidate(rect);
                }
            }
            else if (_currentDrawMode == DrawMode.SetDynamicObstacle)
            {
                if (!_dynamicObstacles.Any(o => o.Location.X == cell.X && o.Location.Y == cell.Y))
                {
                    var obs = new DynamicObstacle(_currentObstacleType, cell);
                    _dynamicObstacles.Add(obs);
                    _mapGrid[cell.X, cell.Y].OccupyingObstacle = obs;
                    _mapGrid[cell.X, cell.Y].IsWalkable = false;
                    _mapGrid.UpdateCellProperties(cell.X, cell.Y);
                    Invalidate(rect);
                }
            }
            else if (_currentDrawMode == DrawMode.SetParkingPoint)
            {
                if (!_parkingPoints.Any(p => p.Location.X == cell.X && p.Location.Y == cell.Y))
                {
                    AddParkingAt(cell);
                }
            }
        }

        private void MapControl_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta > 0 ? ZOOM_STEP : -ZOOM_STEP;
            float oldZoom = _zoomLevel;
            ZoomLevel += delta;

            if (Math.Abs(oldZoom - _zoomLevel) > 0.01f)
            {
                Point mousePos = e.Location;
                _viewOffset = new PointF(
                    _viewOffset.X - (mousePos.X - _viewOffset.X) * (delta / oldZoom),
                    _viewOffset.Y - (mousePos.Y - _viewOffset.Y) * (delta / oldZoom));
                Invalidate();
                ViewChanged?.Invoke(this, EventArgs.Empty);
            }
        } 
  
        #endregion

        #region Public Properties - Robot Definition
        /// <summary>
        /// Gets or sets the current robot definition for drawing
        /// </summary>
        public RobotDefinition CurrentRobot
        {
            get => _selectedRobot;
            set
            {
                _selectedRobot = value;
                if (_selectedRobot != null)
                {
                    // Update legacy properties for backward compatibility
                    UpdateLegacyPropertiesFromRobot(_selectedRobot);
                }
                Invalidate();
            }
        }
        #endregion

        #region Obstacle Drawing Helpers

        /// <summary>
        /// Gets the display color for an obstacle based on its type and priority
        /// </summary>
        /// <param name="obstacleType">Type of obstacle</param>
        /// <returns>Color for the obstacle</returns>
        private Color GetObstacleColor(ObstacleType obstacleType)
        {
            return obstacleType switch
            {
                ObstacleType.Child => Color.FromArgb(220, 231, 76, 60),      // Red
                ObstacleType.Adult => Color.FromArgb(220, 230, 126, 34),     // Orange
                ObstacleType.Animal => Color.FromArgb(220, 241, 196, 15),    // Yellow
                ObstacleType.OtherRobot => Color.FromArgb(220, 52, 152, 219), // Blue
                ObstacleType.Equipment => Color.FromArgb(220, 127, 140, 141), // Gray
                _ => Color.FromArgb(220, 149, 165, 166)                      // Light Gray
            };
        }

        /// <summary>
        /// Gets the icon for an obstacle type
        /// </summary>
        /// <param name="obstacleType">Type of obstacle</param>
        /// <returns>Icon character</returns>
        private string GetObstacleIcon(ObstacleType obstacleType)
        {
            return obstacleType switch
            {
                ObstacleType.Child => "🧒",
                ObstacleType.Adult => "👤",
                ObstacleType.Animal => "🐕",
                ObstacleType.OtherRobot => "🤖",
                ObstacleType.Equipment => "🔧",
                _ => "⚠️"
            };
        }

        /// <summary>
        /// Gets the priority level text for an obstacle
        /// </summary>
        /// <param name="obstacleType">Type of obstacle</param>
        /// <returns>Priority text</returns>
        private string GetPriorityText(ObstacleType obstacleType)
        {
            return obstacleType switch
            {
                ObstacleType.Child => "HIGH",
                ObstacleType.Adult => "MEDIUM-HIGH",
                ObstacleType.Animal => "MEDIUM",
                ObstacleType.OtherRobot => "MEDIUM-LOW",
                ObstacleType.Equipment => "LOW",
                _ => "UNKNOWN"
            };
        }

        /// <summary>
        /// Gets the border color for hotspot cells
        /// </summary>
        private Color GetHotspotBorderColor(double riskLevel)
        {
            if (riskLevel >= 80)
                return Color.FromArgb(255, 231, 76, 60);  // Red - high risk
            if (riskLevel >= 60)
                return Color.FromArgb(255, 230, 126, 34); // Orange - medium risk
            if (riskLevel >= 40)
                return Color.FromArgb(255, 241, 196, 15); // Yellow - low risk
            return Color.FromArgb(255, 52, 152, 219);     // Blue - very low risk
        }

        /// <summary>
        /// Draws detected obstacles on the map with priority-based colors
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="x">Cell X coordinate</param>
        /// <param name="y">Cell Y coordinate</param>
        /// <param name="rect">Cell rectangle</param>
        /// <param name="obstacle">The detected obstacle</param>
        private void DrawDetectedObstacle(Graphics g, int x, int y, Rectangle rect, DetectedObstacle obstacle)
        {
            if (obstacle == null) return;

            Color backColor = GetObstacleColor(obstacle.ObstacleType);

            // Fill cell with obstacle color
            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, rect);
            }

            // Draw priority indicator bar on the left side
            int barWidth = Math.Max(3, rect.Width / 8);
            using (var priorityBrush = new SolidBrush(GetPriorityBarColor(obstacle.ObstacleType)))
            {
                g.FillRectangle(priorityBrush, rect.X, rect.Y, barWidth, rect.Height);
            }

            // Draw obstacle icon
            string icon = GetObstacleIcon(obstacle.ObstacleType);
            using (var font = new Font("Segoe UI Emoji", rect.Height / 2, FontStyle.Regular))
            using (var textBrush = new SolidBrush(Color.White))
            {
                SizeF iconSize = g.MeasureString(icon, font);
                float ix = rect.X + (rect.Width - iconSize.Width) / 2;
                float iy = rect.Y + (rect.Height - iconSize.Height) / 2;
                g.DrawString(icon, font, textBrush, ix, iy);
            }

            // Draw confidence indicator (small circle)
            int confidenceRadius = Math.Max(2, rect.Width / 10);
            int confidenceX = rect.Right - confidenceRadius - 2;
            int confidenceY = rect.Y + 2;

            Color confidenceColor = GetConfidenceColor(obstacle.Confidence);
            using (var confidenceBrush = new SolidBrush(confidenceColor))
            {
                g.FillEllipse(confidenceBrush, confidenceX - confidenceRadius, confidenceY, confidenceRadius * 2, confidenceRadius * 2);
            }

            // Draw border
            using (var borderPen = new Pen(Color.White, 1))
            {
                g.DrawRectangle(borderPen, rect);
            }
        }

        /// <summary>
        /// Gets the priority bar color for an obstacle type
        /// </summary>
        private Color GetPriorityBarColor(ObstacleType obstacleType)
        {
            return obstacleType switch
            {
                ObstacleType.Child => Color.FromArgb(255, 200, 0, 0),      // Dark red
                ObstacleType.Adult => Color.FromArgb(255, 200, 100, 0),    // Dark orange
                ObstacleType.Animal => Color.FromArgb(255, 200, 150, 0),   // Dark yellow
                ObstacleType.OtherRobot => Color.FromArgb(255, 0, 100, 200), // Dark blue
                ObstacleType.Equipment => Color.FromArgb(255, 80, 80, 80),  // Dark gray
                _ => Color.FromArgb(255, 100, 100, 100)
            };
        }

        /// <summary>
        /// Gets confidence indicator color
        /// </summary>
        private Color GetConfidenceColor(double confidence)
        {
            if (confidence >= 0.8)
                return Color.FromArgb(46, 204, 113);  // Green - high confidence
            if (confidence >= 0.5)
                return Color.FromArgb(241, 196, 15);   // Yellow - medium confidence
            return Color.FromArgb(231, 76, 60);        // Red - low confidence
        }

        /// <summary>
        /// Draws hotspot indicators for frequently dangerous cells
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="x">Cell X coordinate</param>
        /// <param name="y">Cell Y coordinate</param>
        /// <param name="rect">Cell rectangle</param>
        /// <param name="riskLevel">Risk level (0-100)</param>
        private void DrawHotspotIndicator(Graphics g, Rectangle rect, double riskLevel)
        {
            if (riskLevel <= 0) return;

            // Draw pulsing border effect
            Color borderColor = GetHotspotBorderColor(riskLevel);
            int borderWidth = riskLevel >= 80 ? 3 : (riskLevel >= 60 ? 2 : 1);

            using (var borderPen = new Pen(borderColor, borderWidth))
            {
                g.DrawRectangle(borderPen, rect);
            }

            // Draw diagonal warning lines for high risk
            if (riskLevel >= 70)
            {
                using (var linePen = new Pen(Color.FromArgb(100, 231, 76, 60), 1))
                {
                    g.DrawLine(linePen, rect.X, rect.Y, rect.Right, rect.Bottom);
                    g.DrawLine(linePen, rect.Right, rect.Y, rect.X, rect.Bottom);
                }
            }

            // Draw risk level text for zoomed-in view
            if (rect.Width >= 40 && riskLevel >= 50)
            {
                string riskText = $"{riskLevel:F0}%";
                using (var font = new Font("Segoe UI", 7, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                using (var backBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                {
                    SizeF textSize = g.MeasureString(riskText, font);
                    float textX = rect.Right - textSize.Width - 2;
                    float textY = rect.Y + 2;
                    g.FillRectangle(backBrush, textX - 1, textY - 1, textSize.Width + 2, textSize.Height + 2);
                    g.DrawString(riskText, font, textBrush, textX, textY);
                }
            }
        }
        #endregion

        #region Legend Drawing

        /// <summary>
        /// Draws the obstacle legend on the map
        /// </summary>
        /// <param name="g">Graphics object</param>
        private void DrawLegend(Graphics g)
        {
            if (!_showLegend) return;

            // Calculate legend size
            int legendWidth = 180;
            int legendHeight = 200;
            int padding = 8;
            int itemHeight = 28;

            // Legend background
            Rectangle legendRect = new Rectangle(_legendPosition.X, _legendPosition.Y, legendWidth, legendHeight);
       
            using (var backBrush = new SolidBrush(Color.FromArgb(240, 30, 35, 45)))
            using (var borderPen = new Pen(Color.FromArgb(100, 255, 255, 255), 1))
            {
                g.FillRectangle(backBrush, legendRect);
                g.DrawRectangle(borderPen, legendRect);
            }

            int y = _legendPosition.Y + padding;
            int x = _legendPosition.X + padding;

            // Title
            using (var titleFont = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var titleBrush = new SolidBrush(Color.White))
            {
                g.DrawString("⚠️ OBSTACLE LEGEND", titleFont, titleBrush, x, y);
            }
            y += 25;

            // Separator line
            using (var linePen = new Pen(Color.FromArgb(100, 255, 255, 255), 1))
            {
                g.DrawLine(linePen, x, y, x + legendWidth - padding * 2, y);
            }
            y += 10;

            // Draw each obstacle type
            var obstacleTypes = new (ObstacleType type, string name, string icon, Color color)[]
            {
        (ObstacleType.Child, "Child", "🧒", Color.FromArgb(231, 76, 60)),
        (ObstacleType.Adult, "Adult", "👤", Color.FromArgb(230, 126, 34)),
        (ObstacleType.Animal, "Animal", "🐕", Color.FromArgb(241, 196, 15)),
        (ObstacleType.OtherRobot, "Other Robot", "🤖", Color.FromArgb(52, 152, 219)),
        (ObstacleType.Equipment, "Equipment", "🔧", Color.FromArgb(127, 140, 141))
            };

            foreach (var item in obstacleTypes)
            {
                // Color box
                using (var colorBrush = new SolidBrush(item.color))
                {
                    g.FillRectangle(colorBrush, x, y, 20, 16);
                }

                // Icon
                using (var iconFont = new Font("Segoe UI Emoji", 10))
                using (var iconBrush = new SolidBrush(Color.White))
                {
                    g.DrawString(item.icon, iconFont, iconBrush, x + 25, y - 2);
                }

                // Text
                using (var textFont = new Font("Segoe UI", 9))
                using (var textBrush = new SolidBrush(Color.FromArgb(255, 255, 255, 255)))  
                {
                    g.DrawString(item.name, textFont, textBrush, x + 45, y);
                }
                 // Priority indicator
                string priority = GetPriorityText(item.type);
                using (var priorityFont = new Font("Segoe UI", 7, FontStyle.Italic))
                using (var priorityBrush = new SolidBrush(Color.FromArgb(150, 255, 255, 255)))
                {
                    SizeF prioritySize = g.MeasureString(priority, priorityFont);
                    g.DrawString(priority, priorityFont, priorityBrush, x + legendWidth - padding * 2 - prioritySize.Width, y + 2);
                }

                y += itemHeight;
            }

            // Separator
            using (var linePen = new Pen(Color.FromArgb(100, 255, 255, 255), 1))
            {
                g.DrawLine(linePen, x, y, x + legendWidth - padding * 2, y);
            }
            y += 10;

        

            // إحصائيات حية
            using (var statsFont = new Font("Segoe UI", 8))
            using (var statsBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
            {
                int totalObstacles = _activeObstacles?.Count ?? 0;
                g.DrawString($"📊 Active: {totalObstacles}", statsFont, statsBrush, x, y);
                y += 18;

                if (_activeObstacles != null)
                {
                    var nearest = _activeObstacles.OrderBy(o => o.DistanceCm).FirstOrDefault();
                    if (nearest != null)
                    {
                        g.DrawString($"🎯 Nearest: {nearest.DistanceCm:F0}cm", statsFont, statsBrush, x, y);
                        y += 18;
                    }
                }
            }
            y += 10;
            // Hotspot indicator
            using (var hotspotBrush = new SolidBrush(Color.FromArgb(60, 231, 76, 60)))
            using (var hotspotPen = new Pen(Color.FromArgb(231, 76, 60), 2))
            {
                g.FillRectangle(hotspotBrush, x, y, 20, 16);
                g.DrawRectangle(hotspotPen, x, y, 20, 16);
            }

            using (var textFont = new Font("Segoe UI", 9))
            using (var textBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                g.DrawString("Hotspot (High Risk)", textFont, textBrush, x + 25, y);
            }
            y += 25;

            // Confidence indicator
            using (var greenBrush = new SolidBrush(Color.FromArgb(46, 204, 113)))
            using (var yellowBrush = new SolidBrush(Color.FromArgb(241, 196, 15)))
            using (var redBrush = new SolidBrush(Color.FromArgb(231, 76, 60)))
            {
                g.FillEllipse(greenBrush, x, y, 12, 12);
                g.FillEllipse(yellowBrush, x + 20, y, 12, 12);
                g.FillEllipse(redBrush, x + 40, y, 12, 12);
            }

            using (var textFont = new Font("Segoe UI", 8))
            using (var textBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                g.DrawString("High", textFont, textBrush, x + 58, y);
                g.DrawString("Medium", textFont, textBrush, x + 100, y);
                g.DrawString("Low", textFont, textBrush, x + 158, y);
            }
            y += 20;

            // Wait time indicator
            using (var waitBrush = new SolidBrush(Color.FromArgb(241, 196, 15)))
            {
                g.FillRectangle(waitBrush, x, y, 20, 16);
            }
            using (var textFont = new Font("Segoe UI", 9))
            using (var textBrush = new SolidBrush(Color.FromArgb(200, 255, 255, 255)))
            {
                g.DrawString("Robot Waiting", textFont, textBrush, x + 25, y);
            }
        }

        #endregion

        #region Wait Countdown Display

        /// <summary>
        /// Starts the wait countdown display
        /// </summary>
        /// <param name="seconds">Total wait time in seconds</param>
        /// <param name="location">Location where robot is waiting</param>
        /// <param name="obstacleType">Type of obstacle causing the wait</param>
        public void StartWaitCountdown(double seconds, Point location, ObstacleType obstacleType)
        {
            _isWaiting = true;
            _remainingWaitSeconds = seconds;
            _waitLocation = location;
            _waitingForObstacle = obstacleType;

            if (_countdownTimer == null)
            {
                _countdownTimer = new System.Windows.Forms.Timer();
                _countdownTimer.Interval = 100;  // Update every 100ms for smooth display
                _countdownTimer.Tick += OnCountdownTick;
            }
            _countdownTimer.Start();

            Invalidate();
        }

        /// <summary>
        /// Stops the wait countdown display
        /// </summary>
        public void StopWaitCountdown()
        {
            _isWaiting = false;
            _countdownTimer?.Stop();
            Invalidate();
        }

        /// <summary>
        /// Updates the remaining wait time
        /// </summary>
        public void UpdateWaitTime(double remainingSeconds)
        {
            _remainingWaitSeconds = remainingSeconds;
            Invalidate();
        }

        private void OnCountdownTick(object sender, EventArgs e)
        {
            if (_isWaiting)
            {
                _remainingWaitSeconds -= 0.1;
                if (_remainingWaitSeconds <= 0)
                {
                    StopWaitCountdown();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Draws the wait countdown indicator on the map
        /// </summary>
        private void DrawWaitCountdown(Graphics g)
        {
            if (!_isWaiting) return;

            // Calculate screen position for the wait indicator
            Rectangle robotRect = GetCellRect(_robotPosition.X, _robotPosition.Y);
            if (robotRect.Width <= 0) return;

            int indicatorX = robotRect.X + robotRect.Width / 2;
            int indicatorY = robotRect.Y - 25;

            // Draw background circle
            int radius = 30;
            using (var backBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            using (var borderPen = new Pen(Color.FromArgb(241, 196, 15), 2))
            {
                g.FillEllipse(backBrush, indicatorX - radius, indicatorY - radius, radius * 2, radius * 2);
                g.DrawEllipse(borderPen, indicatorX - radius, indicatorY - radius, radius * 2, radius * 2);
            }

            // Draw countdown text
            string waitText = $"{_remainingWaitSeconds:F1}s";
            string obstacleIcon = GetObstacleIcon(_waitingForObstacle);
            string fullText = $"{obstacleIcon} {waitText}";

            using (var font = new Font("Segoe UI", 12, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.White))
            {
                SizeF textSize = g.MeasureString(fullText, font);
                float textX = indicatorX - textSize.Width / 2;
                float textY = indicatorY - textSize.Height / 2;
                g.DrawString(fullText, font, textBrush, textX, textY);
            }

            // Draw progress arc (circular progress bar)
            float progress = (float)(_remainingWaitSeconds / 5.0);  // Assuming max 5 seconds
            progress = Math.Min(1, Math.Max(0, progress));

            using (var progressPen = new Pen(Color.FromArgb(46, 204, 113), 3))
            {
                float startAngle = -90;
                float sweepAngle = 360 * progress;
                g.DrawArc(progressPen, indicatorX - radius + 5, indicatorY - radius + 5,
                          (radius - 5) * 2, (radius - 5) * 2, startAngle, sweepAngle);
            }
        }

        #endregion

        #region Legend Dragging

        /// <summary>
        /// Starts dragging the legend
        /// </summary>
        public void StartDragLegend(Point mousePosition)
        {
            _isDraggingLegend = true;
            _legendDragStart = mousePosition;
        }

        /// <summary>
        /// Updates legend position while dragging
        /// </summary>
        public void DragLegend(Point mousePosition)
        {
            if (!_isDraggingLegend) return;
            int dx = mousePosition.X - _legendDragStart.X;
            int dy = mousePosition.Y - _legendDragStart.Y;
            _legendPosition = new Point(_legendPosition.X + dx, _legendPosition.Y + dy);
            _legendDragStart = mousePosition;
            Invalidate();
        }

        /// <summary>
        /// Stops dragging the legend
        /// </summary>
        public void StopDragLegend()
        {
            _isDraggingLegend = false;
        }

        #endregion
    }
}