#region File Header
/// <summary>
/// File: RulerControl.cs
/// Description: Professional ruler control for map measurement
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-28
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
        private const int TICK_LONG = 20;
        private const int TICK_MEDIUM = 15;
        private const int TICK_SHORT = 8;
        #endregion
        #region Private Fields
        private RulerOrientation _orientation;
        private float _scale;
        private int _cellSize;
        private int _gridSize;
        private int _visibleStart;      // بداية المنطقة المرئية (بالخلايا)
        private int _visibleEnd;        // نهاية المنطقة المرئية (بالخلايا)
        private float _zoomLevel;       // مستوى التكبير
        #endregion 

        #region Constructor
        public RulerControl(RulerOrientation orientation)
        {
            _orientation = orientation;
            _scale = 1.0f;
            _cellSize = 30;
            _gridSize = 100;

            this.BackColor = Color.FromArgb(240, 242, 245);
            this.DoubleBuffered = true;

            if (orientation == RulerOrientation.Horizontal)
                this.Height = 80;
            else
                this.Width = 80;
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
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int scaledCellSize = (int)(_cellSize * _scale);
            if (scaledCellSize <= 0) return;

            if (_orientation == RulerOrientation.Horizontal)
            {
                DrawHorizontalRuler(g, scaledCellSize);
            }
            else
            {
                DrawVerticalRuler(g, scaledCellSize);
            }
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// تحديث المسطرة بناءً على المنطقة المرئية من الخريطة
        /// </summary>
        /// <param name="visibleStart">أول خلية مرئية</param>
        /// <param name="visibleEnd">آخر خلية مرئية</param>
        /// <param name="zoomLevel">مستوى التكبير الحالي</param>
        public void UpdateVisibleRange(int visibleStart, int visibleEnd, float zoomLevel)
        {
            _visibleStart = visibleStart;
            _visibleEnd = visibleEnd;
            _zoomLevel = zoomLevel;
            Invalidate();
        }
        #endregion
        #region Private Methods - Horizontal Ruler
        private void DrawHorizontalRuler(Graphics g, int scaledCellSize)
        {
            int centerY = this.Height / 2;

            // الخطوة 1: رسم الأرقام أولاً
            DrawHorizontalNumbers(g, scaledCellSize, centerY);

            // الخطوة 2: رسم الخطوط فوق الأرقام
            DrawHorizontalLines(g, scaledCellSize, centerY);
        }

        private void DrawHorizontalNumbers(Graphics g, int scaledCellSize, int centerY)
        {
            using (var font = new Font("Arial", 10, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                // المنطقة المرئية
                int startX = Math.Max(0, _visibleStart);
                int endX = Math.Min(_gridSize, _visibleEnd);

                // 🔴 الفاصل بين الأرقام (كل 10 خلايا = 10 سم)
                int step = 10;  // ثابت: كل 10 خلايا

                // 🔴 أول رقم يجب أن يكون 0 أو مضاعف 10
                int firstX = 0;
                if (startX > 0)
                {
                    firstX = ((startX + step - 1) / step) * step;
                }

                for (int x = firstX; x <= endX; x += step)
                {
                    if (x < 0) continue;

                    // موقع الرسم على المسطرة
                    int posX = (int)((x - _visibleStart) * scaledCellSize);
                    if (posX < -50 || posX > this.Width + 50) continue;

                    // 🔴 المسافة بالسنتيمتر (وليس رقم الخلية)
                    double distanceCm = x * _scale;
                    string text = distanceCm.ToString("F0");

                    SizeF textSize = g.MeasureString(text, font);
                    float textX = posX - textSize.Width / 2;
                    float textY = centerY - textSize.Height / 2;

                    using (var backBrush = new SolidBrush(Color.White))
                    {
                        g.FillRectangle(backBrush, textX - 2, textY - 2, textSize.Width + 4, textSize.Height + 4);
                    }

                    g.DrawString(text, font, brush, textX, textY);
                }
            }
        }
        private void DrawHorizontalLines(Graphics g, int scaledCellSize, int centerY)
        {
            using (var pen = new Pen(Color.Black, 1))
            {
                int startX = Math.Max(0, _visibleStart);
                int endX = Math.Min(_gridSize, _visibleEnd);

                float firstCellOffset = (_visibleStart - startX) * scaledCellSize;

                for (int x = startX; x <= endX; x++)
                {
                    int posX = (int)((x - _visibleStart) * scaledCellSize - firstCellOffset);
                    if (posX < -50 || posX > this.Width + 50) continue;

                    if (x % 10 == 0)
                    {
                        g.DrawLine(pen, posX, 0, posX, centerY - 5);
                        g.DrawLine(pen, posX, centerY + 5, posX, this.Height);
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
        }
        #endregion

        #region Private Methods - Vertical Ruler
        private void DrawVerticalRuler(Graphics g, int scaledCellSize)
        {
            int centerX = this.Width / 2;

            // الخطوة 1: رسم الأرقام أولاً
            DrawVerticalNumbers(g, scaledCellSize, centerX);

            // الخطوة 2: رسم الخطوط فوق الأرقام
            DrawVerticalLines(g, scaledCellSize, centerX);
        }

        private void DrawVerticalNumbers(Graphics g, int scaledCellSize, int centerX)
        {
            using (var font = new Font("Arial", 10, FontStyle.Bold))
            using (var brush = new SolidBrush(Color.Black))
            {
                int startY = Math.Max(0, _visibleStart);
                int endY = Math.Min(_gridSize, _visibleEnd);

                int step = 10;  // كل 10 خلايا

                int firstY = 0;
                if (startY > 0)
                {
                    firstY = ((startY + step - 1) / step) * step;
                }

                for (int y = firstY; y <= endY; y += step)
                {
                    if (y < 0) continue;

                    int posY = (int)((y - _visibleStart) * scaledCellSize);
                    if (posY < -50 || posY > this.Height + 50) continue;

                    double distanceCm = y * _scale;
                    string text = distanceCm.ToString("F0");

                    SizeF textSize = g.MeasureString(text, font);
                    float textX = centerX - textSize.Width / 2;
                    float textY = posY - textSize.Height / 2;

                    using (var backBrush = new SolidBrush(Color.White))
                    {
                        g.FillRectangle(backBrush, textX - 2, textY - 2, textSize.Width + 4, textSize.Height + 4);
                    }

                    g.DrawString(text, font, brush, textX, textY);
                }
            }
        }
        private void DrawVerticalLines(Graphics g, int scaledCellSize, int centerX)
        {
            using (var pen = new Pen(Color.Black, 1))
            {
                int startY = Math.Max(0, _visibleStart);
                int endY = Math.Min(_gridSize, _visibleEnd);

                float firstCellOffset = (_visibleStart - startY) * scaledCellSize;

                for (int y = startY; y <= endY; y++)
                {
                    int posY = (int)((y - _visibleStart) * scaledCellSize - firstCellOffset);
                    if (posY < -50 || posY > this.Height + 50) continue;

                    if (y % 10 == 0)
                    {
                        g.DrawLine(pen, 0, posY, centerX - 5, posY);
                        g.DrawLine(pen, centerX + 5, posY, this.Width, posY);
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
        }

        /// <summary>
        /// حساب الفاصل بين الأرقام بناءً على حجم الخلية المعروضة
        /// </summary>
        /// <param name="scaledCellSize">حجم الخلية بالبكسل بعد التكبير</param>
        /// <returns>الفاصل بين الأرقام (عدد الخلايا بين كل رقمين)</returns>
     private int CalculateStep(int scaledCellSize)
{
    // الفاصل بين الأرقام بالسنتيمتر = 10 × مستوى التكبير
    double stepCm = 10 * _scale;
    
    // الفاصل بالبكسل = scaledCellSize × مستوى التكبير
    int stepPx = (int)(scaledCellSize * _scale);
    
    // ولكن بما أن scaledCellSize يحتوي أصلاً على _scale،
    // والتباعد يجب أن يكون stepPx بكسل بين كل 10 سم
    if (stepPx < 20) return 20;      // تصغير: كل 20 خلية
    if (stepPx < 40) return 10;      // عادي: كل 10 خلايا
    if (stepPx < 80) return 5;       // تكبير: كل 5 خلايا
    return 1;                         // تكبير عالي: كل خلية
}
        #endregion
    }
}