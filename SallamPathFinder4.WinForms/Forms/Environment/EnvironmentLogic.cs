#region File Header
/// <summary>
/// File: EnvironmentLogic.cs
/// Description: Core business logic for main environment form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Windows.Forms;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Environment
{
    public sealed class EnvironmentLogic
    {
        #region Constants - Layout
        public const int DETECTION_ZONE_INTERVAL_MS = 100;
        public const int DEFAULT_GRID_WIDTH = 100;
        public const int DEFAULT_GRID_HEIGHT = 100;
        public const int RIGHT_PANEL_WIDTH = 340;
        public const int RIGHT_PANEL_COLLAPSED_WIDTH = 40;
        public const int RULER_SIZE = 30;
        public const int DEFAULT_CELL_SIZE = 30;
        public const int CELL_SIZE_MIN = 10;
        public const int CELL_SIZE_MAX = 100;
        #endregion

        #region Constants - Zoom
        public const float ZOOM_MIN = 0.5f;
        public const float ZOOM_MAX = 3.0f;
        public const float ZOOM_STEP = 0.1f;
        public const float ZOOM_DEFAULT = 1.0f;
        #endregion

        #region Constants - Colors
        public static readonly Color COLOR_PRIMARY = Color.FromArgb(52, 73, 94);
        public static readonly Color COLOR_SECONDARY = Color.FromArgb(41, 128, 185);
        public static readonly Color COLOR_SUCCESS = Color.FromArgb(46, 204, 113);
        public static readonly Color COLOR_WARNING = Color.FromArgb(241, 196, 15);
        public static readonly Color COLOR_DANGER = Color.FromArgb(231, 76, 60);
        public static readonly Color COLOR_INFO = Color.FromArgb(52, 152, 219);
        public static readonly Color COLOR_DARK = Color.FromArgb(44, 62, 80);
        public static readonly Color COLOR_LIGHT = Color.FromArgb(236, 240, 241);
        public static readonly Color COLOR_PANEL_BACK = Color.FromArgb(248, 249, 250);
        #endregion

        #region Constants - Robot Defaults
        public const string DEFAULT_ROBOT_NAME = "SallamBot";
        public const double DEFAULT_ROBOT_WIDTH_CM = 60;
        public const double DEFAULT_ROBOT_LENGTH_CM = 60;
        public const double DEFAULT_ROBOT_HEIGHT_CM = 30;
        public const double DEFAULT_ROBOT_SPEED_CM_S = 10;
        public const double DEFAULT_ROBOT_BATTERY_PERCENT = 100;
        public const double DEFAULT_ROBOT_CONSUMPTION_RATE = 1.0;
        public const double DEFAULT_VIEW_ANGLE_DEGREES = 180;
        public const int DEFAULT_DETECTION_RANGE_CELLS = 2;
        #endregion

        #region Private Fields
        private readonly Random _random;
        #endregion

        #region Constructor
        public EnvironmentLogic()
        {
            _random = new Random();
        }
        #endregion

        #region Public Methods - Colors
        public Color GetRandomGoalColor()
        {
            return Color.FromArgb(
                _random.Next(100, 255),
                _random.Next(100, 255),
                _random.Next(100, 255));
        }

        public Color GetAlgorithmColor(string algorithm)
        {
            return algorithm switch
            {
                "AStar" => COLOR_INFO,
                "SPPA" => COLOR_SUCCESS,
                "SPPA_DL" => Color.FromArgb(155, 89, 182),
                "ACO" => COLOR_WARNING,
                "DStar" => Color.FromArgb(230, 126, 34),
                "KNN" => Color.FromArgb(26, 188, 156),
                "BruteForce" => COLOR_DANGER,
                _ => COLOR_PRIMARY
            };
        }

        public Color GetBatteryColor(double percentage)
        {
            if (percentage <= 10) return COLOR_DANGER;
            if (percentage <= 20) return COLOR_WARNING;
            return COLOR_SUCCESS;
        }
        #endregion

        #region Public Methods - Validation
        public bool IsValidGridCell(MapGrid grid, Point cell)
        {
            return grid != null && grid.IsValidCoordinate(cell.X, cell.Y);
        }

        public bool IsValidCellSize(int cellSize)
        {
            return cellSize >= CELL_SIZE_MIN && cellSize <= CELL_SIZE_MAX;
        }

        public bool IsValidZoomLevel(float zoom)
        {
            return zoom >= ZOOM_MIN && zoom <= ZOOM_MAX;
        }

        public float ClampZoom(float zoom)
        {
            return Math.Max(ZOOM_MIN, Math.Min(ZOOM_MAX, zoom));
        }

        public int ClampCellSize(int cellSize)
        {
            return Math.Max(CELL_SIZE_MIN, Math.Min(CELL_SIZE_MAX, cellSize));
        }
        #endregion

        #region Public Methods - Formatting
        public string FormatPathDisplay(int pathLength, double computationTimeMs)
        {
            return $"🟢 Path found! Length: {pathLength} cells, Time: {computationTimeMs:F2}ms";
        }

        public string FormatRobotPosition(Point position, float angle)
        {
            return $"Robot: ({position.X},{position.Y}) {angle:F0}°";
        }

        public string FormatBatteryDisplay(double level)
        {
            return $"🔋 Battery: {level:F1}%";
        }

        public string FormatMousePosition(Point mousePos)
        {
            return $"Mouse: ({mousePos.X},{mousePos.Y})";
        }

        public string FormatCellPosition(Point cellPos)
        {
            return $"Cell: ({cellPos.X},{cellPos.Y})";
        }

        public string FormatRealPosition(Point cellPos, double scaleCmPerCell)
        {
            double realX = cellPos.X * scaleCmPerCell;
            double realY = cellPos.Y * scaleCmPerCell;
            return $"Real: ({realX:F1}cm, {realY:F1}cm)";
        }

        public string FormatCellStatus(MapGrid grid, Point cellPos)
        {
            if (!IsValidGridCell(grid, cellPos))
                return "Cell: Invalid";

            var cell = grid[cellPos.X, cellPos.Y];
            return $"Cell ({cellPos.X},{cellPos.Y}) | Walkable: {cell.IsWalkable}";
        }

        public string FormatTimeMs(double ms)
        {
            if (ms < 1) return $"{ms * 1000:F2} μs";
            if (ms < 1000) return $"{ms:F2} ms";
            return $"{ms / 1000:F2} s";
        }

        public string FormatDistanceCm(double cm)
        {
            if (cm < 100) return $"{cm:F1} cm";
            return $"{cm / 100:F2} m";
        }
        #endregion

        #region Public Methods - Status Messages
        public string GetReadyStatus() => "🟢 Ready";
        public string GetSearchingStatus() => "🟡 Searching...";
        public string GetSimulatingStatus() => "🔵 Simulating...";

        public string GetAddingGoalStatus() => "🟡 Click on map to add Goal point (Press ESC to cancel)";
        public string GetAddingParkingStatus() => "🟡 Click on map to add Parking point (Press ESC to cancel)";

        public string GetMovingGoalStatus(string goalName) => $"🟡 Click on map to move {goalName}";
        public string GetMovingParkingStatus(string parkingName) => $"🟡 Click on map to move {parkingName}";

        public string GetAddingObstacleStatus(ObstacleType type) => $"🟡 Click on map to add {type} obstacle (Press ESC to cancel)";
        public string GetAddingElementStatus(MapElementType element) => $"🟡 Click on map to add {element} (Press ESC to cancel)";
        public string GetSettingWeightStatus(byte weight) => $"📊 Click on map to set {weight}% surface weight (Press ESC to cancel)";

        public string GetOperationCancelledStatus() => "🟢 Operation cancelled. Ready";
        public string GetGoalRemovedStatus(string goalName) => $"❌ Goal {goalName} removed";
        public string GetParkingRemovedStatus(string parkingName) => $"❌ Parking {parkingName} removed";

        public string GetMapLoadedStatus(string fileName) => $"✅ Map loaded: {fileName}";
        public string GetMapSavedStatus(string fileName) => $"💾 Map saved: {fileName}";
        public string GetMapUpdatedStatus(int cellSize, double scale) => $"Map updated: Cell size = {cellSize}px, Scale = {scale}cm/cell";
        public string GetNewMapStatus() => "🟢 New map created";
        public string GetViewResetStatus() => "View reset";
        public string GetMapClearedStatus() => "Map cleared";
        public string GetObstaclesClearedStatus() => "All obstacles cleared";

        public string GetTestingStatus() => "🧪 Testing all algorithms...";
        public string GetTestingCompletedStatus() => "✅ Algorithm testing completed";
        public string GetTestClearedStatus() => "Test results cleared";
        public string GetSingleTestStatus(AlgorithmType type) => $"🧪 Testing {type}...";
        public string GetSingleTestCompletedStatus(AlgorithmType type) => $"✅ {type} testing completed";
        #endregion
        #region Public Methods - About Dialog
        public string GetAboutTitle() => "About SallamPathFinder 4";
        public string GetAboutAppName() => "SallamPathFinder 4";
        public string GetAboutVersion() => "Version 4.0.0";
        public string GetAboutDescription() => "Cognitive Mobility Robot - Advanced Pathfinding Simulation Platform";
        public string GetAboutCopyright() => "Copyright © 2024-2026 Mohamed Elsayed Sallam";
        public string GetAboutDevelopedBy() => "Developed by: Mohamed Elsayed Sallam";
        public string GetAboutEmail() => "Email: mohamedslam2000@yahoo.com";
        public string GetAboutUniversity() => "PhD Student at South Ural State University (SUSU)";
        public string GetAboutSupervisor() => "Supervisor: Prof. Tatiana Anatolyevna Makarovskikh";
        public string GetAboutAlgorithms() => "Supported Algorithms:\n" +
            "• A* (A-Star)\n" +
            "• SPPA (Shortest Path with Precautionary Avoidance)\n" +
            "• SPPA-DL (SPPA with Dynamic Learning)\n" +
            "• ACO (Ant Colony Optimization)\n" +
            "• D* (Dynamic A*)\n" +
            "• KNN (K-Nearest Neighbors)\n" +
            "• Brute Force";
        #endregion
        #region Public Methods - Robot Defaults
        public RobotSettings GetDefaultRobotSettings()
        {
            return new RobotSettings
            {
                RobotName = DEFAULT_ROBOT_NAME,
                WidthCm = DEFAULT_ROBOT_WIDTH_CM,
                LengthCm = DEFAULT_ROBOT_LENGTH_CM,
                HeightCm = DEFAULT_ROBOT_HEIGHT_CM,
                InitialSpeedCmS = DEFAULT_ROBOT_SPEED_CM_S,
                InitialBatteryLevel = DEFAULT_ROBOT_BATTERY_PERCENT,
                BatteryConsumptionRate = DEFAULT_ROBOT_CONSUMPTION_RATE,
                ViewAngleDegrees = DEFAULT_VIEW_ANGLE_DEGREES,
                DetectionRangeCells = DEFAULT_DETECTION_RANGE_CELLS,
                RobotColor = COLOR_PRIMARY
            };
        }
        #endregion

        #region Public Methods - Test Results
        public string FormatAlgorithmTestHeader()
        {
            return "===== ALGORITHM TEST RESULTS =====\n\n";
        }

        public string FormatAlgorithmTestSummary(int successful, int total)
        {
            return $"\n--- SUMMARY ---\nSuccessful: {successful}/{total}\n";
        }

        public string FormatBestAlgorithm(string fastestName, double fastestTime, string shortestName, int shortestLength)
        {
            return $"Fastest: {fastestName} ({fastestTime:F2}ms)\nShortest Path: {shortestName} ({shortestLength} cells)";
        }

        public string FormatSingleTestHeader(AlgorithmType type, Point start, Point end)
        {
            return $"===== {type} TEST RESULTS =====\n\nStart: ({start.X},{start.Y})\nEnd: ({end.X},{end.Y})\n";
        }

        public string FormatSingleTestSuccess(int pathLength, double timeMs, int nodesExplored)
        {
            return $"Success: YES\nPath Length: {pathLength} cells\nComputation Time: {timeMs:F2} ms\nNodes Explored: {nodesExplored}\n";
        }

        public string FormatSingleTestFailure(string errorMessage)
        {
            return $"Success: NO\nError: {errorMessage}\n";
        }
        #endregion

        #region Public Methods - Keyboard Shortcuts
        public string GetKeyboardShortcutsText()
        {
            return @"KEYBOARD SHORTCUTS
═══════════════════════════════════════════════════════════

FILE OPERATIONS
───────────────────────────────────────────────────────────
Ctrl+N          New Map
Ctrl+O          Open Map
Ctrl+S          Save Map

PATHFINDING & SIMULATION
───────────────────────────────────────────────────────────
Ctrl+F          Find Path
F5              Start Simulation
F6              Pause Simulation
F7              Stop Simulation

VIEW
───────────────────────────────────────────────────────────
Ctrl++          Zoom In
Ctrl+-          Zoom Out
Ctrl+0          Reset Zoom
G               Toggle Grid
C               Toggle Coordinates
H               Center on Robot

ROBOT CONTROL
───────────────────────────────────────────────────────────
W/A/S/D         Move Robot (Forward/Left/Back/Right)
Q/E             Strafe Robot (Left/Right)

ROBOT MANAGEMENT
───────────────────────────────────────────────────────────
Ctrl+D          Robot Dashboard
Ctrl+Shift+N    Create New Robot
Ctrl+Shift+M    Manage Robots

EXPERIMENTS
───────────────────────────────────────────────────────────
Ctrl+E          Experiment Designer
Ctrl+R          View Results

HELP
───────────────────────────────────────────────────────────
F1              Help
Ctrl+H          Keyboard Shortcuts
Ctrl+Shift+A    About

OTHER
───────────────────────────────────────────────────────────
ESC             Cancel Current Operation
";
        }
        #endregion
    }
}