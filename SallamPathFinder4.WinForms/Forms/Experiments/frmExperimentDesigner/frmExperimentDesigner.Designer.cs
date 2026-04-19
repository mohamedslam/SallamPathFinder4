#region File Header
/// <summary>
/// File: frmExperimentDesigner.Designer.cs
/// Description: Designer file for experiment designer form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner
{
    partial class frmExperimentDesigner
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;

        // Tab Control
        private System.Windows.Forms.TabControl _mainTabControl;
        private System.Windows.Forms.TabPage _tabExperimentSettings;
        private System.Windows.Forms.TabPage _tabMapSettings;
        private System.Windows.Forms.TabPage _tabAlgorithms;
        private System.Windows.Forms.TabPage _tabRobotSettings;
        private System.Windows.Forms.TabPage _tabMLSettings;

        // Bottom Panel Controls
        private System.Windows.Forms.Panel _bottomPanel;
        private System.Windows.Forms.Panel _buttonPanel;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Button _btnRunComparison;
        private System.Windows.Forms.Button _btnSaveSettings;
        private System.Windows.Forms.Button _btnLoadSettings;
        private System.Windows.Forms.Button _btnCancel;

        // Tab 1: Experiment Settings Controls
        private System.Windows.Forms.GroupBox _grpExperimentSettings;
        private System.Windows.Forms.Label _lblExpName;
        private System.Windows.Forms.TextBox _txtExperimentName;
        private System.Windows.Forms.Label _lblIterations;
        private System.Windows.Forms.NumericUpDown _nudIterations;
        private System.Windows.Forms.CheckBox _chkSaveScreenshots;
        private System.Windows.Forms.CheckBox _chkSaveReplay;
        private System.Windows.Forms.CheckBox _chkShowPathOnScreenshots;
        private System.Windows.Forms.Label _lblSavePath;
        private System.Windows.Forms.TextBox _txtSavePath;
        private System.Windows.Forms.Button _btnBrowseSavePath;
        private System.Windows.Forms.GroupBox _grpInfo;
        private System.Windows.Forms.Label _lblInfo;

        // Tab 2: Map Settings Controls
        private System.Windows.Forms.GroupBox _grpMapSource;
        private System.Windows.Forms.RadioButton _rbLoadMap;
        private System.Windows.Forms.CheckBox _chkUseCurrentMap;
        private System.Windows.Forms.TextBox _txtMapFilePath;
        private System.Windows.Forms.Button _btnBrowseMap;
        private System.Windows.Forms.GroupBox _grpMapProperties;
        private System.Windows.Forms.Label _lblGoals;
        private System.Windows.Forms.NumericUpDown _nudGoalCount;
        private System.Windows.Forms.Label _lblParking;
        private System.Windows.Forms.NumericUpDown _nudParkingCount;
        private System.Windows.Forms.Label _lblStatic;
        private System.Windows.Forms.NumericUpDown _nudStaticObstacles;
        private System.Windows.Forms.Label _lblDynamic;
        private System.Windows.Forms.NumericUpDown _nudDynamicObstacles;

        // NEW: Start Point Controls (أضيفت داخل _grpMapProperties)
        private System.Windows.Forms.CheckBox _chkUseCustomStartPoint;
        private System.Windows.Forms.Label _lblCurrentStartPoint;
        private System.Windows.Forms.Button _btnPickStartPoint;

        // Tab 2: Robot Properties (ضمن Tab Map Settings)
        private System.Windows.Forms.GroupBox _grpRobotProperties;
        private System.Windows.Forms.Label _lblRobotName;
        private System.Windows.Forms.TextBox _txtRobotName;
        private System.Windows.Forms.Label _lblRobotSpeed;
        private System.Windows.Forms.NumericUpDown _nudRobotSpeed;
        private System.Windows.Forms.Label _lblRobotBattery;
        private System.Windows.Forms.NumericUpDown _nudRobotBattery;
        private System.Windows.Forms.Label _lblConsumption;
        private System.Windows.Forms.NumericUpDown _nudConsumptionRate;
        private System.Windows.Forms.Label _lblViewAngle;
        private System.Windows.Forms.NumericUpDown _nudViewAngle;
        private System.Windows.Forms.Label _lblDetection;
        private System.Windows.Forms.NumericUpDown _nudDetectionRange;

        // NEW: Dynamic Charging Controls (أضيفت داخل _grpRobotProperties)
        private System.Windows.Forms.CheckBox _chkEnableDynamicCharging;
        private System.Windows.Forms.Label _lblChargingTime;
        private System.Windows.Forms.NumericUpDown _nudChargingTime;
        private System.Windows.Forms.Label _lblChargingTimeUnit;
        private System.Windows.Forms.Label _lblSafetyMargin;
        private System.Windows.Forms.NumericUpDown _nudSafetyMargin;
        private System.Windows.Forms.Label _lblSafetyMarginUnit;

        // Tab 3: Algorithms Controls
        private System.Windows.Forms.GroupBox _grpAlgorithmSelection;
        private System.Windows.Forms.CheckBox _chkAStar;
        private System.Windows.Forms.CheckBox _chkSPPA;
        private System.Windows.Forms.CheckBox _chkSPPA_DL;
        private System.Windows.Forms.CheckBox _chkACO;
        private System.Windows.Forms.CheckBox _chkDStar;
        private System.Windows.Forms.CheckBox _chkKNN;
        private System.Windows.Forms.CheckBox _chkBruteForce;
        private System.Windows.Forms.GroupBox _grpAlgorithmParameters;
        private System.Windows.Forms.Label _lblHeuristicWeight;
        private System.Windows.Forms.NumericUpDown _nudHeuristicWeight;
        private System.Windows.Forms.Label _lblSearchLimit;
        private System.Windows.Forms.NumericUpDown _nudSearchLimit;
        private System.Windows.Forms.CheckBox _chkAllowDiagonals;
        private System.Windows.Forms.CheckBox _chkHeavyDiagonals;

        // NEW: Goal Ordering (أضيفت داخل _grpAlgorithmParameters)
        private System.Windows.Forms.CheckBox _chkOrderGoalsByDistance;

        // Old Metrics (مبقي للتوافق ولكن مخفي)
        private System.Windows.Forms.Label _lblMetrics;
        private System.Windows.Forms.CheckedListBox _clbDistanceMetrics;

        // Tab 4: ML Settings Controls
        private System.Windows.Forms.GroupBox _grpMLSettings;
        private System.Windows.Forms.CheckBox _chkEnableDynamicLearning;
        private System.Windows.Forms.Label _lblLearningRate;
        private System.Windows.Forms.NumericUpDown _nudLearningRate;
        private System.Windows.Forms.CheckBox _chkUseNeuralNetwork;
        private System.Windows.Forms.CheckBox _chkCollectTrainingData;
        private System.Windows.Forms.CheckBox _chkTrainBeforeExperiment;
        private System.Windows.Forms.Button _btnTrainNow;
        private System.Windows.Forms.ProgressBar _prgTraining;
        private System.Windows.Forms.Label _lblTrainingStatus;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExperimentDesigner));
            _mainTabControl = new TabControl();
            _tabAlgorithms = new TabPage();
            label1 = new Label();
            _lblMetrics = new Label();
            _grpAlgorithmSelection = new GroupBox();
            _chkAStar = new CheckBox();
            _chkSPPA = new CheckBox();
            _chkSPPA_DL = new CheckBox();
            _chkACO = new CheckBox();
            _chkDStar = new CheckBox();
            _chkKNN = new CheckBox();
            _chkBruteForce = new CheckBox();
            _grpAlgorithmParameters = new GroupBox();
            _chkHeavyDiagonals = new CheckBox();
            _lblHeuristicWeight = new Label();
            _nudHeuristicWeight = new NumericUpDown();
            _lblSearchLimit = new Label();
            _nudSearchLimit = new NumericUpDown();
            _chkAllowDiagonals = new CheckBox();
            _chkOrderGoalsByDistance = new CheckBox();
            _grpMLSettings = new GroupBox();
            _chkTrainBeforeExperiment = new CheckBox();
            _chkEnableDynamicLearning = new CheckBox();
            _lblLearningRate = new Label();
            _nudLearningRate = new NumericUpDown();
            _chkUseNeuralNetwork = new CheckBox();
            _chkCollectTrainingData = new CheckBox();
            _btnTrainNow = new Button();
            _prgTraining = new ProgressBar();
            _lblTrainingStatus = new Label();
            _clbDistanceMetrics = new CheckedListBox();
            _tabMapSettings = new TabPage();
            _grpMapSource = new GroupBox();
            _rbLoadMap = new RadioButton();
            _chkUseCurrentMap = new CheckBox();
            _txtMapFilePath = new TextBox();
            _btnBrowseMap = new Button();
            _grpMapProperties = new GroupBox();
            _lblGoals = new Label();
            _nudGoalCount = new NumericUpDown();
            _lblParking = new Label();
            _nudParkingCount = new NumericUpDown();
            _lblStatic = new Label();
            _nudStaticObstacles = new NumericUpDown();
            _lblDynamic = new Label();
            _nudDynamicObstacles = new NumericUpDown();
            _chkUseCustomStartPoint = new CheckBox();
            _lblCurrentStartPoint = new Label();
            _btnPickStartPoint = new Button();
            _grpRobotProperties = new GroupBox();
            _lblRobotName = new Label();
            _txtRobotName = new TextBox();
            _lblRobotSpeed = new Label();
            _nudRobotSpeed = new NumericUpDown();
            _lblRobotBattery = new Label();
            _nudRobotBattery = new NumericUpDown();
            _lblConsumption = new Label();
            _nudConsumptionRate = new NumericUpDown();
            _lblViewAngle = new Label();
            _nudViewAngle = new NumericUpDown();
            _lblDetection = new Label();
            _nudDetectionRange = new NumericUpDown();
            _chkEnableDynamicCharging = new CheckBox();
            _lblChargingTime = new Label();
            _nudChargingTime = new NumericUpDown();
            _lblChargingTimeUnit = new Label();
            _lblSafetyMargin = new Label();
            _nudSafetyMargin = new NumericUpDown();
            _lblSafetyMarginUnit = new Label();
            _tabExperimentSettings = new TabPage();
            _grpExperimentSettings = new GroupBox();
            _lblExpName = new Label();
            _txtExperimentName = new TextBox();
            _lblIterations = new Label();
            _nudIterations = new NumericUpDown();
            _chkSaveScreenshots = new CheckBox();
            _chkSaveReplay = new CheckBox();
            _chkShowPathOnScreenshots = new CheckBox();
            _lblSavePath = new Label();
            _txtSavePath = new TextBox();
            _btnBrowseSavePath = new Button();
            _grpInfo = new GroupBox();
            _lblInfo = new Label();
            _tabRobotSettings = new TabPage();
            _tabMLSettings = new TabPage();
            _bottomPanel = new Panel();
            _buttonPanel = new Panel();
            _btnRunComparison = new Button();
            _btnSaveSettings = new Button();
            _btnLoadSettings = new Button();
            _btnCancel = new Button();
            _lblStatus = new Label();
            _progressBar = new ProgressBar();
            _mainTabControl.SuspendLayout();
            _tabAlgorithms.SuspendLayout();
            _grpAlgorithmSelection.SuspendLayout();
            _grpAlgorithmParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudHeuristicWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudSearchLimit).BeginInit();
            _grpMLSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudLearningRate).BeginInit();
            _tabMapSettings.SuspendLayout();
            _grpMapSource.SuspendLayout();
            _grpMapProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudGoalCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudParkingCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudStaticObstacles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudDynamicObstacles).BeginInit();
            _grpRobotProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudRobotSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotBattery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudConsumptionRate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudViewAngle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudDetectionRange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudChargingTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudSafetyMargin).BeginInit();
            _tabExperimentSettings.SuspendLayout();
            _grpExperimentSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudIterations).BeginInit();
            _grpInfo.SuspendLayout();
            _bottomPanel.SuspendLayout();
            _buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _mainTabControl
            // 
            _mainTabControl.Controls.Add(_tabAlgorithms);
            _mainTabControl.Controls.Add(_tabMapSettings);
            _mainTabControl.Controls.Add(_tabExperimentSettings);
            _mainTabControl.Dock = DockStyle.Fill;
            _mainTabControl.Location = new Point(0, 0);
            _mainTabControl.Name = "_mainTabControl";
            _mainTabControl.SelectedIndex = 0;
            _mainTabControl.Size = new Size(612, 469);
            _mainTabControl.TabIndex = 0;
            // 
            // _tabAlgorithms
            // 
            _tabAlgorithms.Controls.Add(_clbDistanceMetrics);
            _tabAlgorithms.Controls.Add(label1);
            _tabAlgorithms.Controls.Add(_lblMetrics);
            _tabAlgorithms.Controls.Add(_grpAlgorithmSelection);
            _tabAlgorithms.Controls.Add(_grpAlgorithmParameters);
            _tabAlgorithms.Controls.Add(_grpMLSettings);
            _tabAlgorithms.Location = new Point(4, 24);
            _tabAlgorithms.Name = "_tabAlgorithms";
            _tabAlgorithms.Size = new Size(604, 441);
            _tabAlgorithms.TabIndex = 2;
            _tabAlgorithms.Text = "Algorithms & ML";
            // 
            // label1
            // 
            label1.Location = new Point(324, 323);
            label1.Name = "label1";
            label1.Size = new Size(251, 82);
            label1.TabIndex = 9;
            label1.Text = "Manhattan            (|dx| + |dy|)\r\nEuclidean               (√(dx² + dy²))\r\nMaxDXDY              (max(|dx|, |dy|))\r\nDiagonalShortcut  (2·min(dx,dy) + |dx-dy|)\r\nEuclideanNoSQR    (dx² + dy²)";
            // 
            // _lblMetrics
            // 
            _lblMetrics.Location = new Point(12, 299);
            _lblMetrics.Name = "_lblMetrics";
            _lblMetrics.Size = new Size(100, 20);
            _lblMetrics.TabIndex = 9;
            _lblMetrics.Text = "Distance Metrics:";
            // 
            // _grpAlgorithmSelection
            // 
            _grpAlgorithmSelection.Controls.Add(_chkAStar);
            _grpAlgorithmSelection.Controls.Add(_chkSPPA);
            _grpAlgorithmSelection.Controls.Add(_chkSPPA_DL);
            _grpAlgorithmSelection.Controls.Add(_chkACO);
            _grpAlgorithmSelection.Controls.Add(_chkDStar);
            _grpAlgorithmSelection.Controls.Add(_chkKNN);
            _grpAlgorithmSelection.Controls.Add(_chkBruteForce);
            _grpAlgorithmSelection.Location = new Point(12, 12);
            _grpAlgorithmSelection.Name = "_grpAlgorithmSelection";
            _grpAlgorithmSelection.Size = new Size(584, 130);
            _grpAlgorithmSelection.TabIndex = 0;
            _grpAlgorithmSelection.TabStop = false;
            _grpAlgorithmSelection.Text = "Select Algorithms";
            // 
            // _chkAStar
            // 
            _chkAStar.Checked = true;
            _chkAStar.CheckState = CheckState.Checked;
            _chkAStar.Location = new Point(15, 25);
            _chkAStar.Name = "_chkAStar";
            _chkAStar.Size = new Size(220, 20);
            _chkAStar.TabIndex = 0;
            _chkAStar.Text = "A*  |  f(n) = g(n) + h(n)";
            // 
            // _chkSPPA
            // 
            _chkSPPA.Checked = true;
            _chkSPPA.CheckState = CheckState.Checked;
            _chkSPPA.Location = new Point(15, 50);
            _chkSPPA.Name = "_chkSPPA";
            _chkSPPA.Size = new Size(260, 20);
            _chkSPPA.TabIndex = 1;
            _chkSPPA.Text = "SPPA  |  f(n) = g(n) + h(n) + λ·o(n)";
            // 
            // _chkSPPA_DL
            // 
            _chkSPPA_DL.Location = new Point(15, 75);
            _chkSPPA_DL.Name = "_chkSPPA_DL";
            _chkSPPA_DL.Size = new Size(299, 20);
            _chkSPPA_DL.TabIndex = 2;
            _chkSPPA_DL.Text = "SPPA-DL  |  f(n) = g(n) + h(n) + λ·o(n) + α·m(n)";
            // 
            // _chkACO
            // 
            _chkACO.Location = new Point(338, 24);
            _chkACO.Name = "_chkACO";
            _chkACO.Size = new Size(225, 20);
            _chkACO.TabIndex = 3;
            _chkACO.Text = "ACO  |  P_ij = [τ_ij]^α · [η_ij]^β / Σ...";
            // 
            // _chkDStar
            // 
            _chkDStar.Location = new Point(338, 49);
            _chkDStar.Name = "_chkDStar";
            _chkDStar.Size = new Size(202, 20);
            _chkDStar.TabIndex = 4;
            _chkDStar.Text = "D*  |  f(n) = g(n) + h(n) + d(n)";
            // 
            // _chkKNN
            // 
            _chkKNN.Location = new Point(338, 74);
            _chkKNN.Name = "_chkKNN";
            _chkKNN.Size = new Size(240, 20);
            _chkKNN.TabIndex = 5;
            _chkKNN.Text = "KNN  |  d(x,y) = √Σ(x_i - y_i)²";
            // 
            // _chkBruteForce
            // 
            _chkBruteForce.Location = new Point(338, 99);
            _chkBruteForce.Name = "_chkBruteForce";
            _chkBruteForce.Size = new Size(240, 20);
            _chkBruteForce.TabIndex = 6;
            _chkBruteForce.Text = "Brute Force  |  min Σ cost(path)";
            // 
            // _grpAlgorithmParameters
            // 
            _grpAlgorithmParameters.Controls.Add(_chkHeavyDiagonals);
            _grpAlgorithmParameters.Controls.Add(_lblHeuristicWeight);
            _grpAlgorithmParameters.Controls.Add(_nudHeuristicWeight);
            _grpAlgorithmParameters.Controls.Add(_lblSearchLimit);
            _grpAlgorithmParameters.Controls.Add(_nudSearchLimit);
            _grpAlgorithmParameters.Controls.Add(_chkAllowDiagonals);
            _grpAlgorithmParameters.Controls.Add(_chkOrderGoalsByDistance);
            _grpAlgorithmParameters.Location = new Point(12, 150);
            _grpAlgorithmParameters.Name = "_grpAlgorithmParameters";
            _grpAlgorithmParameters.Size = new Size(314, 139);
            _grpAlgorithmParameters.TabIndex = 1;
            _grpAlgorithmParameters.TabStop = false;
            _grpAlgorithmParameters.Text = "Algorithm Parameters";
            // 
            // _chkHeavyDiagonals
            // 
            _chkHeavyDiagonals.Location = new Point(123, 85);
            _chkHeavyDiagonals.Name = "_chkHeavyDiagonals";
            _chkHeavyDiagonals.Size = new Size(188, 20);
            _chkHeavyDiagonals.TabIndex = 5;
            _chkHeavyDiagonals.Text = "Heavy Diagonals (Higher Cost)";
            // 
            // _lblHeuristicWeight
            // 
            _lblHeuristicWeight.Location = new Point(15, 25);
            _lblHeuristicWeight.Name = "_lblHeuristicWeight";
            _lblHeuristicWeight.Size = new Size(120, 20);
            _lblHeuristicWeight.TabIndex = 0;
            _lblHeuristicWeight.Text = "Heuristic Weight (h):";
            // 
            // _nudHeuristicWeight
            // 
            _nudHeuristicWeight.Location = new Point(140, 22);
            _nudHeuristicWeight.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudHeuristicWeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudHeuristicWeight.Name = "_nudHeuristicWeight";
            _nudHeuristicWeight.Size = new Size(60, 23);
            _nudHeuristicWeight.TabIndex = 1;
            _nudHeuristicWeight.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // _lblSearchLimit
            // 
            _lblSearchLimit.Location = new Point(15, 55);
            _lblSearchLimit.Name = "_lblSearchLimit";
            _lblSearchLimit.Size = new Size(114, 20);
            _lblSearchLimit.TabIndex = 2;
            _lblSearchLimit.Text = "Search Limit (max nodes):";
            // 
            // _nudSearchLimit
            // 
            _nudSearchLimit.Location = new Point(140, 52);
            _nudSearchLimit.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            _nudSearchLimit.Minimum = new decimal(new int[] { 1000, 0, 0, 0 });
            _nudSearchLimit.Name = "_nudSearchLimit";
            _nudSearchLimit.Size = new Size(60, 23);
            _nudSearchLimit.TabIndex = 3;
            _nudSearchLimit.Value = new decimal(new int[] { 20000, 0, 0, 0 });
            // 
            // _chkAllowDiagonals
            // 
            _chkAllowDiagonals.Checked = true;
            _chkAllowDiagonals.CheckState = CheckState.Checked;
            _chkAllowDiagonals.Location = new Point(15, 85);
            _chkAllowDiagonals.Name = "_chkAllowDiagonals";
            _chkAllowDiagonals.Size = new Size(114, 20);
            _chkAllowDiagonals.TabIndex = 4;
            _chkAllowDiagonals.Text = "Allow Diagonal Movement";
            // 
            // _chkOrderGoalsByDistance
            // 
            _chkOrderGoalsByDistance.Location = new Point(15, 115);
            _chkOrderGoalsByDistance.Name = "_chkOrderGoalsByDistance";
            _chkOrderGoalsByDistance.Size = new Size(220, 20);
            _chkOrderGoalsByDistance.TabIndex = 6;
            _chkOrderGoalsByDistance.Text = "Order Goals by Distance from Start";
            // 
            // _grpMLSettings
            // 
            _grpMLSettings.Controls.Add(_chkTrainBeforeExperiment);
            _grpMLSettings.Controls.Add(_chkEnableDynamicLearning);
            _grpMLSettings.Controls.Add(_lblLearningRate);
            _grpMLSettings.Controls.Add(_nudLearningRate);
            _grpMLSettings.Controls.Add(_chkUseNeuralNetwork);
            _grpMLSettings.Controls.Add(_chkCollectTrainingData);
            _grpMLSettings.Controls.Add(_btnTrainNow);
            _grpMLSettings.Controls.Add(_prgTraining);
            _grpMLSettings.Controls.Add(_lblTrainingStatus);
            _grpMLSettings.Location = new Point(336, 150);
            _grpMLSettings.Name = "_grpMLSettings";
            _grpMLSettings.Size = new Size(261, 158);
            _grpMLSettings.TabIndex = 2;
            _grpMLSettings.TabStop = false;
            _grpMLSettings.Text = "Machine Learning Options (SPPA-DL only)";
            // 
            // _chkTrainBeforeExperiment
            // 
            _chkTrainBeforeExperiment.Location = new Point(150, 102);
            _chkTrainBeforeExperiment.Name = "_chkTrainBeforeExperiment";
            _chkTrainBeforeExperiment.Size = new Size(105, 20);
            _chkTrainBeforeExperiment.TabIndex = 5;
            _chkTrainBeforeExperiment.Text = "Train Model Before Experiment";
            // 
            // _chkEnableDynamicLearning
            // 
            _chkEnableDynamicLearning.Checked = true;
            _chkEnableDynamicLearning.CheckState = CheckState.Checked;
            _chkEnableDynamicLearning.Location = new Point(15, 25);
            _chkEnableDynamicLearning.Name = "_chkEnableDynamicLearning";
            _chkEnableDynamicLearning.Size = new Size(200, 20);
            _chkEnableDynamicLearning.TabIndex = 0;
            _chkEnableDynamicLearning.Text = "Enable Dynamic Learning";
            // 
            // _lblLearningRate
            // 
            _lblLearningRate.Location = new Point(15, 55);
            _lblLearningRate.Name = "_lblLearningRate";
            _lblLearningRate.Size = new Size(100, 20);
            _lblLearningRate.TabIndex = 1;
            _lblLearningRate.Text = "Learning Rate (α):";
            // 
            // _nudLearningRate
            // 
            _nudLearningRate.DecimalPlaces = 1;
            _nudLearningRate.Location = new Point(120, 52);
            _nudLearningRate.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudLearningRate.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            _nudLearningRate.Name = "_nudLearningRate";
            _nudLearningRate.Size = new Size(60, 23);
            _nudLearningRate.TabIndex = 2;
            _nudLearningRate.Value = new decimal(new int[] { 20, 0, 0, 65536 });
            // 
            // _chkUseNeuralNetwork
            // 
            _chkUseNeuralNetwork.Location = new Point(15, 77);
            _chkUseNeuralNetwork.Name = "_chkUseNeuralNetwork";
            _chkUseNeuralNetwork.Size = new Size(220, 20);
            _chkUseNeuralNetwork.TabIndex = 3;
            _chkUseNeuralNetwork.Text = "Use Neural Network Prediction";
            // 
            // _chkCollectTrainingData
            // 
            _chkCollectTrainingData.Location = new Point(15, 102);
            _chkCollectTrainingData.Name = "_chkCollectTrainingData";
            _chkCollectTrainingData.Size = new Size(140, 20);
            _chkCollectTrainingData.TabIndex = 4;
            _chkCollectTrainingData.Text = "Collect Training Data";
            // 
            // _btnTrainNow
            // 
            _btnTrainNow.BackColor = Color.FromArgb(52, 152, 219);
            _btnTrainNow.FlatStyle = FlatStyle.Flat;
            _btnTrainNow.ForeColor = Color.White;
            _btnTrainNow.Location = new Point(15, 124);
            _btnTrainNow.Name = "_btnTrainNow";
            _btnTrainNow.Size = new Size(112, 30);
            _btnTrainNow.TabIndex = 6;
            _btnTrainNow.Text = "Train Model Now";
            _btnTrainNow.UseVisualStyleBackColor = false;
            // 
            // _prgTraining
            // 
            _prgTraining.Location = new Point(133, 134);
            _prgTraining.Name = "_prgTraining";
            _prgTraining.Size = new Size(122, 18);
            _prgTraining.TabIndex = 7;
            _prgTraining.Visible = false;
            // 
            // _lblTrainingStatus
            // 
            _lblTrainingStatus.Location = new Point(15, 124);
            _lblTrainingStatus.Name = "_lblTrainingStatus";
            _lblTrainingStatus.Size = new Size(200, 20);
            _lblTrainingStatus.TabIndex = 8;
            _lblTrainingStatus.Visible = false;
            // 
            // _clbDistanceMetrics
            // 
            _clbDistanceMetrics.Items.AddRange(new object[] { "Manhattan (|dx| + |dy|)", "Euclidean (√(dx² + dy²))", "MaxDXDY (max(|dx|, |dy|))", "DiagonalShortcut (2·min(dx,dy) + |dx-dy|)", "EuclideanNoSQR (dx² + dy²)" });
            _clbDistanceMetrics.Location = new Point(12, 317);
            _clbDistanceMetrics.Name = "_clbDistanceMetrics";
            _clbDistanceMetrics.Size = new Size(261, 94);
            _clbDistanceMetrics.TabIndex = 10;
            // 
            // _tabMapSettings
            // 
            _tabMapSettings.Controls.Add(_grpMapSource);
            _tabMapSettings.Controls.Add(_grpMapProperties);
            _tabMapSettings.Controls.Add(_grpRobotProperties);
            _tabMapSettings.Location = new Point(4, 24);
            _tabMapSettings.Name = "_tabMapSettings";
            _tabMapSettings.Size = new Size(604, 441);
            _tabMapSettings.TabIndex = 1;
            _tabMapSettings.Text = "Map & Robot";
            // 
            // _grpMapSource
            // 
            _grpMapSource.Controls.Add(_rbLoadMap);
            _grpMapSource.Controls.Add(_chkUseCurrentMap);
            _grpMapSource.Controls.Add(_txtMapFilePath);
            _grpMapSource.Controls.Add(_btnBrowseMap);
            _grpMapSource.Location = new Point(60, 19);
            _grpMapSource.Name = "_grpMapSource";
            _grpMapSource.Size = new Size(451, 90);
            _grpMapSource.TabIndex = 0;
            _grpMapSource.TabStop = false;
            _grpMapSource.Text = "Map Source";
            // 
            // _rbLoadMap
            // 
            _rbLoadMap.Checked = true;
            _rbLoadMap.Location = new Point(15, 25);
            _rbLoadMap.Name = "_rbLoadMap";
            _rbLoadMap.Size = new Size(120, 20);
            _rbLoadMap.TabIndex = 0;
            _rbLoadMap.TabStop = true;
            _rbLoadMap.Text = "Load Existing Map";
            // 
            // _chkUseCurrentMap
            // 
            _chkUseCurrentMap.Checked = true;
            _chkUseCurrentMap.CheckState = CheckState.Checked;
            _chkUseCurrentMap.Location = new Point(150, 25);
            _chkUseCurrentMap.Name = "_chkUseCurrentMap";
            _chkUseCurrentMap.Size = new Size(200, 20);
            _chkUseCurrentMap.TabIndex = 1;
            _chkUseCurrentMap.Text = "Use current map from main form";
            // 
            // _txtMapFilePath
            // 
            _txtMapFilePath.Enabled = false;
            _txtMapFilePath.Location = new Point(15, 50);
            _txtMapFilePath.Name = "_txtMapFilePath";
            _txtMapFilePath.Size = new Size(320, 23);
            _txtMapFilePath.TabIndex = 2;
            // 
            // _btnBrowseMap
            // 
            _btnBrowseMap.BackColor = Color.FromArgb(52, 152, 219);
            _btnBrowseMap.Enabled = false;
            _btnBrowseMap.FlatStyle = FlatStyle.Flat;
            _btnBrowseMap.ForeColor = Color.White;
            _btnBrowseMap.Location = new Point(340, 48);
            _btnBrowseMap.Name = "_btnBrowseMap";
            _btnBrowseMap.Size = new Size(40, 25);
            _btnBrowseMap.TabIndex = 3;
            _btnBrowseMap.Text = "...";
            _btnBrowseMap.UseVisualStyleBackColor = false;
            // 
            // _grpMapProperties
            // 
            _grpMapProperties.Controls.Add(_lblGoals);
            _grpMapProperties.Controls.Add(_nudGoalCount);
            _grpMapProperties.Controls.Add(_lblParking);
            _grpMapProperties.Controls.Add(_nudParkingCount);
            _grpMapProperties.Controls.Add(_lblStatic);
            _grpMapProperties.Controls.Add(_nudStaticObstacles);
            _grpMapProperties.Controls.Add(_lblDynamic);
            _grpMapProperties.Controls.Add(_nudDynamicObstacles);
            _grpMapProperties.Controls.Add(_chkUseCustomStartPoint);
            _grpMapProperties.Controls.Add(_lblCurrentStartPoint);
            _grpMapProperties.Controls.Add(_btnPickStartPoint);
            _grpMapProperties.Location = new Point(60, 115);
            _grpMapProperties.Name = "_grpMapProperties";
            _grpMapProperties.Size = new Size(451, 130);
            _grpMapProperties.TabIndex = 1;
            _grpMapProperties.TabStop = false;
            _grpMapProperties.Text = "Map Properties";
            // 
            // _lblGoals
            // 
            _lblGoals.Location = new Point(15, 25);
            _lblGoals.Name = "_lblGoals";
            _lblGoals.Size = new Size(70, 20);
            _lblGoals.TabIndex = 0;
            _lblGoals.Text = "Goal Count:";
            // 
            // _nudGoalCount
            // 
            _nudGoalCount.Location = new Point(110, 22);
            _nudGoalCount.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            _nudGoalCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudGoalCount.Name = "_nudGoalCount";
            _nudGoalCount.Size = new Size(60, 23);
            _nudGoalCount.TabIndex = 1;
            _nudGoalCount.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // _lblParking
            // 
            _lblParking.Location = new Point(219, 26);
            _lblParking.Name = "_lblParking";
            _lblParking.Size = new Size(80, 20);
            _lblParking.TabIndex = 2;
            _lblParking.Text = "Parking Count:";
            // 
            // _nudParkingCount
            // 
            _nudParkingCount.Location = new Point(326, 23);
            _nudParkingCount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudParkingCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudParkingCount.Name = "_nudParkingCount";
            _nudParkingCount.Size = new Size(60, 23);
            _nudParkingCount.TabIndex = 3;
            _nudParkingCount.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // _lblStatic
            // 
            _lblStatic.Location = new Point(15, 55);
            _lblStatic.Name = "_lblStatic";
            _lblStatic.Size = new Size(90, 20);
            _lblStatic.TabIndex = 4;
            _lblStatic.Text = "Static Obstacles:";
            // 
            // _nudStaticObstacles
            // 
            _nudStaticObstacles.Location = new Point(110, 52);
            _nudStaticObstacles.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _nudStaticObstacles.Name = "_nudStaticObstacles";
            _nudStaticObstacles.Size = new Size(60, 23);
            _nudStaticObstacles.TabIndex = 5;
            _nudStaticObstacles.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // _lblDynamic
            // 
            _lblDynamic.Location = new Point(219, 56);
            _lblDynamic.Name = "_lblDynamic";
            _lblDynamic.Size = new Size(100, 20);
            _lblDynamic.TabIndex = 6;
            _lblDynamic.Text = "Dynamic Obstacles:";
            // 
            // _nudDynamicObstacles
            // 
            _nudDynamicObstacles.Location = new Point(326, 53);
            _nudDynamicObstacles.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            _nudDynamicObstacles.Name = "_nudDynamicObstacles";
            _nudDynamicObstacles.Size = new Size(60, 23);
            _nudDynamicObstacles.TabIndex = 7;
            _nudDynamicObstacles.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // _chkUseCustomStartPoint
            // 
            _chkUseCustomStartPoint.Location = new Point(15, 85);
            _chkUseCustomStartPoint.Name = "_chkUseCustomStartPoint";
            _chkUseCustomStartPoint.Size = new Size(150, 20);
            _chkUseCustomStartPoint.TabIndex = 8;
            _chkUseCustomStartPoint.Text = "Use Custom Start Point";
            // 
            // _lblCurrentStartPoint
            // 
            _lblCurrentStartPoint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            _lblCurrentStartPoint.Location = new Point(15, 108);
            _lblCurrentStartPoint.Name = "_lblCurrentStartPoint";
            _lblCurrentStartPoint.Size = new Size(120, 20);
            _lblCurrentStartPoint.TabIndex = 9;
            _lblCurrentStartPoint.Tag = new Point(10, 10);
            _lblCurrentStartPoint.Text = "Current: (10, 10)";
            // 
            // _btnPickStartPoint
            // 
            _btnPickStartPoint.BackColor = Color.FromArgb(52, 152, 219);
            _btnPickStartPoint.Cursor = Cursors.Hand;
            _btnPickStartPoint.FlatStyle = FlatStyle.Flat;
            _btnPickStartPoint.ForeColor = Color.White;
            _btnPickStartPoint.Location = new Point(140, 105);
            _btnPickStartPoint.Name = "_btnPickStartPoint";
            _btnPickStartPoint.Size = new Size(100, 25);
            _btnPickStartPoint.TabIndex = 10;
            _btnPickStartPoint.Text = "Pick from Map";
            _btnPickStartPoint.UseVisualStyleBackColor = false;
            // 
            // _grpRobotProperties
            // 
            _grpRobotProperties.Controls.Add(_lblRobotName);
            _grpRobotProperties.Controls.Add(_txtRobotName);
            _grpRobotProperties.Controls.Add(_lblRobotSpeed);
            _grpRobotProperties.Controls.Add(_nudRobotSpeed);
            _grpRobotProperties.Controls.Add(_lblRobotBattery);
            _grpRobotProperties.Controls.Add(_nudRobotBattery);
            _grpRobotProperties.Controls.Add(_lblConsumption);
            _grpRobotProperties.Controls.Add(_nudConsumptionRate);
            _grpRobotProperties.Controls.Add(_lblViewAngle);
            _grpRobotProperties.Controls.Add(_nudViewAngle);
            _grpRobotProperties.Controls.Add(_lblDetection);
            _grpRobotProperties.Controls.Add(_nudDetectionRange);
            _grpRobotProperties.Controls.Add(_chkEnableDynamicCharging);
            _grpRobotProperties.Controls.Add(_lblChargingTime);
            _grpRobotProperties.Controls.Add(_nudChargingTime);
            _grpRobotProperties.Controls.Add(_lblChargingTimeUnit);
            _grpRobotProperties.Controls.Add(_lblSafetyMargin);
            _grpRobotProperties.Controls.Add(_nudSafetyMargin);
            _grpRobotProperties.Controls.Add(_lblSafetyMarginUnit);
            _grpRobotProperties.Location = new Point(60, 260);
            _grpRobotProperties.Name = "_grpRobotProperties";
            _grpRobotProperties.Size = new Size(451, 172);
            _grpRobotProperties.TabIndex = 2;
            _grpRobotProperties.TabStop = false;
            _grpRobotProperties.Text = "Robot Configuration";
            // 
            // _lblRobotName
            // 
            _lblRobotName.Location = new Point(15, 25);
            _lblRobotName.Name = "_lblRobotName";
            _lblRobotName.Size = new Size(80, 20);
            _lblRobotName.TabIndex = 0;
            _lblRobotName.Text = "Robot Name:";
            // 
            // _txtRobotName
            // 
            _txtRobotName.Location = new Point(120, 25);
            _txtRobotName.Name = "_txtRobotName";
            _txtRobotName.Size = new Size(80, 23);
            _txtRobotName.TabIndex = 1;
            _txtRobotName.Text = "SallamBot";
            // 
            // _lblRobotSpeed
            // 
            _lblRobotSpeed.Location = new Point(200, 25);
            _lblRobotSpeed.Name = "_lblRobotSpeed";
            _lblRobotSpeed.Size = new Size(80, 20);
            _lblRobotSpeed.TabIndex = 2;
            _lblRobotSpeed.Text = "Speed (cm/s):";
            // 
            // _nudRobotSpeed
            // 
            _nudRobotSpeed.Location = new Point(325, 23);
            _nudRobotSpeed.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudRobotSpeed.Name = "_nudRobotSpeed";
            _nudRobotSpeed.Size = new Size(60, 23);
            _nudRobotSpeed.TabIndex = 3;
            _nudRobotSpeed.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // _lblRobotBattery
            // 
            _lblRobotBattery.Location = new Point(15, 55);
            _lblRobotBattery.Name = "_lblRobotBattery";
            _lblRobotBattery.Size = new Size(70, 20);
            _lblRobotBattery.TabIndex = 4;
            _lblRobotBattery.Text = "Battery (%):";
            // 
            // _nudRobotBattery
            // 
            _nudRobotBattery.Location = new Point(120, 54);
            _nudRobotBattery.Name = "_nudRobotBattery";
            _nudRobotBattery.Size = new Size(60, 23);
            _nudRobotBattery.TabIndex = 5;
            _nudRobotBattery.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // _lblConsumption
            // 
            _lblConsumption.Location = new Point(200, 54);
            _lblConsumption.Name = "_lblConsumption";
            _lblConsumption.Size = new Size(120, 20);
            _lblConsumption.TabIndex = 6;
            _lblConsumption.Text = "Consumption (%/m):";
            // 
            // _nudConsumptionRate
            // 
            _nudConsumptionRate.DecimalPlaces = 1;
            _nudConsumptionRate.Location = new Point(325, 53);
            _nudConsumptionRate.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudConsumptionRate.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            _nudConsumptionRate.Name = "_nudConsumptionRate";
            _nudConsumptionRate.Size = new Size(60, 23);
            _nudConsumptionRate.TabIndex = 7;
            _nudConsumptionRate.Value = new decimal(new int[] { 10, 0, 0, 65536 });
            // 
            // _lblViewAngle
            // 
            _lblViewAngle.Location = new Point(15, 85);
            _lblViewAngle.Name = "_lblViewAngle";
            _lblViewAngle.Size = new Size(100, 20);
            _lblViewAngle.TabIndex = 8;
            _lblViewAngle.Text = "View Angle (deg):";
            // 
            // _nudViewAngle
            // 
            _nudViewAngle.Location = new Point(120, 82);
            _nudViewAngle.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            _nudViewAngle.Minimum = new decimal(new int[] { 45, 0, 0, 0 });
            _nudViewAngle.Name = "_nudViewAngle";
            _nudViewAngle.Size = new Size(60, 23);
            _nudViewAngle.TabIndex = 9;
            _nudViewAngle.Value = new decimal(new int[] { 180, 0, 0, 0 });
            // 
            // _lblDetection
            // 
            _lblDetection.Location = new Point(200, 85);
            _lblDetection.Name = "_lblDetection";
            _lblDetection.Size = new Size(120, 20);
            _lblDetection.TabIndex = 10;
            _lblDetection.Text = "Detection Range (cells):";
            // 
            // _nudDetectionRange
            // 
            _nudDetectionRange.Location = new Point(325, 82);
            _nudDetectionRange.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudDetectionRange.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudDetectionRange.Name = "_nudDetectionRange";
            _nudDetectionRange.Size = new Size(60, 23);
            _nudDetectionRange.TabIndex = 11;
            _nudDetectionRange.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // _chkEnableDynamicCharging
            // 
            _chkEnableDynamicCharging.Location = new Point(15, 115);
            _chkEnableDynamicCharging.Name = "_chkEnableDynamicCharging";
            _chkEnableDynamicCharging.Size = new Size(150, 20);
            _chkEnableDynamicCharging.TabIndex = 12;
            _chkEnableDynamicCharging.Text = "Enable Dynamic Charging";
            // 
            // _lblChargingTime
            // 
            _lblChargingTime.Location = new Point(15, 140);
            _lblChargingTime.Name = "_lblChargingTime";
            _lblChargingTime.Size = new Size(90, 20);
            _lblChargingTime.TabIndex = 13;
            _lblChargingTime.Text = "Charging Time:";
            // 
            // _nudChargingTime
            // 
            _nudChargingTime.Enabled = false;
            _nudChargingTime.Location = new Point(120, 136);
            _nudChargingTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            _nudChargingTime.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            _nudChargingTime.Name = "_nudChargingTime";
            _nudChargingTime.Size = new Size(60, 23);
            _nudChargingTime.TabIndex = 14;
            _nudChargingTime.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // _lblChargingTimeUnit
            // 
            _lblChargingTimeUnit.Location = new Point(200, 139);
            _lblChargingTimeUnit.Name = "_lblChargingTimeUnit";
            _lblChargingTimeUnit.Size = new Size(50, 20);
            _lblChargingTimeUnit.TabIndex = 15;
            _lblChargingTimeUnit.Text = "seconds";
            // 
            // _lblSafetyMargin
            // 
            _lblSafetyMargin.Location = new Point(200, 114);
            _lblSafetyMargin.Name = "_lblSafetyMargin";
            _lblSafetyMargin.Size = new Size(80, 20);
            _lblSafetyMargin.TabIndex = 16;
            _lblSafetyMargin.Text = "Safety Margin:";
            // 
            // _nudSafetyMargin
            // 
            _nudSafetyMargin.DecimalPlaces = 1;
            _nudSafetyMargin.Enabled = false;
            _nudSafetyMargin.Location = new Point(325, 111);
            _nudSafetyMargin.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            _nudSafetyMargin.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            _nudSafetyMargin.Name = "_nudSafetyMargin";
            _nudSafetyMargin.Size = new Size(60, 23);
            _nudSafetyMargin.TabIndex = 17;
            _nudSafetyMargin.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // _lblSafetyMarginUnit
            // 
            _lblSafetyMarginUnit.Location = new Point(391, 114);
            _lblSafetyMarginUnit.Name = "_lblSafetyMarginUnit";
            _lblSafetyMarginUnit.Size = new Size(30, 20);
            _lblSafetyMarginUnit.TabIndex = 18;
            _lblSafetyMarginUnit.Text = "%";
            // 
            // _tabExperimentSettings
            // 
            _tabExperimentSettings.Controls.Add(_grpExperimentSettings);
            _tabExperimentSettings.Controls.Add(_grpInfo);
            _tabExperimentSettings.Location = new Point(4, 24);
            _tabExperimentSettings.Name = "_tabExperimentSettings";
            _tabExperimentSettings.Size = new Size(604, 441);
            _tabExperimentSettings.TabIndex = 0;
            _tabExperimentSettings.Text = "Experiment";
            // 
            // _grpExperimentSettings
            // 
            _grpExperimentSettings.Controls.Add(_lblExpName);
            _grpExperimentSettings.Controls.Add(_txtExperimentName);
            _grpExperimentSettings.Controls.Add(_lblIterations);
            _grpExperimentSettings.Controls.Add(_nudIterations);
            _grpExperimentSettings.Controls.Add(_chkSaveScreenshots);
            _grpExperimentSettings.Controls.Add(_chkSaveReplay);
            _grpExperimentSettings.Controls.Add(_chkShowPathOnScreenshots);
            _grpExperimentSettings.Controls.Add(_lblSavePath);
            _grpExperimentSettings.Controls.Add(_txtSavePath);
            _grpExperimentSettings.Controls.Add(_btnBrowseSavePath);
            _grpExperimentSettings.Location = new Point(12, 12);
            _grpExperimentSettings.Name = "_grpExperimentSettings";
            _grpExperimentSettings.Size = new Size(584, 155);
            _grpExperimentSettings.TabIndex = 0;
            _grpExperimentSettings.TabStop = false;
            _grpExperimentSettings.Text = "Experiment Settings";
            // 
            // _lblExpName
            // 
            _lblExpName.Location = new Point(15, 25);
            _lblExpName.Name = "_lblExpName";
            _lblExpName.Size = new Size(100, 23);
            _lblExpName.TabIndex = 0;
            _lblExpName.Text = "Experiment Name:";
            // 
            // _txtExperimentName
            // 
            _txtExperimentName.Location = new Point(120, 22);
            _txtExperimentName.Name = "_txtExperimentName";
            _txtExperimentName.Size = new Size(250, 23);
            _txtExperimentName.TabIndex = 1;
            // 
            // _lblIterations
            // 
            _lblIterations.Location = new Point(15, 55);
            _lblIterations.Name = "_lblIterations";
            _lblIterations.Size = new Size(70, 23);
            _lblIterations.TabIndex = 2;
            _lblIterations.Text = "Iterations:";
            // 
            // _nudIterations
            // 
            _nudIterations.Location = new Point(90, 52);
            _nudIterations.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudIterations.Name = "_nudIterations";
            _nudIterations.Size = new Size(80, 23);
            _nudIterations.TabIndex = 3;
            _nudIterations.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // _chkSaveScreenshots
            // 
            _chkSaveScreenshots.Checked = true;
            _chkSaveScreenshots.CheckState = CheckState.Checked;
            _chkSaveScreenshots.Location = new Point(180, 52);
            _chkSaveScreenshots.Name = "_chkSaveScreenshots";
            _chkSaveScreenshots.Size = new Size(120, 23);
            _chkSaveScreenshots.TabIndex = 4;
            _chkSaveScreenshots.Text = "Save Screenshots";
            // 
            // _chkSaveReplay
            // 
            _chkSaveReplay.Checked = true;
            _chkSaveReplay.CheckState = CheckState.Checked;
            _chkSaveReplay.Location = new Point(310, 52);
            _chkSaveReplay.Name = "_chkSaveReplay";
            _chkSaveReplay.Size = new Size(100, 23);
            _chkSaveReplay.TabIndex = 5;
            _chkSaveReplay.Text = "Save Replay";
            // 
            // _chkShowPathOnScreenshots
            // 
            _chkShowPathOnScreenshots.Checked = true;
            _chkShowPathOnScreenshots.CheckState = CheckState.Checked;
            _chkShowPathOnScreenshots.Location = new Point(90, 85);
            _chkShowPathOnScreenshots.Name = "_chkShowPathOnScreenshots";
            _chkShowPathOnScreenshots.Size = new Size(180, 23);
            _chkShowPathOnScreenshots.TabIndex = 6;
            _chkShowPathOnScreenshots.Text = "Show Path on Screenshots";
            // 
            // _lblSavePath
            // 
            _lblSavePath.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _lblSavePath.Location = new Point(15, 115);
            _lblSavePath.Name = "_lblSavePath";
            _lblSavePath.Size = new Size(90, 23);
            _lblSavePath.TabIndex = 7;
            _lblSavePath.Text = "Save Location:";
            // 
            // _txtSavePath
            // 
            _txtSavePath.BackColor = Color.WhiteSmoke;
            _txtSavePath.Location = new Point(110, 112);
            _txtSavePath.Name = "_txtSavePath";
            _txtSavePath.ReadOnly = true;
            _txtSavePath.Size = new Size(260, 23);
            _txtSavePath.TabIndex = 8;
            // 
            // _btnBrowseSavePath
            // 
            _btnBrowseSavePath.BackColor = Color.FromArgb(52, 152, 219);
            _btnBrowseSavePath.FlatStyle = FlatStyle.Flat;
            _btnBrowseSavePath.ForeColor = Color.White;
            _btnBrowseSavePath.Location = new Point(376, 110);
            _btnBrowseSavePath.Name = "_btnBrowseSavePath";
            _btnBrowseSavePath.Size = new Size(40, 25);
            _btnBrowseSavePath.TabIndex = 9;
            _btnBrowseSavePath.Text = "...";
            _btnBrowseSavePath.UseVisualStyleBackColor = false;
            // 
            // _grpInfo
            // 
            _grpInfo.Controls.Add(_lblInfo);
            _grpInfo.Location = new Point(12, 175);
            _grpInfo.Name = "_grpInfo";
            _grpInfo.Size = new Size(584, 97);
            _grpInfo.TabIndex = 1;
            _grpInfo.TabStop = false;
            _grpInfo.Text = "Information";
            // 
            // _lblInfo
            // 
            _lblInfo.ForeColor = Color.DarkBlue;
            _lblInfo.Location = new Point(15, 22);
            _lblInfo.Name = "_lblInfo";
            _lblInfo.Size = new Size(590, 60);
            _lblInfo.TabIndex = 0;
            _lblInfo.Text = resources.GetString("_lblInfo.Text");
            // 
            // _tabRobotSettings
            // 
            _tabRobotSettings.Location = new Point(0, 0);
            _tabRobotSettings.Name = "_tabRobotSettings";
            _tabRobotSettings.Size = new Size(200, 100);
            _tabRobotSettings.TabIndex = 0;
            // 
            // _tabMLSettings
            // 
            _tabMLSettings.Location = new Point(0, 0);
            _tabMLSettings.Name = "_tabMLSettings";
            _tabMLSettings.Size = new Size(200, 100);
            _tabMLSettings.TabIndex = 0;
            // 
            // _bottomPanel
            // 
            _bottomPanel.BackColor = Color.FromArgb(240, 242, 245);
            _bottomPanel.Controls.Add(_buttonPanel);
            _bottomPanel.Controls.Add(_lblStatus);
            _bottomPanel.Controls.Add(_progressBar);
            _bottomPanel.Dock = DockStyle.Bottom;
            _bottomPanel.Location = new Point(0, 469);
            _bottomPanel.Name = "_bottomPanel";
            _bottomPanel.Padding = new Padding(5);
            _bottomPanel.Size = new Size(612, 92);
            _bottomPanel.TabIndex = 1;
            // 
            // _buttonPanel
            // 
            _buttonPanel.Controls.Add(_btnRunComparison);
            _buttonPanel.Controls.Add(_btnSaveSettings);
            _buttonPanel.Controls.Add(_btnLoadSettings);
            _buttonPanel.Controls.Add(_btnCancel);
            _buttonPanel.Dock = DockStyle.Top;
            _buttonPanel.Location = new Point(5, 50);
            _buttonPanel.Name = "_buttonPanel";
            _buttonPanel.Size = new Size(602, 35);
            _buttonPanel.TabIndex = 0;
            // 
            // _btnRunComparison
            // 
            _btnRunComparison.BackColor = Color.FromArgb(46, 204, 113);
            _btnRunComparison.Cursor = Cursors.Hand;
            _btnRunComparison.FlatStyle = FlatStyle.Flat;
            _btnRunComparison.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnRunComparison.ForeColor = Color.White;
            _btnRunComparison.Location = new Point(10, 3);
            _btnRunComparison.Name = "_btnRunComparison";
            _btnRunComparison.Size = new Size(130, 30);
            _btnRunComparison.TabIndex = 0;
            _btnRunComparison.Text = "Run Comparison";
            _btnRunComparison.UseVisualStyleBackColor = false;
            // 
            // _btnSaveSettings
            // 
            _btnSaveSettings.BackColor = Color.FromArgb(52, 152, 219);
            _btnSaveSettings.Cursor = Cursors.Hand;
            _btnSaveSettings.FlatStyle = FlatStyle.Flat;
            _btnSaveSettings.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnSaveSettings.ForeColor = Color.White;
            _btnSaveSettings.Location = new Point(150, 3);
            _btnSaveSettings.Name = "_btnSaveSettings";
            _btnSaveSettings.Size = new Size(120, 30);
            _btnSaveSettings.TabIndex = 1;
            _btnSaveSettings.Text = "Save Settings";
            _btnSaveSettings.UseVisualStyleBackColor = false;
            // 
            // _btnLoadSettings
            // 
            _btnLoadSettings.BackColor = Color.FromArgb(52, 152, 219);
            _btnLoadSettings.Cursor = Cursors.Hand;
            _btnLoadSettings.FlatStyle = FlatStyle.Flat;
            _btnLoadSettings.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnLoadSettings.ForeColor = Color.White;
            _btnLoadSettings.Location = new Point(280, 3);
            _btnLoadSettings.Name = "_btnLoadSettings";
            _btnLoadSettings.Size = new Size(120, 30);
            _btnLoadSettings.TabIndex = 2;
            _btnLoadSettings.Text = "Load Settings";
            _btnLoadSettings.UseVisualStyleBackColor = false;
            // 
            // _btnCancel
            // 
            _btnCancel.BackColor = Color.FromArgb(149, 165, 166);
            _btnCancel.Cursor = Cursors.Hand;
            _btnCancel.FlatStyle = FlatStyle.Flat;
            _btnCancel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnCancel.ForeColor = Color.White;
            _btnCancel.Location = new Point(410, 3);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(100, 30);
            _btnCancel.TabIndex = 3;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // _lblStatus
            // 
            _lblStatus.BackColor = Color.FromArgb(240, 240, 240);
            _lblStatus.BorderStyle = BorderStyle.FixedSingle;
            _lblStatus.Dock = DockStyle.Top;
            _lblStatus.Location = new Point(5, 25);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(602, 25);
            _lblStatus.TabIndex = 1;
            _lblStatus.Text = "Ready";
            _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _progressBar
            // 
            _progressBar.Dock = DockStyle.Top;
            _progressBar.Location = new Point(5, 5);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(602, 20);
            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.TabIndex = 2;
            // 
            // frmExperimentDesigner
            // 
            BackColor = Color.White;
            ClientSize = new Size(612, 561);
            Controls.Add(_mainTabControl);
            Controls.Add(_bottomPanel);
            MinimumSize = new Size(500, 500);
            Name = "frmExperimentDesigner";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Experiment Designer";
            _mainTabControl.ResumeLayout(false);
            _tabAlgorithms.ResumeLayout(false);
            _grpAlgorithmSelection.ResumeLayout(false);
            _grpAlgorithmParameters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_nudHeuristicWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudSearchLimit).EndInit();
            _grpMLSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_nudLearningRate).EndInit();
            _tabMapSettings.ResumeLayout(false);
            _grpMapSource.ResumeLayout(false);
            _grpMapSource.PerformLayout();
            _grpMapProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_nudGoalCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudParkingCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudStaticObstacles).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudDynamicObstacles).EndInit();
            _grpRobotProperties.ResumeLayout(false);
            _grpRobotProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_nudRobotSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotBattery).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudConsumptionRate).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudViewAngle).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudDetectionRange).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudChargingTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudSafetyMargin).EndInit();
            _tabExperimentSettings.ResumeLayout(false);
            _grpExperimentSettings.ResumeLayout(false);
            _grpExperimentSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_nudIterations).EndInit();
            _grpInfo.ResumeLayout(false);
            _bottomPanel.ResumeLayout(false);
            _buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
    }
}