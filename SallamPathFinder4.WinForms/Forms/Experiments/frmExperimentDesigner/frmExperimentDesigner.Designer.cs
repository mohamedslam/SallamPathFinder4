#region File Header
 

using SallamPathFinder4.Core.Models.Algorithms;

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
      
        #region Private Fields - Sensitivity Analysis
        private System.Windows.Forms.TabPage _tabSensitivity;
        private System.Windows.Forms.CheckBox _chkEnableSensitivity;
        private System.Windows.Forms.Label _lblParameter;
        private System.Windows.Forms.ComboBox _cboSensitivityParameter;
        private System.Windows.Forms.Label _lblValues;
        private System.Windows.Forms.TextBox _txtSensitivityValues;
        private System.Windows.Forms.Button _btnValidateValues;
        private System.Windows.Forms.DataGridView _dgvSensitivityResults;
        private System.Windows.Forms.Button _btnRunSensitivity;
        private System.Windows.Forms.Label _lblSensitivityStatus;
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
            _grpAlgorithmSelection = new GroupBox();
            _dgvAlgorithems = new DataGridView();
            _grpMLSettings = new GroupBox();
            _chkTrainBeforeExperiment = new CheckBox();
            _chkCollectTrainingData = new CheckBox();
            _chkUseNeuralNetwork = new CheckBox();
            _chkEnableDynamicLearning = new CheckBox();
            _lblLearningRate = new Label();
            _nudLearningRate = new NumericUpDown();
            _btnTrainNow = new Button();
            _prgTraining = new ProgressBar();
            _lblTrainingStatus = new Label();
            _tabMapSettings = new TabPage();
            _grpMapSource = new GroupBox();
            _rbLoadMap = new RadioButton();
            _chkUseCurrentMap = new CheckBox();
            _txtMapFilePath = new TextBox();
            _btnBrowseMap = new Button();
            _grpMapProperties = new GroupBox();
            _nudCellSize = new NumericUpDown();
            _lblCellSize = new Label();
            numericUpDown1 = new NumericUpDown();
            label5 = new Label();
            numericUpDown2 = new NumericUpDown();
            _lblGoals = new Label();
            _nudGoalCount = new NumericUpDown();
            _lblParking = new Label();
            _nudParkingCount = new NumericUpDown();
            label1 = new Label();
            label4 = new Label();
            _nudSemiStaticObstacles = new NumericUpDown();
            _lblStatic = new Label();
            _nudStaticObstacles = new NumericUpDown();
            _lblDynamic = new Label();
            _nudDynamicObstacles = new NumericUpDown();
            _chkUseCustomStartPoint = new CheckBox();
            _lblCurrentStartPoint = new Label();
            _btnPickStartPoint = new Button();
            _grpRobotProperties = new GroupBox();
            _nudSafetyMargin = new NumericUpDown();
            _lblRobotName = new Label();
            _txtRobotName = new TextBox();
            _lblRobotSpeed = new Label();
            _nudRobotSpeed = new NumericUpDown();
            _lblRobotBattery = new Label();
            _nudRobotBattery = new NumericUpDown();
            _lblConsumption = new Label();
            _nudConsumptionRate = new NumericUpDown();
            label3 = new Label();
            _lblViewAngle = new Label();
            _nudHeight = new NumericUpDown();
            label2 = new Label();
            _nudViewAngle = new NumericUpDown();
            _nudWidth = new NumericUpDown();
            _lblDetection = new Label();
            _nudDetectionRange = new NumericUpDown();
            _chkEnableDynamicCharging = new CheckBox();
            _lblChargingTime = new Label();
            _nudChargingTime = new NumericUpDown();
            _lblChargingTimeUnit = new Label();
            _lblSafetyMargin = new Label();
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
            ((System.ComponentModel.ISupportInitialize)_dgvAlgorithems).BeginInit();
            _grpMLSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudLearningRate).BeginInit();
            _tabMapSettings.SuspendLayout();
            _grpMapSource.SuspendLayout();
            _grpMapProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudCellSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudGoalCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudParkingCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudSemiStaticObstacles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudStaticObstacles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudDynamicObstacles).BeginInit();
            _grpRobotProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudSafetyMargin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotBattery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudConsumptionRate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudViewAngle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudDetectionRange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudChargingTime).BeginInit();
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
            _mainTabControl.Size = new Size(591, 518);
            _mainTabControl.TabIndex = 0;
            // 
            // _tabAlgorithms
            // 
            _tabAlgorithms.Controls.Add(_grpAlgorithmSelection);
            _tabAlgorithms.Controls.Add(_grpMLSettings);
            _tabAlgorithms.Location = new Point(4, 24);
            _tabAlgorithms.Name = "_tabAlgorithms";
            _tabAlgorithms.Size = new Size(583, 490);
            _tabAlgorithms.TabIndex = 2;
            _tabAlgorithms.Text = "Algorithms & ML";
            // 
            // _grpAlgorithmSelection
            // 
            _grpAlgorithmSelection.Controls.Add(_dgvAlgorithems);
            _grpAlgorithmSelection.Dock = DockStyle.Fill;
            _grpAlgorithmSelection.Location = new Point(0, 0);
            _grpAlgorithmSelection.Name = "_grpAlgorithmSelection";
            _grpAlgorithmSelection.Size = new Size(583, 348);
            _grpAlgorithmSelection.TabIndex = 0;
            _grpAlgorithmSelection.TabStop = false;
            _grpAlgorithmSelection.Text = "Select Algorithms";
            // 
            // _dgvAlgorithems
            // 
            _dgvAlgorithems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvAlgorithems.BackgroundColor = Color.White;
            _dgvAlgorithems.BorderStyle = BorderStyle.Fixed3D;
            _dgvAlgorithems.Dock = DockStyle.Fill;
            _dgvAlgorithems.GridColor = Color.FromArgb(230, 230, 230);
            _dgvAlgorithems.Location = new Point(3, 19);
            _dgvAlgorithems.Name = "_dgvAlgorithems";
            _dgvAlgorithems.RowHeadersVisible = false;
            _dgvAlgorithems.Size = new Size(577, 326);
            _dgvAlgorithems.TabIndex = 11;
            // 
            // _grpMLSettings
            // 
            _grpMLSettings.Controls.Add(_chkTrainBeforeExperiment);
            _grpMLSettings.Controls.Add(_chkCollectTrainingData);
            _grpMLSettings.Controls.Add(_chkUseNeuralNetwork);
            _grpMLSettings.Controls.Add(_chkEnableDynamicLearning);
            _grpMLSettings.Controls.Add(_lblLearningRate);
            _grpMLSettings.Controls.Add(_nudLearningRate);
            _grpMLSettings.Controls.Add(_btnTrainNow);
            _grpMLSettings.Controls.Add(_prgTraining);
            _grpMLSettings.Controls.Add(_lblTrainingStatus);
            _grpMLSettings.Dock = DockStyle.Bottom;
            _grpMLSettings.Location = new Point(0, 348);
            _grpMLSettings.Name = "_grpMLSettings";
            _grpMLSettings.Size = new Size(583, 142);
            _grpMLSettings.TabIndex = 2;
            _grpMLSettings.TabStop = false;
            _grpMLSettings.Text = "Machine Learning Options (SPPA-DL only)";
            // 
            // _chkTrainBeforeExperiment
            // 
            _chkTrainBeforeExperiment.Location = new Point(192, 51);
            _chkTrainBeforeExperiment.Name = "_chkTrainBeforeExperiment";
            _chkTrainBeforeExperiment.Size = new Size(105, 20);
            _chkTrainBeforeExperiment.TabIndex = 5;
            _chkTrainBeforeExperiment.Text = "Train Model Before Experiment";
            // 
            // _chkCollectTrainingData
            // 
            _chkCollectTrainingData.Location = new Point(15, 52);
            _chkCollectTrainingData.Name = "_chkCollectTrainingData";
            _chkCollectTrainingData.Size = new Size(140, 20);
            _chkCollectTrainingData.TabIndex = 4;
            _chkCollectTrainingData.Text = "Collect Training Data";
            // 
            // _chkUseNeuralNetwork
            // 
            _chkUseNeuralNetwork.Location = new Point(192, 25);
            _chkUseNeuralNetwork.Name = "_chkUseNeuralNetwork";
            _chkUseNeuralNetwork.Size = new Size(220, 20);
            _chkUseNeuralNetwork.TabIndex = 3;
            _chkUseNeuralNetwork.Text = "Use Neural Network Prediction";
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
            _lblLearningRate.Location = new Point(14, 83);
            _lblLearningRate.Name = "_lblLearningRate";
            _lblLearningRate.Size = new Size(100, 20);
            _lblLearningRate.TabIndex = 1;
            _lblLearningRate.Text = "Learning Rate (α):";
            // 
            // _nudLearningRate
            // 
            _nudLearningRate.DecimalPlaces = 1;
            _nudLearningRate.Location = new Point(119, 80);
            _nudLearningRate.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudLearningRate.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            _nudLearningRate.Name = "_nudLearningRate";
            _nudLearningRate.Size = new Size(60, 23);
            _nudLearningRate.TabIndex = 2;
            _nudLearningRate.Value = new decimal(new int[] { 20, 0, 0, 65536 });
            // 
            // _btnTrainNow
            // 
            _btnTrainNow.BackColor = Color.FromArgb(52, 152, 219);
            _btnTrainNow.FlatStyle = FlatStyle.Flat;
            _btnTrainNow.ForeColor = Color.White;
            _btnTrainNow.Location = new Point(185, 77);
            _btnTrainNow.Name = "_btnTrainNow";
            _btnTrainNow.Size = new Size(112, 30);
            _btnTrainNow.TabIndex = 6;
            _btnTrainNow.Text = "Train Model Now";
            _btnTrainNow.UseVisualStyleBackColor = false;
            // 
            // _prgTraining
            // 
            _prgTraining.Location = new Point(303, 89);
            _prgTraining.Name = "_prgTraining";
            _prgTraining.Size = new Size(272, 18);
            _prgTraining.TabIndex = 7;
            _prgTraining.Visible = false;
            // 
            // _lblTrainingStatus
            // 
            _lblTrainingStatus.BorderStyle = BorderStyle.FixedSingle;
            _lblTrainingStatus.Dock = DockStyle.Bottom;
            _lblTrainingStatus.Location = new Point(3, 119);
            _lblTrainingStatus.Name = "_lblTrainingStatus";
            _lblTrainingStatus.Size = new Size(577, 20);
            _lblTrainingStatus.TabIndex = 8;
            _lblTrainingStatus.Visible = false;
            // 
            // _tabMapSettings
            // 
            _tabMapSettings.Controls.Add(_grpMapSource);
            _tabMapSettings.Controls.Add(_grpMapProperties);
            _tabMapSettings.Controls.Add(_grpRobotProperties);
            _tabMapSettings.Location = new Point(4, 24);
            _tabMapSettings.Name = "_tabMapSettings";
            _tabMapSettings.Size = new Size(583, 490);
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
            _grpMapProperties.Controls.Add(_nudCellSize);
            _grpMapProperties.Controls.Add(_lblCellSize);
            _grpMapProperties.Controls.Add(numericUpDown1);
            _grpMapProperties.Controls.Add(label5);
            _grpMapProperties.Controls.Add(numericUpDown2);
            _grpMapProperties.Controls.Add(_lblGoals);
            _grpMapProperties.Controls.Add(_nudGoalCount);
            _grpMapProperties.Controls.Add(_lblParking);
            _grpMapProperties.Controls.Add(_nudParkingCount);
            _grpMapProperties.Controls.Add(label1);
            _grpMapProperties.Controls.Add(label4);
            _grpMapProperties.Controls.Add(_nudSemiStaticObstacles);
            _grpMapProperties.Controls.Add(_lblStatic);
            _grpMapProperties.Controls.Add(_nudStaticObstacles);
            _grpMapProperties.Controls.Add(_lblDynamic);
            _grpMapProperties.Controls.Add(_nudDynamicObstacles);
            _grpMapProperties.Controls.Add(_chkUseCustomStartPoint);
            _grpMapProperties.Controls.Add(_lblCurrentStartPoint);
            _grpMapProperties.Controls.Add(_btnPickStartPoint);
            _grpMapProperties.Location = new Point(60, 115);
            _grpMapProperties.Name = "_grpMapProperties";
            _grpMapProperties.Size = new Size(451, 160);
            _grpMapProperties.TabIndex = 1;
            _grpMapProperties.TabStop = false;
            _grpMapProperties.Text = "Map Properties";
            // 
            // _nudCellSize
            // 
            _nudCellSize.Location = new Point(113, 95);
            _nudCellSize.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudCellSize.Name = "_nudCellSize";
            _nudCellSize.Size = new Size(60, 23);
            _nudCellSize.TabIndex = 12;
            _nudCellSize.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // _lblCellSize
            // 
            _lblCellSize.Location = new Point(17, 95);
            _lblCellSize.Name = "_lblCellSize";
            _lblCellSize.Size = new Size(110, 23);
            _lblCellSize.TabIndex = 11;
            _lblCellSize.Text = "Cell Size (pixels):";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(113, 71);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(60, 23);
            numericUpDown1.TabIndex = 5;
            numericUpDown1.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // label5
            // 
            label5.Location = new Point(18, 52);
            label5.Name = "label5";
            label5.Size = new Size(80, 20);
            label5.TabIndex = 0;
            label5.Text = "Grid Width:";
            // 
            // numericUpDown2
            // 
            numericUpDown2.Location = new Point(113, 47);
            numericUpDown2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(60, 23);
            numericUpDown2.TabIndex = 1;
            numericUpDown2.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // _lblGoals
            // 
            _lblGoals.Location = new Point(18, 132);
            _lblGoals.Name = "_lblGoals";
            _lblGoals.Size = new Size(80, 20);
            _lblGoals.TabIndex = 0;
            _lblGoals.Text = "Goal Count:";
            // 
            // _nudGoalCount
            // 
            _nudGoalCount.Location = new Point(113, 129);
            _nudGoalCount.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            _nudGoalCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudGoalCount.Name = "_nudGoalCount";
            _nudGoalCount.Size = new Size(60, 23);
            _nudGoalCount.TabIndex = 1;
            _nudGoalCount.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // _lblParking
            // 
            _lblParking.Location = new Point(235, 131);
            _lblParking.Name = "_lblParking";
            _lblParking.Size = new Size(90, 20);
            _lblParking.TabIndex = 2;
            _lblParking.Text = "Parking Count:";
            // 
            // _nudParkingCount
            // 
            _nudParkingCount.Location = new Point(330, 129);
            _nudParkingCount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudParkingCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudParkingCount.Name = "_nudParkingCount";
            _nudParkingCount.Size = new Size(60, 23);
            _nudParkingCount.TabIndex = 3;
            _nudParkingCount.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label1
            // 
            label1.Location = new Point(200, 72);
            label1.Name = "label1";
            label1.Size = new Size(125, 20);
            label1.TabIndex = 4;
            label1.Text = "Semi Static Obstacles:";
            // 
            // label4
            // 
            label4.Location = new Point(18, 74);
            label4.Name = "label4";
            label4.Size = new Size(115, 20);
            label4.TabIndex = 4;
            label4.Text = "Grid Height:";
            // 
            // _nudSemiStaticObstacles
            // 
            _nudSemiStaticObstacles.Location = new Point(330, 69);
            _nudSemiStaticObstacles.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _nudSemiStaticObstacles.Name = "_nudSemiStaticObstacles";
            _nudSemiStaticObstacles.Size = new Size(60, 23);
            _nudSemiStaticObstacles.TabIndex = 5;
            _nudSemiStaticObstacles.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // _lblStatic
            // 
            _lblStatic.Location = new Point(200, 52);
            _lblStatic.Name = "_lblStatic";
            _lblStatic.Size = new Size(115, 20);
            _lblStatic.TabIndex = 4;
            _lblStatic.Text = "Static Obstacles:";
            // 
            // _nudStaticObstacles
            // 
            _nudStaticObstacles.Location = new Point(330, 45);
            _nudStaticObstacles.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _nudStaticObstacles.Name = "_nudStaticObstacles";
            _nudStaticObstacles.Size = new Size(60, 23);
            _nudStaticObstacles.TabIndex = 5;
            _nudStaticObstacles.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // _lblDynamic
            // 
            _lblDynamic.Location = new Point(200, 94);
            _lblDynamic.Name = "_lblDynamic";
            _lblDynamic.Size = new Size(125, 20);
            _lblDynamic.TabIndex = 6;
            _lblDynamic.Text = "Dynamic Obstacles:";
            // 
            // _nudDynamicObstacles
            // 
            _nudDynamicObstacles.Location = new Point(330, 93);
            _nudDynamicObstacles.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            _nudDynamicObstacles.Name = "_nudDynamicObstacles";
            _nudDynamicObstacles.Size = new Size(60, 23);
            _nudDynamicObstacles.TabIndex = 7;
            _nudDynamicObstacles.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // _chkUseCustomStartPoint
            // 
            _chkUseCustomStartPoint.Location = new Point(25, 18);
            _chkUseCustomStartPoint.Name = "_chkUseCustomStartPoint";
            _chkUseCustomStartPoint.Size = new Size(150, 20);
            _chkUseCustomStartPoint.TabIndex = 8;
            _chkUseCustomStartPoint.Text = "Use Custom Start Point";
            // 
            // _lblCurrentStartPoint
            // 
            _lblCurrentStartPoint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            _lblCurrentStartPoint.Location = new Point(295, 18);
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
            _btnPickStartPoint.Location = new Point(180, 14);
            _btnPickStartPoint.Name = "_btnPickStartPoint";
            _btnPickStartPoint.Size = new Size(100, 25);
            _btnPickStartPoint.TabIndex = 10;
            _btnPickStartPoint.Text = "Pick from Map";
            _btnPickStartPoint.UseVisualStyleBackColor = false;
            // 
            // _grpRobotProperties
            // 
            _grpRobotProperties.Controls.Add(_nudSafetyMargin);
            _grpRobotProperties.Controls.Add(_lblRobotName);
            _grpRobotProperties.Controls.Add(_txtRobotName);
            _grpRobotProperties.Controls.Add(_lblRobotSpeed);
            _grpRobotProperties.Controls.Add(_nudRobotSpeed);
            _grpRobotProperties.Controls.Add(_lblRobotBattery);
            _grpRobotProperties.Controls.Add(_nudRobotBattery);
            _grpRobotProperties.Controls.Add(_lblConsumption);
            _grpRobotProperties.Controls.Add(_nudConsumptionRate);
            _grpRobotProperties.Controls.Add(label3);
            _grpRobotProperties.Controls.Add(_lblViewAngle);
            _grpRobotProperties.Controls.Add(_nudHeight);
            _grpRobotProperties.Controls.Add(label2);
            _grpRobotProperties.Controls.Add(_nudViewAngle);
            _grpRobotProperties.Controls.Add(_nudWidth);
            _grpRobotProperties.Controls.Add(_lblDetection);
            _grpRobotProperties.Controls.Add(_nudDetectionRange);
            _grpRobotProperties.Controls.Add(_chkEnableDynamicCharging);
            _grpRobotProperties.Controls.Add(_lblChargingTime);
            _grpRobotProperties.Controls.Add(_nudChargingTime);
            _grpRobotProperties.Controls.Add(_lblChargingTimeUnit);
            _grpRobotProperties.Controls.Add(_lblSafetyMargin);
            _grpRobotProperties.Controls.Add(_lblSafetyMarginUnit);
            _grpRobotProperties.Location = new Point(60, 273);
            _grpRobotProperties.Name = "_grpRobotProperties";
            _grpRobotProperties.Size = new Size(451, 214);
            _grpRobotProperties.TabIndex = 2;
            _grpRobotProperties.TabStop = false;
            _grpRobotProperties.Text = "Robot Configuration";
            // 
            // _nudSafetyMargin
            // 
            _nudSafetyMargin.DecimalPlaces = 1;
            _nudSafetyMargin.Enabled = false;
            _nudSafetyMargin.Location = new Point(325, 176);
            _nudSafetyMargin.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            _nudSafetyMargin.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            _nudSafetyMargin.Name = "_nudSafetyMargin";
            _nudSafetyMargin.Size = new Size(60, 23);
            _nudSafetyMargin.TabIndex = 17;
            _nudSafetyMargin.Value = new decimal(new int[] { 10, 0, 0, 0 });
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
            _nudRobotSpeed.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // _lblRobotBattery
            // 
            _lblRobotBattery.Location = new Point(15, 123);
            _lblRobotBattery.Name = "_lblRobotBattery";
            _lblRobotBattery.Size = new Size(99, 20);
            _lblRobotBattery.TabIndex = 4;
            _lblRobotBattery.Text = "Battery (%):";
            // 
            // _nudRobotBattery
            // 
            _nudRobotBattery.Location = new Point(120, 122);
            _nudRobotBattery.Name = "_nudRobotBattery";
            _nudRobotBattery.Size = new Size(60, 23);
            _nudRobotBattery.TabIndex = 5;
            _nudRobotBattery.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // _lblConsumption
            // 
            _lblConsumption.Location = new Point(200, 122);
            _lblConsumption.Name = "_lblConsumption";
            _lblConsumption.Size = new Size(120, 20);
            _lblConsumption.TabIndex = 6;
            _lblConsumption.Text = "Consumption (%/m):";
            // 
            // _nudConsumptionRate
            // 
            _nudConsumptionRate.DecimalPlaces = 1;
            _nudConsumptionRate.Location = new Point(325, 121);
            _nudConsumptionRate.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudConsumptionRate.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            _nudConsumptionRate.Name = "_nudConsumptionRate";
            _nudConsumptionRate.Size = new Size(60, 23);
            _nudConsumptionRate.TabIndex = 7;
            _nudConsumptionRate.Value = new decimal(new int[] { 10, 0, 0, 65536 });
            // 
            // label3
            // 
            label3.Location = new Point(15, 57);
            label3.Name = "label3";
            label3.Size = new Size(80, 20);
            label3.TabIndex = 8;
            label3.Text = "Height:";
            // 
            // _lblViewAngle
            // 
            _lblViewAngle.Location = new Point(15, 85);
            _lblViewAngle.Name = "_lblViewAngle";
            _lblViewAngle.Size = new Size(100, 20);
            _lblViewAngle.TabIndex = 8;
            _lblViewAngle.Text = "View Angle (deg):";
            // 
            // _nudHeight
            // 
            _nudHeight.Location = new Point(120, 54);
            _nudHeight.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            _nudHeight.Name = "_nudHeight";
            _nudHeight.Size = new Size(60, 23);
            _nudHeight.TabIndex = 9;
            _nudHeight.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // label2
            // 
            label2.Location = new Point(210, 57);
            label2.Name = "label2";
            label2.Size = new Size(80, 20);
            label2.TabIndex = 10;
            label2.Text = "Width:";
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
            // _nudWidth
            // 
            _nudWidth.Location = new Point(325, 54);
            _nudWidth.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            _nudWidth.Name = "_nudWidth";
            _nudWidth.Size = new Size(60, 23);
            _nudWidth.TabIndex = 11;
            _nudWidth.Value = new decimal(new int[] { 20, 0, 0, 0 });
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
            _chkEnableDynamicCharging.Location = new Point(15, 151);
            _chkEnableDynamicCharging.Name = "_chkEnableDynamicCharging";
            _chkEnableDynamicCharging.Size = new Size(150, 20);
            _chkEnableDynamicCharging.TabIndex = 12;
            _chkEnableDynamicCharging.Text = "Enable Dynamic Charging";
            // 
            // _lblChargingTime
            // 
            _lblChargingTime.Location = new Point(15, 179);
            _lblChargingTime.Name = "_lblChargingTime";
            _lblChargingTime.Size = new Size(90, 20);
            _lblChargingTime.TabIndex = 13;
            _lblChargingTime.Text = "Charging Time:";
            // 
            // _nudChargingTime
            // 
            _nudChargingTime.Enabled = false;
            _nudChargingTime.Location = new Point(120, 176);
            _nudChargingTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            _nudChargingTime.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            _nudChargingTime.Name = "_nudChargingTime";
            _nudChargingTime.Size = new Size(60, 23);
            _nudChargingTime.TabIndex = 14;
            _nudChargingTime.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // _lblChargingTimeUnit
            // 
            _lblChargingTimeUnit.Location = new Point(183, 179);
            _lblChargingTimeUnit.Name = "_lblChargingTimeUnit";
            _lblChargingTimeUnit.Size = new Size(50, 20);
            _lblChargingTimeUnit.TabIndex = 15;
            _lblChargingTimeUnit.Text = "seconds";
            // 
            // _lblSafetyMargin
            // 
            _lblSafetyMargin.Location = new Point(240, 179);
            _lblSafetyMargin.Name = "_lblSafetyMargin";
            _lblSafetyMargin.Size = new Size(95, 20);
            _lblSafetyMargin.TabIndex = 16;
            _lblSafetyMargin.Text = "Safety Margin:";
            // 
            // _lblSafetyMarginUnit
            // 
            _lblSafetyMarginUnit.Location = new Point(391, 179);
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
            _tabExperimentSettings.Size = new Size(583, 490);
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
            _grpExperimentSettings.Location = new Point(8, 67);
            _grpExperimentSettings.Name = "_grpExperimentSettings";
            _grpExperimentSettings.Size = new Size(563, 155);
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
            _nudIterations.Location = new Point(120, 53);
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
            _chkSaveScreenshots.Location = new Point(210, 53);
            _chkSaveScreenshots.Name = "_chkSaveScreenshots";
            _chkSaveScreenshots.Size = new Size(120, 23);
            _chkSaveScreenshots.TabIndex = 4;
            _chkSaveScreenshots.Text = "Save Screenshots";
            // 
            // _chkSaveReplay
            // 
            _chkSaveReplay.Checked = true;
            _chkSaveReplay.CheckState = CheckState.Checked;
            _chkSaveReplay.Location = new Point(340, 53);
            _chkSaveReplay.Name = "_chkSaveReplay";
            _chkSaveReplay.Size = new Size(100, 23);
            _chkSaveReplay.TabIndex = 5;
            _chkSaveReplay.Text = "Save Replay";
            // 
            // _chkShowPathOnScreenshots
            // 
            _chkShowPathOnScreenshots.Checked = true;
            _chkShowPathOnScreenshots.CheckState = CheckState.Checked;
            _chkShowPathOnScreenshots.Location = new Point(120, 83);
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
            _txtSavePath.Location = new Point(120, 112);
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
            _btnBrowseSavePath.Location = new Point(386, 110);
            _btnBrowseSavePath.Name = "_btnBrowseSavePath";
            _btnBrowseSavePath.Size = new Size(40, 25);
            _btnBrowseSavePath.TabIndex = 9;
            _btnBrowseSavePath.Text = "...";
            _btnBrowseSavePath.UseVisualStyleBackColor = false;
            // 
            // _grpInfo
            // 
            _grpInfo.Controls.Add(_lblInfo);
            _grpInfo.Location = new Point(8, 300);
            _grpInfo.Name = "_grpInfo";
            _grpInfo.Size = new Size(563, 162);
            _grpInfo.TabIndex = 1;
            _grpInfo.TabStop = false;
            _grpInfo.Text = "Information";
            // 
            // _lblInfo
            // 
            _lblInfo.Font = new Font("Segoe UI", 10F);
            _lblInfo.ForeColor = Color.DarkBlue;
            _lblInfo.Location = new Point(15, 47);
            _lblInfo.Name = "_lblInfo";
            _lblInfo.Size = new Size(506, 75);
            _lblInfo.TabIndex = 0;
            _lblInfo.Text = resources.GetString("_lblInfo.Text");
            // 
            // _bottomPanel
            // 
            _bottomPanel.BackColor = Color.FromArgb(240, 242, 245);
            _bottomPanel.Controls.Add(_buttonPanel);
            _bottomPanel.Controls.Add(_lblStatus);
            _bottomPanel.Controls.Add(_progressBar);
            _bottomPanel.Dock = DockStyle.Bottom;
            _bottomPanel.Location = new Point(0, 518);
            _bottomPanel.Name = "_bottomPanel";
            _bottomPanel.Padding = new Padding(5);
            _bottomPanel.Size = new Size(591, 92);
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
            _buttonPanel.Size = new Size(581, 35);
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
            _lblStatus.Size = new Size(581, 25);
            _lblStatus.TabIndex = 1;
            _lblStatus.Text = "Ready";
            _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _progressBar
            // 
            _progressBar.Dock = DockStyle.Top;
            _progressBar.Location = new Point(5, 5);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(581, 20);
            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.TabIndex = 2;
            // 
            // frmExperimentDesigner
            // 
            BackColor = Color.White;
            ClientSize = new Size(591, 610);
            Controls.Add(_mainTabControl);
            Controls.Add(_bottomPanel);
            MinimumSize = new Size(500, 500);
            Name = "frmExperimentDesigner";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Experiment Designer";
            _mainTabControl.ResumeLayout(false);
            _tabAlgorithms.ResumeLayout(false);
            _grpAlgorithmSelection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_dgvAlgorithems).EndInit();
            _grpMLSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_nudLearningRate).EndInit();
            _tabMapSettings.ResumeLayout(false);
            _grpMapSource.ResumeLayout(false);
            _grpMapSource.PerformLayout();
            _grpMapProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_nudCellSize).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudGoalCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudParkingCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudSemiStaticObstacles).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudStaticObstacles).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudDynamicObstacles).EndInit();
            _grpRobotProperties.ResumeLayout(false);
            _grpRobotProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_nudSafetyMargin).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotBattery).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudConsumptionRate).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudViewAngle).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudDetectionRange).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudChargingTime).EndInit();
            _tabExperimentSettings.ResumeLayout(false);
            _grpExperimentSettings.ResumeLayout(false);
            _grpExperimentSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_nudIterations).EndInit();
            _grpInfo.ResumeLayout(false);
            _bottomPanel.ResumeLayout(false);
            _buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void InitializeSensitivityTab()
        {
            _tabSensitivity = new TabPage("🔬 Sensitivity Analysis");

            // Create panel
            var pnlSensitivity = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoScroll = true
            };

            int y = 10;

            // Enable CheckBox
            _chkEnableSensitivity = new CheckBox
            {
                Text = "Enable Sensitivity Analysis",
                Location = new Point(10, y),
                Size = new Size(200, 25),
                Checked = false
            };
            _chkEnableSensitivity.CheckedChanged += (s, e) => UpdateSensitivityControlsState();
            pnlSensitivity.Controls.Add(_chkEnableSensitivity);
            y += 35;

            // Parameter selection
            _lblParameter = new Label
            {
                Text = "Parameter to analyze:",
                Location = new Point(10, y),
                Size = new Size(150, 25),
                Enabled = false
            };
            pnlSensitivity.Controls.Add(_lblParameter);

            _cboSensitivityParameter = new ComboBox
            {
                Location = new Point(160, y),
                Size = new Size(180, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            _cboSensitivityParameter.Items.AddRange(new string[]
            {
        "Lambda (λ) - Obstacle weight",
        "LearningRate (α) - Memory weight (SPPA-DL only)",
        "PredictionWeight (β) - ML risk weight (SPPA-DL only)",
        "Alpha_S - Static obstacle weight",
        "Alpha_SS - Semi-static obstacle weight",
        "Alpha_D - Dynamic obstacle weight"
            });
            _cboSensitivityParameter.SelectedIndex = 0;
            pnlSensitivity.Controls.Add(_cboSensitivityParameter);
            y += 35;

            // Values input
            _lblValues = new Label
            {
                Text = "Values (comma-separated):",
                Location = new Point(10, y),
                Size = new Size(150, 25),
                Enabled = false
            };
            pnlSensitivity.Controls.Add(_lblValues);

            _txtSensitivityValues = new TextBox
            {
                Location = new Point(160, y),
                Size = new Size(200, 25),
                Text = "1.0,1.5,2.0,2.5,3.0",
                Enabled = false
            };
            pnlSensitivity.Controls.Add(_txtSensitivityValues);

            _btnValidateValues = new Button
            {
                Text = "Validate",
                Location = new Point(370, y),
                Size = new Size(70, 25),
                Enabled = false,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnValidateValues.Click += BtnValidateValues_Click;
            pnlSensitivity.Controls.Add(_btnValidateValues);
            y += 35;

            // Results DataGridView
            _dgvSensitivityResults = new DataGridView
            {
                Location = new Point(10, y),
                Size = new Size(550, 250),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };

            // Add columns
            _dgvSensitivityResults.Columns.Add("ParamValue", "Parameter Value");
            _dgvSensitivityResults.Columns.Add("PathLength", "Path Length (cells)");
            _dgvSensitivityResults.Columns.Add("TimeMs", "Time (ms)");
            _dgvSensitivityResults.Columns.Add("Success", "Success");
            _dgvSensitivityResults.Columns.Add("Collisions", "Collisions");

            pnlSensitivity.Controls.Add(_dgvSensitivityResults);
            y += 260;

            // Run button
            _btnRunSensitivity = new Button
            {
                Text = "▶ Run Sensitivity Analysis",
                Location = new Point(10, y),
                Size = new Size(200, 35),
                Enabled = false,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnRunSensitivity.Click += BtnRunSensitivity_Click;
            pnlSensitivity.Controls.Add(_btnRunSensitivity);
            y += 45;

            // Status label
            _lblSensitivityStatus = new Label
            {
                Text = "Ready",
                Location = new Point(10, y),
                Size = new Size(400, 25),
                ForeColor = Color.Gray
            };
            pnlSensitivity.Controls.Add(_lblSensitivityStatus);

            _tabSensitivity.Controls.Add(pnlSensitivity);
            _mainTabControl.TabPages.Add(_tabSensitivity);
        }

        private void UpdateSensitivityControlsState()
        {
            bool enabled = _chkEnableSensitivity.Checked;
            _lblParameter.Enabled = enabled;
            _cboSensitivityParameter.Enabled = enabled;
            _lblValues.Enabled = enabled;
            _txtSensitivityValues.Enabled = enabled;
            _btnValidateValues.Enabled = enabled;
            _btnRunSensitivity.Enabled = enabled;
        }
        #endregion

        private Label label1;
        private NumericUpDown _nudSemiStaticObstacles;
        private DataGridView _dgvAlgorithems;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewComboBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewButtonColumn AlgorithmPrameters;
        private DataGridViewCheckBoxColumn colEnabled;
        private DataGridViewTextBoxColumn colAlgorithm;
        private DataGridViewComboBoxColumn colMetric;
        private DataGridViewTextBoxColumn colParameters;
        private DataGridViewButtonColumn colEdit;
        private DataGridViewButtonColumn colDuplicate;

        private void SetupAlgorithmGrid()
        {
            _dgvAlgorithems.AllowUserToAddRows = false;
            _dgvAlgorithems.AllowUserToDeleteRows = false;
            _dgvAlgorithems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvAlgorithems.BackgroundColor = Color.White;
            _dgvAlgorithems.BorderStyle = BorderStyle.Fixed3D;
            _dgvAlgorithems.RowHeadersVisible = false;
            _dgvAlgorithems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvAlgorithems.MultiSelect = false;
            _dgvAlgorithems.Dock = DockStyle.Fill;
            _dgvAlgorithems.Location = new Point(3, 22);
            _dgvAlgorithems.Size = new Size(609, 288);
            _dgvAlgorithems.TabIndex = 0;

            // مسح الأعمدة الحالية
            _dgvAlgorithems.Columns.Clear();

            // عمود Enabled (CheckBox) - حجم صغير جداً
            colEnabled = new DataGridViewCheckBoxColumn
            {
                Name = "colEnabled",
                HeaderText = "",
                Width = 30,  // حجم صغير جداً ليتسع فقط للـ CheckBox
                FillWeight = 3,
                Resizable = DataGridViewTriState.False,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            // عمود Algorithm - حجم مناسب
            colAlgorithm = new DataGridViewTextBoxColumn
            {
                Name = "colAlgorithm",
                HeaderText = "Algorithm",
                Width = 90,  // تصغير الحجم
                FillWeight = 15,
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };

            // عمود Metric (ComboBox) - حجم مناسب
            colMetric = new DataGridViewComboBoxColumn
            {
                Name = "colMetric",
                HeaderText = "Metric",
                Width = 100,  // تصغير الحجم
                FillWeight = 18,
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            };
            colMetric.Items.AddRange(new string[] {
        "Manhattan", "Euclidean", "MaxDXDY",
        "DiagonalShortcut", "EuclideanNoSQR"
    });

            // عمود Parameters - يأخذ المساحة المتبقية الأكبر
            colParameters = new DataGridViewTextBoxColumn
            {
                Name = "colParameters",
                HeaderText = "Parameters",
                FillWeight = 50,  // أكبر وزن
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill  // يملأ المساحة المتبقية
            };

            // عمود Edit (زر التعديل) - حجم صغير جداً
            colEdit = new DataGridViewButtonColumn
            {
                Name = "colEdit",
                HeaderText = "",
                Text = "✎",
                UseColumnTextForButtonValue = true,
                Width = 25,  // حجم صغير جداً
                FillWeight = 2,
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Resizable = DataGridViewTriState.False
            };

            // عمود Duplicate (زر النسخ) - حجم صغير جداً
            colDuplicate = new DataGridViewButtonColumn
            {
                Name = "colDuplicate",
                HeaderText = "",
                Text = "📋",
                UseColumnTextForButtonValue = true,
                Width = 25,  // حجم صغير جداً
                FillWeight = 2,
                FlatStyle = FlatStyle.Flat,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Resizable = DataGridViewTriState.False
            };

            // إضافة الأعمدة
            _dgvAlgorithems.Columns.Add(colEnabled);
            _dgvAlgorithems.Columns.Add(colAlgorithm);
            _dgvAlgorithems.Columns.Add(colMetric);
            _dgvAlgorithems.Columns.Add(colParameters);
            _dgvAlgorithems.Columns.Add(colEdit);
            _dgvAlgorithems.Columns.Add(colDuplicate);

            // إعداد ارتفاع الصفوف ليكون مناسباً
            _dgvAlgorithems.RowTemplate.Height = 28;

            // إعداد نمط الخلايا لتحسين المظهر
            _dgvAlgorithems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _dgvAlgorithems.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            _dgvAlgorithems.EnableHeadersVisualStyles = false;
            _dgvAlgorithems.ColumnHeadersHeight = 30;
            _dgvAlgorithems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // إضافة الصفوف الافتراضية
            AddDefaultAlgorithmRows();
        }
        private void AddDefaultAlgorithmRows()
        {
            // A*
            int rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = true;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = "AStar";
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = "Manhattan";
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = "h=2, Limit=20000";
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParameters("AStar");

            // SPPA
            rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = true;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = "SPPA";
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = "Manhattan";
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = "λ=1.5, h=2";
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParameters("SPPA");

            // SPPA_DL
            rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = true;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = "SPPA_DL";
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = "Manhattan";
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = "λ=1.5, α=2.0";
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParameters("SPPA_DL");

            // ACO
            rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = false;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = "ACO";
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = "Manhattan";
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = "Ants=20, Iter=100";
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParameters("ACO");

            // DStar
            rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = false;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = "DStar";
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = "Manhattan";
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = "Range=10";
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParameters("DStar");

            // KNN
            rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = false;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = "KNN";
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = "Manhattan";
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = "K=5";
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParameters("KNN");

            // BruteForce
            rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = false;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = "BruteForce";
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = "Manhattan";
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = "Depth=100";
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParameters("BruteForce");

            // RRT
            rowIndex = _dgvAlgorithems.Rows.Add();
            _dgvAlgorithems.Rows[rowIndex].Cells["colEnabled"].Value = false;
            _dgvAlgorithems.Rows[rowIndex].Cells["colAlgorithm"].Value = "RRT";
            _dgvAlgorithems.Rows[rowIndex].Cells["colMetric"].Value = "Manhattan";
            _dgvAlgorithems.Rows[rowIndex].Cells["colParameters"].Value = "Iter=5000";
            _dgvAlgorithems.Rows[rowIndex].Tag = GetDefaultParameters("RRT"); 
        }


        private Dictionary<string, object> GetDefaultParameters(string algorithmName)
        {
            return AlgorithmParametersRegistry.GetDefaultParameters(algorithmName);
        }

        private Label label3;
        private NumericUpDown _nudHeight;
        private Label label2;
        private NumericUpDown _nudWidth;
        private Label label5;
        private NumericUpDown numericUpDown2;
        private Label label4;
        private NumericUpDown numericUpDown1;
        private NumericUpDown _nudCellSize;
        private Label _lblCellSize;
    }
}