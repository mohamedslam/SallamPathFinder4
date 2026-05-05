#region File Header
/// <summary>
/// File: AlgorithmSettingsPanel.cs
/// Description: Advanced algorithm settings panel with common and algorithm-specific configurations
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-29
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.WinForms.Helpers;
#endregion

namespace SallamPathFinder4.WinForms.Panels
{
    #region Class Documentation
    /// <summary>
    /// Panel for configuring algorithm settings
    /// Supports common settings, algorithm-specific parameters, and search visualization
    /// </summary>
    #endregion
    public sealed class AlgorithmSettingsPanel : Panel
    {
        #region Constants
        private const int RIGHT_PANEL_WIDTH = 380;
        private const int SETTINGS_CONTAINER_WIDTH = 340;
        private const int SETTINGS_CONTAINER_HEIGHT = 520;
        private const int CONTROL_WIDTH = 120;
        private const int LABEL_WIDTH = 100;
        private const int ROW_HEIGHT = 30;
        private const int GROUP_HEADER_HEIGHT = 25;
        private const int BUTTON_PANEL_HEIGHT = 50;

        // Default values
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
        private Label _lblMetricDescription;
        #endregion

        #region Private Fields - ACO Settings
        private NumericUpDown _nudACOAnts;
        private NumericUpDown _nudACOEvaporation;
        private NumericUpDown _nudACOAlpha;
        private NumericUpDown _nudACOBeta;
        private NumericUpDown _nudACOIterations;
        private Panel _pnlACOSettings;
        #endregion

        #region Private Fields - KNN Settings
        private NumericUpDown _nudKNNK;
        private NumericUpDown _nudKNNRadius;
        private Panel _pnlKNNSettings;
        #endregion

        #region Private Fields - D* Settings
        private CheckBox _chkDStarDynamic;
        private NumericUpDown _nudDStarRange;
        private Panel _pnlDStarSettings;
        #endregion

        #region Private Fields - Brute Force Settings
        private NumericUpDown _nudBFDepth;
        private NumericUpDown _nudBFIterations;
        private Panel _pnlBruteForceSettings;
        #endregion
      
        #region Private Fields - RRT Settings
        private NumericUpDown _nudRRTIterations;
        private NumericUpDown _nudRRTStepSize;
        private NumericUpDown _nudRRTGoalBias;
        private CheckBox _chkRRTSmooth;
        private CheckBox _chkRRTBidirectional;
        private Panel _pnlRRTSettings;
        #endregion

        #region Private Fields - PRM Settings
        private NumericUpDown _nudPRMSamples;
        private NumericUpDown _nudPRMConnectionRadius;
        private NumericUpDown _nudPRMMaxNeighbors;
        private NumericUpDown _nudPRMSampleBias;
        private Panel _pnlPRMSettings;
        #endregion

        #region Private Fields - PSO Settings
        private NumericUpDown _nudPSOPopulation;
        private NumericUpDown _nudPSOMaxIterations;
        private NumericUpDown _nudPSOInertia;
        private NumericUpDown _nudPSOCognitive;
        private NumericUpDown _nudPSOSocial;
        private NumericUpDown _nudPSOPathSegments;
        private CheckBox _chkPSOAdaptiveInertia;
        private Panel _pnlPSOSettings;
        #endregion

        #region Private Fields - GA Settings
        private NumericUpDown _nudGAPopulation;
        private NumericUpDown _nudGAGenerations;
        private NumericUpDown _nudGACrossoverRate;
        private NumericUpDown _nudGAMutationRate;
        private NumericUpDown _nudGAEliteRatio;
        private NumericUpDown _nudGATournamentSize;
        private CheckBox _chkGAAdaptiveMutation;
        private Panel _pnlGASettings;
        #endregion

        #region Private Fields - RRT* Settings
        private NumericUpDown _nudRRTStarIterations;
        private NumericUpDown _nudRRTStarStepSize;
        private NumericUpDown _nudRRTStarGoalBias;
        private NumericUpDown _nudRRTStarRewiringRadius;
        private NumericUpDown _nudRRTStarGoalRadius;
        private CheckBox _chkRRTStarInformedSampling;
        private CheckBox _chkRRTStarSmoothPath;
        private Panel _pnlRRTStarSettings;
        #endregion
        
        #region Private Fields - Visualization
        private CheckBox _chkEnableVisualization;
        private TrackBar _speedSlider;
        private Label _lblSpeed;
        private Button _btnPause;
        private Button _btnResume;
        private Button _btnStop;
        private Panel _pnlVisualization;
        #endregion

        #region Private Fields - Action Buttons
        private Button _btnFindPath;
        #endregion

        #region Private Fields - GIF Recording
        private Button _btnStartRecording;
        private Button _btnStopRecording;
        private Label _lblRecordingStatus;
        private GifRecorder _gifRecorder;
        private bool _isRecording = false;
        #endregion

