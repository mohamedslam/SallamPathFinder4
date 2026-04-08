#region File Header
/// <summary>
/// File: frmMapSettings.cs
/// Description: Form for configuring map settings (grid size, cell size, scale)
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Map;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Settings.frmMapSettings
{
    /// <summary>
    /// Form for configuring map settings
    /// </summary>
    public sealed partial class frmMapSettings : Form
    {
        #region Private Fields
        private readonly MapGrid _mapGrid;
        private readonly MapSettingsLogic _logic;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the selected cell size
        /// </summary>
        public int CellSize { get; private set; }

        /// <summary>
        /// Gets the selected scale (cm per cell)
        /// </summary>
        public double Scale { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the map settings form
        /// </summary>
        public frmMapSettings(MapGrid grid, int currentCellSize, double currentScale)
        {
            _mapGrid = grid ?? throw new ArgumentNullException(nameof(grid));
            _logic = new MapSettingsLogic();

            CellSize = currentCellSize;
            Scale = currentScale;

            InitializeComponent();
            WireEvents();
            LoadSettings();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            _btnApply.Click += BtnApply_Click;
            _btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Loads current settings into UI controls
        /// </summary>
        private void LoadSettings()
        {
            _nudWidth.Value = Math.Clamp(_mapGrid.Width, _nudWidth.Minimum, _nudWidth.Maximum);
            _nudHeight.Value = Math.Clamp(_mapGrid.Height, _nudHeight.Minimum, _nudHeight.Maximum);
            _nudCellSize.Value = Math.Clamp(CellSize, (int)_nudCellSize.Minimum, (int)_nudCellSize.Maximum);
            _nudScale.Value = (decimal)Math.Clamp(Scale, (double)_nudScale.Minimum, (double)_nudScale.Maximum);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles apply button click - validates and applies settings
        /// </summary>
        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (!_logic.ValidateSettings((int)_nudWidth.Value, (int)_nudHeight.Value))
            {
                MessageBox.Show("Grid dimensions must be between 10 and 500 cells.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Resize grid
            _mapGrid.Resize((int)_nudWidth.Value, (int)_nudHeight.Value);

            // Update settings
            CellSize = (int)_nudCellSize.Value;
            Scale = (double)_nudScale.Value;

            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
    }
}