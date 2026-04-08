#region File Header
/// <summary>
/// File: DoorGroupManager.cs
/// Description: Manages door groups with random opening/closing behavior
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Enums;
using SallamPathFinder4.Core.Models.Map;
#endregion

namespace SallamPathFinder4.Services.Simulation
{
    public sealed class DoorGroupManager : IDisposable
    {
        #region Constants
        private const double MIN_UPDATE_INTERVAL_SECONDS = 2.0;
        private const double MAX_UPDATE_INTERVAL_SECONDS = 5.0;
        #endregion

        #region Private Fields
        private readonly MapGrid _mapGrid;
        private readonly List<DoorGroup> _doorGroups;
        private readonly Random _random;
        private readonly CancellationTokenSource _cts;
        private bool _isRunning;
        private bool _isDisposed;
        #endregion

        #region Events
        public event Action<DoorGroup, bool> DoorStateChanged;
        #endregion

        #region Constructor
        public DoorGroupManager(MapGrid grid)
        {
            _mapGrid = grid ?? throw new ArgumentNullException(nameof(grid));
            _doorGroups = new List<DoorGroup>();
            _random = new Random();
            _cts = new CancellationTokenSource();
            _isRunning = false;
        }
        #endregion

        #region Properties
        public IReadOnlyList<DoorGroup> DoorGroups => _doorGroups.AsReadOnly();
        public bool IsRunning => _isRunning;
        #endregion

        #region Public Methods
        public void FindDoorGroups()
        {
            _doorGroups.Clear();
            var visited = new HashSet<(int, int)>();

            for (int x = 0; x < _mapGrid.Width; x++)
            {
                for (int y = 0; y < _mapGrid.Height; y++)
                {
                    var cell = _mapGrid[x, y];
                    if (cell.ElementType == MapElementType.Door && !visited.Contains((x, y)))
                    {
                        var groupCells = GetAdjacentDoorCells(x, y, visited);
                        if (groupCells.Count > 0)
                        {
                            var doorGroup = new DoorGroup(groupCells);
                            doorGroup.Id = _doorGroups.Count + 1;
                            _doorGroups.Add(doorGroup);
                            System.Diagnostics.Debug.WriteLine($"DoorGroup {doorGroup.Id}: {groupCells.Count} cells");
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Found {_doorGroups.Count} door groups");
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;
            System.Diagnostics.Debug.WriteLine($"DoorGroupManager Start() called, DoorGroups count: {_doorGroups.Count}");
            Task.Run(() => RunAsync(_cts.Token));
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel();
        }

        public bool IsClosedDoor(Point cell)
        {
            var gridCell = _mapGrid[cell.X, cell.Y];
            if (gridCell.ElementType != MapElementType.Door)
                return false;
            return !gridCell.IsDoorOpen;
        }

        public DoorGroup GetDoorGroupAt(Point cell)
        {
            return _doorGroups.FirstOrDefault(g => g.Cells.Any(c => c.X == cell.X && c.Y == cell.Y));
        }
        #endregion

        #region Private Methods
        private List<Point> GetAdjacentDoorCells(int startX, int startY, HashSet<(int, int)> visited)
        {
            var cells = new List<Point>();
            var queue = new Queue<Point>();
            queue.Enqueue(new Point(startX, startY));
            visited.Add((startX, startY));

            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                cells.Add(current);

                for (int i = 0; i < 4; i++)
                {
                    int nx = current.X + dx[i];
                    int ny = current.Y + dy[i];

                    if (!_mapGrid.IsValidCoordinate(nx, ny))
                        continue;

                    if (visited.Contains((nx, ny)))
                        continue;

                    var neighbor = _mapGrid[nx, ny];
                    if (neighbor.ElementType == MapElementType.Door)
                    {
                        visited.Add((nx, ny));
                        queue.Enqueue(new Point(nx, ny));
                    }
                }
            }

            return cells;
        }

        private async Task RunAsync(CancellationToken token)
        {
            System.Diagnostics.Debug.WriteLine("DoorGroupManager RunAsync STARTED");
            System.Diagnostics.Debug.WriteLine($"DoorGroups count: {_doorGroups.Count}");

            foreach (var group in _doorGroups)
            {
                group.UpdateIntervalSeconds = GetRandomInterval();
                System.Diagnostics.Debug.WriteLine($"DoorGroup {group.Id}: Initial interval = {group.UpdateIntervalSeconds:F2}s, Cells={group.Cells.Count}");
            }

            while (_isRunning && !token.IsCancellationRequested)
            {
                System.Diagnostics.Debug.WriteLine($"RunAsync loop iteration, _isRunning={_isRunning}");

                var groupsToUpdate = _doorGroups.Where(g => g.UpdateIntervalSeconds <= 0).ToList();
                System.Diagnostics.Debug.WriteLine($"Groups to update: {groupsToUpdate.Count}");

                foreach (var group in groupsToUpdate)
                {
                    System.Diagnostics.Debug.WriteLine($"DoorGroup {group.Id}: Interval <= 0, toggling...");
                    await ToggleDoorGroup(group);
                    group.UpdateIntervalSeconds = GetRandomInterval();
                    System.Diagnostics.Debug.WriteLine($"DoorGroup {group.Id}: New interval = {group.UpdateIntervalSeconds:F2}s");
                }

                foreach (var group in _doorGroups)
                {
                    if (group.UpdateIntervalSeconds > 0)
                    {
                        group.UpdateIntervalSeconds -= 0.1;
                    }
                }

                await Task.Delay(100, token);
            }

            System.Diagnostics.Debug.WriteLine("DoorGroupManager RunAsync ENDED");
        }
        private async Task ToggleDoorGroup(DoorGroup group)
        {
            if (!_isRunning) return;

            bool newState = !group.IsOpen;
            group.IsOpen = newState;

            System.Diagnostics.Debug.WriteLine($"DoorGroup {group.Id} toggling to {(newState ? "OPEN" : "CLOSED")}");

            foreach (var cell in group.Cells)
            {
                var gridCell = _mapGrid[cell.X, cell.Y];
                gridCell.IsDoorOpen = newState;
                _mapGrid.UpdateCellProperties(cell.X, cell.Y);
            }

            DoorStateChanged?.Invoke(group, newState);
            await Task.CompletedTask;
        }

        private double GetRandomInterval()
        {
            return MIN_UPDATE_INTERVAL_SECONDS + (_random.NextDouble() * (MAX_UPDATE_INTERVAL_SECONDS - MIN_UPDATE_INTERVAL_SECONDS));
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                Stop();
                _cts?.Dispose();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    #region DoorGroup Class
    public sealed class DoorGroup
    {
        public DoorGroup(List<Point> cells)
        {
            Cells = cells ?? new List<Point>();
            IsOpen = true;
            UpdateIntervalSeconds = 0;
        }

        public List<Point> Cells { get; set; }
        public bool IsOpen { get; set; }
        public double UpdateIntervalSeconds { get; set; }
        public int Id { get; set; }

        public override string ToString()
        {
            return $"DoorGroup {Id}: {Cells.Count} cells, {(IsOpen ? "Open" : "Closed")}";
        }
    }
    #endregion
}