#region File Header
/// <summary>
/// File: AlgorithmSettingsPanel.cs
/// Description: Advanced algorithm settings panel with algorithm-specific configurations
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-13
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.WinForms.Controls;
#endregion

namespace SallamPathFinder4.WinForms.Panels
{
    #region Class Documentation
    /// <summary>
    /// Panel for configuring algorithm settings
    /// Supports common settings and algorithm-specific parameters
    /// </summary>
    #endregion
    public sealed class AlgorithmSettingsPanel : Panel
    {
        #region Constants
        private const int RIGHT_PANEL_WIDTH = 380;
        private const int SETTINGS_CONTAINER_WIDTH = 310;
        private const int SETTINGS_CONTAINER_HEIGHT = 420;
        private const int BUTTON_PANEL_HEIGHT = 50;
        private const int CONTROL_WIDTH = 120;
        private const int LABEL_WIDTH = 100;
        private const int ROW_HEIGHT = 30;
        private const int GROUP_HEADER_HEIGHT = 25;
        private const int DEFAULT_HEURISTIC_WEIGHT = 2;
        private const int DEFAULT_SEARCH_LIMIT = 50000;
        private const int DEFAULT_ACO_ANTS = 20;
        private const int DEFAULT_ACO_ITERATIONS = 100;
        private const int DEFAULT_KNN_NEIGHBORS = 3;
        private const int DEFAULT_KNN_RADIUS = 5;
        private const int DEFAULT_DSTAR_RANGE = 5;
        private const int DEFAULT_BF_DEPTH = 5000;
        private const int DEFAULT_BF_ITERATIONS = 100000;
        private const double DEFAULT_ACO_EVAPORATION = 0.1;
        private const double DEFAULT_ACO_ALPHA = 1.0;
        private const double DEFAULT_ACO_BETA = 2.0; 
        #endregion

        #region Private Fields - Common Settings
        private ComboBox _cboAlgorithmType;
        private Panel _pnlSettingsContainer;
        private NumericUpDown _nudHeuristicWeight;
        private NumericUpDown _nudSearchLimit;
        private CheckBox _chkAllowDiagonals;
        private CheckBox _chkHeavyDiagonals;
        private ComboBox _cboDistanceMetric;
        private Label _lblDistanceMetric;
        private Label _lblMetricDescription;
        #endregion

        #region Private Fields - ACO Settings
        private NumericUpDown _nudACOAnts;
        private NumericUpDown _nudACOEvaporation;
        private NumericUpDown _nudACOAlpha;
        private NumericUpDown _nudACOBeta;
        private NumericUpDown _nudACOIterations;
        #endregion

        #region Private Fields - KNN Settings
        private NumericUpDown _nudKNNK;
        private NumericUpDown _nudKNNRadius;
        #endregion

        #region Private Fields - D* Settings
        private CheckBox _chkDStarDynamic;
        private NumericUpDown _nudDStarRange;
        #endregion

        #region Private Fields - Brute Force Settings
        private NumericUpDown _nudBFDepth;
        private NumericUpDown _nudBFIterations;
        #endregion

        #region Private Fields - Action Buttons
        private Button _btnFindPath;
        private Button _btnStartSimulation;
        private Button _btnStopSimulation;
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
        public DistanceMetric SelectedMetric
        {
            get
            {
                return _cboDistanceMetric?.SelectedIndex switch
                {
                    0 => DistanceMetric.Manhattan,
                    1 => DistanceMetric.Euclidean,
                    2 => DistanceMetric.MaxDXDY,
                    3 => DistanceMetric.DiagonalShortcut,
                    4 => DistanceMetric.EuclideanNoSQR,
                    5 => DistanceMetric.Custom,
                    _ => DistanceMetric.Manhattan
                };
            }
        }

        /// <summary>
        /// Sets the selected algorithm by index (0-6)
        /// </summary>
        public void SetAlgorithmByIndex(int index)
        {
            if (_cboAlgorithmType != null && index >= 0 && index < _cboAlgorithmType.Items.Count)
            {
                _cboAlgorithmType.SelectedIndex = index;
            }
        }
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
            this.Width = RIGHT_PANEL_WIDTH;

            int y = 5;

