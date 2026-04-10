#region File Header
/// <summary>
/// File: frmObstacleSettings.cs
/// Description: Form for configuring dynamic obstacle settings
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.WinForms.ViewModels;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Settings.frmObstacleSettings
{
    /// <summary>
    /// Form for configuring dynamic obstacle settings
    /// </summary>
    public sealed partial class frmObstacleSettings : Form
    {
        #region Private Fields
        private readonly ObstacleSettingsViewModel _viewModel;
        private readonly ObstacleSettingsLogic _logic;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the obstacle settings form
        /// </summary>
        public frmObstacleSettings(ObstacleSettingsViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _logic = new ObstacleSettingsLogic();

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
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
            btnReset.Click += BtnReset_Click;

            trbDirectionChange.Scroll += (s, e) => UpdateDirectionLabel();
            trbMaxTurnAngle.Scroll += (s, e) => UpdateTurnLabel();
        }

        /// <summary>
        /// Loads settings into UI controls
        /// </summary>
        private void LoadSettings()
        {
            var settings = _viewModel.GetSettings();

            // Load type settings
            _dgvTypeSettings.Rows.Clear();
            foreach (ObstacleType type in Enum.GetValues(typeof(ObstacleType)))
            {
                var typeSettings = settings.TypeSettings[type];
                _dgvTypeSettings.Rows.Add(type.ToString(), typeSettings.Speed, typeSettings.MovementRandomness,
                    typeSettings.Radius, typeSettings.Weight);
            }

            // Load general settings
            _chkRandomMovement.Checked = settings.EnableRandomMovement;
            _chkFollowWaypoints.Checked = settings.FollowWaypoints;
            _chkAvoidRobot.Checked = settings.AvoidRobot;
            _chkAttractToRobot.Checked = settings.AttractToRobot;
            trbDirectionChange.Value = (int)(settings.DirectionChangeProbability * 100);
            trbMaxTurnAngle.Value = (int)settings.MaxTurnAngle;
            cboCollisionResponse.SelectedIndex = (int)settings.CollisionResponse;
            nudInitialCount.Value = settings.InitialObstacleCount;
            nudMaxCount.Value = settings.MaxObstacleCount;
            chkDynamicSpawning.Checked = settings.DynamicSpawning;
            nudSpawnInterval.Value = (decimal)settings.SpawnIntervalSeconds;

            UpdateDirectionLabel();
            UpdateTurnLabel();
        }

        private void UpdateDirectionLabel()
        {
            lblDirectionValue.Text = $"{trbDirectionChange.Value}%";
        }

        private void UpdateTurnLabel()
        {
            lblTurnValue.Text = $"{trbMaxTurnAngle.Value}°";
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles save button click
        /// </summary>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            var settings = new ObstacleSettings();

            // Save type settings
            foreach (DataGridViewRow row in _dgvTypeSettings.Rows)
            {
                if (row.Cells[0].Value == null) continue;

                var type = (ObstacleType)Enum.Parse(typeof(ObstacleType), row.Cells[0].Value.ToString());
                settings.TypeSettings[type] = new ObstacleTypeSettings
                {
                    Speed = Convert.ToDouble(row.Cells[1].Value),
                    MovementRandomness = Convert.ToDouble(row.Cells[2].Value),
                    Radius = Convert.ToDouble(row.Cells[3].Value),
                    Weight = Convert.ToDouble(row.Cells[4].Value)
                };
            }

            // Save general settings
            settings.EnableRandomMovement = _chkRandomMovement.Checked;
            settings.FollowWaypoints = _chkFollowWaypoints.Checked;
            settings.AvoidRobot = _chkAvoidRobot.Checked;
            settings.AttractToRobot = _chkAttractToRobot.Checked;
            settings.DirectionChangeProbability = trbDirectionChange.Value / 100.0;
            settings.MaxTurnAngle = trbMaxTurnAngle.Value;
            settings.CollisionResponse = (CollisionResponseType)cboCollisionResponse.SelectedIndex;
            settings.InitialObstacleCount = (int)nudInitialCount.Value;
            settings.MaxObstacleCount = (int)nudMaxCount.Value;
            settings.DynamicSpawning = chkDynamicSpawning.Checked;
            settings.SpawnIntervalSeconds = (double)nudSpawnInterval.Value;

            _viewModel.SaveSettings(settings);
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Handles reset button click
        /// </summary>
        private void BtnReset_Click(object sender, EventArgs e)
        {
            _viewModel.ResetToDefaults();
            LoadSettings();
        }
        #endregion
    }
}