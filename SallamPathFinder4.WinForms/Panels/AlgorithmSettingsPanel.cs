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

        #region Private Fields - RRT Settings
        private NumericUpDown _nudRRTIterations;
        private NumericUpDown _nudRRTStepSize;
        private NumericUpDown _nudRRTGoalBias;
        private CheckBox _chkRRTStar;
        private CheckBox _chkRRTSmooth;
        private CheckBox _chkRRTBidirectional;
        #endregion

        #region Private Fields - PRM Settings
        private NumericUpDown _nudPRMSamples;
        private NumericUpDown _nudPRMConnectionRadius;
        private NumericUpDown _nudPRMMaxNeighbors;
        private NumericUpDown _nudPRMSampleBias;
        private CheckBox _chkPRMUseKDTree;
        private CheckBox _chkPRMLazyCollision;
        #endregion

        #region Private Fields - Action Buttons
        private Button _btnFindPath;
        private Button _btnStartSimulation;
        private Button _btnStopSimulation;
        #endregion

        #region Private Fields - PSO Settings
        private NumericUpDown _nudPSOPopulation;
        private NumericUpDown _nudPSOMaxIterations;
        private NumericUpDown _nudPSOInertia;
        private NumericUpDown _nudPSOCognitive;
        private NumericUpDown _nudPSOSocial;
        private NumericUpDown _nudPSOPathSegments;
        private CheckBox _chkPSOAdaptiveInertia;
        private CheckBox _chkPSOElitism;
        #endregion

        #region Private Fields - GA Settings
        private NumericUpDown _nudGAPopulation;
        private NumericUpDown _nudGAGenerations;
        private NumericUpDown _nudGACrossoverRate;
        private NumericUpDown _nudGAMutationRate;
        private NumericUpDown _nudGAEliteRatio;
        private NumericUpDown _nudGATournamentSize;
        private CheckBox _chkGAAdaptiveMutation;
        #endregion

        #region Private Fields - RRT* Settings
        private NumericUpDown _nudRRTStarIterations;
        private NumericUpDown _nudRRTStarStepSize;
        private NumericUpDown _nudRRTStarGoalBias;
        private NumericUpDown _nudRRTStarRewiringRadius;
        private NumericUpDown _nudRRTStarGoalRadius;
        private CheckBox _chkRRTStarInformedSampling;
        private CheckBox _chkRRTStarSmoothPath;
        private CheckBox _chkRRTStarBidirectional;
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
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboAlgorithmType.Items.AddRange(new string[]
            {
        "A*", "SPPA", "SPPA-DL", "ACO", "D*", "KNN", "Brute Force",
        "RRT", "PRM", "PSO", "GA", "RRT*"
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

        private void CreateRRTSettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            var lblTitle = new Label
            {
                Text = "═══════════ RRT SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182)  // Purple color
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;

            // Max Iterations
            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(10, y), AutoSize = true };
            _nudRRTIterations = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 100,
                Minimum = 100,
                Maximum = 50000,
                Value = 5000,
                Increment = 500
            };
            _pnlSettingsContainer.Controls.Add(lblIter);
            _pnlSettingsContainer.Controls.Add(_nudRRTIterations);
            y += ROW_HEIGHT;

            // Step Size
            var lblStep = new Label { Text = "Step Size (cells):", Location = new Point(10, y), AutoSize = true };
            _nudRRTStepSize = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 0.5M,
                Maximum = 10M,
                Value = 1.0M,
                DecimalPlaces = 1,
                Increment = 0.5M
            };
            _pnlSettingsContainer.Controls.Add(lblStep);
            _pnlSettingsContainer.Controls.Add(_nudRRTStepSize);
            y += ROW_HEIGHT;

            // Goal Bias
            var lblBias = new Label { Text = "Goal Bias (0-1):", Location = new Point(10, y), AutoSize = true };
            _nudRRTGoalBias = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 0,
                Maximum = 100,
                Value = 10,
                DecimalPlaces = 0
            };
            var lblBiasPercent = new Label { Text = "%", Location = new Point(215, y - 3), AutoSize = true };
            _pnlSettingsContainer.Controls.Add(lblBias);
            _pnlSettingsContainer.Controls.Add(_nudRRTGoalBias);
            _pnlSettingsContainer.Controls.Add(lblBiasPercent);
            y += ROW_HEIGHT;

            // RRT* Option
            _chkRRTStar = new CheckBox
            {
                Text = "Use RRT* (Optimal Rewiring)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkRRTStar);
            y += ROW_HEIGHT - 5;

            // Path Smoothing
            _chkRRTSmooth = new CheckBox
            {
                Text = "Smooth Path (Remove Redundant Points)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkRRTSmooth);
            y += ROW_HEIGHT - 5;

            // Bidirectional RRT
            _chkRRTBidirectional = new CheckBox
            {
                Text = "Bidirectional RRT (Two Trees)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = false
            };
            _pnlSettingsContainer.Controls.Add(_chkRRTBidirectional);

            RegisterRRTEvents();
        }

        private void RegisterRRTEvents()
        {
            _nudRRTIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStepSize.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTGoalBias.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTStar.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTSmooth.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTBidirectional.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
       
        #region Private Methods - PRM Settings
        private void CreatePRMSettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            var lblTitle = new Label
            {
                Text = "═══════════ PRM SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219)  // Blue color
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;

            // Number of Samples
            var lblSamples = new Label
            {
                Text = "Number of Samples:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPRMSamples = new NumericUpDown
            {
                Location = new Point(160, y - 3),
                Width = 100,
                Minimum = 50,
                Maximum = 5000,
                Value = 500,
                Increment = 50
            };
            _pnlSettingsContainer.Controls.Add(lblSamples);
            _pnlSettingsContainer.Controls.Add(_nudPRMSamples);
            y += ROW_HEIGHT;

            // Connection Radius
            var lblRadius = new Label
            {
                Text = "Connection Radius (cells):",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPRMConnectionRadius = new NumericUpDown
            {
                Location = new Point(180, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 50,
                Value = 10,
                DecimalPlaces = 1,
                Increment = 0.5M
            };
            _pnlSettingsContainer.Controls.Add(lblRadius);
            _pnlSettingsContainer.Controls.Add(_nudPRMConnectionRadius);
            y += ROW_HEIGHT;

            // Max Neighbors
            var lblNeighbors = new Label
            {
                Text = "Max Neighbors:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPRMMaxNeighbors = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 50,
                Value = 15
            };
            var lblNeighborsInfo = new Label
            {
                Text = "(limits connections per node)",
                Location = new Point(205, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblNeighbors);
            _pnlSettingsContainer.Controls.Add(_nudPRMMaxNeighbors);
            _pnlSettingsContainer.Controls.Add(lblNeighborsInfo);
            y += ROW_HEIGHT;

            // Sample Bias
            var lblBias = new Label
            {
                Text = "Sample Bias (near obstacles):",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPRMSampleBias = new NumericUpDown
            {
                Location = new Point(180, y - 3),
                Width = 80,
                Minimum = 0,
                Maximum = 100,
                Value = 10,
                DecimalPlaces = 0
            };
            var lblBiasPercent = new Label
            {
                Text = "%",
                Location = new Point(265, y - 3),
                AutoSize = true
            };
            _pnlSettingsContainer.Controls.Add(lblBias);
            _pnlSettingsContainer.Controls.Add(_nudPRMSampleBias);
            _pnlSettingsContainer.Controls.Add(lblBiasPercent);
            y += ROW_HEIGHT;

            // Use KD-Tree (Performance)
            _chkPRMUseKDTree = new CheckBox
            {
                Text = "Use KD-Tree for faster neighbor search",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkPRMUseKDTree);
            y += ROW_HEIGHT - 5;

            // Lazy Collision Check
            _chkPRMLazyCollision = new CheckBox
            {
                Text = "Lazy Collision Check (faster, less accurate)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = false
            };
            _pnlSettingsContainer.Controls.Add(_chkPRMLazyCollision);
            y += ROW_HEIGHT;

            // Info label
            var lblInfo = new Label
            {
                Text = "ℹ️ PRM builds a roadmap of samples, then uses Dijkstra for queries.\n" +
                       "Best for multi-query scenarios in static environments.",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            _pnlSettingsContainer.Controls.Add(lblInfo);

            RegisterPRMEvents();
        }

        private void RegisterPRMEvents()
        {
            _nudPRMSamples.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPRMConnectionRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPRMMaxNeighbors.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPRMSampleBias.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkPRMUseKDTree.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkPRMLazyCollision.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets PRM configuration values
        /// </summary>
        public (int samples, double radius, int maxNeighbors, double bias, bool useKDTree, bool lazyCollision) GetPRMConfig()
        {
            return (
                (int)_nudPRMSamples.Value,
                (double)_nudPRMConnectionRadius.Value,
                (int)_nudPRMMaxNeighbors.Value,
                (double)_nudPRMSampleBias.Value / 100.0,
                _chkPRMUseKDTree.Checked,
                _chkPRMLazyCollision.Checked
            );
        }
        #endregion

        #region Private Methods - PSO Settings
        private void CreatePSOSettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            var lblTitle = new Label
            {
                Text = "═══════════ PSO SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)  // Green color
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;

            // Population Size
            var lblPopulation = new Label
            {
                Text = "Population Size:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPSOPopulation = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 10,
                Maximum = 200,
                Value = 50,
                Increment = 10
            };
            _pnlSettingsContainer.Controls.Add(lblPopulation);
            _pnlSettingsContainer.Controls.Add(_nudPSOPopulation);
            y += ROW_HEIGHT;

            // Max Iterations
            var lblIterations = new Label
            {
                Text = "Max Iterations:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPSOMaxIterations = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 100,
                Minimum = 20,
                Maximum = 500,
                Value = 100,
                Increment = 20
            };
            _pnlSettingsContainer.Controls.Add(lblIterations);
            _pnlSettingsContainer.Controls.Add(_nudPSOMaxIterations);
            y += ROW_HEIGHT;

            // Inertia Weight
            var lblInertia = new Label
            {
                Text = "Inertia Weight (w):",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPSOInertia = new NumericUpDown
            {
                Location = new Point(140, y - 3),
                Width = 70,
                Minimum = 1,
                Maximum = 100,
                Value = 70,
                DecimalPlaces = 2
            };
            var lblInertiaInfo = new Label
            {
                Text = "(0.1 - 1.0)",
                Location = new Point(215, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblInertia);
            _pnlSettingsContainer.Controls.Add(_nudPSOInertia);
            _pnlSettingsContainer.Controls.Add(lblInertiaInfo);
            y += ROW_HEIGHT;

            // Cognitive Weight
            var lblCognitive = new Label
            {
                Text = "Cognitive Weight (c1):",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPSOCognitive = new NumericUpDown
            {
                Location = new Point(150, y - 3),
                Width = 70,
                Minimum = 1,
                Maximum = 300,
                Value = 150,
                DecimalPlaces = 2
            };
            var lblCognitiveInfo = new Label
            {
                Text = "(0.1 - 3.0)",
                Location = new Point(225, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblCognitive);
            _pnlSettingsContainer.Controls.Add(_nudPSOCognitive);
            _pnlSettingsContainer.Controls.Add(lblCognitiveInfo);
            y += ROW_HEIGHT;

            // Social Weight
            var lblSocial = new Label
            {
                Text = "Social Weight (c2):",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPSOSocial = new NumericUpDown
            {
                Location = new Point(140, y - 3),
                Width = 70,
                Minimum = 1,
                Maximum = 300,
                Value = 150,
                DecimalPlaces = 2
            };
            var lblSocialInfo = new Label
            {
                Text = "(0.1 - 3.0)",
                Location = new Point(215, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblSocial);
            _pnlSettingsContainer.Controls.Add(_nudPSOSocial);
            _pnlSettingsContainer.Controls.Add(lblSocialInfo);
            y += ROW_HEIGHT;

            // Path Segments
            var lblSegments = new Label
            {
                Text = "Path Segments:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudPSOPathSegments = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = 80,
                Minimum = 5,
                Maximum = 50,
                Value = 20
            };
            var lblSegmentsInfo = new Label
            {
                Text = "(more segments = smoother paths)",
                Location = new Point(205, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblSegments);
            _pnlSettingsContainer.Controls.Add(_nudPSOPathSegments);
            _pnlSettingsContainer.Controls.Add(lblSegmentsInfo);
            y += ROW_HEIGHT;

            // Adaptive Inertia
            _chkPSOAdaptiveInertia = new CheckBox
            {
                Text = "Adaptive Inertia (decrease over time)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkPSOAdaptiveInertia);
            y += ROW_HEIGHT - 5;

            // Elitism
            _chkPSOElitism = new CheckBox
            {
                Text = "Use Elitism (keep best particle)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkPSOElitism);
            y += ROW_HEIGHT;

            // Info label
            var lblInfo = new Label
            {
                Text = "ℹ️ PSO simulates bird flocking behavior.\n" +
                       "Each particle represents a candidate path.",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            _pnlSettingsContainer.Controls.Add(lblInfo);

            RegisterPSOEvents();
        }

        private void RegisterPSOEvents()
        {
            _nudPSOPopulation.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPSOMaxIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPSOInertia.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPSOCognitive.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPSOSocial.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPSOPathSegments.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkPSOAdaptiveInertia.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkPSOElitism.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets PSO configuration values
        /// </summary>
        public (int population, int maxIterations, double inertia, double cognitive, double social,
                 int pathSegments, bool adaptiveInertia, bool elitism) GetPSOConfig()
        {
            return (
                (int)_nudPSOPopulation.Value,
                (int)_nudPSOMaxIterations.Value,
                (double)_nudPSOInertia.Value / 100.0,
                (double)_nudPSOCognitive.Value / 100.0,
                (double)_nudPSOSocial.Value / 100.0,
                (int)_nudPSOPathSegments.Value,
                _chkPSOAdaptiveInertia.Checked,
                _chkPSOElitism.Checked
            );
        }
        #endregion

        #region Private Methods - GA Settings
        private void CreateGASettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            var lblTitle = new Label
            {
                Text = "═══════════ GA SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 196, 15)  // Yellow color
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;

            // Population Size
            var lblPopulation = new Label { Text = "Population Size:", Location = new Point(10, y), AutoSize = true };
            _nudGAPopulation = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 20,
                Maximum = 500,
                Value = 100,
                Increment = 10
            };
            _pnlSettingsContainer.Controls.Add(lblPopulation);
            _pnlSettingsContainer.Controls.Add(_nudGAPopulation);
            y += ROW_HEIGHT;

            // Max Generations
            var lblGenerations = new Label { Text = "Max Generations:", Location = new Point(10, y), AutoSize = true };
            _nudGAGenerations = new NumericUpDown
            {
                Location = new Point(140, y - 3),
                Width = 100,
                Minimum = 20,
                Maximum = 1000,
                Value = 200,
                Increment = 20
            };
            _pnlSettingsContainer.Controls.Add(lblGenerations);
            _pnlSettingsContainer.Controls.Add(_nudGAGenerations);
            y += ROW_HEIGHT;

            // Crossover Rate
            var lblCrossover = new Label { Text = "Crossover Rate:", Location = new Point(10, y), AutoSize = true };
            _nudGACrossoverRate = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 0,
                Maximum = 100,
                Value = 80,
                DecimalPlaces = 0
            };
            var lblCrossoverPercent = new Label { Text = "%", Location = new Point(215, y - 3), AutoSize = true };
            _pnlSettingsContainer.Controls.Add(lblCrossover);
            _pnlSettingsContainer.Controls.Add(_nudGACrossoverRate);
            _pnlSettingsContainer.Controls.Add(lblCrossoverPercent);
            y += ROW_HEIGHT;

            // Mutation Rate
            var lblMutation = new Label { Text = "Mutation Rate:", Location = new Point(10, y), AutoSize = true };
            _nudGAMutationRate = new NumericUpDown
            {
                Location = new Point(120, y - 3),
                Width = 80,
                Minimum = 0,
                Maximum = 50,
                Value = 10,
                DecimalPlaces = 0
            };
            var lblMutationPercent = new Label { Text = "%", Location = new Point(205, y - 3), AutoSize = true };
            _pnlSettingsContainer.Controls.Add(lblMutation);
            _pnlSettingsContainer.Controls.Add(_nudGAMutationRate);
            _pnlSettingsContainer.Controls.Add(lblMutationPercent);
            y += ROW_HEIGHT;

            // Elite Ratio
            var lblElite = new Label { Text = "Elite Ratio:", Location = new Point(10, y), AutoSize = true };
            _nudGAEliteRatio = new NumericUpDown
            {
                Location = new Point(110, y - 3),
                Width = 80,
                Minimum = 0,
                Maximum = 30,
                Value = 10,
                DecimalPlaces = 0
            };
            var lblElitePercent = new Label { Text = "%", Location = new Point(195, y - 3), AutoSize = true };
            _pnlSettingsContainer.Controls.Add(lblElite);
            _pnlSettingsContainer.Controls.Add(_nudGAEliteRatio);
            _pnlSettingsContainer.Controls.Add(lblElitePercent);
            y += ROW_HEIGHT;

            // Tournament Size
            var lblTournament = new Label { Text = "Tournament Size:", Location = new Point(10, y), AutoSize = true };
            _nudGATournamentSize = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 60,
                Minimum = 2,
                Maximum = 10,
                Value = 3
            };
            _pnlSettingsContainer.Controls.Add(lblTournament);
            _pnlSettingsContainer.Controls.Add(_nudGATournamentSize);
            y += ROW_HEIGHT;

            // Adaptive Mutation
            _chkGAAdaptiveMutation = new CheckBox
            {
                Text = "Adaptive Mutation (decrease over time)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkGAAdaptiveMutation);
            y += ROW_HEIGHT;

            RegisterGAEvents();
        }

        private void RegisterGAEvents()
        {
            _nudGAPopulation.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudGAGenerations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudGACrossoverRate.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudGAMutationRate.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudGAEliteRatio.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudGATournamentSize.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkGAAdaptiveMutation.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets GA configuration values
        /// </summary>
        public (int population, int maxGenerations, double crossoverRate, double mutationRate,
                 double eliteRatio, int tournamentSize, bool adaptiveMutation) GetGAConfig()
        {
            return (
                (int)_nudGAPopulation.Value,
                (int)_nudGAGenerations.Value,
                (double)_nudGACrossoverRate.Value / 100.0,
                (double)_nudGAMutationRate.Value / 100.0,
                (double)_nudGAEliteRatio.Value / 100.0,
                (int)_nudGATournamentSize.Value,
                _chkGAAdaptiveMutation.Checked
            );
        }
        #endregion

        #region Private Methods - RRT* Settings
        /// <summary>
        /// Creates RRT* (RRT-Star) specific settings panel
        /// </summary>
        private void CreateRRTStarSettings()
        {
            int y = GetCommonSettingsBottom() + 10;

            #region Title
            var lblTitle = new Label
            {
                Text = "═══════════ RRT* SPECIFIC SETTINGS ═══════════",
                Location = new Point(5, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182)  // Purple color
            };
            _pnlSettingsContainer.Controls.Add(lblTitle);
            y += GROUP_HEADER_HEIGHT;
            #endregion

            #region Max Iterations
            var lblIterations = new Label
            {
                Text = "Max Iterations:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudRRTStarIterations = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 100,
                Minimum = 100,
                Maximum = 50000,
                Value = 5000,
                Increment = 500
            };
            var lblIterationsInfo = new Label
            {
                Text = "(more iterations = better path)",
                Location = new Point(235, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblIterations);
            _pnlSettingsContainer.Controls.Add(_nudRRTStarIterations);
            _pnlSettingsContainer.Controls.Add(lblIterationsInfo);
            y += ROW_HEIGHT;
            #endregion

            #region Step Size
            var lblStepSize = new Label
            {
                Text = "Step Size (cells):",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudRRTStarStepSize = new NumericUpDown
            {
                Location = new Point(140, y - 3),
                Width = 80,
                Minimum = 5,
                Maximum = 100,
                Value = 10,
                DecimalPlaces = 1,
                Increment = 5
            };
            var lblStepSizeInfo = new Label
            {
                Text = "(0.5 - 10.0)",
                Location = new Point(225, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblStepSize);
            _pnlSettingsContainer.Controls.Add(_nudRRTStarStepSize);
            _pnlSettingsContainer.Controls.Add(lblStepSizeInfo);
            y += ROW_HEIGHT;
            #endregion

            #region Goal Bias
            var lblGoalBias = new Label
            {
                Text = "Goal Bias:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudRRTStarGoalBias = new NumericUpDown
            {
                Location = new Point(100, y - 3),
                Width = 80,
                Minimum = 0,
                Maximum = 100,
                Value = 10,
                DecimalPlaces = 0
            };
            var lblGoalBiasPercent = new Label
            {
                Text = "% (0-100)",
                Location = new Point(185, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            var lblGoalBiasInfo = new Label
            {
                Text = "higher = more direct to goal",
                Location = new Point(235, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblGoalBias);
            _pnlSettingsContainer.Controls.Add(_nudRRTStarGoalBias);
            _pnlSettingsContainer.Controls.Add(lblGoalBiasPercent);
            _pnlSettingsContainer.Controls.Add(lblGoalBiasInfo);
            y += ROW_HEIGHT;
            #endregion

            #region Rewiring Radius
            var lblRewiringRadius = new Label
            {
                Text = "Rewiring Radius:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudRRTStarRewiringRadius = new NumericUpDown
            {
                Location = new Point(130, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 100,
                Value = 10,
                DecimalPlaces = 1,
                Increment = 5
            };
            var lblRewiringInfo = new Label
            {
                Text = "(larger = better optimality)",
                Location = new Point(215, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblRewiringRadius);
            _pnlSettingsContainer.Controls.Add(_nudRRTStarRewiringRadius);
            _pnlSettingsContainer.Controls.Add(lblRewiringInfo);
            y += ROW_HEIGHT;
            #endregion

            #region Goal Sample Radius
            var lblGoalRadius = new Label
            {
                Text = "Goal Sample Radius:",
                Location = new Point(10, y),
                AutoSize = true
            };
            _nudRRTStarGoalRadius = new NumericUpDown
            {
                Location = new Point(150, y - 3),
                Width = 80,
                Minimum = 1,
                Maximum = 100,
                Value = 20,
                DecimalPlaces = 1,
                Increment = 5
            };
            var lblGoalRadiusInfo = new Label
            {
                Text = "(cells)",
                Location = new Point(235, y - 3),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            _pnlSettingsContainer.Controls.Add(lblGoalRadius);
            _pnlSettingsContainer.Controls.Add(_nudRRTStarGoalRadius);
            _pnlSettingsContainer.Controls.Add(lblGoalRadiusInfo);
            y += ROW_HEIGHT;
            #endregion

            #region Informed Sampling
            _chkRRTStarInformedSampling = new CheckBox
            {
                Text = "Informed Sampling (focus on optimal region)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkRRTStarInformedSampling);
            y += ROW_HEIGHT - 5;
            #endregion

            #region Path Smoothing
            _chkRRTStarSmoothPath = new CheckBox
            {
                Text = "Smooth Path (remove redundant points)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = true
            };
            _pnlSettingsContainer.Controls.Add(_chkRRTStarSmoothPath);
            y += ROW_HEIGHT - 5;
            #endregion

            #region Bidirectional RRT*
            _chkRRTStarBidirectional = new CheckBox
            {
                Text = "Bidirectional Search (two trees)",
                Location = new Point(10, y),
                AutoSize = true,
                Checked = false
            };
            _pnlSettingsContainer.Controls.Add(_chkRRTStarBidirectional);
            y += ROW_HEIGHT;
            #endregion

            #region Info Label
            var lblInfo = new Label
            {
                Text = "ℹ️ RRT* is asymptotically optimal.\n" +
                       "   - Rewiring improves path quality over time\n" +
                       "   - Informed sampling focuses search on optimal region\n" +
                       "   - Bidirectional can find paths faster",
                Location = new Point(10, y),
                Size = new Size(280, 45),
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            _pnlSettingsContainer.Controls.Add(lblInfo);
            y += 50;
            #endregion

            RegisterRRTStarEvents();
        }

        /// <summary>
        /// Registers events for RRT* settings
        /// </summary>
        private void RegisterRRTStarEvents()
        {
            _nudRRTStarIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStarStepSize.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStarGoalBias.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStarRewiringRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStarGoalRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTStarInformedSampling.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTStarSmoothPath.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTStarBidirectional.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets RRT* configuration values
        /// </summary>
        public (int maxIterations, double stepSize, double goalBias, double rewiringRadius,
                 double goalRadius, bool informedSampling, bool smoothPath, bool bidirectional) GetRRTStarConfig()
        {
            return (
                (int)_nudRRTStarIterations.Value,
                (double)_nudRRTStarStepSize.Value / 10.0,
                (double)_nudRRTStarGoalBias.Value / 100.0,
                (double)_nudRRTStarRewiringRadius.Value / 10.0,
                (double)_nudRRTStarGoalRadius.Value / 10.0,
                _chkRRTStarInformedSampling.Checked,
                _chkRRTStarSmoothPath.Checked,
                _chkRRTStarBidirectional.Checked
            );
        }
        #endregion

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
                case AlgorithmType.RRT:
                    CreateRRTSettings();
                    break;
                case AlgorithmType.PRM:
                    CreatePRMSettings();
                    break;
                case AlgorithmType.PSO:
                    CreatePSOSettings();
                    break;
                case AlgorithmType.GA:
                    CreateGASettings();
                    break;
                case AlgorithmType.RRTStar:
                    CreateRRTStarSettings();
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
                "RRT" => AlgorithmType.RRT,
                "PRM" => AlgorithmType.PRM,
                "PSO" => AlgorithmType.PSO,
                "GA" => AlgorithmType.GA,
                "RRT*" => AlgorithmType.RRTStar,
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