            CreateAlgorithmSelection(ref y);
            CreateSettingsContainer(ref y);
            CreateCommonSettings();
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
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboAlgorithmType.Items.AddRange(new string[]
            {
                "A*", "SPPA", "SPPA-DL", "ACO", "D*", "KNN", "Brute Force"
            });
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
                Size = new Size(SETTINGS_CONTAINER_WIDTH, SETTINGS_CONTAINER_HEIGHT),
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
                Text = "═══════════ COMMON SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            _pnlSettingsContainer.Controls.Add(lblCommon);
            y += 25;

            // Heuristic Weight
            var lblHeuristic = new Label
            {
                Text = "Heuristic Weight:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudHeuristicWeight = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 10,
                Value = DEFAULT_HEURISTIC_WEIGHT,
                DecimalPlaces = 1
            };
            _pnlSettingsContainer.Controls.Add(lblHeuristic);
            _pnlSettingsContainer.Controls.Add(_nudHeuristicWeight);
            y += ROW_HEIGHT;

            // Search Limit
            var lblSearchLimit = new Label
            {
                Text = "Search Limit:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudSearchLimit = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 100,
                Minimum = 1000,
                Maximum = 1000000,
                Value = DEFAULT_SEARCH_LIMIT
            };
            _pnlSettingsContainer.Controls.Add(lblSearchLimit);
            _pnlSettingsContainer.Controls.Add(_nudSearchLimit);
            y += ROW_HEIGHT;

            // Allow Diagonals
            _chkAllowDiagonals = new CheckBox
            {
                Text = "Allow Diagonal Movement",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkAllowDiagonals);
            y += ROW_HEIGHT - 5;

            // Heavy Diagonals
            _chkHeavyDiagonals = new CheckBox
            {
                Text = "Heavy Diagonals (Higher Cost)",
                Location = new Point(10, y),
                AutoSize = true
            };
            _pnlSettingsContainer.Controls.Add(_chkHeavyDiagonals);
            y += ROW_HEIGHT;

