#region File Header
/// <summary>
/// File: frmSaveMapOptions.cs
/// Description: Dialog for selecting map save options
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.WinForms.Forms.Settings.frmSaveMapOptions
{
    /// <summary>
    /// Dialog for selecting which map components to save
    /// </summary>
    public sealed partial class frmSaveMapOptions : Form
    {
        #region Properties
        /// <summary>
        /// Gets whether to save map grid and surface weights
        /// </summary>
        public bool SaveMap { get; private set; }

        /// <summary>
        /// Gets whether to save static elements
        /// </summary>
        public bool SaveElements { get; private set; }

        /// <summary>
        /// Gets whether to save parking points
        /// </summary>
        public bool SaveParkingPoints { get; private set; }

        /// <summary>
        /// Gets whether to save goal points
        /// </summary>
        public bool SaveGoals { get; private set; }

        /// <summary>
        /// Gets whether to save dynamic obstacles
        /// </summary>
        public bool SaveDynamicObstacles { get; private set; }

        /// <summary>
        /// Gets whether to save current path
        /// </summary>
        public bool SavePath { get; private set; }

        /// <summary>
        /// Gets whether to save robot information
        /// </summary>
        public bool SaveRobotInfo { get; private set; }
        #endregion

        #region Constructor
        public frmSaveMapOptions()
        {
            InitializeComponent();
            WireEvents();
            SetDefaultValues();
        }
        #endregion

        #region Private Methods
        private void WireEvents()
        {
            _btnSave.Click += BtnSave_Click;
            _btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        private void SetDefaultValues()
        {
            _chkMap.Checked = true;
            _chkElements.Checked = true;
            _chkParking.Checked = true;
            _chkGoals.Checked = true;
            _chkObstacles.Checked = true;
            _chkPath.Checked = false;
            _chkRobot.Checked = true;
        }
        #endregion

        #region Event Handlers
        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveMap = _chkMap.Checked;
            SaveElements = _chkElements.Checked;
            SaveParkingPoints = _chkParking.Checked;
            SaveGoals = _chkGoals.Checked;
            SaveDynamicObstacles = _chkObstacles.Checked;
            SavePath = _chkPath.Checked;
            SaveRobotInfo = _chkRobot.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }
        #endregion
    }
}