        #region Events
        public event EventHandler SettingsChanged;
        public event EventHandler FindPathRequested;
        public event EventHandler VisualizationSettingsChanged;
        public event EventHandler PauseRequested;
        public event EventHandler ResumeRequested;
        public event EventHandler StopRequested;
        public event EventHandler StartRecordingRequested;
        public event EventHandler StopRecordingRequested;
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
        #endregion

        #region Properties - Visualization
        public bool IsVisualizationEnabled => _chkEnableVisualization?.Checked ?? false;
        public int GetSpeedDelayMs()
        {
            if (!IsVisualizationEnabled) return 0;
            if (_speedSlider == null) return 0;
            if (_speedSlider.Value == 0) return 0;
            return _speedSlider.Value * 5;
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
            CreateAlgorithmSpecificPanels();
            CreateVisualizationControls();
            CreateActionButtons(ref y);

            // Initially show settings for default algorithm (A*)
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
                Location = new Point(130, y - 3),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboAlgorithmType.Items.AddRange(new string[]
            {
                "A*", "SPPA", "SPPA-DL", "ACO", "D*", "KNN", "Brute Force","RRT", "PRM", "PSO", "GA", "RRT*"
    
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

            // Header
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
                Location = new Point(140, y - 3),
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
                Location = new Point(140, y - 3),
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

            // Distance Metric
            var lblMetric = new Label
            {
                Text = "Distance Metric:",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _cboDistanceMetric = new ComboBox
            {
                Location = new Point(140, y - 3),
                Width = 180,
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

            _lblMetricDescription = new Label
            {
                Text = "Best for 4-directional movement (no diagonals)",
                Location = new Point(140, y + 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 7, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            _pnlSettingsContainer.Controls.Add(lblMetric);
            _pnlSettingsContainer.Controls.Add(_cboDistanceMetric);
            _pnlSettingsContainer.Controls.Add(_lblMetricDescription);

            // Register events
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

        private void CreateAlgorithmSpecificPanels()
        {
            // ACO Settings Panel
            _pnlACOSettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 130),
                Visible = false
            };

            int ay = 5;
            var lblACOTitle = new Label
            {
                Text = "═══════════ ACO SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, ay),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219)
            };
            _pnlACOSettings.Controls.Add(lblACOTitle);
            ay += 25;

            var lblAnts = new Label { Text = "Number of Ants:", Location = new Point(5, ay), AutoSize = true };
            _nudACOAnts = new NumericUpDown { Location = new Point(120, ay - 3), Width = 80, Minimum = 1, Maximum = 200, Value = DEFAULT_ACO_ANTS };
            _pnlACOSettings.Controls.Add(lblAnts);
            _pnlACOSettings.Controls.Add(_nudACOAnts);
            ay += ROW_HEIGHT;

            var lblEvaporation = new Label { Text = "Evaporation Rate (0-1):", Location = new Point(5, ay), AutoSize = true };
            _nudACOEvaporation = new NumericUpDown { Location = new Point(150, ay - 3), Width = 80, Minimum = 0, Maximum = 100, Value = (decimal)(DEFAULT_ACO_EVAPORATION * 100), DecimalPlaces = 2 };
            _pnlACOSettings.Controls.Add(lblEvaporation);
            _pnlACOSettings.Controls.Add(_nudACOEvaporation);
            ay += ROW_HEIGHT;

            var lblAlpha = new Label { Text = "Alpha (α):", Location = new Point(5, ay), AutoSize = true };
            _nudACOAlpha = new NumericUpDown { Location = new Point(80, ay - 3), Width = 70, Minimum = 0, Maximum = 50, Value = (decimal)DEFAULT_ACO_ALPHA, DecimalPlaces = 1 };
            var lblBeta = new Label { Text = "Beta (β):", Location = new Point(160, ay), AutoSize = true };
            _nudACOBeta = new NumericUpDown { Location = new Point(210, ay - 3), Width = 70, Minimum = 0, Maximum = 50, Value = (decimal)DEFAULT_ACO_BETA, DecimalPlaces = 1 };
            _pnlACOSettings.Controls.Add(lblAlpha);
            _pnlACOSettings.Controls.Add(_nudACOAlpha);
            _pnlACOSettings.Controls.Add(lblBeta);
            _pnlACOSettings.Controls.Add(_nudACOBeta);
            ay += ROW_HEIGHT;

            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(5, ay), AutoSize = true };
            _nudACOIterations = new NumericUpDown { Location = new Point(120, ay - 3), Width = 80, Minimum = 10, Maximum = 500, Value = DEFAULT_ACO_ITERATIONS };
            _pnlACOSettings.Controls.Add(lblIter);
            _pnlACOSettings.Controls.Add(_nudACOIterations);

            _pnlSettingsContainer.Controls.Add(_pnlACOSettings);

            // KNN Settings Panel
            _pnlKNNSettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 80),
                Visible = false
            };

            int ky = 5;
            var lblKNNSettings = new Label
            {
                Text = "═══════════ KNN SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, ky),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            _pnlKNNSettings.Controls.Add(lblKNNSettings);
            ky += 25;

            var lblK = new Label { Text = "K Neighbors:", Location = new Point(5, ky), AutoSize = true };
            _nudKNNK = new NumericUpDown { Location = new Point(120, ky - 3), Width = 80, Minimum = 1, Maximum = 20, Value = DEFAULT_KNN_NEIGHBORS };
            _pnlKNNSettings.Controls.Add(lblK);
            _pnlKNNSettings.Controls.Add(_nudKNNK);
            ky += ROW_HEIGHT;

            var lblRadius = new Label { Text = "Search Radius (cells):", Location = new Point(5, ky), AutoSize = true };
            _nudKNNRadius = new NumericUpDown { Location = new Point(140, ky - 3), Width = 80, Minimum = 1, Maximum = 20, Value = DEFAULT_KNN_RADIUS };
            _pnlKNNSettings.Controls.Add(lblRadius);
            _pnlKNNSettings.Controls.Add(_nudKNNRadius);

            _pnlSettingsContainer.Controls.Add(_pnlKNNSettings);

            // D* Settings Panel
            _pnlDStarSettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 70),
                Visible = false
            };

            int dy = 5;
            var lblDStarTitle = new Label
            {
                Text = "═══════════ D* SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, dy),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 196, 15)
            };
            _pnlDStarSettings.Controls.Add(lblDStarTitle);
            dy += 25;

            _chkDStarDynamic = new CheckBox
            {
                Text = "Enable Dynamic Replanning",
                Location = new Point(5, dy),
                AutoSize = true,
                Checked = true
            };
            _pnlDStarSettings.Controls.Add(_chkDStarDynamic);
            dy += ROW_HEIGHT - 5;

            var lblRange = new Label { Text = "Replanning Range (cells):", Location = new Point(5, dy), AutoSize = true };
            _nudDStarRange = new NumericUpDown { Location = new Point(160, dy - 3), Width = 60, Minimum = 1, Maximum = 50, Value = DEFAULT_DSTAR_RANGE };
            _pnlDStarSettings.Controls.Add(lblRange);
            _pnlDStarSettings.Controls.Add(_nudDStarRange);

            _pnlSettingsContainer.Controls.Add(_pnlDStarSettings);

            // Brute Force Settings Panel
            _pnlBruteForceSettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 80),
                Visible = false
            };

