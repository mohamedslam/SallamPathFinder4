#region File Header
/// <summary>
/// File: MapGrid.cs
/// Description: Manages the 2D grid of cells for the map
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.Core.Models.Map
{
    #region Class Documentation
    /// <summary>
    /// Manages the grid-based map with cells, dimensions, and navigation properties
    /// Provides methods for cell access, validation, resizing, and bulk updates
    /// </summary>
    #endregion
    public sealed class MapGrid
    {
        #region Constants
        private const int MIN_GRID_SIZE = 10;
        private const int MAX_GRID_SIZE = 500;
        private const int DEFAULT_GRID_SIZE = 100;
        #endregion

        #region Private Fields
        private Cell[,] _cells;
        private int _width;
        private int _height;
        private readonly object _lockObject = new object();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new map grid with specified dimensions
        /// </summary>
        /// <param name="width">Number of columns (X dimension)</param>
        /// <param name="height">Number of rows (Y dimension)</param>
        /// <exception cref="ArgumentException">Thrown when width or height is invalid</exception>
        public MapGrid(int width, int height)
        {
            #region Validation
            if (width < MIN_GRID_SIZE || width > MAX_GRID_SIZE)
                throw new ArgumentException($"Width must be between {MIN_GRID_SIZE} and {MAX_GRID_SIZE}", nameof(width));

            if (height < MIN_GRID_SIZE || height > MAX_GRID_SIZE)
                throw new ArgumentException($"Height must be between {MIN_GRID_SIZE} and {MAX_GRID_SIZE}", nameof(height));
            #endregion

            _width = width;
            _height = height;
            _cells = new Cell[width, height];

            InitializeCells();
        }
        #endregion

        #region Public Properties
        /// <summary>Width of the grid (number of columns)</summary>
        public int Width => _width;

        /// <summary>Height of the grid (number of rows)</summary>
        public int Height => _height;

        /// <summary>
        /// Indexer to access or modify a cell by its coordinates
        /// </summary>
        /// <param name="x">X coordinate (column)</param>
        /// <param name="y">Y coordinate (row)</param>
        /// <returns>The cell at the specified coordinates</returns>
        public Cell this[int x, int y]
        {
            get
            {
                ValidateCoordinate(x, y);
                return _cells[x, y];
            }
            set
            {
                ValidateCoordinate(x, y);
                lock (_lockObject)
                {
                    _cells[x, y] = value ?? throw new ArgumentNullException(nameof(value));
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes all cells in the grid with default values
        /// </summary>
        private void InitializeCells()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _cells[x, y] = new Cell(x, y);
                }
            }
        }

        /// <summary>
        /// Validates that coordinates are within grid bounds
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are invalid</exception>
        private void ValidateCoordinate(int x, int y)
        {
            if (!IsValidCoordinate(x, y))
                throw new ArgumentOutOfRangeException($"Invalid coordinates: ({x}, {y}). Grid size: {_width}x{_height}");
        }
        #endregion

        #region Public Methods - Validation
        /// <summary>
        /// Checks if the specified coordinates are within grid bounds
        /// </summary>
        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }
        #endregion

        #region Public Methods - Cell Updates
        /// <summary>
        /// Updates walkability and cost for a single cell
        /// </summary>
        public void UpdateCellProperties(int x, int y)
        {
            if (!IsValidCoordinate(x, y)) return;

            lock (_lockObject)
            {
                var cell = _cells[x, y];
                cell.ElementType = cell.ElementType; // Triggers internal update
            }
        }

        /// <summary>
        /// Updates walkability and cost for all cells in the grid
        /// Call this after bulk operations or map loading
        /// </summary>
        public void UpdateAllCellProperties()
        {
            lock (_lockObject)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        var cell = _cells[x, y];
                        cell.ElementType = cell.ElementType; // Triggers internal update
                    }
                }
            }
        }

        /// <summary>
        /// Resets all cells to default empty state
        /// </summary>
        public void Reset()
        {
            lock (_lockObject)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        _cells[x, y].Reset();
                    }
                }
            }
        }
        #endregion

        #region Public Methods - Grid Manipulation
        /// <summary>
        /// Resizes the grid to new dimensions
        /// Preserves existing cell data where possible
        /// </summary>
        public void Resize(int newWidth, int newHeight)
        {
            #region Validation
            if (newWidth == _width && newHeight == _height) return;

            if (newWidth < MIN_GRID_SIZE || newWidth > MAX_GRID_SIZE)
                throw new ArgumentException($"New width must be between {MIN_GRID_SIZE} and {MAX_GRID_SIZE}", nameof(newWidth));

            if (newHeight < MIN_GRID_SIZE || newHeight > MAX_GRID_SIZE)
                throw new ArgumentException($"New height must be between {MIN_GRID_SIZE} and {MAX_GRID_SIZE}", nameof(newHeight));
            #endregion

            lock (_lockObject)
            {
                var newCells = new Cell[newWidth, newHeight];

                // Initialize new cells
                for (int x = 0; x < newWidth; x++)
                {
                    for (int y = 0; y < newHeight; y++)
                    {
                        if (x < _width && y < _height && _cells[x, y] != null)
                        {
                            // Preserve existing cell data
                            newCells[x, y] = _cells[x, y];
                        }
                        else
                        {
                            // Create new cell
                            newCells[x, y] = new Cell(x, y);
                        }
                    }
                }

                _cells = newCells;
                _width = newWidth;
                _height = newHeight;
            }
        }

        /// <summary>
        /// Creates a deep copy of the entire grid
        /// </summary>
        public MapGrid Clone()
        {
            lock (_lockObject)
            {
                var clone = new MapGrid(_width, _height);

                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        clone._cells[x, y] = _cells[x, y].Clone();
                    }
                }

                return clone;
            }
        }
        #endregion

        #region Public Methods - Statistics
        /// <summary>
        /// Gets the number of walkable cells in the grid
        /// </summary>
        public int GetWalkableCellCount()
        {
            int count = 0;
            lock (_lockObject)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        if (_cells[x, y].IsWalkable) count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Gets the number of cells with a specific element type
        /// </summary>
        public int GetElementCount(MapElementType elementType)
        {
            int count = 0;
            lock (_lockObject)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        if (_cells[x, y].ElementType == elementType) count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Gets the average surface weight of all cells
        /// </summary>
        public double GetAverageSurfaceWeight()
        {
            double total = 0;
            int totalCells = _width * _height;

            lock (_lockObject)
            {
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        total += _cells[x, y].SurfaceWeight;
                    }
                }
            }

            return total / totalCells;
        }
        #endregion

        #region Public Methods - Door Groups
        /// <summary>
        /// Finds all door groups in the grid
        /// </summary>
        public List<List<Point>> FindDoorGroups()
        {
            var groups = new List<List<Point>>();
            var visited = new HashSet<(int, int)>();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_cells[x, y].ElementType == MapElementType.Door && !visited.Contains((x, y)))
                    {
                        var group = GetAdjacentDoorCells(x, y, visited);
                        if (group.Count > 0)
                        {
                            groups.Add(group);
                        }
                    }
                }
            }

            return groups;
        }

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

                    if (!IsValidCoordinate(nx, ny))
                        continue;

                    if (visited.Contains((nx, ny)))
                        continue;

                    if (_cells[nx, ny].ElementType == MapElementType.Door)
                    {
                        visited.Add((nx, ny));
                        queue.Enqueue(new Point(nx, ny));
                    }
                }
            }

            return cells;
        }
        #endregion
    }
}