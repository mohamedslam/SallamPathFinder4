#region File Header
/// <summary>
/// File: RulerControl.cs
/// Description: Professional ruler control for map measurement
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.WinForms.Controls
{
    public sealed class RulerControl : Panel
    {
        #region Enums
        public enum RulerOrientation
        {
            Horizontal,
            Vertical
        }
        #endregion

        #region Constants
        private const int TICK_LONG = 15;
        private const int TICK_MEDIUM = 10;
        private const int TICK_SHORT = 5;
        #endregion

        #region Private Fields
        private RulerOrientation _orientation;
        private float _scale;
        private int _cellSize;
        private int _gridSize;
        #endregion

        #region Constructor
        public RulerControl(RulerOrientation orientation)
        {
            _orientation = orientation;
            _scale = 1.0f;
            _cellSize = 20;
            _gridSize = 100;

            this.BackColor = Color.FromArgb(240, 242, 245);
            this.DoubleBuffered = true;

            if (orientation == RulerOrientation.Horizontal)
                this.Height = 28;
            else
                this.Width = 28;
        }
        #endregion

        #region Properties
        public float Scale
        {
            get => _scale;
            set { _scale = value; Invalidate(); }
        }

        public int CellSize
        {
            get => _cellSize;
            set { _cellSize = value; Invalidate(); }
        }

        public int GridSize
        {
            get => _gridSize;
            set { _gridSize = value; Invalidate(); }
        }
        #endregion

        #region Protected Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using var pen = new Pen(Color.FromArgb(120, 120, 120), 1);
            using var font = new Font("Consolas", 7);
            using var textBrush = new SolidBrush(Color.FromArgb(60, 60, 60));

            if (_orientation == RulerOrientation.Horizontal)
            {
                DrawHorizontalRuler(g, pen, font, textBrush);
            }
            else
            {
                DrawVerticalRuler(g, pen, font, textBrush);
            }
        }
        #endregion

        #region Private Methods
        private void DrawHorizontalRuler(Graphics g, Pen pen, Font font, Brush textBrush)
        {
            int scaledCellSize = (int)(_cellSize * _scale);

            for (int x = 0; x <= _gridSize; x++)
            {
                int posX = x * scaledCellSize;
                if (posX > this.Width) break;

                if (x % 10 == 0)
                {
                    g.DrawLine(pen, posX, 0, posX, TICK_LONG);

                    double cm = x * _scale;
                    string text = cm.ToString("0.#");
                    SizeF textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, textBrush, posX - textSize.Width / 2, TICK_LONG + 2);
                }
                else if (x % 5 == 0)
                {
                    g.DrawLine(pen, posX, 0, posX, TICK_MEDIUM);
                }
                else
                {
                    g.DrawLine(pen, posX, this.Height - TICK_SHORT, posX, this.Height);
                }
            }
        }

        private void DrawVerticalRuler(Graphics g, Pen pen, Font font, Brush textBrush)
        {
            int scaledCellSize = (int)(_cellSize * _scale);

            for (int y = 0; y <= _gridSize; y++)
            {
                int posY = y * scaledCellSize;
                if (posY > this.Height) break;

                if (y % 10 == 0)
                {
                    g.DrawLine(pen, 0, posY, TICK_LONG, posY);

                    double cm = y * _scale;
                    string text = cm.ToString("0.#");
                    SizeF textSize = g.MeasureString(text, font);
                    g.DrawString(text, font, textBrush, TICK_LONG + 2, posY - textSize.Height / 2);
                }
                else if (y % 5 == 0)
                {
                    g.DrawLine(pen, 0, posY, TICK_MEDIUM, posY);
                }
                else
                {
                    g.DrawLine(pen, this.Width - TICK_SHORT, posY, this.Width, posY);
                }
            }
        }
        #endregion
    }
}