            int bfy = 5;
            var lblBFTitle = new Label
            {
                Text = "═══════════ BRUTE FORCE SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, bfy),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60)
            };
            _pnlBruteForceSettings.Controls.Add(lblBFTitle);
            bfy += 25;

            var lblDepth = new Label { Text = "Max Search Depth:", Location = new Point(5, bfy), AutoSize = true };
            _nudBFDepth = new NumericUpDown { Location = new Point(130, bfy - 3), Width = 100, Minimum = 100, Maximum = 50000, Value = DEFAULT_BF_DEPTH };
            _pnlBruteForceSettings.Controls.Add(lblDepth);
            _pnlBruteForceSettings.Controls.Add(_nudBFDepth);
            bfy += ROW_HEIGHT;

            var lblBFIterations = new Label { Text = "Max Iterations:", Location = new Point(5, bfy), AutoSize = true };
            _nudBFIterations = new NumericUpDown { Location = new Point(130, bfy - 3), Width = 100, Minimum = 1000, Maximum = 500000, Value = DEFAULT_BF_ITERATIONS };
            _pnlBruteForceSettings.Controls.Add(lblIter);
            _pnlBruteForceSettings.Controls.Add(_nudBFIterations);

            _pnlSettingsContainer.Controls.Add(_pnlBruteForceSettings);

            // Register events
            RegisterAlgorithmSpecificEvents();

            CreateRRTSettings();
            CreatePRMSettings();
            CreatePSOSettings();
            CreateGASettings();
            CreateRRTStarSettings();
        }