            // Distance Metric Label
            _lblDistanceMetric = new Label
            {
                Text = "Distance Metric:",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _pnlSettingsContainer.Controls.Add(_lblDistanceMetric);

            // Distance Metric ComboBox
            _cboDistanceMetric = new ComboBox
            {
                Location = new Point(130, y - 3),
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboDistanceMetric.Items.AddRange(new string[]
            {
                "Manhattan (|dx| + |dy|)",
                "Euclidean (√(dx² + dy²))",
                "MaxDXDY (max(|dx|, |dy|))",
                "DiagonalShortcut (2·min(dx,dy) + |dx-dy|)",
                "EuclideanNoSQR (dx² + dy²)",
                "Custom"
            });
            _cboDistanceMetric.SelectedIndex = 0;
            _cboDistanceMetric.SelectedIndexChanged += (s, e) =>
            {
                UpdateMetricDescription();
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            };
            _pnlSettingsContainer.Controls.Add(_cboDistanceMetric);
            y += ROW_HEIGHT - 5;

            // Metric Description
            _lblMetricDescription = new Label
            {
                Text = "Best for 4-directional movement (no diagonals)",
                Location = new Point(130, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(_lblMetricDescription);
            y += ROW_HEIGHT;

            RegisterCommonEvents();
        }

        private void RegisterCommonEvents()
        {
            _nudHeuristicWeight.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudSearchLimit.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkAllowDiagonals.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkHeavyDiagonals.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateMetricDescription()
        {
            string description = _cboDistanceMetric?.SelectedIndex switch
            {
                0 => "Best for 4-directional movement (no diagonals)",
                1 => "Best for 8-directional movement with free diagonals",
                2 => "Best for 8-directional movement when diagonal cost = straight",
                3 => "Optimized for 8-directional movement with diagonal cost = 2",
                4 => "Faster computation (no sqrt), may overestimate",
                5 => "Custom heuristic - can be overridden",
                _ => "Select a distance metric"
            };

            if (_lblMetricDescription != null)
            {
                _lblMetricDescription.Text = description;
            }
        }
        #endregion

        #region Algorithm-Specific Settings Creation
        private void CreateACOSettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            var lblTitle = new Label
            {
                Text = "═══════════ ACO SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219)
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;

            var lblAnts = new Label { Text = "Number of Ants:", Location = new Point(10, y), AutoSize = true };
            _nudACOAnts = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 200,
                Value = DEFAULT_ACO_ANTS
            };
            _pnlSettingsContainer.Controls.Add(lblAnts);
            _pnlSettingsContainer.Controls.Add(_nudACOAnts);
            y += ROW_HEIGHT;

            var lblEvaporation = new Label { Text = "Evaporation Rate (0-1):", Location = new Point(10, y), AutoSize = true };
            _nudACOEvaporation = new NumericUpDown
            {
                Location = new Point(160, y - 3),
                Width = 80,
                Minimum = 0,
                Maximum = 100,
                Value = (decimal)(DEFAULT_ACO_EVAPORATION * 100),
                DecimalPlaces = 2
            };
            _pnlSettingsContainer.Controls.Add(lblEvaporation);
            _pnlSettingsContainer.Controls.Add(_nudACOEvaporation);
            y += ROW_HEIGHT;

            var lblAlpha = new Label { Text = "Alpha (α):", Location = new Point(10, y), AutoSize = true };
            _nudACOAlpha = new NumericUpDown
            {
                Location = new Point(80, y - 3),
                Width = 70,
                Minimum = 0,
                Maximum = 50,
                Value = (decimal)DEFAULT_ACO_ALPHA,
                DecimalPlaces = 1
            };
            var lblBeta = new Label { Text = "Beta (β):", Location = new Point(165, y), AutoSize = true };
            _nudACOBeta = new NumericUpDown
            {
                Location = new Point(215, y - 3),
                Width = 70,
                Minimum = 0,
                Maximum = 50,
                Value = (decimal)DEFAULT_ACO_BETA,
                DecimalPlaces = 1
            };
            _pnlSettingsContainer.Controls.Add(lblAlpha);
            _pnlSettingsContainer.Controls.Add(_nudACOAlpha);
            _pnlSettingsContainer.Controls.Add(lblBeta);
            _pnlSettingsContainer.Controls.Add(_nudACOBeta);
            y += ROW_HEIGHT;

            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(10, y), AutoSize = true };
            _nudACOIterations = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 10,
                Maximum = 500,
                Value = DEFAULT_ACO_ITERATIONS
            };
            _pnlSettingsContainer.Controls.Add(lblIter);
            _pnlSettingsContainer.Controls.Add(_nudACOIterations);

            RegisterACOEvents();
        }

        private void CreateKNNSettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            var lblTitle = new Label
            {
                Text = "═══════════ KNN SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;

            var lblK = new Label { Text = "K Neighbors:", Location = new Point(10, y), AutoSize = true };
            _nudKNNK = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 20,
                Value = DEFAULT_KNN_NEIGHBORS
            };
            _pnlSettingsContainer.Controls.Add(lblK);
            _pnlSettingsContainer.Controls.Add(_nudKNNK);
            y += ROW_HEIGHT;

            var lblRadius = new Label { Text = "Search Radius (cells):", Location = new Point(10, y), AutoSize = true };
            _nudKNNRadius = new NumericUpDown
            {
                Location = new Point(150, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 20,
                Value = DEFAULT_KNN_RADIUS
            };
            _pnlSettingsContainer.Controls.Add(lblRadius);
            _pnlSettingsContainer.Controls.Add(_nudKNNRadius);

            RegisterKNNEvents();
        }

        private void CreateDStarSettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            var lblTitle = new Label
            {
                Text = "═══════════ D* SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 196, 15)
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;

            _chkDStarDynamic = new CheckBox
            {
                Text = "Enable Dynamic Replanning",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkDStarDynamic);
            y += ROW_HEIGHT - 5;

            var lblRange = new Label { Text = "Replanning Range (cells):", Location = new Point(10, y), AutoSize = true };
            _nudDStarRange = new NumericUpDown
            {
                Location = new Point(170, y - 3),
                Width = 60,
                Minimum = 1,
                Maximum = 50,
                Value = DEFAULT_DSTAR_RANGE
            };
            _pnlSettingsContainer.Controls.Add(lblRange);
            _pnlSettingsContainer.Controls.Add(_nudDStarRange);

            RegisterDStarEvents();
        }

        private void CreateBruteForceSettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            var lblTitle = new Label
            {
                Text = "═══════════ BRUTE FORCE SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60)
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;

            var lblDepth = new Label { Text = "Max Search Depth:", Location = new Point(10, y), AutoSize = true };
            _nudBFDepth = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 100,
                Minimum = 100,
                Maximum = 50000,
                Value = DEFAULT_BF_DEPTH
            };
            _pnlSettingsContainer.Controls.Add(lblDepth);
            _pnlSettingsContainer.Controls.Add(_nudBFDepth);
            y += ROW_HEIGHT;

            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(10, y), AutoSize = true };
            _nudBFIterations = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 100,
                Minimum = 1000,
                Maximum = 500000,
                Value = DEFAULT_BF_ITERATIONS
            };
            _pnlSettingsContainer.Controls.Add(lblIter);
            _pnlSettingsContainer.Controls.Add(_nudBFIterations);

