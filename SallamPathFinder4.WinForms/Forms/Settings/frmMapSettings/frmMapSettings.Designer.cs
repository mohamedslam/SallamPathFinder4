#region File Header
/// <summary>
/// File: frmMapSettings.Designer.cs
/// Description: Designer file for map settings form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Settings.frmMapSettings
{
    partial class frmMapSettings
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.Label _lblTitle;
        private System.Windows.Forms.Label _lblWidth;
        private System.Windows.Forms.NumericUpDown _nudWidth;
        private System.Windows.Forms.Label _lblHeight;
        private System.Windows.Forms.NumericUpDown _nudHeight;
        private System.Windows.Forms.Label _lblCellSize;
        private System.Windows.Forms.NumericUpDown _nudCellSize;
        private System.Windows.Forms.Label _lblScale;
        private System.Windows.Forms.NumericUpDown _nudScale;
        private System.Windows.Forms.Button _btnApply;
        private System.Windows.Forms.Button _btnCancel;
        #endregion

        #region Constructor
        public frmMapSettings()
        {
            InitializeComponent();
        }
        #endregion

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region Initialize Component
        private void InitializeComponent()
        {
            _components = new System.ComponentModel.Container();

            // Form Settings
            this.Text = "Map Settings";
            this.Size = new System.Drawing.Size(360, 300);
            this.MinimumSize = new System.Drawing.Size(360, 300);
            this.MaximumSize = new System.Drawing.Size(360, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            // Title
            _lblTitle = new System.Windows.Forms.Label();
            _lblTitle.Text = "Configure Map Properties";
            _lblTitle.Location = new System.Drawing.Point(15, 15);
            _lblTitle.Size = new System.Drawing.Size(200, 25);
            _lblTitle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            // Width
            _lblWidth = new System.Windows.Forms.Label();
            _lblWidth.Text = "Grid Width (cells):";
            _lblWidth.Location = new System.Drawing.Point(15, 55);
            _lblWidth.Size = new System.Drawing.Size(110, 23);

            _nudWidth = new System.Windows.Forms.NumericUpDown();
            _nudWidth.Location = new System.Drawing.Point(130, 52);
            _nudWidth.Size = new System.Drawing.Size(80, 23);
            _nudWidth.Minimum = 10;
            _nudWidth.Maximum = 500;
            _nudWidth.Value = 100;

            // Height
            _lblHeight = new System.Windows.Forms.Label();
            _lblHeight.Text = "Grid Height (cells):";
            _lblHeight.Location = new System.Drawing.Point(15, 85);
            _lblHeight.Size = new System.Drawing.Size(110, 23);

            _nudHeight = new System.Windows.Forms.NumericUpDown();
            _nudHeight.Location = new System.Drawing.Point(130, 82);
            _nudHeight.Size = new System.Drawing.Size(80, 23);
            _nudHeight.Minimum = 10;
            _nudHeight.Maximum = 500;
            _nudHeight.Value = 100;

            // Cell Size
            _lblCellSize = new System.Windows.Forms.Label();
            _lblCellSize.Text = "Cell Size (pixels):";
            _lblCellSize.Location = new System.Drawing.Point(15, 115);
            _lblCellSize.Size = new System.Drawing.Size(110, 23);

            _nudCellSize = new System.Windows.Forms.NumericUpDown();
            _nudCellSize.Location = new System.Drawing.Point(130, 112);
            _nudCellSize.Size = new System.Drawing.Size(80, 23);
            _nudCellSize.Minimum = 10;
            _nudCellSize.Maximum = 100;
            _nudCellSize.Value = 30;

            // Scale
            _lblScale = new System.Windows.Forms.Label();
            _lblScale.Text = "Scale (cm/cell):";
            _lblScale.Location = new System.Drawing.Point(15, 145);
            _lblScale.Size = new System.Drawing.Size(110, 23);

            _nudScale = new System.Windows.Forms.NumericUpDown();
            _nudScale.Location = new System.Drawing.Point(130, 142);
            _nudScale.Size = new System.Drawing.Size(80, 23);
            _nudScale.Minimum = 0.1M;
            _nudScale.Maximum = 100;
            _nudScale.Value = 10;
            _nudScale.DecimalPlaces = 1;
            _nudScale.Increment = 0.1M;

            // Apply Button
            _btnApply = new System.Windows.Forms.Button();
            _btnApply.Text = "Apply";
            _btnApply.Location = new System.Drawing.Point(50, 190);
            _btnApply.Size = new System.Drawing.Size(100, 35);
            _btnApply.FlatStyle = FlatStyle.Flat;
            _btnApply.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            _btnApply.ForeColor = System.Drawing.Color.White;
            _btnApply.Cursor = Cursors.Hand;

            // Cancel Button
            _btnCancel = new System.Windows.Forms.Button();
            _btnCancel.Text = "Cancel";
            _btnCancel.Location = new System.Drawing.Point(170, 190);
            _btnCancel.Size = new System.Drawing.Size(100, 35);
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            _btnCancel.ForeColor = System.Drawing.Color.White;
            _btnCancel.Cursor = Cursors.Hand;

            // Add controls to form
            this.Controls.Add(_lblTitle);
            this.Controls.Add(_lblWidth);
            this.Controls.Add(_nudWidth);
            this.Controls.Add(_lblHeight);
            this.Controls.Add(_nudHeight);
            this.Controls.Add(_lblCellSize);
            this.Controls.Add(_nudCellSize);
            this.Controls.Add(_lblScale);
            this.Controls.Add(_nudScale);
            this.Controls.Add(_btnApply);
            this.Controls.Add(_btnCancel);
        }
        #endregion
    }
}