#region File Header
/// <summary>
/// File: frmLoadExperimentSettings.cs
/// Description: Dialog for loading saved experiment settings from JSON file
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Experiments;
using SallamPathFinder4.WinForms.Forms.Experiments.frmLoadExperimentSettings.Core;
using SallamPathFinder4.WinForms.Forms.Shared;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmLoadExperimentSettings
{
    /// <summary>
    /// Dialog form for loading experiment settings from a JSON configuration file
    /// </summary>
    public sealed partial class frmLoadExperimentSettings : Form
    {
        #region Private Fields
        private readonly LoadExperimentSettingsLogic _logic;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the loaded experiment settings
        /// </summary>
        public ExperimentSettings LoadedSettings { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the load experiment settings dialog
        /// </summary>
        public frmLoadExperimentSettings()
        {
            InitializeComponent();
            _logic = new LoadExperimentSettingsLogic();
            WireEvents();
            LoadSettingsList();
        }
        #endregion

        #region Private Methods - Initialization
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            _lstSettings.SelectedIndexChanged += LstSettings_SelectedIndexChanged;
            _btnLoad.Click += BtnLoad_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            _btnRefresh.Click += (s, e) => LoadSettingsList();
        }

        /// <summary>
        /// Loads the list of saved settings files
        /// </summary>
        private void LoadSettingsList()
        {
            _lstSettings.Items.Clear();
            var files = _logic.GetAllSettingsFiles();

            foreach (var file in files)
            {
                string displayText = $"{file.Name} ({file.SaveDate:yyyy-MM-dd HH:mm})";
                _lstSettings.Items.Add(displayText);
            }

            _lblStatus.Text = $"{files.Count} setting file(s) found";
            _btnLoad.Enabled = false;
            _btnDelete.Enabled = false;
            _txtPreview.Clear();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles selection change in the settings list
        /// </summary>
        private void LstSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = _lstSettings.SelectedIndex;
            bool hasSelection = index >= 0;

            _btnLoad.Enabled = hasSelection;
            _btnDelete.Enabled = hasSelection;

            if (hasSelection)
            {
                var file = _logic.GetSettingsFiles()[index];
                _txtPreview.Text = _logic.GetPreviewText(file);
            }
            else
            {
                _txtPreview.Clear();
            }
        }

        /// <summary>
        /// Handles load button click - loads selected settings
        /// </summary>
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            int index = _lstSettings.SelectedIndex;
            if (index < 0) return;

            var file = _logic.GetSettingsFiles()[index];
            LoadedSettings = _logic.LoadFromFile(file.FilePath);

            if (LoadedSettings != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(
                    "Error loading settings file.\n\nThe file may be corrupted or in an invalid format.",
                    "Load Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles delete button click - deletes selected settings file
        /// </summary>
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            int index = _lstSettings.SelectedIndex;
            if (index < 0) return;

            var file = _logic.GetSettingsFiles()[index];

            var result = MessageBox.Show(
                $"Delete '{file.Name}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                if (_logic.DeleteFile(file.FilePath))
                {
                    LoadSettingsList();
                }
                else
                {
                    MessageBox.Show(
                        "Error deleting file.\n\nPlease check file permissions.",
                        "Delete Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}