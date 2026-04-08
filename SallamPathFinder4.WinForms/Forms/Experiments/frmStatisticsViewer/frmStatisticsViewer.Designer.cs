#region File Header
/// <summary>
/// File: frmStatisticsViewer.Designer.cs
/// Description: Designer file for statistics viewer form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmStatisticsViewer
{
    partial class frmStatisticsViewer
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.Label _lblSummary;
        private System.Windows.Forms.DataGridView _dgvAlgorithmStats;
        private System.Windows.Forms.DataGridView _dgvMetricStats;
        private System.Windows.Forms.DataGridView _dgvCollisionStats;
        private System.Windows.Forms.Button _btnExportCharts;
        private System.Windows.Forms.Button _btnExportData;
        private System.Windows.Forms.Button _btnClose;
        #endregion

        #region Chart Controls
        private System.Windows.Forms.DataVisualization.Charting.Chart _chartTime;
        private System.Windows.Forms.DataVisualization.Charting.Chart _chartSuccess;
        private System.Windows.Forms.DataVisualization.Charting.Chart _chartCollisions;
        private System.Windows.Forms.DataVisualization.Charting.Chart _chartSpeed;
        private System.Windows.Forms.DataVisualization.Charting.Chart _chartTrend;
        #endregion

        #region Constructor
        public frmStatisticsViewer()
        {
            InitializeComponent();
        }
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
            _components = new System.ComponentModel.Container();

            // Form Settings
            this.Text = "Advanced Statistics Viewer";
            this.Size = new System.Drawing.Size(1100, 750);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.White;

            // Tab Control
            _tabControl = new System.Windows.Forms.TabControl();
            _tabControl.Dock = DockStyle.Fill;

            // Summary Tab
            var tabSummary = new System.Windows.Forms.TabPage("Summary");
            var summaryPanel = new System.Windows.Forms.Panel();
            summaryPanel.Dock = DockStyle.Fill;
            summaryPanel.Padding = new System.Windows.Forms.Padding(10);

            _lblSummary = new System.Windows.Forms.Label();
            _lblSummary.Dock = DockStyle.Top;
            _lblSummary.Height = 200;
            _lblSummary.Font = new System.Drawing.Font("Segoe UI", 11);
            _lblSummary.TextAlign = ContentAlignment.MiddleLeft;
            _lblSummary.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            _lblSummary.Padding = new System.Windows.Forms.Padding(10);

            summaryPanel.Controls.Add(_lblSummary);
            tabSummary.Controls.Add(summaryPanel);

            // Algorithm Stats Tab
            var tabAlgorithms = new System.Windows.Forms.TabPage("By Algorithm");
            var algoPanel = new System.Windows.Forms.Panel();
            algoPanel.Dock = DockStyle.Fill;
            algoPanel.Padding = new System.Windows.Forms.Padding(10);

            _dgvAlgorithmStats = CreateDataGridView();
            _dgvAlgorithmStats.Columns.Add("Algorithm", "Algorithm");
            _dgvAlgorithmStats.Columns.Add("TotalRuns", "Total Runs");
            _dgvAlgorithmStats.Columns.Add("SuccessCount", "Success");
            _dgvAlgorithmStats.Columns.Add("SuccessRate", "Success Rate (%)");
            _dgvAlgorithmStats.Columns.Add("AvgTime", "Avg Time (ms)");
            _dgvAlgorithmStats.Columns.Add("MinTime", "Min Time (ms)");
            _dgvAlgorithmStats.Columns.Add("MaxTime", "Max Time (ms)");
            _dgvAlgorithmStats.Columns.Add("StdDevTime", "Std Dev Time");
            _dgvAlgorithmStats.Columns.Add("AvgLength", "Avg Length");
            _dgvAlgorithmStats.Columns.Add("MinLength", "Min Length");
            _dgvAlgorithmStats.Columns.Add("MaxLength", "Max Length");
            _dgvAlgorithmStats.Columns.Add("AvgBattery", "Avg Battery (%)");
            _dgvAlgorithmStats.Columns.Add("AvgCollisions", "Avg Collisions");
            _dgvAlgorithmStats.Columns.Add("AvgErrors", "Avg Errors");
            _dgvAlgorithmStats.Columns.Add("AvgSpeed", "Avg Speed (cm/s)");

            algoPanel.Controls.Add(_dgvAlgorithmStats);
            tabAlgorithms.Controls.Add(algoPanel);

            // Metric Stats Tab
            var tabMetrics = new System.Windows.Forms.TabPage("By Metric");
            var metricPanel = new System.Windows.Forms.Panel();
            metricPanel.Dock = DockStyle.Fill;
            metricPanel.Padding = new System.Windows.Forms.Padding(10);

            _dgvMetricStats = CreateDataGridView();
            _dgvMetricStats.Columns.Add("Metric", "Distance Metric");
            _dgvMetricStats.Columns.Add("TotalRuns", "Total Runs");
            _dgvMetricStats.Columns.Add("SuccessRate", "Success Rate (%)");
            _dgvMetricStats.Columns.Add("AvgTime", "Avg Time (ms)");
            _dgvMetricStats.Columns.Add("AvgLength", "Avg Length");
            _dgvMetricStats.Columns.Add("AvgBattery", "Avg Battery (%)");
            _dgvMetricStats.Columns.Add("AvgCollisions", "Avg Collisions");

            metricPanel.Controls.Add(_dgvMetricStats);
            tabMetrics.Controls.Add(metricPanel);

            // Collision Stats Tab
            var tabCollisions = new System.Windows.Forms.TabPage("Collisions & Errors");
            var collisionPanel = new System.Windows.Forms.Panel();
            collisionPanel.Dock = DockStyle.Fill;
            collisionPanel.Padding = new System.Windows.Forms.Padding(10);

            _dgvCollisionStats = CreateDataGridView();
            _dgvCollisionStats.Columns.Add("Algorithm", "Algorithm");
            _dgvCollisionStats.Columns.Add("TotalCollisions", "Total Collisions");
            _dgvCollisionStats.Columns.Add("AvgCollisions", "Avg Collisions");
            _dgvCollisionStats.Columns.Add("MaxCollisions", "Max Collisions");
            _dgvCollisionStats.Columns.Add("TotalErrors", "Total Errors");
            _dgvCollisionStats.Columns.Add("AvgErrors", "Avg Errors");
            _dgvCollisionStats.Columns.Add("FailureRate", "Failure Rate (%)");

            collisionPanel.Controls.Add(_dgvCollisionStats);
            tabCollisions.Controls.Add(collisionPanel);

            // Charts Tab
            var tabCharts = new System.Windows.Forms.TabPage("Performance Charts");
            var chartsPanel = new System.Windows.Forms.Panel();
            chartsPanel.Dock = DockStyle.Fill;
            chartsPanel.AutoScroll = true;

            int chartWidth = 500;
            int chartHeight = 280;

            _chartTime = CreateChart("Average Computation Time by Algorithm", chartWidth, chartHeight);
            _chartSuccess = CreateChart("Success Rate by Algorithm", chartWidth, chartHeight);
            _chartCollisions = CreateChart("Average Collisions by Algorithm", chartWidth, chartHeight);
            _chartSpeed = CreateChart("Average Speed by Algorithm", chartWidth, chartHeight);

            _chartTime.Location = new System.Drawing.Point(10, 10);
            _chartSuccess.Location = new System.Drawing.Point(chartWidth + 20, 10);
            _chartCollisions.Location = new System.Drawing.Point(10, chartHeight + 20);
            _chartSpeed.Location = new System.Drawing.Point(chartWidth + 20, chartHeight + 20);

            chartsPanel.Controls.Add(_chartTime);
            chartsPanel.Controls.Add(_chartSuccess);
            chartsPanel.Controls.Add(_chartCollisions);
            chartsPanel.Controls.Add(_chartSpeed);
            tabCharts.Controls.Add(chartsPanel);

            // Trends Tab
            var tabTrends = new System.Windows.Forms.TabPage("Performance Trends");
            var trendsPanel = new System.Windows.Forms.Panel();
            trendsPanel.Dock = DockStyle.Fill;
            trendsPanel.Padding = new System.Windows.Forms.Padding(10);

            var grpTrend = new System.Windows.Forms.GroupBox();
            grpTrend.Text = "Performance Trends by Iteration";
            grpTrend.Dock = DockStyle.Fill;
            grpTrend.Padding = new System.Windows.Forms.Padding(5);

            _chartTrend = CreateChart("Performance Trends by Iteration", 1000, 350);
            _chartTrend.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series("Time"));
            _chartTrend.Series.Add(new System.Windows.Forms.DataVisualization.Charting.Series("Length"));
            _chartTrend.Series["Time"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            _chartTrend.Series["Length"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            grpTrend.Controls.Add(_chartTrend);
            trendsPanel.Controls.Add(grpTrend);
            tabTrends.Controls.Add(trendsPanel);

            // Add tabs to TabControl
            _tabControl.TabPages.Add(tabSummary);
            _tabControl.TabPages.Add(tabAlgorithms);
            _tabControl.TabPages.Add(tabMetrics);
            _tabControl.TabPages.Add(tabCollisions);
            _tabControl.TabPages.Add(tabCharts);
            _tabControl.TabPages.Add(tabTrends);

            // Bottom Panel
            var bottomPanel = new System.Windows.Forms.Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 45;
            bottomPanel.Padding = new System.Windows.Forms.Padding(10);
            bottomPanel.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);

            _btnExportCharts = new System.Windows.Forms.Button();
            _btnExportCharts.Text = "Export Charts";
            _btnExportCharts.Location = new System.Drawing.Point(10, 8);
            _btnExportCharts.Size = new System.Drawing.Size(120, 28);
            _btnExportCharts.FlatStyle = FlatStyle.Flat;
            _btnExportCharts.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            _btnExportCharts.ForeColor = System.Drawing.Color.White;
            _btnExportCharts.Cursor = Cursors.Hand;

            _btnExportData = new System.Windows.Forms.Button();
            _btnExportData.Text = "Export Data";
            _btnExportData.Location = new System.Drawing.Point(140, 8);
            _btnExportData.Size = new System.Drawing.Size(120, 28);
            _btnExportData.FlatStyle = FlatStyle.Flat;
            _btnExportData.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            _btnExportData.ForeColor = System.Drawing.Color.White;
            _btnExportData.Cursor = Cursors.Hand;

            _btnClose = new System.Windows.Forms.Button();
            _btnClose.Text = "Close";
            _btnClose.Location = new System.Drawing.Point(270, 8);
            _btnClose.Size = new System.Drawing.Size(100, 28);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            _btnClose.ForeColor = System.Drawing.Color.White;
            _btnClose.Cursor = Cursors.Hand;

            bottomPanel.Controls.Add(_btnExportCharts);
            bottomPanel.Controls.Add(_btnExportData);
            bottomPanel.Controls.Add(_btnClose);

            // Add controls to form
            this.Controls.Add(_tabControl);
            this.Controls.Add(bottomPanel);
        }

        private System.Windows.Forms.DataGridView CreateDataGridView()
        {
            var dgv = new System.Windows.Forms.DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgv.BackgroundColor = System.Drawing.Color.White;
            dgv.RowHeadersVisible = false;
            return dgv;
        }

        private System.Windows.Forms.DataVisualization.Charting.Chart CreateChart(string title, int width, int height)
        {
            var chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chart.Size = new System.Drawing.Size(width, height);
            chart.BackColor = System.Drawing.Color.White;
            chart.ChartAreas.Add(new System.Windows.Forms.DataVisualization.Charting.ChartArea());
            chart.Legends.Add(new System.Windows.Forms.DataVisualization.Charting.Legend());
            chart.Titles.Add(title);
            return chart;
        }
        #endregion
    }
}