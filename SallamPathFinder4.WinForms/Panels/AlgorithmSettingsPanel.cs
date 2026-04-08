#region File Header
/// <summary>
/// File: AlgorithmSettingsPanel.cs
/// Description: Advanced algorithm settings panel with algorithm-specific configurations
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Windows.Forms;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.WinForms.Panels
{
    public sealed class AlgorithmSettingsPanel : Panel
    {
        #region Private Fields
        private ComboBox _cboAlgorithmType;
        private Panel _pnlSettingsContainer;
        private NumericUpDown _nudHeuristicWeight;
        private NumericUpDown _nudSearchLimit;
        private CheckBox _chkAllowDiagonals;
        private CheckBox _chkHeavyDiagonals;
        private NumericUpDown _nudACOAnts;
        private NumericUpDown _nudACOEvaporation;
        private NumericUpDown _nudACOAlpha;
        private NumericUpDown _nudACOBeta;
        private NumericUpDown _nudACOIterations;
        private NumericUpDown _nudKNNK;
        private NumericUpDown _nudKNNRadius;
        private NumericUpDown _nudDStarRange;
        private CheckBox _chkDStarDynamic;
        private NumericUpDown _nudBFDepth;
        private NumericUpDown _nudBFIterations;
        private Button _btnFindPath;
        private Button _btnStartSimulation;
        private Button _btnStopSimulation;
        #endregion

        #region Constants
        private const int SETTINGS_CONTAINER_HEIGHT = 420;
        private const int BUTTON_PANEL_HEIGHT = 45;
        #endregion

        #region Events
        public event EventHandler SettingsChanged;
        public event EventHandler FindPathRequested;
        public event EventHandler StartSimulationRequested;
        public event EventHandler StopSimulationRequested;
        #endregion

        #region Properties
        public AlgorithmType CurrentAlgorithm { get; private set; } = AlgorithmType.AStar;
        public int HeuristicWeight => (int)_nudHeuristicWeight.Value;
        public int SearchLimit => (int)_nudSearchLimit.Value;
        public bool AllowDiagonals => _chkAllowDiagonals.Checked;
        public bool HeavyDiagonals => _chkHeavyDiagonals.Checked;
        #endregion

        #region Constructor
        public AlgorithmSettingsPanel()
        {
            InitializeComponents();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(5);
            this.BackColor = Color.White;
            this.AutoScroll = true;

            int y = 5;

            CreateAlgorithmSelection(ref y);
            CreateSettingsContainer(ref y);
            CreateCommonSettings();
            CreateACOSettings();
            CreateKNNSettings();
            CreateDStarSettings();
            CreateBruteForceSettings();
            CreateActionButtons(ref y);

            ShowSettingsForAlgorithm(AlgorithmType.AStar);
        }

        private void CreateAlgorithmSelection(ref int y)
        {
            var lblAlgorithm = new Label
            {
                Text = "Select Algorithm:",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            _cboAlgorithmType = new ComboBox
            {
                Location = new Point(120, y - 3),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboAlgorithmType.Items.AddRange(new string[] { "A*", "SPPA", "SPPA-DL", "ACO", "D*", "KNN", "Brute Force" });
            _cboAlgorithmType.SelectedIndex = 0;
            _cboAlgorithmType.SelectedIndexChanged += CboAlgorithmType_SelectedIndexChanged;

            this.Controls.Add(lblAlgorithm);
            this.Controls.Add(_cboAlgorithmType);
            y += 35;
        }

        private void CreateSettingsContainer(ref int y)
        {
            _pnlSettingsContainer = new Panel
            {
                Location = new Point(5, y),
                Size = new Size(330, SETTINGS_CONTAINER_HEIGHT),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(_pnlSettingsContainer);
            y += SETTINGS_CONTAINER_HEIGHT + 10;
        }

        private void CreateCommonSettings()
        {
            int y = 5;

            var lblCommon = new Label
            {
                Text = "═══════════ Common Settings ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            _pnlSettingsContainer.Controls.Add(lblCommon);
            y += 25;

            var lblHeuristic = new Label { Text = "Heuristic Weight:", Location = new Point(10, y), AutoSize = true };
            _nudHeuristicWeight = new NumericUpDown { Location = new Point(130, y - 3), Width = 80, Minimum = 1, Maximum = 10, Value = 2, DecimalPlaces = 1 };
            _pnlSettingsContainer.Controls.Add(lblHeuristic);
            _pnlSettingsContainer.Controls.Add(_nudHeuristicWeight);
            y += 30;

            var lblSearchLimit = new Label { Text = "Search Limit:", Location = new Point(10, y), AutoSize = true };
            _nudSearchLimit = new NumericUpDown { Location = new Point(130, y - 3), Width = 100, Minimum = 1000, Maximum = 1000000, Value = 50000 };
            _pnlSettingsContainer.Controls.Add(lblSearchLimit);
            _pnlSettingsContainer.Controls.Add(_nudSearchLimit);
            y += 30;

            _chkAllowDiagonals = new CheckBox { Text = "Allow Diagonal Movement", Location = new Point(10, y), AutoSize = true, Checked = true };
            _pnlSettingsContainer.Controls.Add(_chkAllowDiagonals);
            y += 25;

            _chkHeavyDiagonals = new CheckBox { Text = "Heavy Diagonals (Higher Cost)", Location = new Point(10, y), AutoSize = true };
            _pnlSettingsContainer.Controls.Add(_chkHeavyDiagonals);
            y += 35;

            RegisterCommonEvents();
        }

        private void RegisterCommonEvents()
        {
            _nudHeuristicWeight.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudSearchLimit.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkAllowDiagonals.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkHeavyDiagonals.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CreateACOSettings()
        {
            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ ACO Specific Settings ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219)
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += 22;

            var lblAnts = new Label { Text = "Number of Ants:", Location = new Point(10, y), AutoSize = true };
            _nudACOAnts = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 1, Maximum = 200, Value = 20 };
            _pnlSettingsContainer.Controls.Add(lblAnts);
            _pnlSettingsContainer.Controls.Add(_nudACOAnts);
            y += 28;

            var lblEvaporation = new Label { Text = "Evaporation Rate (0-1):", Location = new Point(10, y), AutoSize = true };
            _nudACOEvaporation = new NumericUpDown { Location = new Point(160, y - 3), Width = 80, Minimum = 0, Maximum = 100, Value = 10, DecimalPlaces = 2 };
            _pnlSettingsContainer.Controls.Add(lblEvaporation);
            _pnlSettingsContainer.Controls.Add(_nudACOEvaporation);
            y += 28;

            var lblAlpha = new Label { Text = "Alpha (α):", Location = new Point(10, y), AutoSize = true };
            _nudACOAlpha = new NumericUpDown { Location = new Point(80, y - 3), Width = 70, Minimum = 0, Maximum = 50, Value = 10, DecimalPlaces = 1 };
            var lblBeta = new Label { Text = "Beta (β):", Location = new Point(165, y), AutoSize = true };
            _nudACOBeta = new NumericUpDown { Location = new Point(215, y - 3), Width = 70, Minimum = 0, Maximum = 50, Value = 20, DecimalPlaces = 1 };
            _pnlSettingsContainer.Controls.Add(lblAlpha);
            _pnlSettingsContainer.Controls.Add(_nudACOAlpha);
            _pnlSettingsContainer.Controls.Add(lblBeta);
            _pnlSettingsContainer.Controls.Add(_nudACOBeta);
            y += 28;

            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(10, y), AutoSize = true };
            _nudACOIterations = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 10, Maximum = 500, Value = 100 };
            _pnlSettingsContainer.Controls.Add(lblIter);
            _pnlSettingsContainer.Controls.Add(_nudACOIterations);

            RegisterACOEvents();
            SetACOVisibility(false);
        }

        private void RegisterACOEvents()
        {
            _nudACOAnts.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudACOEvaporation.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudACOAlpha.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudACOBeta.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudACOIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CreateKNNSettings()
        {
            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ KNN Specific Settings ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += 22;

            var lblK = new Label { Text = "K Neighbors:", Location = new Point(10, y), AutoSize = true };
            _nudKNNK = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 1, Maximum = 20, Value = 3 };
            _pnlSettingsContainer.Controls.Add(lblK);
            _pnlSettingsContainer.Controls.Add(_nudKNNK);
            y += 28;

            var lblRadius = new Label { Text = "Search Radius (cells):", Location = new Point(10, y), AutoSize = true };
            _nudKNNRadius = new NumericUpDown { Location = new Point(150, y - 3), Width = 80, Minimum = 1, Maximum = 20, Value = 5 };
            _pnlSettingsContainer.Controls.Add(lblRadius);
            _pnlSettingsContainer.Controls.Add(_nudKNNRadius);

            RegisterKNNEvents();
            SetKNNVisibility(false);
        }

        private void RegisterKNNEvents()
        {
            _nudKNNK.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudKNNRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CreateDStarSettings()
        {
            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ D* Specific Settings ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 196, 15)
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += 22;

            _chkDStarDynamic = new CheckBox { Text = "Enable Dynamic Replanning", Location = new Point(10, y), AutoSize = true, Checked = true };
            _pnlSettingsContainer.Controls.Add(_chkDStarDynamic);
            y += 25;

            var lblRange = new Label { Text = "Replanning Range (cells):", Location = new Point(10, y), AutoSize = true };
            _nudDStarRange = new NumericUpDown { Location = new Point(170, y - 3), Width = 60, Minimum = 1, Maximum = 50, Value = 5 };
            _pnlSettingsContainer.Controls.Add(lblRange);
            _pnlSettingsContainer.Controls.Add(_nudDStarRange);

            RegisterDStarEvents();
            SetDStarVisibility(false);
        }

        private void RegisterDStarEvents()
        {
            _chkDStarDynamic.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudDStarRange.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CreateBruteForceSettings()
        {
            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ Brute Force Specific Settings ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60)
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += 22;

            var lblDepth = new Label { Text = "Max Search Depth:", Location = new Point(10, y), AutoSize = true };
            _nudBFDepth = new NumericUpDown { Location = new Point(130, y - 3), Width = 100, Minimum = 100, Maximum = 50000, Value = 5000 };
            _pnlSettingsContainer.Controls.Add(lblDepth);
            _pnlSettingsContainer.Controls.Add(_nudBFDepth);
            y += 28;

            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(10, y), AutoSize = true };
            _nudBFIterations = new NumericUpDown { Location = new Point(120, y - 3), Width = 100, Minimum = 1000, Maximum = 500000, Value = 100000 };
            _pnlSettingsContainer.Controls.Add(lblIter);
            _pnlSettingsContainer.Controls.Add(_nudBFIterations);

            RegisterBruteForceEvents();
            SetBruteForceVisibility(false);
        }

        private void RegisterBruteForceEvents()
        {
            _nudBFDepth.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudBFIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CreateActionButtons(ref int y)
        {
            var pnlButtons = new Panel
            {
                Location = new Point(5, y),
                Size = new Size(330, BUTTON_PANEL_HEIGHT),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            _btnFindPath = new Button
            {
                Text = "🔍 Find Path",
                Location = new Point(5, 8),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            _btnFindPath.Click += (s, e) => FindPathRequested?.Invoke(this, EventArgs.Empty);

            _btnStartSimulation = new Button
            {
                Text = "▶ Start",
                Location = new Point(110, 8),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            _btnStartSimulation.Click += (s, e) => StartSimulationRequested?.Invoke(this, EventArgs.Empty);

            _btnStopSimulation = new Button
            {
                Text = "⏹ Stop",
                Location = new Point(215, 8),
                Size = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            _btnStopSimulation.Click += (s, e) => StopSimulationRequested?.Invoke(this, EventArgs.Empty);

            pnlButtons.Controls.Add(_btnFindPath);
            pnlButtons.Controls.Add(_btnStartSimulation);
            pnlButtons.Controls.Add(_btnStopSimulation);

            this.Controls.Add(pnlButtons);
        }
        #endregion

        #region Visibility Management
        private void CboAlgorithmType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = _cboAlgorithmType.SelectedItem.ToString();
            CurrentAlgorithm = selected switch
            {
                "A*" => AlgorithmType.AStar,
                "SPPA" => AlgorithmType.SPPA,
                "SPPA-DL" => AlgorithmType.SPPA_DL,
                "ACO" => AlgorithmType.ACO,
                "D*" => AlgorithmType.DStar,
                "KNN" => AlgorithmType.KNN,
                "Brute Force" => AlgorithmType.BruteForce,
                _ => AlgorithmType.AStar
            };

            ShowSettingsForAlgorithm(CurrentAlgorithm);
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ShowSettingsForAlgorithm(AlgorithmType algorithm)
        {
            SetACOVisibility(false);
            SetKNNVisibility(false);
            SetDStarVisibility(false);
            SetBruteForceVisibility(false);

            switch (algorithm)
            {
                case AlgorithmType.ACO:
                    SetACOVisibility(true);
                    break;
                case AlgorithmType.KNN:
                    SetKNNVisibility(true);
                    break;
                case AlgorithmType.DStar:
                    SetDStarVisibility(true);
                    break;
                case AlgorithmType.BruteForce:
                    SetBruteForceVisibility(true);
                    break;
            }
        }

        private void SetACOVisibility(bool visible)
        {
            foreach (Control ctrl in _pnlSettingsContainer.Controls)
            {
                if (ctrl is Label lbl && lbl.Text.Contains("ACO"))
                    lbl.Visible = visible;
                if (ctrl == _nudACOAnts || ctrl == _nudACOEvaporation || ctrl == _nudACOAlpha || ctrl == _nudACOBeta || ctrl == _nudACOIterations)
                    ctrl.Visible = visible;
                if (ctrl is Label lbl2 && (lbl2.Text == "Number of Ants:" || lbl2.Text == "Evaporation Rate (0-1):" ||
                    lbl2.Text == "Alpha (α):" || lbl2.Text == "Beta (β):" || lbl2.Text == "Max Iterations:"))
                    lbl2.Visible = visible;
            }
        }

        private void SetKNNVisibility(bool visible)
        {
            foreach (Control ctrl in _pnlSettingsContainer.Controls)
            {
                if (ctrl is Label lbl && lbl.Text.Contains("KNN"))
                    lbl.Visible = visible;
                if (ctrl == _nudKNNK || ctrl == _nudKNNRadius)
                    ctrl.Visible = visible;
                if (ctrl is Label lbl2 && (lbl2.Text == "K Neighbors:" || lbl2.Text == "Search Radius (cells):"))
                    lbl2.Visible = visible;
            }
        }

        private void SetDStarVisibility(bool visible)
        {
            foreach (Control ctrl in _pnlSettingsContainer.Controls)
            {
                if (ctrl is Label lbl && lbl.Text.Contains("D*"))
                    lbl.Visible = visible;
                if (ctrl == _chkDStarDynamic || ctrl == _nudDStarRange)
                    ctrl.Visible = visible;
                if (ctrl is Label lbl2 && lbl2.Text == "Replanning Range (cells):")
                    lbl2.Visible = visible;
            }
        }

        private void SetBruteForceVisibility(bool visible)
        {
            foreach (Control ctrl in _pnlSettingsContainer.Controls)
            {
                if (ctrl is Label lbl && lbl.Text.Contains("Brute Force"))
                    lbl.Visible = visible;
                if (ctrl == _nudBFDepth || ctrl == _nudBFIterations)
                    ctrl.Visible = visible;
                if (ctrl is Label lbl2 && (lbl2.Text == "Max Search Depth:" || lbl2.Text == "Max Iterations:"))
                    lbl2.Visible = visible;
            }
        }
        #endregion

        #region Public Methods
        public void SetButtonStates(bool isSimulating, bool isPaused)
        {
            _btnFindPath.Enabled = !isSimulating;
            _btnStartSimulation.Enabled = !isSimulating;
            _btnStopSimulation.Enabled = isSimulating;
        }
        #endregion
    }
}