#region File Header
/// <summary>
/// File: CommonConstants.cs
/// Description: Shared constants used across the entire application
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Helpers
{
    #region Class Documentation
    /// <summary>
    /// Centralized constants for the application
    /// Following DRY principle - single source of truth
    /// </summary>
    #endregion
    public static class CommonConstants
    {
        #region Grid Constants
        /// <summary>Minimum grid width/height in cells</summary>
        public const int MIN_GRID_SIZE = 10;

        /// <summary>Maximum grid width/height in cells</summary>
        public const int MAX_GRID_SIZE = 500;

        /// <summary>Default grid size</summary>
        public const int DEFAULT_GRID_SIZE = 100;

        /// <summary>Minimum cell size in pixels</summary>
        public const int MIN_CELL_SIZE_PX = 10;

        /// <summary>Maximum cell size in pixels</summary>
        public const int MAX_CELL_SIZE_PX = 100;

        /// <summary>Default cell size in pixels</summary>
        public const int DEFAULT_CELL_SIZE_PX = 20;

        /// <summary>Default scale in cm per cell</summary>
        public const double DEFAULT_SCALE_CM_PER_CELL = 10.0;
        #endregion

        #region Robot Constants
        /// <summary>Default robot width in cm</summary>
        public const double DEFAULT_ROBOT_WIDTH_CM = 60;

        /// <summary>Default robot length in cm</summary>
        public const double DEFAULT_ROBOT_LENGTH_CM = 60;

        /// <summary>Default robot height in cm</summary>
        public const double DEFAULT_ROBOT_HEIGHT_CM = 30;

        /// <summary>Default robot speed in cm/s</summary>
        public const double DEFAULT_ROBOT_SPEED_CM_S = 10;

        /// <summary>Maximum robot speed in cm/s</summary>
        public const double MAX_ROBOT_SPEED_CM_S = 100;

        /// <summary>Default battery level percent</summary>
        public const double DEFAULT_BATTERY_PERCENT = 100.0;

        /// <summary>Low battery warning threshold percent</summary>
        public const double LOW_BATTERY_THRESHOLD = 20.0;

        /// <summary>Critical battery threshold percent</summary>
        public const double CRITICAL_BATTERY_THRESHOLD = 10.0;
        #endregion

        #region Pathfinding Constants
        /// <summary>Default heuristic weight</summary>
        public const int DEFAULT_HEURISTIC_WEIGHT = 2;

        /// <summary>Default search limit (nodes)</summary>
        public const int DEFAULT_SEARCH_LIMIT = 50000;

        /// <summary>Diagonal movement cost factor (√2)</summary>
        public const double DIAGONAL_COST_FACTOR = 1.4142135623730951;
        #endregion

        #region Simulation Constants
        /// <summary>Default step delay in seconds</summary>
        public const double DEFAULT_STEP_DELAY_SECONDS = 1.0;

        /// <summary>Default detection range in cells</summary>
        public const int DEFAULT_DETECTION_RANGE_CELLS = 2;

        /// <summary>Default field of view in degrees</summary>
        public const double DEFAULT_FIELD_OF_VIEW_DEGREES = 180.0;
        #endregion

        #region Learning Constants
        /// <summary>Default learning rate (alpha)</summary>
        public const double DEFAULT_LEARNING_RATE = 2.0;

        /// <summary>Default evaporation rate for ACO</summary>
        public const double DEFAULT_EVAPORATION_RATE = 0.1;

        /// <summary>Default number of ants for ACO</summary>
        public const int DEFAULT_ANT_COUNT = 20;

        /// <summary>Default iterations for ACO</summary>
        public const int DEFAULT_ACO_ITERATIONS = 100;
        #endregion

        #region File Constants
        /// <summary>Map file extension</summary>
        public const string MAP_FILE_EXTENSION = ".smap";

        /// <summary>Experiment directory name</summary>
        public const string EXPERIMENTS_DIRECTORY = "Experiments";

        /// <summary>Obstacle memory file name</summary>
        public const string OBSTACLE_MEMORY_FILE = "ObstacleMemory.json";

        /// <summary>Obstacle settings file name</summary>
        public const string OBSTACLE_SETTINGS_FILE = "ObstacleSettings.json";
        #endregion

        #region UI Constants
        /// <summary>Default form width</summary>
        public const int DEFAULT_FORM_WIDTH = 1200;

        /// <summary>Default form height</summary>
        public const int DEFAULT_FORM_HEIGHT = 800;

        /// <summary>Right panel width in pixels</summary>
        public const int RIGHT_PANEL_WIDTH = 340;

        /// <summary>Ruler width in pixels</summary>
        public const int RULER_SIZE_PX = 30;
        #endregion

        #region Validation Constants
        /// <summary>Minimum surface weight</summary>
        public const byte MIN_SURFACE_WEIGHT = 1;

        /// <summary>Maximum surface weight</summary>
        public const byte MAX_SURFACE_WEIGHT = 100;

        /// <summary>Minimum ramp difficulty</summary>
        public const byte MIN_RAMP_DIFFICULTY = 0;

        /// <summary>Maximum ramp difficulty</summary>
        public const byte MAX_RAMP_DIFFICULTY = 100;
        #endregion
    }
}