            RegisterBruteForceEvents();
        }

        private int GetCommonSettingsBottom()
        {
            int bottom = 0;
            foreach (Control control in _pnlSettingsContainer.Controls)
            {
                if (control == _lblMetricDescription)
                {
                    bottom = control.Bottom;
                    break;
                }
            }
            return bottom > 0 ? bottom : 200;
        }
        #endregion

        #region Event Registration
        private void RegisterACOEvents()
        {
            _nudACOAnts.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudACOEvaporation.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudACOAlpha.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudACOBeta.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudACOIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RegisterKNNEvents()
        {
            _nudKNNK.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudKNNRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RegisterDStarEvents()
        {
            _chkDStarDynamic.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudDStarRange.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RegisterBruteForceEvents()
        {
            _nudBFDepth.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudBFIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion 

        #region Panel Management
        private void ClearAlgorithmPanels()
        {
            var controlsToRemove = new List<Control>();

            foreach (Control control in _pnlSettingsContainer.Controls)
            {
                // Keep common settings controls
                if (control == _nudHeuristicWeight || control == _nudSearchLimit)
                    continue;
                if (control == _chkAllowDiagonals || control == _chkHeavyDiagonals)
                    continue;
                if (control == _lblDistanceMetric || control == _cboDistanceMetric)
                    continue;
                if (control == _lblMetricDescription)
                    continue;

                // Keep the common settings label
                if (control is Label lbl && lbl.Text.Contains("COMMON SETTINGS"))
                    continue;

                // Remove algorithm-specific controls
                controlsToRemove.Add(control);
            }

            foreach (var control in controlsToRemove)
            {
                _pnlSettingsContainer.Controls.Remove(control);
                control.Dispose();
            }
        }

        private void ShowSettingsForAlgorithm(AlgorithmType algorithm)
        {
            ClearAlgorithmPanels();

            switch (algorithm)
            {
                case AlgorithmType.ACO:
                    CreateACOSettings();
                    break;
                case AlgorithmType.KNN:
                    CreateKNNSettings();
                    break;
                case AlgorithmType.DStar:
                    CreateDStarSettings();
                    break;
                case AlgorithmType.BruteForce:
                    CreateBruteForceSettings();
                    break;
                default:
                    // A*, SPPA, SPPA-DL have no specific settings
                    break;
            }
        }

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
        #endregion

        #region Action Buttons
        private void CreateActionButtons(ref int y)
        {
            var pnlButtons = new Panel
            {
                Location = new Point(5, y),
                Size = new Size(SETTINGS_CONTAINER_WIDTH, BUTTON_PANEL_HEIGHT),
                BackColor = Color.FromArgb(248, 249, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            _btnFindPath = new Button
            {
                Text = "🔍 Find Path",
                Location = new Point(5, 10),
                Size = new Size(95, 32),
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
                Location = new Point(105, 10),
                Size = new Size(95, 32),
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
                Location = new Point(205, 10),
                Size = new Size(95, 32),
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

        public void SetButtonStates(bool isSimulating, bool isPaused)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetButtonStates(isSimulating, isPaused)));
                return;
            }

            _btnFindPath.Enabled = !isSimulating;
            _btnStartSimulation.Enabled = !isSimulating;
            _btnStopSimulation.Enabled = isSimulating;
        }
        #endregion


    }
}