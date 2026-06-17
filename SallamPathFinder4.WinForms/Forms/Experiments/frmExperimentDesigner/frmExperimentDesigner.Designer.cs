#region File Header
/// <summary>
/// File: frmExperimentDesigner.Designer.cs
/// Description: Designer file for experiment designer form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-12
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmExperimentDesigner
{
    partial class frmExperimentDesigner
    {
        private System.ComponentModel.IContainer components = null;

        // Tab Control
        private TabControl _mainTabControl;
        private TabPage _tabExperimentSettings;
        private TabPage _tabMapSettings;
        private TabPage _tabAlgorithms;
        private TabPage _tabSensitivity;

        // Bottom Panel Controls
        private Panel _bottomPanel;
        private Panel _buttonPanel;
        private ProgressBar _progressBar;
        private Label _lblStatus;
        private Button _btnRunComparison;
        private Button _btnSaveSettings;
        private Button _btnLoadSettings;
        private Button _btnCancel;

        // Tab 1: Experiment Settings Controls
        private GroupBox _grpExperimentSettings;
        private Label _lblExpName;
        private TextBox _txtExperimentName;
        private Label _lblIterations;
        private NumericUpDown _nudIterations;
        private CheckBox _chkSaveScreenshots;
        private CheckBox _chkSaveReplay;
        private CheckBox _chkShowPathOnScreenshots;
        private Label _lblSavePath;
        private TextBox _txtSavePath;
        private Button _btnBrowseSavePath;
        private GroupBox _grpInfo;
        private Label _lblInfo;

        // Tab 2: Map Settings Controls - Map Source
        private GroupBox _grpMapSource;
        private RadioButton _rbLoadMap;
        private CheckBox _chkUseCurrentMap;
        private TextBox _txtMapFilePath;
        private Button _btnBrowseMap;

        // Tab 2: Map Properties
        private GroupBox _grpMapProperties;
        private Label _lblGoals;
        private NumericUpDown _nudGoalCount;
        private Label _lblParking;
        private NumericUpDown _nudParkingCount;
        private Label _lblStatic;
        private NumericUpDown _nudStaticObstacles;
        private Label _lblDynamic;
        private NumericUpDown _nudDynamicObstacles;
        private CheckBox _chkUseCustomStartPoint;
        private Label _lblCurrentStartPoint;
        private Button _btnPickStartPoint;
        private NumericUpDown _nudCellSize;
        private Label _lblCellSize;

        // Grid Dimension Controls
        private Label _lblGridWidth;
        private NumericUpDown _nudGridWidth;
        private Label _lblGridHeight;
        private NumericUpDown _nudGridHeight;

        // Semi-Static and Rough Terrain Controls
        private Label _lblSemiStatic;
        private NumericUpDown _nudSemiStaticObstacles;
        private Label _lblRoughTerrain;
        private NumericUpDown _nudRoughTerrain;

        // Tab 2: Robot Properties
        private GroupBox _grpRobotProperties;
        private Label _lblRobotName;
        private TextBox _txtRobotName;
        private Label _lblRobotSpeed;
        private NumericUpDown _nudRobotSpeed;
        private Label _lblRobotBattery;
        private NumericUpDown _nudRobotBattery;
        private Label _lblConsumption;
        private NumericUpDown _nudConsumptionRate;
        private Label _lblViewAngle;
        private NumericUpDown _nudViewAngle;
        private Label _lblDetection;
        private NumericUpDown _nudDetectionRange;
        private CheckBox _chkEnableDynamicCharging;
        private Label _lblChargingTime;
        private NumericUpDown _nudChargingTime;
        private Label _lblSafetyMargin;
        private NumericUpDown _nudSafetyMargin;
        private Label _lblSafetyMarginUnit;

        // Tab 3: Algorithms - DataGridView
        private GroupBox _grpAlgorithmSelection;
        private DataGridView _dgvAlgorithems;

        // ML Settings
        private GroupBox _grpMLSettings;
        private CheckBox _chkEnableDynamicLearning;
        private Label _lblLearningRate;
        private NumericUpDown _nudLearningRate;
        private CheckBox _chkUseNeuralNetwork;
        private CheckBox _chkCollectTrainingData;
        private CheckBox _chkTrainBeforeExperiment;
        private Button _btnTrainNow;
        private ProgressBar _prgTraining;
        private Label _lblTrainingStatus;

        // Sensitivity Analysis Controls
        private CheckBox _chkEnableSensitivity;
        private Label _lblParameter;
        private ComboBox _cboSensitivityParameter;
        private Label _lblValues;
        private TextBox _txtSensitivityValues;
        private Button _btnValidateValues;
        private DataGridView _dgvSensitivityResults;
        private Button _btnRunSensitivity;
        private Label _lblSensitivityStatus;       

        // DataGridView columns
        private CheckBox _chkSelectAll;
        private DataGridViewCheckBoxColumn colEnabled;
        private DataGridViewTextBoxColumn colAlgorithm;
        private DataGridViewComboBoxColumn colMetric;
        private DataGridViewTextBoxColumn colParameters;
        private DataGridViewButtonColumn colEdit;
        private DataGridViewButtonColumn colDuplicate;
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExperimentDesigner));
            _mainTabControl = new TabControl();
            _tabAlgorithms = new TabPage();
            _grpAlgorithmSelection = new GroupBox();
            _dgvAlgorithems = new DataGridView();
            _grpMLSettings = new GroupBox();
            _chkTrainBeforeExperiment = new CheckBox();
            _btnTrainNow = new Button();
            _chkEnableDynamicLearning = new CheckBox();
            _lblLearningRate = new Label();
            _nudLearningRate = new NumericUpDown();
            _chkUseNeuralNetwork = new CheckBox();
            _chkCollectTrainingData = new CheckBox();
            _prgTraining = new ProgressBar();
            _lblTrainingStatus = new Label();
            _tabMapSettings = new TabPage();
            groupBox1 = new GroupBox();
            _nudDynamicObstacles = new NumericUpDown();
            _lblStatic = new Label();
            _lblDynamic = new Label();
            _nudStaticObstacles = new NumericUpDown();
            _nudRoughTerrain = new NumericUpDown();
            _lblRoughTerrain = new Label();
            _nudSemiStaticObstacles = new NumericUpDown();
            _lblSemiStatic = new Label();
            _grpMapPoints = new GroupBox();
            _btnPickStartPoint = new Button();
            _chkUseCustomStartPoint = new CheckBox();
            _lblCurrentStartPoint = new Label();
            _lblParking = new Label();
            _nudParkingCount = new NumericUpDown();
            _nudGoalCount = new NumericUpDown();
            _lblGoals = new Label();
            _grpMapSource = new GroupBox();
            _rbLoadMap = new RadioButton();
            _chkUseCurrentMap = new CheckBox();
            _txtMapFilePath = new TextBox();
            _btnBrowseMap = new Button();
            _grpMapProperties = new GroupBox();
            _lblGridWidth = new Label();
            _nudGridWidth = new NumericUpDown();
            _lblGridHeight = new Label();
            _nudGridHeight = new NumericUpDown();
            _lblCellSize = new Label();
            _nudCellSize = new NumericUpDown();
            _grpRobotProperties = new GroupBox();
            _nudDetectionRange = new NumericUpDown();
            _nudRobotHeight = new NumericUpDown();
            _nudRobotWidth = new NumericUpDown();
            _nudViewAngle = new NumericUpDown();
            _lblRobotName = new Label();
            _txtRobotName = new TextBox();
            _lblRobotSpeed = new Label();
            _nudRobotSpeed = new NumericUpDown();
            _lblRobotBattery = new Label();
            _nudRobotBattery = new NumericUpDown();
            _lblConsumption = new Label();
            _nudConsumptionRate = new NumericUpDown();
            label2 = new Label();
            _lblViewAngle = new Label();
            label1 = new Label();
            _lblDetection = new Label();
            _chkEnableDynamicCharging = new CheckBox();
            _lblChargingTime = new Label();
            _nudChargingTime = new NumericUpDown();
            _lblSafetyMargin = new Label();
            _nudSafetyMargin = new NumericUpDown();
            _lblSafetyMarginUnit = new Label();
            _tabSensitivity = new TabPage();
            _chkEnableSensitivity = new CheckBox();
            pnlSensitivity = new Panel();
            _dgvSensitivityResults = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            _lblParameter = new Label();
            _cboSensitivityParameter = new ComboBox();
            _lblValues = new Label();
            _txtSensitivityValues = new TextBox();
            _btnValidateValues = new Button();
            _btnRunSensitivity = new Button();
            _lblSensitivityStatus = new Label();
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
            colEnabled = new DataGridViewCheckBoxColumn();
            colAlgorithm = new DataGridViewTextBoxColumn();
            colMetric = new DataGridViewComboBoxColumn();
            colParameters = new DataGridViewTextBoxColumn();
            colEdit = new DataGridViewButtonColumn();
            colDuplicate = new DataGridViewButtonColumn();
            _cboAlgorithm = new ComboBox();
            _lblAlgorithm = new Label();
            _mainTabControl.SuspendLayout();
            _tabAlgorithms.SuspendLayout();
            _grpAlgorithmSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_dgvAlgorithems).BeginInit();
            _grpMLSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudLearningRate).BeginInit();
            _tabMapSettings.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudDynamicObstacles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudStaticObstacles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudRoughTerrain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudSemiStaticObstacles).BeginInit();
            _grpMapPoints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudParkingCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudGoalCount).BeginInit();
            _grpMapSource.SuspendLayout();
            _grpMapProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudGridWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudGridHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudCellSize).BeginInit();
            _grpRobotProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_nudDetectionRange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudViewAngle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotBattery).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudConsumptionRate).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudChargingTime).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_nudSafetyMargin).BeginInit();
            _tabSensitivity.SuspendLayout();
            pnlSensitivity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_dgvSensitivityResults).BeginInit();
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
            _mainTabControl.Controls.Add(_tabSensitivity);
            _mainTabControl.Controls.Add(_tabExperimentSettings);
            _mainTabControl.Dock = DockStyle.Fill;
            _mainTabControl.Location = new Point(0, 0);
            _mainTabControl.Name = "_mainTabControl";
            _mainTabControl.SelectedIndex = 0;
            _mainTabControl.Size = new Size(650, 631);
            _mainTabControl.TabIndex = 0;
            // 
            // _tabAlgorithms
            // 
            _chkSelectAll = new CheckBox
            {
                Text = "Select All",
                Location = new Point(10, 0),
                Size = new Size(100, 25),
                Checked = true  // افتراضياً، SPPA فقط مفعل لكننا سنضبطه لاحقاً
            };
            _grpAlgorithmSelection.Controls.Add(_chkSelectAll);
            _tabAlgorithms.Controls.Add(_grpAlgorithmSelection);
            _tabAlgorithms.Controls.Add(_grpMLSettings);
            _tabAlgorithms.Location = new Point(4, 24);
            _tabAlgorithms.Name = "_tabAlgorithms";
            _tabAlgorithms.Size = new Size(642, 603);
            _tabAlgorithms.TabIndex = 0;
            _tabAlgorithms.Text = "Algorithms & ML";
            // 
            // _grpAlgorithmSelection
            // 
        
            _grpAlgorithmSelection.Controls.Add(_dgvAlgorithems);
            _grpAlgorithmSelection.Dock = DockStyle.Fill;
            _grpAlgorithmSelection.Location = new Point(0, 0);
            _grpAlgorithmSelection.Name = "_grpAlgorithmSelection";
            _grpAlgorithmSelection.Size = new Size(642, 496);
            _grpAlgorithmSelection.TabIndex = 0;
            _grpAlgorithmSelection.TabStop = false;
            _grpAlgorithmSelection.Text = "Select Algorithms";
            // 
            // _dgvAlgorithems
            // 
            _dgvAlgorithems.AllowUserToAddRows = false;
            _dgvAlgorithems.AllowUserToDeleteRows = false;
            _dgvAlgorithems.BackgroundColor = Color.White;
            _dgvAlgorithems.Dock = DockStyle.Fill;
            _dgvAlgorithems.GridColor = Color.FromArgb(230, 230, 230);
            _dgvAlgorithems.Location = new Point(3, 19);
            _dgvAlgorithems.MultiSelect = false;
            _dgvAlgorithems.Name = "_dgvAlgorithems";
            _dgvAlgorithems.RowHeadersVisible = false;
            _dgvAlgorithems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvAlgorithems.Size = new Size(636, 474);
            _dgvAlgorithems.TabIndex = 0;
            // 
            // _grpMLSettings
            // 
            _grpMLSettings.Controls.Add(_chkTrainBeforeExperiment);
            _grpMLSettings.Controls.Add(_btnTrainNow);
            _grpMLSettings.Controls.Add(_chkEnableDynamicLearning);
            _grpMLSettings.Controls.Add(_lblLearningRate);
            _grpMLSettings.Controls.Add(_nudLearningRate);
            _grpMLSettings.Controls.Add(_chkUseNeuralNetwork);
            _grpMLSettings.Controls.Add(_chkCollectTrainingData);
            _grpMLSettings.Controls.Add(_prgTraining);
            _grpMLSettings.Controls.Add(_lblTrainingStatus);
            _grpMLSettings.Dock = DockStyle.Bottom;
            _grpMLSettings.Location = new Point(0, 496);
            _grpMLSettings.Name = "_grpMLSettings";
            _grpMLSettings.Size = new Size(642, 107);
            _grpMLSettings.TabIndex = 1;
            _grpMLSettings.TabStop = false;
            _grpMLSettings.Text = "Machine Learning Options (SPPA-DL only)";
            // 
            // _chkTrainBeforeExperiment
            // 
            _chkTrainBeforeExperiment.Location = new Point(384, 24);
            _chkTrainBeforeExperiment.Name = "_chkTrainBeforeExperiment";
            _chkTrainBeforeExperiment.Size = new Size(124, 25);
            _chkTrainBeforeExperiment.TabIndex = 5;
            _chkTrainBeforeExperiment.Text = "Train Before Experiment";
            // 
            // _btnTrainNow
            // 
            _btnTrainNow.BackColor = Color.FromArgb(52, 152, 219);
            _btnTrainNow.FlatStyle = FlatStyle.Flat;
            _btnTrainNow.ForeColor = Color.White;
            _btnTrainNow.Location = new Point(514, 19);
            _btnTrainNow.Name = "_btnTrainNow";
            _btnTrainNow.Size = new Size(120, 28);
            _btnTrainNow.TabIndex = 6;
            _btnTrainNow.Text = "Train Model Now";
            _btnTrainNow.UseVisualStyleBackColor = false;
            // 
            // _chkEnableDynamicLearning
            // 
            _chkEnableDynamicLearning.Checked = true;
            _chkEnableDynamicLearning.CheckState = CheckState.Checked;
            _chkEnableDynamicLearning.Location = new Point(15, 25);
            _chkEnableDynamicLearning.Name = "_chkEnableDynamicLearning";
            _chkEnableDynamicLearning.Size = new Size(160, 25);
            _chkEnableDynamicLearning.TabIndex = 0;
            _chkEnableDynamicLearning.Text = "Enable Dynamic Learning";
            // 
            // _lblLearningRate
            // 
            _lblLearningRate.Location = new Point(15, 55);
            _lblLearningRate.Name = "_lblLearningRate";
            _lblLearningRate.Size = new Size(100, 25);
            _lblLearningRate.TabIndex = 1;
            _lblLearningRate.Text = "Learning Rate (α):";
            // 
            // _nudLearningRate
            // 
            _nudLearningRate.DecimalPlaces = 1;
            _nudLearningRate.Location = new Point(120, 53);
            _nudLearningRate.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudLearningRate.Minimum = new decimal(new int[] { 1, 0, 0, 65536 });
            _nudLearningRate.Name = "_nudLearningRate";
            _nudLearningRate.Size = new Size(80, 23);
            _nudLearningRate.TabIndex = 2;
            _nudLearningRate.Value = new decimal(new int[] { 20, 0, 0, 65536 });
            // 
            // _chkUseNeuralNetwork
            // 
            _chkUseNeuralNetwork.Location = new Point(230, 25);
            _chkUseNeuralNetwork.Name = "_chkUseNeuralNetwork";
            _chkUseNeuralNetwork.Size = new Size(180, 25);
            _chkUseNeuralNetwork.TabIndex = 3;
            _chkUseNeuralNetwork.Text = "Use Neural Network Prediction";
            // 
            // _chkCollectTrainingData
            // 
            _chkCollectTrainingData.Location = new Point(230, 55);
            _chkCollectTrainingData.Name = "_chkCollectTrainingData";
            _chkCollectTrainingData.Size = new Size(150, 25);
            _chkCollectTrainingData.TabIndex = 4;
            _chkCollectTrainingData.Text = "Collect Training Data";
            // 
            // _prgTraining
            // 
            _prgTraining.Location = new Point(386, 62);
            _prgTraining.Name = "_prgTraining";
            _prgTraining.Size = new Size(248, 18);
            _prgTraining.TabIndex = 7;
            _prgTraining.Visible = false;
            // 
            // _lblTrainingStatus
            // 
            _lblTrainingStatus.BorderStyle = BorderStyle.FixedSingle;
            _lblTrainingStatus.Dock = DockStyle.Bottom;
            _lblTrainingStatus.Location = new Point(3, 84);
            _lblTrainingStatus.Name = "_lblTrainingStatus";
            _lblTrainingStatus.Size = new Size(636, 20);
            _lblTrainingStatus.TabIndex = 8;
            _lblTrainingStatus.Visible = false;
            // 
            // _tabMapSettings
            // 
            _tabMapSettings.Controls.Add(groupBox1);
            _tabMapSettings.Controls.Add(_grpMapPoints);
            _tabMapSettings.Controls.Add(_grpMapSource);
            _tabMapSettings.Controls.Add(_grpMapProperties);
            _tabMapSettings.Controls.Add(_grpRobotProperties);
            _tabMapSettings.Location = new Point(4, 24);
            _tabMapSettings.Name = "_tabMapSettings";
            _tabMapSettings.Size = new Size(642, 603);
            _tabMapSettings.TabIndex = 1;
            _tabMapSettings.Text = "Map & Robot";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(_nudDynamicObstacles);
            groupBox1.Controls.Add(_lblStatic);
            groupBox1.Controls.Add(_lblDynamic);
            groupBox1.Controls.Add(_nudStaticObstacles);
            groupBox1.Controls.Add(_nudRoughTerrain);
            groupBox1.Controls.Add(_lblRoughTerrain);
            groupBox1.Controls.Add(_nudSemiStaticObstacles);
            groupBox1.Controls.Add(_lblSemiStatic);
            groupBox1.Location = new Point(80, 175);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(480, 85);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Obstacles";
            // 
            // _nudDynamicObstacles
            // 
            _nudDynamicObstacles.Location = new Point(364, 52);
            _nudDynamicObstacles.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            _nudDynamicObstacles.Name = "_nudDynamicObstacles";
            _nudDynamicObstacles.Size = new Size(80, 23);
            _nudDynamicObstacles.TabIndex = 20;
            _nudDynamicObstacles.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // _lblStatic
            // 
            _lblStatic.Location = new Point(13, 27);
            _lblStatic.Name = "_lblStatic";
            _lblStatic.Size = new Size(110, 25);
            _lblStatic.TabIndex = 17;
            _lblStatic.Text = "Static Obstacles:";
            // 
            // _lblDynamic
            // 
            _lblDynamic.Location = new Point(225, 54);
            _lblDynamic.Name = "_lblDynamic";
            _lblDynamic.Size = new Size(120, 25);
            _lblDynamic.TabIndex = 19;
            _lblDynamic.Text = "Dynamic Obstacles:";
            // 
            // _nudStaticObstacles
            // 
            _nudStaticObstacles.Location = new Point(123, 25);
            _nudStaticObstacles.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _nudStaticObstacles.Name = "_nudStaticObstacles";
            _nudStaticObstacles.Size = new Size(80, 23);
            _nudStaticObstacles.TabIndex = 18;
            _nudStaticObstacles.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // _nudRoughTerrain
            // 
            _nudRoughTerrain.Enabled = false;
            _nudRoughTerrain.Location = new Point(364, 25);
            _nudRoughTerrain.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _nudRoughTerrain.Name = "_nudRoughTerrain";
            _nudRoughTerrain.Size = new Size(80, 23);
            _nudRoughTerrain.TabIndex = 7;
            _nudRoughTerrain.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // _lblRoughTerrain
            // 
            _lblRoughTerrain.Location = new Point(225, 27);
            _lblRoughTerrain.Name = "_lblRoughTerrain";
            _lblRoughTerrain.Size = new Size(90, 25);
            _lblRoughTerrain.TabIndex = 6;
            _lblRoughTerrain.Text = "Rough Terrain:";
            // 
            // _nudSemiStaticObstacles
            // 
            _nudSemiStaticObstacles.Enabled = false;
            _nudSemiStaticObstacles.Location = new Point(123, 52);
            _nudSemiStaticObstacles.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            _nudSemiStaticObstacles.Name = "_nudSemiStaticObstacles";
            _nudSemiStaticObstacles.Size = new Size(80, 23);
            _nudSemiStaticObstacles.TabIndex = 5;
            _nudSemiStaticObstacles.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // _lblSemiStatic
            // 
            _lblSemiStatic.Location = new Point(13, 54);
            _lblSemiStatic.Name = "_lblSemiStatic";
            _lblSemiStatic.Size = new Size(80, 25);
            _lblSemiStatic.TabIndex = 4;
            _lblSemiStatic.Text = "Semi-Static:";
            // 
            // _grpMapPoints
            // 
            _grpMapPoints.Controls.Add(_btnPickStartPoint);
            _grpMapPoints.Controls.Add(_chkUseCustomStartPoint);
            _grpMapPoints.Controls.Add(_lblCurrentStartPoint);
            _grpMapPoints.Controls.Add(_lblParking);
            _grpMapPoints.Controls.Add(_nudParkingCount);
            _grpMapPoints.Controls.Add(_nudGoalCount);
            _grpMapPoints.Controls.Add(_lblGoals);
            _grpMapPoints.Location = new Point(80, 268);
            _grpMapPoints.Name = "_grpMapPoints";
            _grpMapPoints.Size = new Size(480, 95);
            _grpMapPoints.TabIndex = 3;
            _grpMapPoints.TabStop = false;
            _grpMapPoints.Text = "Points";
            // 
            // _btnPickStartPoint
            // 
            _btnPickStartPoint.BackColor = Color.FromArgb(52, 152, 219);
            _btnPickStartPoint.Cursor = Cursors.Hand;
            _btnPickStartPoint.FlatStyle = FlatStyle.Flat;
            _btnPickStartPoint.ForeColor = Color.White;
            _btnPickStartPoint.Location = new Point(342, 26);
            _btnPickStartPoint.Name = "_btnPickStartPoint";
            _btnPickStartPoint.Size = new Size(102, 25);
            _btnPickStartPoint.TabIndex = 10;
            _btnPickStartPoint.Text = "Pick from Map";
            _btnPickStartPoint.UseVisualStyleBackColor = false;
            // 
            // _chkUseCustomStartPoint
            // 
            _chkUseCustomStartPoint.Location = new Point(148, 27);
            _chkUseCustomStartPoint.Name = "_chkUseCustomStartPoint";
            _chkUseCustomStartPoint.Size = new Size(150, 25);
            _chkUseCustomStartPoint.TabIndex = 9;
            _chkUseCustomStartPoint.Text = "Use Custom Start Point";
            // 
            // _lblCurrentStartPoint
            // 
            _lblCurrentStartPoint.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            _lblCurrentStartPoint.Location = new Point(23, 29);
            _lblCurrentStartPoint.Name = "_lblCurrentStartPoint";
            _lblCurrentStartPoint.Size = new Size(120, 25);
            _lblCurrentStartPoint.TabIndex = 8;
            _lblCurrentStartPoint.Text = "Current Start: (10, 10)";
            // 
            // _lblParking
            // 
            _lblParking.Location = new Point(23, 65);
            _lblParking.Name = "_lblParking";
            _lblParking.Size = new Size(90, 25);
            _lblParking.TabIndex = 15;
            _lblParking.Text = "Parking Count:";
            // 
            // _nudParkingCount
            // 
            _nudParkingCount.Location = new Point(123, 58);
            _nudParkingCount.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudParkingCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudParkingCount.Name = "_nudParkingCount";
            _nudParkingCount.Size = new Size(80, 23);
            _nudParkingCount.TabIndex = 16;
            _nudParkingCount.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // _nudGoalCount
            // 
            _nudGoalCount.Location = new Point(364, 58);
            _nudGoalCount.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            _nudGoalCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudGoalCount.Name = "_nudGoalCount";
            _nudGoalCount.Size = new Size(80, 23);
            _nudGoalCount.TabIndex = 14;
            _nudGoalCount.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // _lblGoals
            // 
            _lblGoals.Location = new Point(231, 65);
            _lblGoals.Name = "_lblGoals";
            _lblGoals.Size = new Size(80, 25);
            _lblGoals.TabIndex = 13;
            _lblGoals.Text = "Goal Count:";
            // 
            // _grpMapSource
            // 
            _grpMapSource.Controls.Add(_rbLoadMap);
            _grpMapSource.Controls.Add(_chkUseCurrentMap);
            _grpMapSource.Controls.Add(_txtMapFilePath);
            _grpMapSource.Controls.Add(_btnBrowseMap);
            _grpMapSource.Location = new Point(80, 15);
            _grpMapSource.Name = "_grpMapSource";
            _grpMapSource.Size = new Size(480, 79);
            _grpMapSource.TabIndex = 0;
            _grpMapSource.TabStop = false;
            _grpMapSource.Text = "Map Source";
            // 
            // _rbLoadMap
            // 
            _rbLoadMap.Checked = true;
            _rbLoadMap.Location = new Point(15, 20);
            _rbLoadMap.Name = "_rbLoadMap";
            _rbLoadMap.Size = new Size(120, 25);
            _rbLoadMap.TabIndex = 0;
            _rbLoadMap.TabStop = true;
            _rbLoadMap.Text = "Load External Map";
            // 
            // _chkUseCurrentMap
            // 
            _chkUseCurrentMap.Checked = true;
            _chkUseCurrentMap.CheckState = CheckState.Checked;
            _chkUseCurrentMap.Location = new Point(140, 20);
            _chkUseCurrentMap.Name = "_chkUseCurrentMap";
            _chkUseCurrentMap.Size = new Size(200, 25);
            _chkUseCurrentMap.TabIndex = 1;
            _chkUseCurrentMap.Text = "Use Current Map from Main Form";
            // 
            // _txtMapFilePath
            // 
            _txtMapFilePath.Enabled = false;
            _txtMapFilePath.Location = new Point(15, 50);
            _txtMapFilePath.Name = "_txtMapFilePath";
            _txtMapFilePath.Size = new Size(350, 23);
            _txtMapFilePath.TabIndex = 2;
            // 
            // _btnBrowseMap
            // 
            _btnBrowseMap.BackColor = Color.FromArgb(52, 152, 219);
            _btnBrowseMap.Enabled = false;
            _btnBrowseMap.FlatStyle = FlatStyle.Flat;
            _btnBrowseMap.ForeColor = Color.White;
            _btnBrowseMap.Location = new Point(375, 48);
            _btnBrowseMap.Name = "_btnBrowseMap";
            _btnBrowseMap.Size = new Size(40, 25);
            _btnBrowseMap.TabIndex = 3;
            _btnBrowseMap.Text = "...";
            _btnBrowseMap.UseVisualStyleBackColor = false;
            // 
            // _grpMapProperties
            // 
            _grpMapProperties.Controls.Add(_lblGridWidth);
            _grpMapProperties.Controls.Add(_nudGridWidth);
            _grpMapProperties.Controls.Add(_lblGridHeight);
            _grpMapProperties.Controls.Add(_nudGridHeight);
            _grpMapProperties.Controls.Add(_lblCellSize);
            _grpMapProperties.Controls.Add(_nudCellSize);
            _grpMapProperties.Location = new Point(80, 96);
            _grpMapProperties.Name = "_grpMapProperties";
            _grpMapProperties.Size = new Size(480, 79);
            _grpMapProperties.TabIndex = 1;
            _grpMapProperties.TabStop = false;
            _grpMapProperties.Text = "Map Properties";
            // 
            // _lblGridWidth
            // 
            _lblGridWidth.Location = new Point(7, 27);
            _lblGridWidth.Name = "_lblGridWidth";
            _lblGridWidth.Size = new Size(80, 25);
            _lblGridWidth.TabIndex = 0;
            _lblGridWidth.Text = "Grid Width:";
            // 
            // _nudGridWidth
            // 
            _nudGridWidth.Enabled = false;
            _nudGridWidth.Location = new Point(123, 20);
            _nudGridWidth.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            _nudGridWidth.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudGridWidth.Name = "_nudGridWidth";
            _nudGridWidth.Size = new Size(80, 23);
            _nudGridWidth.TabIndex = 1;
            _nudGridWidth.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // _lblGridHeight
            // 
            _lblGridHeight.Location = new Point(248, 23);
            _lblGridHeight.Name = "_lblGridHeight";
            _lblGridHeight.Size = new Size(80, 25);
            _lblGridHeight.TabIndex = 2;
            _lblGridHeight.Text = "Grid Height:";
            // 
            // _nudGridHeight
            // 
            _nudGridHeight.Enabled = false;
            _nudGridHeight.Location = new Point(364, 22);
            _nudGridHeight.Name = "_nudGridHeight";
            _nudGridHeight.Size = new Size(80, 23);
            _nudGridHeight.TabIndex = 3;
            _nudGridHeight.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // _lblCellSize
            // 
            _lblCellSize.Location = new Point(6, 50);
            _lblCellSize.Name = "_lblCellSize";
            _lblCellSize.Size = new Size(110, 25);
            _lblCellSize.TabIndex = 11;
            _lblCellSize.Text = "Cell Size (pixels):";
            // 
            // _nudCellSize
            // 
            _nudCellSize.Location = new Point(123, 46);
            _nudCellSize.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudCellSize.Name = "_nudCellSize";
            _nudCellSize.Size = new Size(80, 23);
            _nudCellSize.TabIndex = 12;
            _nudCellSize.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // _grpRobotProperties
            // 
            _grpRobotProperties.Controls.Add(_nudDetectionRange);
            _grpRobotProperties.Controls.Add(_nudRobotHeight);
            _grpRobotProperties.Controls.Add(_nudRobotWidth);
            _grpRobotProperties.Controls.Add(_nudViewAngle);
            _grpRobotProperties.Controls.Add(_lblRobotName);
            _grpRobotProperties.Controls.Add(_txtRobotName);
            _grpRobotProperties.Controls.Add(_lblRobotSpeed);
            _grpRobotProperties.Controls.Add(_nudRobotSpeed);
            _grpRobotProperties.Controls.Add(_lblRobotBattery);
            _grpRobotProperties.Controls.Add(_nudRobotBattery);
            _grpRobotProperties.Controls.Add(_lblConsumption);
            _grpRobotProperties.Controls.Add(_nudConsumptionRate);
            _grpRobotProperties.Controls.Add(label2);
            _grpRobotProperties.Controls.Add(_lblViewAngle);
            _grpRobotProperties.Controls.Add(label1);
            _grpRobotProperties.Controls.Add(_lblDetection);
            _grpRobotProperties.Controls.Add(_chkEnableDynamicCharging);
            _grpRobotProperties.Controls.Add(_lblChargingTime);
            _grpRobotProperties.Controls.Add(_nudChargingTime);
            _grpRobotProperties.Controls.Add(_lblSafetyMargin);
            _grpRobotProperties.Controls.Add(_nudSafetyMargin);
            _grpRobotProperties.Controls.Add(_lblSafetyMarginUnit);
            _grpRobotProperties.Location = new Point(80, 366);
            _grpRobotProperties.Name = "_grpRobotProperties";
            _grpRobotProperties.Size = new Size(480, 213);
            _grpRobotProperties.TabIndex = 2;
            _grpRobotProperties.TabStop = false;
            _grpRobotProperties.Text = "Robot Configuration";
            // 
            // _nudDetectionRange
            // 
            _nudDetectionRange.Location = new Point(374, 76);
            _nudDetectionRange.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            _nudDetectionRange.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudDetectionRange.Name = "_nudDetectionRange";
            _nudDetectionRange.Size = new Size(80, 23);
            _nudDetectionRange.TabIndex = 11;
            _nudDetectionRange.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // _nudRobotHeight
            // 
            _nudRobotHeight.Location = new Point(372, 47);
            _nudRobotHeight.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            _nudRobotHeight.Name = "_nudRobotHeight";
            _nudRobotHeight.Size = new Size(80, 23);
            _nudRobotHeight.TabIndex = 9;
            _nudRobotHeight.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // _nudRobotWidth
            // 
            _nudRobotWidth.Location = new Point(131, 46);
            _nudRobotWidth.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            _nudRobotWidth.Name = "_nudRobotWidth";
            _nudRobotWidth.Size = new Size(80, 23);
            _nudRobotWidth.TabIndex = 9;
            _nudRobotWidth.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // _nudViewAngle
            // 
            _nudViewAngle.Location = new Point(131, 75);
            _nudViewAngle.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            _nudViewAngle.Minimum = new decimal(new int[] { 45, 0, 0, 0 });
            _nudViewAngle.Name = "_nudViewAngle";
            _nudViewAngle.Size = new Size(80, 23);
            _nudViewAngle.TabIndex = 9;
            _nudViewAngle.Value = new decimal(new int[] { 180, 0, 0, 0 });
            // 
            // _lblRobotName
            // 
            _lblRobotName.Location = new Point(15, 25);
            _lblRobotName.Name = "_lblRobotName";
            _lblRobotName.Size = new Size(80, 25);
            _lblRobotName.TabIndex = 0;
            _lblRobotName.Text = "Robot Name:";
            // 
            // _txtRobotName
            // 
            _txtRobotName.Location = new Point(131, 22);
            _txtRobotName.Name = "_txtRobotName";
            _txtRobotName.Size = new Size(80, 23);
            _txtRobotName.TabIndex = 1;
            _txtRobotName.Text = "SallamBot";
            // 
            // _lblRobotSpeed
            // 
            _lblRobotSpeed.Location = new Point(245, 25);
            _lblRobotSpeed.Name = "_lblRobotSpeed";
            _lblRobotSpeed.Size = new Size(85, 25);
            _lblRobotSpeed.TabIndex = 2;
            _lblRobotSpeed.Text = "Speed (cm/s):";
            // 
            // _nudRobotSpeed
            // 
            _nudRobotSpeed.Location = new Point(372, 23);
            _nudRobotSpeed.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudRobotSpeed.Name = "_nudRobotSpeed";
            _nudRobotSpeed.Size = new Size(80, 23);
            _nudRobotSpeed.TabIndex = 3;
            _nudRobotSpeed.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // _lblRobotBattery
            // 
            _lblRobotBattery.Location = new Point(17, 116);
            _lblRobotBattery.Name = "_lblRobotBattery";
            _lblRobotBattery.Size = new Size(80, 25);
            _lblRobotBattery.TabIndex = 4;
            _lblRobotBattery.Text = "Battery (%):";
            // 
            // _nudRobotBattery
            // 
            _nudRobotBattery.Location = new Point(131, 113);
            _nudRobotBattery.Name = "_nudRobotBattery";
            _nudRobotBattery.Size = new Size(80, 23);
            _nudRobotBattery.TabIndex = 5;
            _nudRobotBattery.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // _lblConsumption
            // 
            _lblConsumption.Location = new Point(245, 116);
            _lblConsumption.Name = "_lblConsumption";
            _lblConsumption.Size = new Size(121, 25);
            _lblConsumption.TabIndex = 6;
            _lblConsumption.Text = "Consumption (%/m):";
            // 
            // _nudConsumptionRate
            // 
            _nudConsumptionRate.DecimalPlaces = 1;
            _nudConsumptionRate.Location = new Point(372, 114);
            _nudConsumptionRate.Name = "_nudConsumptionRate";
            _nudConsumptionRate.Size = new Size(80, 23);
            _nudConsumptionRate.TabIndex = 7;
            _nudConsumptionRate.Value = new decimal(new int[] { 1, 0, 0, 65536 });
            // 
            // label2
            // 
            label2.Location = new Point(16, 48);
            label2.Name = "label2";
            label2.Size = new Size(88, 25);
            label2.TabIndex = 8;
            label2.Text = "Height (cm):";
            // 
            // _lblViewAngle
            // 
            _lblViewAngle.Location = new Point(7, 78);
            _lblViewAngle.Name = "_lblViewAngle";
            _lblViewAngle.Size = new Size(100, 25);
            _lblViewAngle.TabIndex = 8;
            _lblViewAngle.Text = "View Angle (deg):";
            // 
            // label1
            // 
            label1.Location = new Point(247, 49);
            label1.Name = "label1";
            label1.Size = new Size(110, 25);
            label1.TabIndex = 10;
            label1.Text = "Width (cm):";
            // 
            // _lblDetection
            // 
            _lblDetection.Location = new Point(247, 78);
            _lblDetection.Name = "_lblDetection";
            _lblDetection.Size = new Size(110, 25);
            _lblDetection.TabIndex = 10;
            _lblDetection.Text = "Detection Range:";
            // 
            // _chkEnableDynamicCharging
            // 
            _chkEnableDynamicCharging.Location = new Point(16, 142);
            _chkEnableDynamicCharging.Name = "_chkEnableDynamicCharging";
            _chkEnableDynamicCharging.Size = new Size(160, 25);
            _chkEnableDynamicCharging.TabIndex = 12;
            _chkEnableDynamicCharging.Text = "Enable Dynamic Charging";
            // 
            // _lblChargingTime
            // 
            _lblChargingTime.Location = new Point(251, 171);
            _lblChargingTime.Name = "_lblChargingTime";
            _lblChargingTime.Size = new Size(120, 25);
            _lblChargingTime.TabIndex = 13;
            _lblChargingTime.Text = "Charging Time (sec):";
            // 
            // _nudChargingTime
            // 
            _nudChargingTime.Enabled = false;
            _nudChargingTime.Location = new Point(372, 168);
            _nudChargingTime.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            _nudChargingTime.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            _nudChargingTime.Name = "_nudChargingTime";
            _nudChargingTime.Size = new Size(80, 23);
            _nudChargingTime.TabIndex = 14;
            _nudChargingTime.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // _lblSafetyMargin
            // 
            _lblSafetyMargin.Location = new Point(16, 173);
            _lblSafetyMargin.Name = "_lblSafetyMargin";
            _lblSafetyMargin.Size = new Size(90, 25);
            _lblSafetyMargin.TabIndex = 16;
            _lblSafetyMargin.Text = "Safety Margin:";
            // 
            // _nudSafetyMargin
            // 
            _nudSafetyMargin.DecimalPlaces = 1;
            _nudSafetyMargin.Enabled = false;
            _nudSafetyMargin.Location = new Point(131, 169);
            _nudSafetyMargin.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            _nudSafetyMargin.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            _nudSafetyMargin.Name = "_nudSafetyMargin";
            _nudSafetyMargin.Size = new Size(80, 23);
            _nudSafetyMargin.TabIndex = 17;
            _nudSafetyMargin.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // _lblSafetyMarginUnit
            // 
            _lblSafetyMarginUnit.Location = new Point(219, 171);
            _lblSafetyMarginUnit.Name = "_lblSafetyMarginUnit";
            _lblSafetyMarginUnit.Size = new Size(25, 20);
            _lblSafetyMarginUnit.TabIndex = 18;
            _lblSafetyMarginUnit.Text = "%";
            // 
            // _tabSensitivity
            // 
            _tabSensitivity.Controls.Add(_chkEnableSensitivity);
            _tabSensitivity.Controls.Add(pnlSensitivity);
            _tabSensitivity.Location = new Point(4, 24);
            _tabSensitivity.Name = "_tabSensitivity";
            _tabSensitivity.Size = new Size(642, 603);
            _tabSensitivity.TabIndex = 2;
            _tabSensitivity.Text = "Sensitivity Analysis";
            // 
            // _chkEnableSensitivity
            // 
            _chkEnableSensitivity.Location = new Point(51, 15);
            _chkEnableSensitivity.Name = "_chkEnableSensitivity";
            _chkEnableSensitivity.Size = new Size(121, 24);
            _chkEnableSensitivity.TabIndex = 0;
            _chkEnableSensitivity.Text = "EnableSensitivity";
            // 
            // pnlSensitivity
            // 
            pnlSensitivity.BorderStyle = BorderStyle.FixedSingle;
            pnlSensitivity.Controls.Add(_dgvSensitivityResults);
            pnlSensitivity.Controls.Add(_lblAlgorithm);
            pnlSensitivity.Controls.Add(_lblParameter);
            pnlSensitivity.Controls.Add(_cboAlgorithm);
            pnlSensitivity.Controls.Add(_cboSensitivityParameter);
            pnlSensitivity.Controls.Add(_lblValues);
            pnlSensitivity.Controls.Add(_txtSensitivityValues);
            pnlSensitivity.Controls.Add(_btnValidateValues);
            pnlSensitivity.Controls.Add(_btnRunSensitivity);
            pnlSensitivity.Controls.Add(_lblSensitivityStatus);
            pnlSensitivity.Location = new Point(50, 45);
            pnlSensitivity.Name = "pnlSensitivity";
            pnlSensitivity.Size = new Size(548, 502);
            pnlSensitivity.TabIndex = 0;
            // 
            // _dgvSensitivityResults
            // 
            _dgvSensitivityResults.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5 });
            _dgvSensitivityResults.Dock = DockStyle.Bottom;
            _dgvSensitivityResults.Location = new Point(0, 141);
            _dgvSensitivityResults.Name = "_dgvSensitivityResults";
            _dgvSensitivityResults.Size = new Size(546, 359);
            _dgvSensitivityResults.TabIndex = 6;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Parameter Value";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Path Length (cells)";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Time (ms)";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.HeaderText = "Success";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Collisions";
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // _lblParameter
            // 
            _lblParameter.Location = new Point(44, 38);
            _lblParameter.Name = "_lblParameter";
            _lblParameter.Size = new Size(100, 23);
            _lblParameter.TabIndex = 1;
            _lblParameter.Text = "Parameter:";
            // 
            // _cboSensitivityParameter
            // 
            _cboSensitivityParameter.Items.AddRange(new object[] { "Lambda (λ) - Obstacle weight", "LearningRate (α) - Memory weight (SPPA-DL only)", "PredictionWeight (β) - ML risk weight (SPPA-DL only)", "Alpha_S - Static obstacle weight", "Alpha_SS - Semi-static obstacle weight", "Alpha_D - Dynamic obstacle weight" });
            _cboSensitivityParameter.Location = new Point(150, 36);
            _cboSensitivityParameter.Name = "_cboSensitivityParameter";
            _cboSensitivityParameter.Size = new Size(229, 23);
            _cboSensitivityParameter.TabIndex = 2;
            _cboSensitivityParameter.Text = "Lambda (λ) - Obstacle weight";
            // 
            // _lblValues
            // 
            _lblValues.Location = new Point(44, 70);
            _lblValues.Name = "_lblValues";
            _lblValues.Size = new Size(100, 23);
            _lblValues.TabIndex = 3;
            _lblValues.Text = "Values:";
            // 
            // _txtSensitivityValues
            // 
            _txtSensitivityValues.Location = new Point(150, 69);
            _txtSensitivityValues.Name = "_txtSensitivityValues";
            _txtSensitivityValues.Size = new Size(121, 23);
            _txtSensitivityValues.TabIndex = 4;
            // 
            // _btnValidateValues
            // 
            _btnValidateValues.Location = new Point(277, 63);
            _btnValidateValues.Name = "_btnValidateValues";
            _btnValidateValues.Size = new Size(102, 23);
            _btnValidateValues.TabIndex = 5;
            _btnValidateValues.Text = "Validate";
            // 
            // _btnRunSensitivity
            // 
            _btnRunSensitivity.BackColor = Color.FromArgb(52, 152, 219);
            _btnRunSensitivity.FlatStyle = FlatStyle.Flat;
            _btnRunSensitivity.ForeColor = Color.White;
            _btnRunSensitivity.Location = new Point(277, 90);
            _btnRunSensitivity.Name = "_btnRunSensitivity";
            _btnRunSensitivity.Size = new Size(102, 26);
            _btnRunSensitivity.TabIndex = 7;
            _btnRunSensitivity.Text = "RunSensitivity";
            _btnRunSensitivity.UseVisualStyleBackColor = false;
            // 
            // _lblSensitivityStatus
            // 
            _lblSensitivityStatus.BorderStyle = BorderStyle.FixedSingle;
            _lblSensitivityStatus.Location = new Point(44, 93);
            _lblSensitivityStatus.Name = "_lblSensitivityStatus";
            _lblSensitivityStatus.Size = new Size(227, 23);
            _lblSensitivityStatus.TabIndex = 8;
            _lblSensitivityStatus.Text = "SensitivityStatus:";
            // 
            // _tabExperimentSettings
            // 
            _tabExperimentSettings.Controls.Add(_grpExperimentSettings);
            _tabExperimentSettings.Controls.Add(_grpInfo);
            _tabExperimentSettings.Location = new Point(4, 24);
            _tabExperimentSettings.Name = "_tabExperimentSettings";
            _tabExperimentSettings.Size = new Size(642, 603);
            _tabExperimentSettings.TabIndex = 3;
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
            _grpExperimentSettings.Location = new Point(50, 20);
            _grpExperimentSettings.Name = "_grpExperimentSettings";
            _grpExperimentSettings.Size = new Size(540, 160);
            _grpExperimentSettings.TabIndex = 0;
            _grpExperimentSettings.TabStop = false;
            _grpExperimentSettings.Text = "Experiment Settings";
            // 
            // _lblExpName
            // 
            _lblExpName.Location = new Point(15, 30);
            _lblExpName.Name = "_lblExpName";
            _lblExpName.Size = new Size(110, 25);
            _lblExpName.TabIndex = 0;
            _lblExpName.Text = "Experiment Name:";
            // 
            // _txtExperimentName
            // 
            _txtExperimentName.Location = new Point(140, 28);
            _txtExperimentName.Name = "_txtExperimentName";
            _txtExperimentName.Size = new Size(280, 23);
            _txtExperimentName.TabIndex = 1;
            _txtExperimentName.Text = "Experiment";
            // 
            // _lblIterations
            // 
            _lblIterations.Location = new Point(15, 65);
            _lblIterations.Name = "_lblIterations";
            _lblIterations.Size = new Size(70, 25);
            _lblIterations.TabIndex = 2;
            _lblIterations.Text = "Iterations:";
            // 
            // _nudIterations
            // 
            _nudIterations.Location = new Point(140, 63);
            _nudIterations.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            _nudIterations.Name = "_nudIterations";
            _nudIterations.Size = new Size(80, 23);
            _nudIterations.TabIndex = 3;
            _nudIterations.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // _chkSaveScreenshots
            // 
            _chkSaveScreenshots.Checked = true;
            _chkSaveScreenshots.CheckState = CheckState.Checked;
            _chkSaveScreenshots.Location = new Point(250, 63);
            _chkSaveScreenshots.Name = "_chkSaveScreenshots";
            _chkSaveScreenshots.Size = new Size(120, 25);
            _chkSaveScreenshots.TabIndex = 4;
            _chkSaveScreenshots.Text = "Save Screenshots";
            // 
            // _chkSaveReplay
            // 
            _chkSaveReplay.Checked = true;
            _chkSaveReplay.CheckState = CheckState.Checked;
            _chkSaveReplay.Location = new Point(380, 63);
            _chkSaveReplay.Name = "_chkSaveReplay";
            _chkSaveReplay.Size = new Size(100, 25);
            _chkSaveReplay.TabIndex = 5;
            _chkSaveReplay.Text = "Save Replay";
            // 
            // _chkShowPathOnScreenshots
            // 
            _chkShowPathOnScreenshots.Checked = true;
            _chkShowPathOnScreenshots.CheckState = CheckState.Checked;
            _chkShowPathOnScreenshots.Location = new Point(140, 95);
            _chkShowPathOnScreenshots.Name = "_chkShowPathOnScreenshots";
            _chkShowPathOnScreenshots.Size = new Size(200, 25);
            _chkShowPathOnScreenshots.TabIndex = 6;
            _chkShowPathOnScreenshots.Text = "Show Path on Screenshots";
            // 
            // _lblSavePath
            // 
            _lblSavePath.Location = new Point(15, 130);
            _lblSavePath.Name = "_lblSavePath";
            _lblSavePath.Size = new Size(90, 25);
            _lblSavePath.TabIndex = 7;
            _lblSavePath.Text = "Save Location:";
            // 
            // _txtSavePath
            // 
            _txtSavePath.BackColor = Color.WhiteSmoke;
            _txtSavePath.Location = new Point(140, 128);
            _txtSavePath.Name = "_txtSavePath";
            _txtSavePath.ReadOnly = true;
            _txtSavePath.Size = new Size(320, 23);
            _txtSavePath.TabIndex = 8;
            // 
            // _btnBrowseSavePath
            // 
            _btnBrowseSavePath.BackColor = Color.FromArgb(52, 152, 219);
            _btnBrowseSavePath.FlatStyle = FlatStyle.Flat;
            _btnBrowseSavePath.ForeColor = Color.White;
            _btnBrowseSavePath.Location = new Point(470, 126);
            _btnBrowseSavePath.Name = "_btnBrowseSavePath";
            _btnBrowseSavePath.Size = new Size(40, 25);
            _btnBrowseSavePath.TabIndex = 9;
            _btnBrowseSavePath.Text = "...";
            _btnBrowseSavePath.UseVisualStyleBackColor = false;
            // 
            // _grpInfo
            // 
            _grpInfo.Controls.Add(_lblInfo);
            _grpInfo.Location = new Point(50, 200);
            _grpInfo.Name = "_grpInfo";
            _grpInfo.Size = new Size(540, 138);
            _grpInfo.TabIndex = 1;
            _grpInfo.TabStop = false;
            _grpInfo.Text = "Information";
            // 
            // _lblInfo
            // 
            _lblInfo.Font = new Font("Segoe UI", 9F);
            _lblInfo.ForeColor = Color.DarkBlue;
            _lblInfo.Location = new Point(15, 20);
            _lblInfo.Name = "_lblInfo";
            _lblInfo.Size = new Size(510, 108);
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
            _bottomPanel.Location = new Point(0, 631);
            _bottomPanel.Name = "_bottomPanel";
            _bottomPanel.Padding = new Padding(5);
            _bottomPanel.Size = new Size(650, 85);
            _bottomPanel.TabIndex = 1;
            // 
            // _buttonPanel
            // 
            _buttonPanel.Controls.Add(_btnRunComparison);
            _buttonPanel.Controls.Add(_btnSaveSettings);
            _buttonPanel.Controls.Add(_btnLoadSettings);
            _buttonPanel.Controls.Add(_btnCancel);
            _buttonPanel.Dock = DockStyle.Top;
            _buttonPanel.Location = new Point(5, 47);
            _buttonPanel.Name = "_buttonPanel";
            _buttonPanel.Size = new Size(640, 35);
            _buttonPanel.TabIndex = 0;
            // 
            // _btnRunComparison
            // 
            _btnRunComparison.BackColor = Color.FromArgb(46, 204, 113);
            _btnRunComparison.Cursor = Cursors.Hand;
            _btnRunComparison.FlatStyle = FlatStyle.Flat;
            _btnRunComparison.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _btnRunComparison.ForeColor = Color.White;
            _btnRunComparison.Location = new Point(10, 5);
            _btnRunComparison.Name = "_btnRunComparison";
            _btnRunComparison.Size = new Size(130, 28);
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
            _btnSaveSettings.Location = new Point(150, 5);
            _btnSaveSettings.Name = "_btnSaveSettings";
            _btnSaveSettings.Size = new Size(120, 28);
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
            _btnLoadSettings.Location = new Point(280, 5);
            _btnLoadSettings.Name = "_btnLoadSettings";
            _btnLoadSettings.Size = new Size(120, 28);
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
            _btnCancel.Location = new Point(410, 5);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(100, 28);
            _btnCancel.TabIndex = 3;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = false;
            // 
            // _lblStatus
            // 
            _lblStatus.BackColor = Color.White;
            _lblStatus.BorderStyle = BorderStyle.FixedSingle;
            _lblStatus.Dock = DockStyle.Top;
            _lblStatus.Location = new Point(5, 22);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Size = new Size(640, 25);
            _lblStatus.TabIndex = 1;
            _lblStatus.Text = "Ready";
            _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _progressBar
            // 
            _progressBar.Dock = DockStyle.Top;
            _progressBar.Location = new Point(5, 5);
            _progressBar.Name = "_progressBar";
            _progressBar.Size = new Size(640, 17);
            _progressBar.Style = ProgressBarStyle.Continuous;
            _progressBar.TabIndex = 2;
            // 
            // colEnabled
            // 
            colEnabled.Name = "colEnabled";
            // 
            // colAlgorithm
            // 
            colAlgorithm.Name = "colAlgorithm";
            // 
            // colMetric
            // 
            colMetric.Name = "colMetric";
            // 
            // colParameters
            // 
            colParameters.Name = "colParameters";
            // 
            // colEdit
            // 
            colEdit.Name = "colEdit";
            // 
            // colDuplicate
            // 
            colDuplicate.Name = "colDuplicate";
            // 
            // _cboAlgorithm
            // 
            _cboAlgorithm.Text = "Lambda (λ) - Obstacle weight";
            _cboAlgorithm.Location = new Point(150, 9);
            _cboAlgorithm.Name = "_cboAlgorithm";
            _cboAlgorithm.Size = new Size(229, 23);
            _cboAlgorithm.TabIndex = 2;
            _cboAlgorithm.Text = "Lambda (λ) - Obstacle weight";
            // 
            // _lblAlgorithm
            // 
            _lblAlgorithm.Location = new Point(44, 11);
            _lblAlgorithm.Name = "_lblAlgorithm";
            _lblAlgorithm.Size = new Size(100, 23);
            _lblAlgorithm.TabIndex = 1;
            _lblAlgorithm.Text = "Algorithm:";
            // 
            // frmExperimentDesigner
            // 
            BackColor = Color.White;
            ClientSize = new Size(650, 716);
            Controls.Add(_mainTabControl);
            Controls.Add(_bottomPanel);
            MinimumSize = new Size(600, 600);
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
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_nudDynamicObstacles).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudStaticObstacles).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudRoughTerrain).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudSemiStaticObstacles).EndInit();
            _grpMapPoints.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_nudParkingCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudGoalCount).EndInit();
            _grpMapSource.ResumeLayout(false);
            _grpMapSource.PerformLayout();
            _grpMapProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_nudGridWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudGridHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudCellSize).EndInit();
            _grpRobotProperties.ResumeLayout(false);
            _grpRobotProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_nudDetectionRange).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudViewAngle).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudRobotBattery).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudConsumptionRate).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudChargingTime).EndInit();
            ((System.ComponentModel.ISupportInitialize)_nudSafetyMargin).EndInit();
            _tabSensitivity.ResumeLayout(false);
            pnlSensitivity.ResumeLayout(false);
            pnlSensitivity.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_dgvSensitivityResults).EndInit();
            _tabExperimentSettings.ResumeLayout(false);
            _grpExperimentSettings.ResumeLayout(false);
            _grpExperimentSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_nudIterations).EndInit();
            _grpInfo.ResumeLayout(false);
            _bottomPanel.ResumeLayout(false);
            _buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel pnlSensitivity;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private NumericUpDown _nudRobotHeight;
        private NumericUpDown _nudRobotWidth;
        private Label label2;
        private Label label1;
        private GroupBox _grpMapPoints;
        private GroupBox groupBox1;
        private Label _lblAlgorithm;
        private ComboBox _cboAlgorithm;
    }
}