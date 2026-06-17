#region File Header
/// <summary>
/// File: frmSaveExperimentSettings.cs
/// Description: Dialog for saving experiment settings to JSON file
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.WinForms.Forms.Experiments.frmSaveExperimentSettings.Core;
using SallamPathFinder4.WinForms.Forms.Shared;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmSaveExperimentSettings
{
    /// <summary>
    /// Dialog form for saving experiment settings to a JSON configuration file
    /// </summary>
    public sealed partial class frmSaveExperimentSettings : Form
    {
        #region Private Fields
        private readonly SaveExperimentSettingsLogic _logic;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the save experiment settings dialog
        /// </summary>
        /// <param name="settings">Current experiment settings to save</param>
        public frmSaveExperimentSettings(ExperimentSettings settings)
        {
            InitializeComponent();
            _logic = new SaveExperimentSettingsLogic(settings);
            LoadSettings();
            WireEvents();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Loads the current settings into the UI controls
        /// </summary>
        private void LoadSettings()
        {
            _txtFileName.Text = _logic.GetDefaultFileName();
            _txtDescription.Text = string.Empty;
        }

        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            _btnSave.Click += BtnSave_Click;
            _btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles save button click - validates input and saves settings to file
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            string fileName = _txtFileName.Text.Trim();

            if (!_logic.ValidateFileName(fileName))
            {
                MessageBox.Show(
                    "Please enter a valid file name.\n\nFile name cannot be empty and cannot contain invalid characters.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string description = _txtDescription.Text.Trim();

            if (_logic.SaveToFile(fileName, description))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(
                    "Error saving settings file.\n\nPlease check if you have write permissions.",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}