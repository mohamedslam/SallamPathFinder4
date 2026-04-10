#region File Header
/// <summary>
/// File: PathDisplayPanel.cs
/// Description: Panel for displaying path details and statistics
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.WinForms.Panels
{
    public sealed class PathDisplayPanel : Panel
    {
        #region Private Fields
        private ListView _lvPathSteps;
        private Label _lblTotalCells;
        private Label _lblTotalTime;
        private Label _lblTotalLength;
        private Label _lblTitle;
        private Button _btnSave;
        #endregion

        #region Constants
        private const int LISTVIEW_HEIGHT = 280;
        #endregion

        #region Constructor
        public PathDisplayPanel()
        {
            InitializeComponents();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(5);

            _lblTitle = new Label
            {
                Text = "📊 PATH RESULTS",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(5, 5),
                AutoSize = true
            };
            this.Controls.Add(_lblTitle);

            var statsPanel = new Panel { Location = new Point(5, 30), Size = new Size(280, 60) };

            _lblTotalCells = new Label { Text = "Total Cells: 0", Location = new Point(0, 0), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            _lblTotalTime = new Label { Text = "Computation Time: 0 ms", Location = new Point(0, 20), AutoSize = true };
            _lblTotalLength = new Label { Text = "Path Length: 0 cm", Location = new Point(0, 40), AutoSize = true };

            statsPanel.Controls.Add(_lblTotalCells);
            statsPanel.Controls.Add(_lblTotalTime);
            statsPanel.Controls.Add(_lblTotalLength);
            this.Controls.Add(statsPanel);

            _lvPathSteps = new ListView
            {
                Location = new Point(5, 95),
                Size = new Size(280, LISTVIEW_HEIGHT),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            _lvPathSteps.Columns.Add("Step", 50);
            _lvPathSteps.Columns.Add("X", 50);
            _lvPathSteps.Columns.Add("Y", 50);
            _lvPathSteps.Columns.Add("Segment", 100);
            this.Controls.Add(_lvPathSteps);

            _btnSave = new Button
            {
                Text = "💾 Save Path",
                Location = new Point(5, 385),
                Size = new Size(280, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnSave.Click += BtnSave_Click;
            this.Controls.Add(_btnSave);
        }
        #endregion

        #region Public Methods
        public void ClearPath()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ClearPath));
                return;
            }

            _lvPathSteps.Items.Clear();
            _lblTotalCells.Text = "Total Cells: 0";
            _lblTotalTime.Text = "Computation Time: 0 ms";
            _lblTotalLength.Text = "Path Length: 0 cm";
        }

        public void AddPathStep(int stepNumber, int x, int y, string segmentName, Color segmentColor)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddPathStep(stepNumber, x, y, segmentName, segmentColor)));
                return;
            }

            var item = new ListViewItem(stepNumber.ToString());
            item.SubItems.Add(x.ToString());
            item.SubItems.Add(y.ToString());
            item.SubItems.Add(segmentName);
            item.BackColor = Color.FromArgb(50, segmentColor);
            _lvPathSteps.Items.Add(item);
            _lvPathSteps.EnsureVisible(_lvPathSteps.Items.Count - 1);
        }

        public void UpdateStats(int totalCells, double timeMs, double lengthCm)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStats(totalCells, timeMs, lengthCm)));
                return;
            }

            _lblTotalCells.Text = $"Total Cells: {totalCells}";
            _lblTotalTime.Text = $"Computation Time: {timeMs:F2} ms";
            _lblTotalLength.Text = $"Path Length: {lengthCm:F1} cm";
        }
        #endregion

        #region Event Handlers
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_lvPathSteps.Items.Count == 0)
            {
                MessageBox.Show("No path to save. Run pathfinding first.", "Save Path", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv)|*.csv";
            sfd.DefaultExt = "csv";
            sfd.FileName = $"Path_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using var writer = new StreamWriter(sfd.FileName);
                writer.WriteLine("Step,X,Y,Segment");
                foreach (ListViewItem item in _lvPathSteps.Items)
                {
                    writer.WriteLine($"{item.Text},{item.SubItems[1].Text},{item.SubItems[2].Text},{item.SubItems[3].Text}");
                }
                writer.WriteLine();
                writer.WriteLine($"Total Cells,{_lblTotalCells.Text.Replace("Total Cells: ", "")}");
                writer.WriteLine($"Computation Time,{_lblTotalTime.Text.Replace("Computation Time: ", "")}");
                writer.WriteLine($"Path Length,{_lblTotalLength.Text.Replace("Path Length: ", "")}");
                MessageBox.Show($"Path saved to {sfd.FileName}", "Save Path", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion
    }
}