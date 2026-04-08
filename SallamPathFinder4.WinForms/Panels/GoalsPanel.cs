#region File Header
/// <summary>
/// File: GoalsPanel.cs
/// Description: Panel for managing goal points on the map
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Goals;
#endregion

namespace SallamPathFinder4.WinForms.Panels
{
    public sealed class GoalsPanel : Panel
    {
        #region Private Fields
        private readonly ObservableCollection<GoalPoint> _goals;
        private ListView _lstGoals;
        private Button _btnAdd;
        private Button _btnRemove;
        private Button _btnMove;
        private Label _lblTitle;
        private Label _lblCount;
        #endregion

        #region Events
        public event EventHandler<Point> GoalAddRequested;
        public event EventHandler<GoalPoint> GoalRemoveRequested;
        public event EventHandler<GoalPoint> GoalMoveRequested;
        #endregion

        #region Constructor
        public GoalsPanel(ObservableCollection<GoalPoint> goals)
        {
            _goals = goals;
            InitializeComponents();
            BindData();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(5);

            _lblTitle = new Label
            {
                Text = "🎯 GOALS",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(5, 5),
                AutoSize = true
            };

            _lblCount = new Label
            {
                Text = "Count: 0",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(5, 28),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            _lstGoals = new ListView
            {
                Location = new Point(5, 50),
                Size = new Size(280, 180),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable
            };
            _lstGoals.Columns.Add("Name", 60);
            _lstGoals.Columns.Add("Location", 100);
            _lstGoals.Columns.Add("Color", 80);

            _btnAdd = new Button
            {
                Text = "➕ Add Goal",
                Location = new Point(5, 240),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnAdd.Click += BtnAdd_Click;

            _btnRemove = new Button
            {
                Text = "✖ Remove",
                Location = new Point(100, 240),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnRemove.Click += BtnRemove_Click;

            _btnMove = new Button
            {
                Text = "↻ Move",
                Location = new Point(195, 240),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnMove.Click += BtnMove_Click;

            this.Controls.Add(_lblTitle);
            this.Controls.Add(_lblCount);
            this.Controls.Add(_lstGoals);
            this.Controls.Add(_btnAdd);
            this.Controls.Add(_btnRemove);
            this.Controls.Add(_btnMove);
        }

        private void BindData()
        {
            if (_goals == null) return;

            _goals.CollectionChanged += OnCollectionChanged;
            RefreshList();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshList();
        }
        #endregion

        #region Public Methods
        public void RefreshList()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(RefreshList));
                return;
            }

            _lstGoals.Items.Clear();

            if (_goals == null) return;

            foreach (var goal in _goals)
            {
                var item = new ListViewItem(goal.Name);
                item.SubItems.Add($"({goal.Location.X},{goal.Location.Y})");
                item.SubItems.Add("");
                item.BackColor = Color.FromArgb(50, goal.Color);
                _lstGoals.Items.Add(item);
            }

            _lblCount.Text = $"Count: {_goals.Count}";
        }
        #endregion

        #region Event Handlers
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            GoalAddRequested?.Invoke(this, new Point(10, 10));
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (_lstGoals.SelectedItems.Count > 0)
            {
                int index = _lstGoals.SelectedItems[0].Index;
                if (_goals != null && index < _goals.Count)
                {
                    GoalRemoveRequested?.Invoke(this, _goals[index]);
                }
            }
        }

        private void BtnMove_Click(object sender, EventArgs e)
        {
            if (_lstGoals.SelectedItems.Count > 0)
            {
                int index = _lstGoals.SelectedItems[0].Index;
                if (_goals != null && index < _goals.Count)
                {
                    GoalMoveRequested?.Invoke(this, _goals[index]);
                }
            }
        }
        #endregion
    }
}