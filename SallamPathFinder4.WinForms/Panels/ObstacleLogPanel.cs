#region File Header
/// <summary>
/// File: ObstacleLogPanel.cs
/// Description: Panel for displaying collision and obstacle detection logs
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Experiments;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#endregion

namespace SallamPathFinder4.WinForms.Panels
{
    public sealed class ObstacleLogPanel : Panel
    {
        #region Private Fields
        private readonly ObservableCollection<CollisionRecord> _obstacleLog;
        private ListBox _lstLog;
        private Button _btnClear;
        private Label _lblTitle;
        private Label _lblCount;
        #endregion

        #region Constructor
        public ObstacleLogPanel(ObservableCollection<CollisionRecord> obstacleLog)
        {
            _obstacleLog = obstacleLog;
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
                Text = "⚠️ OBSTACLE LOG",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(5, 5),
                AutoSize = true
            };

            _lblCount = new Label
            {
                Text = "Total: 0 collisions",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(5, 28),
                AutoSize = true,
                ForeColor = Color.FromArgb(231, 76, 60)
            };

            _lstLog = new ListBox
            {
                Location = new Point(5, 50),
                Size = new Size(310, 320),
                Font = new Font("Consolas", 9),
                IntegralHeight = false,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            _btnClear = new Button
            {
                Text = "🗑 Clear Log",
                Location = new Point(5, 380),
                Size = new Size(310, 30),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnClear.Click += BtnClear_Click;

            this.Controls.Add(_lblTitle);
            this.Controls.Add(_lblCount);
            this.Controls.Add(_lstLog);
            this.Controls.Add(_btnClear);
        }

        private void BindData()
        {
            if (_obstacleLog == null) return;

            _obstacleLog.CollectionChanged += OnCollectionChanged;
            RefreshListBox();
            UpdateCount();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    RefreshListBox();
                    UpdateCount();
                }));
            }
            else
            {
                RefreshListBox();
                UpdateCount();
            }
        }
        #endregion

        #region Private Methods
        private void RefreshListBox()
        {
            if (_lstLog == null || IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new Action(RefreshListBox));
                return;
            }

            try
            {
                _lstLog.Items.Clear();

                if (_obstacleLog == null || _obstacleLog.Count == 0) return;

                for (int i = _obstacleLog.Count - 1; i >= 0; i--)
                {
                    var record = _obstacleLog[i];
                    if (record != null)
                    {
                        _lstLog.Items.Add(GetDisplayText(record));
                    }
                }

                if (_lstLog.Items.Count > 0)
                    _lstLog.TopIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing obstacle log: {ex.Message}");
            }
        }

        private void UpdateCount()
        {
            if (_lblCount == null || IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new Action(UpdateCount));
                return;
            }

            int count = _obstacleLog?.Count ?? 0;
            _lblCount.Text = $"Total: {count} collision{(count != 1 ? "s" : "")}";
            _lblCount.ForeColor = count > 0 ? Color.FromArgb(231, 76, 60) : Color.Gray;
        }

        private static string GetDisplayText(CollisionRecord record)
        {
            if (record == null) return "Unknown collision";

            string icon = record.ObstacleType switch
            {
                ObstacleType.Adult => "👤",
                ObstacleType.Child => "🧒",
                ObstacleType.Animal => "🐕",
                ObstacleType.OtherRobot => "🤖",
                ObstacleType.Equipment => "🔧",
                _ => "⚠️"
            };

            return $"[{record.Timestamp:HH:mm:ss}] {icon} {record.ObstacleType} at ({record.Location.X},{record.Location.Y})";
        }
        #endregion

        #region Event Handlers
        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (_obstacleLog != null)
            {
                _obstacleLog.Clear();
                RefreshListBox();
                UpdateCount();
            }
        }
        #endregion
    }
}