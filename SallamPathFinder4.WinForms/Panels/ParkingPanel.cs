#region File Header
/// <summary>
/// File: ParkingPanel.cs
/// Description: Panel for managing parking points on the map
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
    public sealed class ParkingPanel : Panel
    {
        #region Private Fields
        private readonly ObservableCollection<ParkingPoint> _parkingPoints;
        private ListBox _lstParking;
        private Button _btnAdd;
        private Button _btnRemove;
        private Button _btnMove;
        private Label _lblTitle;
        private Label _lblCount;
        #endregion

        #region Events
        public event EventHandler<Point> ParkingAddRequested;
        public event EventHandler<ParkingPoint> ParkingRemoveRequested;
        public event EventHandler<ParkingPoint> ParkingMoveRequested;
        #endregion

        #region Constructor
        public ParkingPanel(ObservableCollection<ParkingPoint> parkingPoints)
        {
            _parkingPoints = parkingPoints;
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
                Text = "🅿️ PARKING POINTS",
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

            _lstParking = new ListBox
            {
                Location = new Point(5, 50),
                Size = new Size(280, 150),
                Font = new Font("Consolas", 9),
                IntegralHeight = false
            };

            _btnAdd = new Button
            {
                Text = "➕ Add Parking",
                Location = new Point(5, 210),
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
                Location = new Point(100, 210),
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
                Location = new Point(195, 210),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnMove.Click += BtnMove_Click;

            this.Controls.Add(_lblTitle);
            this.Controls.Add(_lblCount);
            this.Controls.Add(_lstParking);
            this.Controls.Add(_btnAdd);
            this.Controls.Add(_btnRemove);
            this.Controls.Add(_btnMove);
        }

        private void BindData()
        {
            if (_parkingPoints == null) return;

            _parkingPoints.CollectionChanged += OnCollectionChanged;
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

            if (_parkingPoints == null) return;

            _lstParking.DataSource = null;
            _lstParking.DataSource = _parkingPoints;
            _lstParking.DisplayMember = "ToString";
            _lblCount.Text = $"Count: {_parkingPoints.Count}";
        }
        #endregion

        #region Event Handlers
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ParkingAddRequested?.Invoke(this, new Point(5, 5));
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (_lstParking.SelectedItem is ParkingPoint selected)
            {
                ParkingRemoveRequested?.Invoke(this, selected);
            }
        }

        private void BtnMove_Click(object sender, EventArgs e)
        {
            if (_lstParking.SelectedItem is ParkingPoint selected)
            {
                ParkingMoveRequested?.Invoke(this, selected);
            }
        }
        #endregion
    }
}