        private void RegisterAlgorithmSpecificEvents()
        {
            if (_nudACOAnts != null) _nudACOAnts.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudACOEvaporation != null) _nudACOEvaporation.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudACOAlpha != null) _nudACOAlpha.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudACOBeta != null) _nudACOBeta.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudACOIterations != null) _nudACOIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudKNNK != null) _nudKNNK.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudKNNRadius != null) _nudKNNRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_chkDStarDynamic != null) _chkDStarDynamic.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudDStarRange != null) _nudDStarRange.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudBFDepth != null) _nudBFDepth.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            if (_nudBFIterations != null) _nudBFIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private int GetCommonSettingsBottom()
        {
            // Return the Y position after common settings
            if (_lblMetricDescription != null)
            {
                return _lblMetricDescription.Bottom + 10;
            }
            return 200;
        }

        private void CreateVisualizationControls()
        {
            int y = GetCommonSettingsBottom() + 5;

            // Adjust position based on algorithm-specific panels visibility
            if (_pnlACOSettings.Visible || _pnlKNNSettings.Visible || _pnlDStarSettings.Visible || _pnlBruteForceSettings.Visible)
            {
                y += 100;
            }

            _pnlVisualization = new Panel
            {
                Location = new Point(5, y),
                Size = new Size(320, 160),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250)
            };

            int vy = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ SEARCH VISUALIZATION ═══════════",
                Location = new Point(5, vy),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182)
            };
            _pnlVisualization.Controls.Add(lblTitle);
            vy += 25;

            _chkEnableVisualization = new CheckBox
            {
                Text = "Enable Search Visualization (Shows cells during search)",
                Location = new Point(10, vy),
                Size = new Size(290, 22),
                Checked = false
            };
            _chkEnableVisualization.CheckedChanged += (s, e) =>
            {
                bool enabled = _chkEnableVisualization.Checked;
                _speedSlider.Enabled = enabled;
                _lblSpeed.Enabled = enabled;
                _btnPause.Enabled = enabled;
                _btnResume.Enabled = enabled;
                _btnStop.Enabled = enabled;
                VisualizationSettingsChanged?.Invoke(this, EventArgs.Empty);
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            };
            _pnlVisualization.Controls.Add(_chkEnableVisualization);
            vy += 28;

            _lblSpeed = new Label
            {
                Text = "Visualization Speed: Fast (No delay)",
                Location = new Point(10, vy),
                Size = new Size(290, 20),
                Enabled = false
            };
            _pnlVisualization.Controls.Add(_lblSpeed);
            vy += 20;

            _speedSlider = new TrackBar
            {
                Location = new Point(10, vy),
                Size = new Size(290, 45),
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                TickFrequency = 10,
                Enabled = false
            };
            _speedSlider.Scroll += (s, e) =>
            {
                UpdateSpeedLabel();
                VisualizationSettingsChanged?.Invoke(this, EventArgs.Empty);
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            };
            _pnlVisualization.Controls.Add(_speedSlider);
            vy += 45;

            _btnPause = new Button
            {
                Text = "⏸ Pause",
                Location = new Point(10, vy),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(241, 196, 15),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            _btnPause.Click += (s, e) => PauseRequested?.Invoke(this, EventArgs.Empty);
            _pnlVisualization.Controls.Add(_btnPause);

            _btnResume = new Button
            {
                Text = "▶ Resume",
                Location = new Point(95, vy),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            _btnResume.Click += (s, e) => ResumeRequested?.Invoke(this, EventArgs.Empty);
            _pnlVisualization.Controls.Add(_btnResume);

            _btnStop = new Button
            {
                Text = "⏹ Stop",
                Location = new Point(180, vy),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            _btnStop.Click += (s, e) => StopRequested?.Invoke(this, EventArgs.Empty);
            _pnlVisualization.Controls.Add(_btnStop);

            _pnlSettingsContainer.Controls.Add(_pnlVisualization);
            CreateRecordingControls();
        }

        private void UpdateSpeedLabel()
        {
            if (_lblSpeed == null) return;

            if (!_chkEnableVisualization.Checked)
            {
                _lblSpeed.Text = "Visualization Speed: Disabled";
                return;
            }

            if (_speedSlider.Value == 0)
                _lblSpeed.Text = "Visualization Speed: Instant (No delay)";
            else if (_speedSlider.Value <= 20)
                _lblSpeed.Text = "Visualization Speed: Very Fast";
            else if (_speedSlider.Value <= 50)
                _lblSpeed.Text = "Visualization Speed: Medium";
            else if (_speedSlider.Value <= 80)
                _lblSpeed.Text = "Visualization Speed: Slow";
            else
                _lblSpeed.Text = "Visualization Speed: Very Slow (Educational)";
        }

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
                Location = new Point(100, 10),
                Size = new Size(120, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            _btnFindPath.Click += (s, e) => FindPathRequested?.Invoke(this, EventArgs.Empty);

            pnlButtons.Controls.Add(_btnFindPath);
            this.Controls.Add(pnlButtons);
        }

        public void SetAlgorithmByIndex(int index)
        {
            if (_cboAlgorithmType != null && index >= 0 && index < _cboAlgorithmType.Items.Count)
            {
                _cboAlgorithmType.SelectedIndex = index;
            }
        }
        #endregion

        #region Panel Management
        private void ClearAlgorithmPanels()
        {
            if (_pnlACOSettings != null) _pnlACOSettings.Visible = false;
            if (_pnlKNNSettings != null) _pnlKNNSettings.Visible = false;
            if (_pnlDStarSettings != null) _pnlDStarSettings.Visible = false;
            if (_pnlBruteForceSettings != null) _pnlBruteForceSettings.Visible = false;
            if (_pnlRRTSettings != null) _pnlRRTSettings.Visible = false;
            if (_pnlPRMSettings != null) _pnlPRMSettings.Visible = false;
            if (_pnlPSOSettings != null) _pnlPSOSettings.Visible = false;
            if (_pnlGASettings != null) _pnlGASettings.Visible = false;
            if (_pnlRRTStarSettings != null) _pnlRRTStarSettings.Visible = false;
        }

        private void ShowSettingsForAlgorithm(AlgorithmType algorithm)
        {
            ClearAlgorithmPanels();

            int newY = GetCommonSettingsBottom() + 5;

            switch (algorithm)
            {
                case AlgorithmType.ACO:
                    if (_pnlACOSettings != null)
                    {
                        _pnlACOSettings.Visible = true;
                        _pnlACOSettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlACOSettings.Bottom + 5;
                    }
                    break;
                case AlgorithmType.KNN:
                    if (_pnlKNNSettings != null)
                    {
                        _pnlKNNSettings.Visible = true;
                        _pnlKNNSettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlKNNSettings.Bottom + 5;
                    }
                    break;
                case AlgorithmType.DStar:
                    if (_pnlDStarSettings != null)
                    {
                        _pnlDStarSettings.Visible = true;
                        _pnlDStarSettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlDStarSettings.Bottom + 5;
                    }
                    break;
                case AlgorithmType.BruteForce:
                    if (_pnlBruteForceSettings != null)
                    {
                        _pnlBruteForceSettings.Visible = true;
                        _pnlBruteForceSettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlBruteForceSettings.Bottom + 5;
                    }
                    break;
                case AlgorithmType.RRT:
                    if (_pnlRRTSettings != null)
                    {
                        _pnlRRTSettings.Visible = true;
                        _pnlRRTSettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlRRTSettings.Bottom + 5;
                    }
                    break;
                case AlgorithmType.PRM:
                    if (_pnlPRMSettings != null)
                    {
                        _pnlPRMSettings.Visible = true;
                        _pnlPRMSettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlPRMSettings.Bottom + 5;
                    }
                    break;
                case AlgorithmType.PSO:
                    if (_pnlPSOSettings != null)
                    {
                        _pnlPSOSettings.Visible = true;
                        _pnlPSOSettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlPSOSettings.Bottom + 5;
                    }
                    break;
                case AlgorithmType.GA:
                    if (_pnlGASettings != null)
                    {
                        _pnlGASettings.Visible = true;
                        _pnlGASettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlGASettings.Bottom + 5;
                    }
                    break;
                case AlgorithmType.RRTStar:
                    if (_pnlRRTStarSettings != null)
                    {
                        _pnlRRTStarSettings.Visible = true;
                        _pnlRRTStarSettings.Location = new Point(5, GetCommonSettingsBottom() + 5);
                        newY = _pnlRRTStarSettings.Bottom + 5;
                    }
                    break;
                default:
                    // A*, SPPA, SPPA-DL have no specific settings
                    break;
            }

            // Reposition visualization panel
            if (_pnlVisualization != null)
            {
                _pnlVisualization.Location = new Point(5, newY + 10);
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
        #region Private Methods - RRT Settings
        private void CreateRRTSettings()
        {
            _pnlRRTSettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 140),
                Visible = false
            };

            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ RRT SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219)
            };
            _pnlRRTSettings.Controls.Add(lblTitle);
            y += 25;

            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(5, y), AutoSize = true };
            _nudRRTIterations = new NumericUpDown { Location = new Point(130, y - 3), Width = 100, Minimum = 100, Maximum = 50000, Value = 5000, Increment = 500 };
            _pnlRRTSettings.Controls.Add(lblIter);
            _pnlRRTSettings.Controls.Add(_nudRRTIterations);
            y += ROW_HEIGHT;

            var lblStep = new Label { Text = "Step Size (cells):", Location = new Point(5, y), AutoSize = true };
            _nudRRTStepSize = new NumericUpDown { Location = new Point(140, y - 3), Width = 80, Minimum = 5, Maximum = 100, Value = 10, DecimalPlaces = 1, Increment = 5 };
            _pnlRRTSettings.Controls.Add(lblStep);
            _pnlRRTSettings.Controls.Add(_nudRRTStepSize);
            y += ROW_HEIGHT;

            var lblBias = new Label { Text = "Goal Bias (%):", Location = new Point(5, y), AutoSize = true };
            _nudRRTGoalBias = new NumericUpDown { Location = new Point(100, y - 3), Width = 80, Minimum = 0, Maximum = 100, Value = 10 };
            _pnlRRTSettings.Controls.Add(lblBias);
            _pnlRRTSettings.Controls.Add(_nudRRTGoalBias);
            y += ROW_HEIGHT;

            _chkRRTSmooth = new CheckBox { Text = "Smooth Path", Location = new Point(5, y), AutoSize = true, Checked = true };
            _pnlRRTSettings.Controls.Add(_chkRRTSmooth);
            y += ROW_HEIGHT - 5;

            _chkRRTBidirectional = new CheckBox { Text = "Bidirectional Search", Location = new Point(5, y), AutoSize = true, Checked = false };
            _pnlRRTSettings.Controls.Add(_chkRRTBidirectional);

            _pnlSettingsContainer.Controls.Add(_pnlRRTSettings);

            RegisterRRTEvents();
        }

        private void RegisterRRTEvents()
        {
            _nudRRTIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStepSize.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTGoalBias.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTSmooth.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTBidirectional.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Private Methods - PRM Settings
        private void CreatePRMSettings()
        {
            _pnlPRMSettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 130),
                Visible = false
            };

            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ PRM SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 152, 219)
            };
            _pnlPRMSettings.Controls.Add(lblTitle);
            y += 25;

            var lblSamples = new Label { Text = "Number of Samples:", Location = new Point(5, y), AutoSize = true };
            _nudPRMSamples = new NumericUpDown { Location = new Point(140, y - 3), Width = 100, Minimum = 50, Maximum = 5000, Value = 500, Increment = 50 };
            _pnlPRMSettings.Controls.Add(lblSamples);
            _pnlPRMSettings.Controls.Add(_nudPRMSamples);
            y += ROW_HEIGHT;

            var lblRadius = new Label { Text = "Connection Radius (cells):", Location = new Point(5, y), AutoSize = true };
            _nudPRMConnectionRadius = new NumericUpDown { Location = new Point(160, y - 3), Width = 80, Minimum = 1, Maximum = 50, Value = 10, DecimalPlaces = 1 };
            _pnlPRMSettings.Controls.Add(lblRadius);
            _pnlPRMSettings.Controls.Add(_nudPRMConnectionRadius);
            y += ROW_HEIGHT;

            var lblNeighbors = new Label { Text = "Max Neighbors:", Location = new Point(5, y), AutoSize = true };
            _nudPRMMaxNeighbors = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 1, Maximum = 50, Value = 15 };
            _pnlPRMSettings.Controls.Add(lblNeighbors);
            _pnlPRMSettings.Controls.Add(_nudPRMMaxNeighbors);
            y += ROW_HEIGHT;

            var lblBias = new Label { Text = "Sample Bias (%):", Location = new Point(5, y), AutoSize = true };
            _nudPRMSampleBias = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 0, Maximum = 100, Value = 10 };
            _pnlPRMSettings.Controls.Add(lblBias);
            _pnlPRMSettings.Controls.Add(_nudPRMSampleBias);

            _pnlSettingsContainer.Controls.Add(_pnlPRMSettings);

            RegisterPRMEvents();
        }

        private void RegisterPRMEvents()
        {
            _nudPRMSamples.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPRMConnectionRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPRMMaxNeighbors.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudPRMSampleBias.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Private Methods - PSO Settings
        private void CreatePSOSettings()
        {
            _pnlPSOSettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 160),
                Visible = false
            };

            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ PSO SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113)
            };
            _pnlPSOSettings.Controls.Add(lblTitle);
            y += 25;

            var lblPopulation = new Label { Text = "Population Size:", Location = new Point(5, y), AutoSize = true };
            _nudPSOPopulation = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 10, Maximum = 200, Value = 50 };
            _pnlPSOSettings.Controls.Add(lblPopulation);
            _pnlPSOSettings.Controls.Add(_nudPSOPopulation);
            y += ROW_HEIGHT;

            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(5, y), AutoSize = true };
            _nudPSOMaxIterations = new NumericUpDown { Location = new Point(120, y - 3), Width = 100, Minimum = 20, Maximum = 500, Value = 100 };
            _pnlPSOSettings.Controls.Add(lblIter);
            _pnlPSOSettings.Controls.Add(_nudPSOMaxIterations);
            y += ROW_HEIGHT;

            var lblInertia = new Label { Text = "Inertia Weight:", Location = new Point(5, y), AutoSize = true };
            _nudPSOInertia = new NumericUpDown { Location = new Point(120, y - 3), Width = 70, Minimum = 1, Maximum = 100, Value = 70, DecimalPlaces = 2 };
            _pnlPSOSettings.Controls.Add(lblInertia);
            _pnlPSOSettings.Controls.Add(_nudPSOInertia);
            y += ROW_HEIGHT;

            var lblCognitive = new Label { Text = "Cognitive Weight:", Location = new Point(5, y), AutoSize = true };
            _nudPSOCognitive = new NumericUpDown { Location = new Point(130, y - 3), Width = 70, Minimum = 1, Maximum = 300, Value = 150, DecimalPlaces = 2 };
            var lblSocial = new Label { Text = "Social Weight:", Location = new Point(210, y), AutoSize = true };
            _nudPSOSocial = new NumericUpDown { Location = new Point(290, y - 3), Width = 70, Minimum = 1, Maximum = 300, Value = 150, DecimalPlaces = 2 };
            _pnlPSOSettings.Controls.Add(lblCognitive);
            _pnlPSOSettings.Controls.Add(_nudPSOCognitive);
            _pnlPSOSettings.Controls.Add(lblSocial);
            _pnlPSOSettings.Controls.Add(_nudPSOSocial);
            y += ROW_HEIGHT;

            var lblSegments = new Label { Text = "Path Segments:", Location = new Point(5, y), AutoSize = true };
            _nudPSOPathSegments = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 5, Maximum = 50, Value = 20 };
            _pnlPSOSettings.Controls.Add(lblSegments);
            _pnlPSOSettings.Controls.Add(_nudPSOPathSegments);
            y += ROW_HEIGHT;

            _chkPSOAdaptiveInertia = new CheckBox { Text = "Adaptive Inertia", Location = new Point(5, y), AutoSize = true, Checked = true };
            _pnlPSOSettings.Controls.Add(_chkPSOAdaptiveInertia);

            _pnlSettingsContainer.Controls.Add(_pnlPSOSettings);

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
        }
        #endregion

        #region Private Methods - GA Settings
        private void CreateGASettings()
        {
            _pnlGASettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 160),
                Visible = false
            };

            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ GA SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(241, 196, 15)
            };
            _pnlGASettings.Controls.Add(lblTitle);
            y += 25;

            var lblPopulation = new Label { Text = "Population Size:", Location = new Point(5, y), AutoSize = true };
            _nudGAPopulation = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 20, Maximum = 500, Value = 100 };
            _pnlGASettings.Controls.Add(lblPopulation);
            _pnlGASettings.Controls.Add(_nudGAPopulation);
            y += ROW_HEIGHT;

            var lblGenerations = new Label { Text = "Max Generations:", Location = new Point(5, y), AutoSize = true };
            _nudGAGenerations = new NumericUpDown { Location = new Point(130, y - 3), Width = 100, Minimum = 20, Maximum = 1000, Value = 200 };
            _pnlGASettings.Controls.Add(lblGenerations);
            _pnlGASettings.Controls.Add(_nudGAGenerations);
            y += ROW_HEIGHT;

            var lblCrossover = new Label { Text = "Crossover Rate (%):", Location = new Point(5, y), AutoSize = true };
            _nudGACrossoverRate = new NumericUpDown { Location = new Point(130, y - 3), Width = 80, Minimum = 0, Maximum = 100, Value = 80 };
            _pnlGASettings.Controls.Add(lblCrossover);
            _pnlGASettings.Controls.Add(_nudGACrossoverRate);
            y += ROW_HEIGHT;

            var lblMutation = new Label { Text = "Mutation Rate (%):", Location = new Point(5, y), AutoSize = true };
            _nudGAMutationRate = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 0, Maximum = 50, Value = 10 };
            _pnlGASettings.Controls.Add(lblMutation);
            _pnlGASettings.Controls.Add(_nudGAMutationRate);
            y += ROW_HEIGHT;

            var lblElite = new Label { Text = "Elite Ratio (%):", Location = new Point(5, y), AutoSize = true };
            _nudGAEliteRatio = new NumericUpDown { Location = new Point(110, y - 3), Width = 80, Minimum = 0, Maximum = 30, Value = 10 };
            _pnlGASettings.Controls.Add(lblElite);
            _pnlGASettings.Controls.Add(_nudGAEliteRatio);
            y += ROW_HEIGHT;

            var lblTournament = new Label { Text = "Tournament Size:", Location = new Point(5, y), AutoSize = true };
            _nudGATournamentSize = new NumericUpDown { Location = new Point(120, y - 3), Width = 60, Minimum = 2, Maximum = 10, Value = 3 };
            _pnlGASettings.Controls.Add(lblTournament);
            _pnlGASettings.Controls.Add(_nudGATournamentSize);
            y += ROW_HEIGHT;

            _chkGAAdaptiveMutation = new CheckBox { Text = "Adaptive Mutation", Location = new Point(5, y), AutoSize = true, Checked = true };
            _pnlGASettings.Controls.Add(_chkGAAdaptiveMutation);

            _pnlSettingsContainer.Controls.Add(_pnlGASettings);

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
        #endregion

        #region Private Methods - RRT* Settings
        private void CreateRRTStarSettings()
        {
            _pnlRRTStarSettings = new Panel
            {
                Location = new Point(5, GetCommonSettingsBottom() + 5),
                Size = new Size(310, 180),
                Visible = false
            };

            int y = 5;

            var lblTitle = new Label
            {
                Text = "═══════════ RRT* SPECIFIC SETTINGS ═══════════",
                Location = new Point(0, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(155, 89, 182)
            };
            _pnlRRTStarSettings.Controls.Add(lblTitle);
            y += 25;

            var lblIter = new Label { Text = "Max Iterations:", Location = new Point(5, y), AutoSize = true };
            _nudRRTStarIterations = new NumericUpDown { Location = new Point(130, y - 3), Width = 100, Minimum = 100, Maximum = 50000, Value = 5000 };
            _pnlRRTStarSettings.Controls.Add(lblIter);
            _pnlRRTStarSettings.Controls.Add(_nudRRTStarIterations);
            y += ROW_HEIGHT;

            var lblStep = new Label { Text = "Step Size (cells):", Location = new Point(5, y), AutoSize = true };
            _nudRRTStarStepSize = new NumericUpDown { Location = new Point(140, y - 3), Width = 80, Minimum = 5, Maximum = 100, Value = 10, DecimalPlaces = 1 };
            _pnlRRTStarSettings.Controls.Add(lblStep);
            _pnlRRTStarSettings.Controls.Add(_nudRRTStarStepSize);
            y += ROW_HEIGHT;

            var lblBias = new Label { Text = "Goal Bias (%):", Location = new Point(5, y), AutoSize = true };
            _nudRRTStarGoalBias = new NumericUpDown { Location = new Point(100, y - 3), Width = 80, Minimum = 0, Maximum = 100, Value = 10 };
            _pnlRRTStarSettings.Controls.Add(lblBias);
            _pnlRRTStarSettings.Controls.Add(_nudRRTStarGoalBias);
            y += ROW_HEIGHT;

            var lblRewiring = new Label { Text = "Rewiring Radius:", Location = new Point(5, y), AutoSize = true };
            _nudRRTStarRewiringRadius = new NumericUpDown { Location = new Point(120, y - 3), Width = 80, Minimum = 1, Maximum = 100, Value = 10, DecimalPlaces = 1 };
            _pnlRRTStarSettings.Controls.Add(lblRewiring);
            _pnlRRTStarSettings.Controls.Add(_nudRRTStarRewiringRadius);
            y += ROW_HEIGHT;

            var lblGoalRadius = new Label { Text = "Goal Radius:", Location = new Point(5, y), AutoSize = true };
            _nudRRTStarGoalRadius = new NumericUpDown { Location = new Point(100, y - 3), Width = 80, Minimum = 1, Maximum = 100, Value = 20, DecimalPlaces = 1 };
            _pnlRRTStarSettings.Controls.Add(lblGoalRadius);
            _pnlRRTStarSettings.Controls.Add(_nudRRTStarGoalRadius);
            y += ROW_HEIGHT;

            _chkRRTStarInformedSampling = new CheckBox { Text = "Informed Sampling", Location = new Point(5, y), AutoSize = true, Checked = true };
            _pnlRRTStarSettings.Controls.Add(_chkRRTStarInformedSampling);
            y += ROW_HEIGHT - 5;

            _chkRRTStarSmoothPath = new CheckBox { Text = "Smooth Path", Location = new Point(5, y), AutoSize = true, Checked = true };
            _pnlRRTStarSettings.Controls.Add(_chkRRTStarSmoothPath);

            _pnlSettingsContainer.Controls.Add(_pnlRRTStarSettings);

            RegisterRRTStarEvents();
        }

        private void RegisterRRTStarEvents()
        {
            _nudRRTStarIterations.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStarStepSize.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStarGoalBias.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStarRewiringRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _nudRRTStarGoalRadius.ValueChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTStarInformedSampling.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            _chkRRTStarSmoothPath.CheckedChanged += (s, e) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion
         
        #region Private Methods - Recording Controls
        private void CreateRecordingControls()
        {
            int y = _btnStop.Bottom + 15;

            var lblSeparator = new Label
            {
                Text = "═══════════ GIF RECORDING ═══════════",
                Location = new Point(10, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            _pnlVisualization.Controls.Add(lblSeparator);
            y += 25;

            _btnStartRecording = new Button
            {
                Text = "● بدء التسجيل",
                Location = new Point(10, y),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            _btnStartRecording.Click += (s, e) => StartRecordingRequested?.Invoke(this, EventArgs.Empty);
            _pnlVisualization.Controls.Add(_btnStartRecording);

            _btnStopRecording = new Button
            {
                Text = "■ إيقاف التسجيل",
                Location = new Point(120, y),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            _btnStopRecording.Click += (s, e) => StopRecordingRequested?.Invoke(this, EventArgs.Empty);
            _pnlVisualization.Controls.Add(_btnStopRecording);

            _lblRecordingStatus = new Label
            {
                Text = "⚫ غير نشط",
                Location = new Point(230, y + 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            _pnlVisualization.Controls.Add(_lblRecordingStatus);

            // تحديث حالة الأزرار عند تفعيل التصور
            _chkEnableVisualization.CheckedChanged += (s, e) =>
            {
                bool enabled = _chkEnableVisualization.Checked;
                _btnStartRecording.Enabled = enabled && !_isRecording;
                _btnStopRecording.Enabled = enabled && _isRecording;
            };
        }

        public  void UpdateRecordingStatus(bool isRecording, int frameCount = 0)
        {
            _isRecording = isRecording;

            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateRecordingStatus(isRecording, frameCount)));
                return;
            }

            _btnStartRecording.Enabled = _chkEnableVisualization.Checked && !isRecording;
            _btnStopRecording.Enabled = _chkEnableVisualization.Checked && isRecording;

            if (isRecording)
            {
                _lblRecordingStatus.Text = $"🔴 Record... {frameCount} Fram";
                _lblRecordingStatus.ForeColor = Color.FromArgb(231, 76, 60);
            }
            else if (frameCount > 0)
            {
                _lblRecordingStatus.Text = $"✅ Saved {frameCount} Fram";
                _lblRecordingStatus.ForeColor = Color.FromArgb(46, 204, 113);
            }
            else
            {
                _lblRecordingStatus.Text = "⚫ UnActive";
                _lblRecordingStatus.ForeColor = Color.Gray;
            }
        }
        #endregion
    }
}