#region File Header
/// <summary>
/// File: PathDisplayPanel.cs
/// Description: Panel for displaying path details and statistics
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Imports NameSpace
using SallamPathFinder4.Core.Models.Path;
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
            #region Private Fields - Path Groups
            private List<PathGroup> _pathGroups;
            private int _currentGroupIndex;
            #endregion
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

        #region Public Methods - Path Groups

        /// <summary>
        /// Clears all path groups and display
        /// </summary>
        public void ClearPathGroups()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ClearPathGroups));
                return;
            }

            _pathGroups?.Clear();
            _lvPathSteps.Items.Clear();
            _currentGroupIndex = 0;

            _lblTotalCells.Text = "Total Cells: 0";
            _lblTotalTime.Text = "Computation Time: 0 ms";
            _lblTotalLength.Text = "Path Length: 0 cm";
        }

        /// <summary>
        /// Adds a path group to the display
        /// </summary>
        /// <param name="groupName">Name of the group (e.g., "🎯 Goal 1", "🔋 Charging Path")</param>
        /// <param name="groupColor">Color of the group (matches goal color)</param>
        /// <param name="nodes">List of path nodes in this group</param>
        public void AddPathGroup(string groupName, Color groupColor, List<PathNode> nodes)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddPathGroup(groupName, groupColor, nodes)));
                return;
            }

            if (_pathGroups == null)
            {
                _pathGroups = new List<PathGroup>();
            }

            var group = new PathGroup
            {
                Name = groupName,
                Color = groupColor,
                Nodes = nodes,
                StartStep = _currentGroupIndex + 1,
                EndStep = _currentGroupIndex + nodes.Count
            };

            _pathGroups.Add(group);

            // Add group header
            var headerItem = new ListViewItem(groupName);
            headerItem.BackColor = Color.FromArgb(50, groupColor);
            headerItem.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            headerItem.ForeColor = Color.FromArgb(52, 73, 94);
            _lvPathSteps.Items.Add(headerItem);

            // Add nodes
            foreach (var node in nodes)
            {
                _currentGroupIndex++;
                var item = new ListViewItem(_currentGroupIndex.ToString());
                item.SubItems.Add(node.X.ToString());
                item.SubItems.Add(node.Y.ToString());
                item.SubItems.Add(groupName);
                item.BackColor = Color.FromArgb(30, groupColor);
                _lvPathSteps.Items.Add(item);
            }

            // Auto-scroll to last item
            if (_lvPathSteps.Items.Count > 0)
            {
                _lvPathSteps.EnsureVisible(_lvPathSteps.Items.Count - 1);
            }
        }

        /// <summary>
        /// Displays a complete path with multiple colored segments
        /// </summary>
        /// <param name="coloredPaths">List of colored paths to display</param>
        /// <param name="scaleCmPerCell">Scale for real length calculation</param>
        public void DisplayColoredPaths(List<ColoredPath> coloredPaths, double scaleCmPerCell)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => DisplayColoredPaths(coloredPaths, scaleCmPerCell)));
                return;
            }

            ClearPathGroups();

            if (coloredPaths == null || coloredPaths.Count == 0) return;

            int totalCells = 0;
            int groupNumber = 1;

            foreach (var coloredPath in coloredPaths)
            {
                if (coloredPath == null || coloredPath.Nodes == null || coloredPath.Nodes.Count == 0) continue;

                string groupName = GetGroupName(coloredPath.Type, groupNumber);
                AddPathGroup(groupName, coloredPath.Color, coloredPath.Nodes.ToList());

                totalCells += coloredPath.Nodes.Count;

                if (coloredPath.Type != PathType.Return)
                {
                    groupNumber++;
                }
            }

            // Update statistics (time and length need to be set separately)
            _lblTotalCells.Text = $"Total Cells: {totalCells}";
            _lblTotalLength.Text = $"Path Length: {totalCells * scaleCmPerCell:F1} cm";
        }

        /// <summary>
        /// Gets the display name for a path type
        /// </summary>
        private string GetGroupName(PathType type, int goalNumber)
        {
            return type switch
            {
                PathType.Normal => $"🎯 Goal {goalNumber}",
                PathType.Return => "🏁 Return to Parking",
                PathType.Charging => "🔋 Charging Path",
                _ => $"Segment {goalNumber}"
            };
        }

        /// <summary>
        /// Updates statistics with computation time
        /// </summary>
        public void UpdateStatsWithGroups(int totalCells, double timeMs, double lengthCm)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatsWithGroups(totalCells, timeMs, lengthCm)));
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
   
    /// <summary>
    /// Represents a group of path nodes belonging to a specific goal or segment
    /// </summary>
    public class PathGroup
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public List<PathNode> Nodes { get; set; }
        public int StartStep { get; set; }
        public int EndStep { get; set; }

        public override string ToString()
        {
            return $"{Name}: {Nodes.Count} cells";
        }
    }  
} 