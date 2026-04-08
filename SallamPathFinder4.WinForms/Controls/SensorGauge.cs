#region File Header
/// <summary>
/// File: SensorGauge.cs
/// Description: Visual gauge control for displaying sensor readings
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
#endregion

namespace SallamPathFinder4.WinForms.Controls
{
    public sealed class SensorGauge : UserControl
    {
        #region Constants
        private const double MAX_DISTANCE = 5.0;
        private const int WARNING_THRESHOLD = 2;
        private const int DANGER_THRESHOLD = 1;
        #endregion

        #region Private Fields
        private string _label;
        private string _orientation;
        private double _value;
        private double _maxValue;
        #endregion

        #region Constructor
        public SensorGauge(string label, string orientation)
        {
            _label = label;
            _orientation = orientation;
            _value = 0;
            _maxValue = MAX_DISTANCE;

            this.Size = new Size(80, 80);
            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;
        }
        #endregion

        #region Properties
        public double Value
        {
            get => _value;
            set
            {
                _value = Math.Min(_maxValue, Math.Max(0, value));
                Invalidate();
            }
        }

        public double MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = Math.Max(0.1, value);
                Invalidate();
            }
        }

        public string LabelText
        {
            get => _label;
            set
            {
                _label = value;
                Invalidate();
            }
        }
        #endregion

        #region Protected Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int centerX = this.Width / 2;
            int centerY = this.Height / 2;
            int radius = Math.Min(this.Width, this.Height) / 2 - 10;

            // Draw background circle
            using (var pen = new Pen(Color.FromArgb(100, 100, 100), 1))
            using (var brush = new SolidBrush(Color.FromArgb(30, 30, 30)))
            {
                g.FillEllipse(brush, centerX - radius, centerY - radius, radius * 2, radius * 2);
                g.DrawEllipse(pen, centerX - radius, centerY - radius, radius * 2, radius * 2);
            }

            // Draw value arc
            double percentage = _value / _maxValue;
            float angle = (float)(percentage * 360);

            Color arcColor = GetArcColor();
            using (var pen = new Pen(arcColor, 3))
            {
                g.DrawArc(pen, centerX - radius + 5, centerY - radius + 5,
                    (radius - 5) * 2, (radius - 5) * 2, -90, angle);
            }

            // Draw label and value text
            using (var font = new Font("Segoe UI", 8, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.White))
            {
                string displayText = $"{_label}\n{_value:F1}m";
                SizeF textSize = g.MeasureString(displayText, font);
                PointF textPoint = new PointF(centerX - textSize.Width / 2, centerY - textSize.Height / 2);
                g.DrawString(displayText, font, brush, textPoint);
            }

            // Draw orientation indicator
            using (var iconFont = new Font("Segoe UI", 12, FontStyle.Regular))
            using (var brush = new SolidBrush(Color.FromArgb(150, 150, 150)))
            {
                string icon = _orientation switch
                {
                    "Top" => "▲",
                    "Bottom" => "▼",
                    "Left" => "◀",
                    "Right" => "▶",
                    _ => "●"
                };

                SizeF iconSize = g.MeasureString(icon, iconFont);
                float iconX = centerX - iconSize.Width / 2;
                float iconY = centerY - radius - 15;
                g.DrawString(icon, iconFont, brush, iconX, iconY);
            }
        }
        #endregion

        #region Private Methods
        private Color GetArcColor()
        {
            if (_value <= DANGER_THRESHOLD)
                return Color.FromArgb(231, 76, 60);

            if (_value <= WARNING_THRESHOLD)
                return Color.FromArgb(241, 196, 15);

            return Color.FromArgb(46, 204, 113);
        }
        #endregion
    }
}