#region File Header
/// <summary>
/// File: RobotDesignerCanvas.cs
/// Description: Professional canvas for robot design with real-time preview
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-29
/// </summary>
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Robot;

namespace SallamPathFinder4.WinForms.Controls
{
    public class RobotDesignerCanvas : UserControl
    {
        #region Private Fields
        private RobotDefinition _currentRobot;
        private Point _centerPoint;
        private float _zoom = 1.0f;
        private Point _dragStart;
        private bool _isDragging;
        private Point _offset = Point.Empty;
        private SimpleSensor _selectedSensor;
        private bool _isDraggingSensor;
        private Point _lastMousePos;
        private float _previewAngle = 0;
        private bool _showSensorFOV = true;
        private bool _showGrid = true;
        private bool _showCoordinates = true;
        #endregion

        #region Properties
        public RobotDefinition CurrentRobot
        {
            get => _currentRobot;
            set
            {
                _currentRobot = value;
                Invalidate();
            }
        }

        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = Math.Clamp(value, 0.3f, 3.0f);
                Invalidate();
            }
        }

        public float PreviewAngle
        {
            get => _previewAngle;
            set
            {
                _previewAngle = value % 360;
                Invalidate();
            }
        }

        public bool ShowSensorFOV
        {
            get => _showSensorFOV;
            set { _showSensorFOV = value; Invalidate(); }
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

        #region Events
        public event EventHandler<SimpleSensor> SensorSelected;
        public event EventHandler SensorAdded;
        public event EventHandler SensorRemoved;
        public event EventHandler RobotChanged;
        #endregion

        #region Constructor
        public RobotDesignerCanvas()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer, true);

            this.Size = new Size(600, 600);
            this.BackColor = Color.FromArgb(240, 242, 245);

            InitializeCanvas();
        }
        #endregion

        #region Initialization
        private void InitializeCanvas()
        {
            _centerPoint = new Point(this.Width / 2, this.Height / 2);

            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp += OnMouseUp;
            this.MouseWheel += OnMouseWheel;
            this.Paint += OnPaint;
            this.Resize += (s, e) => { _centerPoint = new Point(this.Width / 2, this.Height / 2); Invalidate(); };
        }
        #endregion

        #region Drawing - Main
        private void OnPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TranslateTransform(_centerPoint.X + _offset.X, _centerPoint.Y + _offset.Y);
            e.Graphics.ScaleTransform(_zoom, _zoom);

            if (_showGrid) DrawGrid(e.Graphics);
            DrawRobot(e.Graphics);
            DrawSensors(e.Graphics);

            if (_isDraggingSensor && _selectedSensor != null)
            {
                DrawSensorPlacementGuide(e.Graphics);
            }
        }

        private void DrawGrid(Graphics g)
        {
            int gridSize = 20;
            using (var gridPen = new Pen(Color.FromArgb(200, 200, 200), 1))
            {
                for (int x = -500; x <= 500; x += gridSize)
                {
                    g.DrawLine(gridPen, x, -500, x, 500);
                    g.DrawLine(gridPen, -500, x, 500, x);
                }
            }

            using (var originPen = new Pen(Color.FromArgb(100, 100, 100), 2))
            {
                g.DrawLine(originPen, -20, 0, 20, 0);
                g.DrawLine(originPen, 0, -20, 0, 20);
            }

            if (_showCoordinates)
            {
                using (var coordPen = new Pen(Color.FromArgb(150, 150, 150), 1))
                using (var font = new Font("Consolas", 8))
                using (var brush = new SolidBrush(Color.Gray))
                {
                    for (int x = -200; x <= 200; x += 40)
                    {
                        if (x != 0) g.DrawString($"{x}", font, brush, x - 10, 5);
                    }
                    for (int y = -200; y <= 200; y += 40)
                    {
                        if (y != 0) g.DrawString($"{y}", font, brush, 5, y - 5);
                    }
                }
            }
        }
        #endregion

        #region Drawing - Robot Body
        private void DrawRobot(Graphics g)
        {
            if (_currentRobot == null) return;

            var state = g.Save();
            g.RotateTransform(_previewAngle);

            var appearance = _currentRobot.Appearance;
            int width = (int)appearance.Width;
            int height = (int)appearance.Height;
            var rect = new Rectangle(-width / 2, -height / 2, width, height);
            Color bodyColor = GetBodyColor();

            using (var bodyBrush = new SolidBrush(bodyColor))
            using (var borderPen = new Pen(Color.FromArgb(44, 62, 80), 2))
            {
                DrawShape(g, rect, appearance.ShapeType, bodyBrush, borderPen);
            }

            // Special drawing based on robot type
            if (_currentRobot.RobotType == RobotType.Flying)
            {
                DrawDrone(g, width, height);
            }
            else if (_currentRobot.RobotType == RobotType.Tracked)
            {
                DrawTracks(g, width, height);
            }
            else if (appearance.ShowWheels)
            {
                DrawWheels(g, width, height);
            }

            if (appearance.ShowDirectionArrow)
            {
                DrawDirectionArrow(g, height);
            }

            g.Restore(state);
        }

        private void DrawShape(Graphics g, Rectangle rect, RobotShapeType shapeType, Brush brush, Pen pen)
        {
            switch (shapeType)
            {
                case RobotShapeType.Rectangle:
                    g.FillRectangle(brush, rect);
                    g.DrawRectangle(pen, rect);
                    break;
                case RobotShapeType.Square:
                    int size = Math.Min(rect.Width, rect.Height);
                    var squareRect = new Rectangle(-size / 2, -size / 2, size, size);
                    g.FillRectangle(brush, squareRect);
                    g.DrawRectangle(pen, squareRect);
                    break;
                case RobotShapeType.Circle:
                    g.FillEllipse(brush, rect);
                    g.DrawEllipse(pen, rect);
                    break;
                case RobotShapeType.RoundedRect:
                    int radius = Math.Min(rect.Width, rect.Height) / 4;
                    using (var path = GetRoundedRectanglePath(rect, radius))
                    {
                        g.FillPath(brush, path);
                        g.DrawPath(pen, path);
                    }
                    break;
                case RobotShapeType.Triangle:
                    var trianglePoints = new Point[]
                    {
                        new Point(0, -rect.Height / 2),
                        new Point(-rect.Width / 2, rect.Height / 2),
                        new Point(rect.Width / 2, rect.Height / 2)
                    };
                    g.FillPolygon(brush, trianglePoints);
                    g.DrawPolygon(pen, trianglePoints);
                    break;
                case RobotShapeType.Hexagon:
                    var hexagonPoints = GetHexagonPoints(rect.Width, rect.Height);
                    g.FillPolygon(brush, hexagonPoints);
                    g.DrawPolygon(pen, hexagonPoints);
                    break;
            }
        }

        private void DrawDrone(Graphics g, int width, int height)
        {
            Color bodyColor = GetBodyColor();
            int armLength = width / 2;
            int motorSize = width / 5;

            using (var armPen = new Pen(Color.FromArgb(100, 100, 100), 3))
            using (var motorBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
            using (var propellerBrush = new SolidBrush(Color.FromArgb(200, 200, 200)))
            {
                g.DrawLine(armPen, -armLength, -armLength, armLength, armLength);
                g.DrawLine(armPen, -armLength, armLength, armLength, -armLength);

                Point[] motorPositions = new Point[]
                {
                    new Point(-armLength, -armLength),
                    new Point(armLength, -armLength),
                    new Point(-armLength, armLength),
                    new Point(armLength, armLength)
                };

                foreach (var pos in motorPositions)
                {
                    g.FillEllipse(motorBrush, pos.X - motorSize / 2, pos.Y - motorSize / 2, motorSize, motorSize);
                    g.DrawEllipse(new Pen(Color.Black, 1), pos.X - motorSize / 2, pos.Y - motorSize / 2, motorSize, motorSize);

                    var propState = g.Save();
                    g.TranslateTransform(pos.X, pos.Y);
                    g.RotateTransform(_previewAngle);
                    int propLength = motorSize * 2;
                    g.FillRectangle(propellerBrush, -propLength / 2, -motorSize / 4, propLength, motorSize / 2);
                    g.Restore(propState);
                }
            }
        }

        private void DrawWheels(Graphics g, int width, int height)
        {
            int wheelWidth = (int)(width * 0.15);
            int wheelHeight = (int)(height * 0.25);
            int wheelOffset = width / 3;

            using (var wheelBrush = new SolidBrush(Color.FromArgb(44, 62, 80)))
            {
                g.FillEllipse(wheelBrush, -wheelOffset - wheelWidth / 2, height / 2 - wheelHeight / 2, wheelWidth, wheelHeight);
                g.FillEllipse(wheelBrush, wheelOffset - wheelWidth / 2, height / 2 - wheelHeight / 2, wheelWidth, wheelHeight);

                if (_currentRobot.RobotType == RobotType.Omnidirectional)
                {
                    g.FillEllipse(wheelBrush, -wheelOffset - wheelWidth / 2, -height / 2 - wheelHeight / 2, wheelWidth, wheelHeight);
                    g.FillEllipse(wheelBrush, wheelOffset - wheelWidth / 2, -height / 2 - wheelHeight / 2, wheelWidth, wheelHeight);
                }
            }
        }

        private void DrawTracks(Graphics g, int width, int height)
        {
            int trackWidth = (int)(width * 0.2);
            int trackHeight = height;

            using (var trackBrush = new SolidBrush(Color.FromArgb(60, 60, 60)))
            {
                g.FillRectangle(trackBrush, -width / 2 - trackWidth / 2, -height / 2, trackWidth, trackHeight);
                g.FillRectangle(trackBrush, width / 2 - trackWidth / 2, -height / 2, trackWidth, trackHeight);

                for (int i = -height / 2; i < height / 2; i += 8)
                {
                    g.FillRectangle(trackBrush, -width / 2 - trackWidth / 2 + 2, i, 3, 5);
                    g.FillRectangle(trackBrush, width / 2 - trackWidth / 2 + 2, i, 3, 5);
                }
            }
        }

        private void DrawDirectionArrow(Graphics g, int height)
        {
            using (var arrowBrush = new SolidBrush(Color.FromArgb(46, 204, 113)))
            {
                var arrowPoints = new Point[]
                {
                    new Point(0, -height / 2 - 5),
                    new Point(-8, -height / 2 + 12),
                    new Point(8, -height / 2 + 12)
                };
                g.FillPolygon(arrowBrush, arrowPoints);
            }
        }

        private Color GetBodyColor()
        {
            if (_currentRobot == null || string.IsNullOrEmpty(_currentRobot.Appearance.Color))
                return Color.FromArgb(52, 152, 219);
            try
            {
                return ColorTranslator.FromHtml(_currentRobot.Appearance.Color);
            }
            catch
            {
                return Color.FromArgb(52, 152, 219);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.X + rect.Width - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.X + rect.Width - radius * 2, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Y + rect.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Point[] GetHexagonPoints(int width, int height)
        {
            return new Point[]
            {
                new Point(0, -height / 2),
                new Point(width / 2, -height / 3),
                new Point(width / 2, height / 3),
                new Point(0, height / 2),
                new Point(-width / 2, height / 3),
                new Point(-width / 2, -height / 3)
            };
        }
        #endregion

        #region Drawing - Sensors
        private void DrawSensors(Graphics g)
        {
            if (_currentRobot?.Sensors == null) return;

            foreach (var sensor in _currentRobot.Sensors)
            {
                if (!sensor.IsEnabled) continue;
                DrawSensor(g, sensor);
                if (_showSensorFOV && sensor.FieldOfView > 0)
                {
                    DrawFieldOfView(g, sensor);
                }
            }
        }

        private void DrawSensor(Graphics g, SimpleSensor sensor)
        {
            int sensorSize = 12;
            Point pos = new Point(sensor.PositionX, sensor.PositionY);
            Color sensorColor = GetSensorColor(sensor.SensorType);

            using (var brush = new SolidBrush(sensorColor))
            using (var borderPen = new Pen(Color.Black, 2))
            using (var innerBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(brush, pos.X - sensorSize / 2, pos.Y - sensorSize / 2, sensorSize, sensorSize);
                g.DrawEllipse(borderPen, pos.X - sensorSize / 2, pos.Y - sensorSize / 2, sensorSize, sensorSize);
                g.FillEllipse(innerBrush, pos.X - sensorSize / 4, pos.Y - sensorSize / 4, sensorSize / 2, sensorSize / 2);
            }

            double radAngle = sensor.MountAngle * Math.PI / 180.0;
            int lineEndX = pos.X + (int)(25 * Math.Cos(radAngle));
            int lineEndY = pos.Y + (int)(25 * Math.Sin(radAngle));

            using (var linePen = new Pen(Color.FromArgb(150, 0, 0, 0), 1.5f))
            {
                linePen.EndCap = LineCap.ArrowAnchor;
                g.DrawLine(linePen, pos.X, pos.Y, lineEndX, lineEndY);
            }
        }

        private void DrawFieldOfView(Graphics g, SimpleSensor sensor)
        {
            double radAngle = sensor.MountAngle * Math.PI / 180.0;
            double halfFOV = sensor.FieldOfView * Math.PI / 360.0;
            Point sensorPos = new Point(sensor.PositionX, sensor.PositionY);
            int arcRadius = (int)Math.Min(sensor.MaxRange, 80);

            Point end1 = new Point(
                sensorPos.X + (int)(arcRadius * Math.Cos(radAngle - halfFOV)),
                sensorPos.Y + (int)(arcRadius * Math.Sin(radAngle - halfFOV)));
            Point end2 = new Point(
                sensorPos.X + (int)(arcRadius * Math.Cos(radAngle + halfFOV)),
                sensorPos.Y + (int)(arcRadius * Math.Sin(radAngle + halfFOV)));

            using (var fovBrush = new SolidBrush(Color.FromArgb(50, 52, 152, 219)))
            using (var fovPen = new Pen(Color.FromArgb(100, 52, 152, 219), 1))
            {
                Point[] conePoints = { sensorPos, end1, end2 };
                g.FillPolygon(fovBrush, conePoints);
                g.DrawPolygon(fovPen, conePoints);
            }
        }

        private Color GetSensorColor(string sensorType)
        {
            switch (sensorType)
            {
                case "Ultrasonic": return Color.FromArgb(46, 204, 113);
                case "Infrared": return Color.FromArgb(231, 76, 60);
                case "Lidar": return Color.FromArgb(44, 62, 80);
                case "Camera": return Color.FromArgb(155, 89, 182);
                case "Proximity": return Color.FromArgb(241, 196, 15);
                case "Temperature": return Color.FromArgb(230, 126, 34);
                case "Pressure": return Color.FromArgb(52, 73, 94);
                case "Humidity": return Color.FromArgb(26, 188, 156);
                case "GPS": return Color.FromArgb(22, 160, 133);
                case "IMU": return Color.FromArgb(52, 152, 219);
                default: return Color.FromArgb(52, 152, 219);
            }
        }

        private void DrawSensorPlacementGuide(Graphics g)
        {
            if (_selectedSensor == null) return;

            using (var guidePen = new Pen(Color.FromArgb(150, 46, 204, 113), 2))
            {
                guidePen.DashStyle = DashStyle.Dash;
                g.DrawEllipse(guidePen, _selectedSensor.PositionX - 15, _selectedSensor.PositionY - 15, 30, 30);
            }
        }
        #endregion

        #region Mouse Events
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Point canvasPoint = ScreenToCanvas(e.Location);
            _selectedSensor = GetSensorAtPosition(canvasPoint);

            if (_selectedSensor != null && e.Button == MouseButtons.Left)
            {
                _isDraggingSensor = true;
                _lastMousePos = canvasPoint;
                SensorSelected?.Invoke(this, _selectedSensor);
                Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (_selectedSensor != null)
                {
                    _selectedSensor.IsEnabled = !_selectedSensor.IsEnabled;
                    Invalidate();
                }
            }
            else
            {
                _isDragging = true;
                _dragStart = e.Location;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point canvasPoint = ScreenToCanvas(e.Location);

            if (_isDraggingSensor && _selectedSensor != null)
            {
                int dx = canvasPoint.X - _lastMousePos.X;
                int dy = canvasPoint.Y - _lastMousePos.Y;
                _selectedSensor.PositionX = Math.Clamp(_selectedSensor.PositionX + dx, -150, 150);
                _selectedSensor.PositionY = Math.Clamp(_selectedSensor.PositionY + dy, -150, 150);
                _lastMousePos = canvasPoint;
                Invalidate();
                RobotChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (_isDragging)
            {
                int dx = e.X - _dragStart.X;
                int dy = e.Y - _dragStart.Y;
                _offset = new Point(_offset.X + dx, _offset.Y + dy);
                _dragStart = e.Location;
                Invalidate();
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _isDragging = false;
            _isDraggingSensor = false;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            Zoom += e.Delta > 0 ? 0.1f : -0.1f;
        }

        private Point ScreenToCanvas(Point screenPoint)
        {
            return new Point(
                (int)((screenPoint.X - _centerPoint.X - _offset.X) / _zoom),
                (int)((screenPoint.Y - _centerPoint.Y - _offset.Y) / _zoom));
        }

        private SimpleSensor GetSensorAtPosition(Point canvasPoint)
        {
            if (_currentRobot?.Sensors == null) return null;

            foreach (var sensor in _currentRobot.Sensors)
            {
                int dx = canvasPoint.X - sensor.PositionX;
                int dy = canvasPoint.Y - sensor.PositionY;
                if (Math.Sqrt(dx * dx + dy * dy) < 15)
                {
                    return sensor;
                }
            }
            return null;
        }
        #endregion

        #region Public Methods
        public void DeleteSelectedSensor()
        {
            if (_selectedSensor != null && _currentRobot != null)
            {
                _currentRobot.Sensors.Remove(_selectedSensor);
                SensorRemoved?.Invoke(this, EventArgs.Empty);
                _selectedSensor = null;
                Invalidate();
            }
        }

        public void ClearSelection()
        {
            _selectedSensor = null;
            Invalidate();
        }

        public void ResetView()
        {
            _offset = Point.Empty;
            Zoom = 1.0f;
            PreviewAngle = 0;
        }

        public void RefreshCanvas()
        {
            Invalidate();
        }
        #endregion
    }
}