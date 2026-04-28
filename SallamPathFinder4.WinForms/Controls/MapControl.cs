#region File Header
/// <summary>
/// File: MapControl.cs
/// Description: Professional map control for grid-based pathfinding visualization
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Goals;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Core.Models.Path;
using System.Drawing.Drawing2D;
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
        #endregion

        #region Private Fields - Special Cells
        private Point _startPoint;
        private HashSet<Point> _collisionCells;
        private HashSet<Point> _scannedCells;
        private HashSet<Point> _invalidPathCells;
        /// <summary>
        /// Dictionary of start points with their index (S0, S1, S2, ...)
        /// </summary>
        private Dictionary<(int, int), int> _startPoints;
        private int _nextStartIndex;
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
        #endregion

        #region Private Fields - Robot
        private Point _robotPosition;
        private float _robotAngle;
        private bool _showRobot;
        #endregion

        #region Private Fields - Obstacles
        private List<DynamicObstacle> _dynamicObstacles;
        private ObstacleType _currentObstacleType;
        #endregion

        #region Private Fields - Detection Zone
        private bool _showDetectionZone;
        private Color _detectionZoneColor;
        private List<Point> _detectionZoneCells;
        private int _detectionRangeCells;
        #endregion

        #region Private Fields - Interaction
        private bool _isPanning;
        private Point _lastMousePosition;
        private Random _random;
        #endregion

        #region Private Fields - Start Point
        private Point _robotStartPoint;
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

            _goals = new List<GoalPoint>();
            _parkingPoints = new List<ParkingPoint>();
            _coloredPaths = new List<ColoredPath>();
            _dynamicObstacles = new List<DynamicObstacle>();
            _detectionZoneCells = new List<Point>();
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
            _showDetectionZone = true;
            _detectionZoneColor = Color.FromArgb(80, 52, 152, 219);
            _detectionRangeCells = 2;

            this.MouseWheel += MapControl_MouseWheel;

            _collisionCells = new HashSet<Point>();
            _scannedCells = new HashSet<Point>();
            _invalidPathCells = new HashSet<Point>();
            _startPoint = new Point(10, 10);
            _startPoints = new Dictionary<(int, int), int>();
            _nextStartIndex = 0;
            _collisionCells = new HashSet<Point>();
            _scannedCells = new HashSet<Point>();
            _invalidPathCells = new HashSet<Point>();
        }

        private void InitializeComponent()
        {
            this.Name = "MapControl";
            this.Size = new Size(400, 400);
        }
        #endregion

        #region Public Properties - Core

        public double ScaleCmPerCell { get; set; }

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
        public bool ShowDetectionZone
        {
            get => _showDetectionZone;
            set { _showDetectionZone = value; Invalidate(); }
        }

        public Color DetectionZoneColor
        {
            get => _detectionZoneColor;
            set { _detectionZoneColor = value; Invalidate(); }
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
        #endregion

        #region Public Properties - Start Point

        /// <summary>
        /// Gets or sets the robot start point on the map
        /// </summary>
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

        /// <summary>
        /// Indicates whether a custom start point is set
        /// </summary>
        public bool HasCustomStartPoint => _robotStartPoint != Point.Empty;

        #endregion
      
        #region Public Properties - View
        /// <summary>
        /// الحصول على إزاحة العرض (للتحريك)
        /// </summary>
        public PointF ViewOffset => _viewOffset;
        #endregion

        #region Public Methods - Start Points

        /// <summary>
        /// Sets the current start point (clears previous and adds new)
        /// </summary>
        public void SetCurrentStartPoint(Point location)
        {
            ResetStartPoints();
            AddStartPoint(location);
            _robotStartPoint = location;

            // Also update the actual robot position
            this.RobotPosition = location;

            System.Diagnostics.Debug.WriteLine($"[MapControl] Start point set to ({location.X},{location.Y})");
        }

        /// <summary>
        /// Resets start point counter for new experiment
        /// </summary>
        public void ResetStartPoints()
        {
            _startPoints?.Clear();
            _nextStartIndex = 0;
            _robotStartPoint = Point.Empty;
        }

        /// <summary>
        /// Adds a new start point with auto-incremented index
        /// </summary>
        public void AddStartPoint(Point location)
        {
            if (_startPoints == null)
            {
                _startPoints = new Dictionary<(int, int), int>();
            }

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
        public void UpdateRobotPosition(Point position, float angle)
        {
            _robotPosition = position;
            _robotAngle = angle;
            Invalidate();
        }

        public void MoveRobotManually(Keys key, int stepSize = 1, float rotationAngle = 15f)
        {
            if (_mapGrid == null || !_mapGrid.IsValidCoordinate(_robotPosition.X, _robotPosition.Y)) return;

            Point newPosition = _robotPosition;
            float newAngle = _robotAngle;

            switch (key)
            {
                case Keys.W:
                    newPosition = new Point(
                        _robotPosition.X + (int)(stepSize * Math.Cos(_robotAngle * Math.PI / 180)),
                        _robotPosition.Y + (int)(stepSize * Math.Sin(_robotAngle * Math.PI / 180)));
                    break;
                case Keys.S:
                    newPosition = new Point(
                        _robotPosition.X - (int)(stepSize * Math.Cos(_robotAngle * Math.PI / 180)),
                        _robotPosition.Y - (int)(stepSize * Math.Sin(_robotAngle * Math.PI / 180)));
                    break;
                case Keys.A:
                    newAngle = _robotAngle - rotationAngle;
                    break;
                case Keys.D:
                    newAngle = _robotAngle + rotationAngle;
                    break;
                case Keys.Q:
                    newPosition = new Point(
                        _robotPosition.X + (int)(stepSize * Math.Cos((_robotAngle - 90) * Math.PI / 180)),
                        _robotPosition.Y + (int)(stepSize * Math.Sin((_robotAngle - 90) * Math.PI / 180)));
                    break;
                case Keys.E:
                    newPosition = new Point(
                        _robotPosition.X + (int)(stepSize * Math.Cos((_robotAngle + 90) * Math.PI / 180)),
                        _robotPosition.Y + (int)(stepSize * Math.Sin((_robotAngle + 90) * Math.PI / 180)));
                    break;
            }

            if (_mapGrid.IsValidCoordinate(newPosition.X, newPosition.Y))
            {
                var cell = _mapGrid[newPosition.X, newPosition.Y];
                if (cell.IsWalkable && cell.OccupyingObstacle == null)
                {
                    _robotPosition = newPosition;
                    _robotAngle = newAngle;
                    Invalidate();
                }
            }
        }
        #endregion

        #region Public Methods - Detection Zone
        public void UpdateDetectionZone(List<Point> cells)
        {
            _detectionZoneCells = cells ?? new List<Point>();
            Invalidate();
        }

        public void ClearDetectionZone()
        {
            _detectionZoneCells.Clear();
            Invalidate();
        }

        public void SetDetectionRange(int range)
        {
            _detectionRangeCells = Math.Max(1, Math.Min(10, range));
            Invalidate();
        }
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

        #region Events
        public event EventHandler ViewChanged;  // حدث جديد
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

            // ========== رسم الخلايا (الحلقة المزدوجة) ==========
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
                    DrawDetectionZone(g, x, y, rect);
                    DrawGridLines(g, rect);
                    DrawSpecialCells(g, x, y, rect);
                    // Add this line
                    // Draw coordinates if enabled
                    if (_showCoordinates)
                    {
                        DrawCoordinates(g, rect, x, y);
                    }
                }
            }

            // ========== رسم خطوط المسارات (مرة واحدة فقط) ==========
            DrawPathLines(g);

            // Draw robot on top
            DrawRobot(g);

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

                    //using (var brush = new SolidBrush(Color.FromArgb(150, 160, 100)))
                    //{
                    //    g.FillRectangle(brush, rect);
                    //}
                    //using (var pen = new Pen(cell.IsDoorOpen ? Color.DarkGreen : Color.Brown, 2))
                    //{
                    //    int midX = rect.X + rect.Width / 2;
                    //    g.DrawLine(pen, midX, rect.Y, midX, rect.Y + rect.Height);
                    //}
                    // Different color based on door state
                    Color doorColor = cell.IsDoorOpen
        ? Color.FromArgb(150, 160, 100)   // Open - light olive
        : Color.FromArgb(180, 139, 69, 19); // Closed - brown

                    using (var brush = new SolidBrush(doorColor))
                    {
                        g.FillRectangle(brush, rect);
                    }

                    using (var pen = new Pen(cell.IsDoorOpen ? Color.DarkGreen : Color.DarkRed, 2))
                    {
                        int midX = rect.X + rect.Width / 2;
                        g.DrawLine(pen, midX, rect.Y, midX, rect.Y + rect.Height);
                    }

                    // Show lock icon for closed door
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

        #region Private Methods - Drawing Paths

        private void DrawPaths(Graphics g, int x, int y, Rectangle rect)
        {
            if (_coloredPaths == null) return;

            foreach (var coloredPath in _coloredPaths)
            {
                if (coloredPath == null || coloredPath.Nodes == null) continue;

                // مسار العودة والمسار الأزرق يتم رسمه كخطوط بين الخلايا
                if (coloredPath.Type == PathType.Return || coloredPath.Type == PathType.Charging)
                {
                    continue;
                }

                // المسار العادي (Normal) - تلوين الخلايا
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

                // استخدام القلم مع الإعدادات المناسبة
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

        #endregion

        //// دالة إضافية لرسم الخط المتقطع لمسار العودة
        //private void DrawDashedReturnPath(Graphics g)
        //{
        //    if (_coloredPaths == null) return;

        //    foreach (var coloredPath in _coloredPaths)
        //    {
        //        if (coloredPath == null || !coloredPath.IsReturnPath || coloredPath.Nodes == null || coloredPath.Nodes.Count < 2)
        //            continue;

        //        using (var pen = new Pen(coloredPath.Color, 4))
        //        {
        //            pen.DashStyle = DashStyle.Dash;

        //            for (int i = 1; i < coloredPath.Nodes.Count; i++)
        //            {
        //                var from = coloredPath.Nodes[i - 1];
        //                var to = coloredPath.Nodes[i];
        //                var rectFrom = GetCellRect(from.X, from.Y);
        //                var rectTo = GetCellRect(to.X, to.Y);
        //                var fromCenter = new Point(rectFrom.X + rectFrom.Width / 2, rectFrom.Y + rectFrom.Height / 2);
        //                var toCenter = new Point(rectTo.X + rectTo.Width / 2, rectTo.Y + rectTo.Height / 2);
        //                g.DrawLine(pen, fromCenter, toCenter);
        //            }
        //        }
        //    }
        //}

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

        private void DrawDetectionZone(Graphics g, int x, int y, Rectangle rect)
        {
            if (_showDetectionZone && _detectionZoneCells != null)
            {
                foreach (var cellPos in _detectionZoneCells)
                {
                    if (cellPos.X == x && cellPos.Y == y)
                    {
                        using (var brush = new SolidBrush(_detectionZoneColor))
                        {
                            g.FillRectangle(brush, rect);
                        }
                        break;
                    }
                }
            }
        }

        private void DrawGridLines(Graphics g, Rectangle rect)
        {
            if (!_showGrid) return;

            using (var pen = new Pen(Color.FromArgb(120, 120, 120), 1))
            {
                g.DrawRectangle(pen, rect);
            }
        }

        private void DrawRobot(Graphics g)
        {
            if (!_showRobot || _mapGrid == null || !_mapGrid.IsValidCoordinate(_robotPosition.X, _robotPosition.Y)) return;

            Rectangle robotRect = GetCellRect(_robotPosition.X, _robotPosition.Y);
            if (robotRect.Width <= 0 || robotRect.Height <= 0) return;

            var state = g.Save();
            g.TranslateTransform(robotRect.X + robotRect.Width / 2, robotRect.Y + robotRect.Height / 2);
            g.RotateTransform(_robotAngle);

            int size = Math.Min(robotRect.Width, robotRect.Height);
            PointF[] triangle = new PointF[]
            {
                new PointF(size / 2, 0),
                new PointF(-size / 2, -size / 2),
                new PointF(-size / 2, size / 2)
            };

            using (var brush = new SolidBrush(Color.FromArgb(200, 52, 73, 94)))
            {
                g.FillPolygon(brush, triangle);
            }

            using (var pen = new Pen(Color.White, 1))
            {
                g.DrawPolygon(pen, triangle);
            }

            g.Restore(state);
        }
        #endregion

        /// <summary>
        /// Draws special cells (Start Point S, Collision C, Potential Collision PC, Invalid Path X)
        /// </summary>
        private void DrawSpecialCells(Graphics g, int x, int y, Rectangle rect)
        {
            // Draw Start Point (S0, S1, S2, etc.)
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
                // Use Yellow for better visibility
                DrawCellText(g, rect, startText, Color.Yellow);
                return;
            }

            // Draw Collision Cell (C) - Red
            if (_collisionCells != null && _collisionCells.Contains(new Point(x, y)))
            {
                using (var brush = new SolidBrush(Color.FromArgb(200, 231, 76, 60)))
                {
                    g.FillRectangle(brush, rect);
                }
                // Use White for contrast on red
                DrawCellText(g, rect, "C", Color.White);
                return;
            }

            // Draw Invalid Path Cell (X) - Dark Red with Yellow text
            if (_invalidPathCells != null && _invalidPathCells.Contains(new Point(x, y)))
            {
                using (var brush = new SolidBrush(Color.FromArgb(200, 139, 0, 0)))  // Darker red for contrast
                {
                    g.FillRectangle(brush, rect);
                }
                // Use Yellow for visibility on dark background
                DrawCellText(g, rect, "X", Color.Yellow);
                return;
            }

            // Draw Scanned Cell (PC) - Orange
            if (_scannedCells != null && _scannedCells.Contains(new Point(x, y)))
            {
                using (var brush = new SolidBrush(Color.FromArgb(200, 241, 196, 15)))
                {
                    g.FillRectangle(brush, rect);
                }
                // Use White or Dark text depending on background
                DrawCellText(g, rect, "PC", Color.Black);
            }
        }

        /// <summary>
        /// Draws text centered in a cell
        /// </summary>
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

        #region Protected Methods - Mouse Events
        protected override void OnMouseDown(MouseEventArgs e)
        {
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
            if (e.Button == MouseButtons.Middle)
            {
                _isPanning = false;
                this.Cursor = Cursors.Default;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
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
    }
}