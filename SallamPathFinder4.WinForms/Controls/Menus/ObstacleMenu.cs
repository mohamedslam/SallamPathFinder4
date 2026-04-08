#region File Header
/// <summary>
/// File: ObstacleMenu.cs
/// Description: Menu for obstacles - organized into surface weights, static, semi-static, and dynamic categories
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Windows.Forms;
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.WinForms.Controls.Menus
{
    public sealed class ObstacleMenu : ToolStripDropDownButton
    {
        #region Private Fields
        private readonly MapControl _mapControl;
        #endregion

        #region Events
        public event EventHandler<ObstacleType> ObstacleTypeSelected;
        #endregion

        #region Constructor
        public ObstacleMenu(MapControl mapControl)
        {
            _mapControl = mapControl;
            this.Text = "🚧 Obstacles";
            this.DisplayStyle = ToolStripItemDisplayStyle.Text;
            BuildMenu();
        }
        #endregion

        #region Menu Building
        private void BuildMenu()
        {
            CreateSurfaceWeightsMenu();
            CreateSeparator();
            CreateStaticObstaclesMenu();
            CreateSeparator();
            CreateSemiStaticObstaclesMenu();
            CreateSeparator();
            CreateDynamicObstaclesMenu();
            CreateSeparator();
            CreateClearAllItem();
        }

        private void CreateSurfaceWeightsMenu()
        {
            var weightMenu = new ToolStripMenuItem("📊 Surface Weights");

            // Add weight options from 0% to 100% in steps of 5
            for (int weight = 0; weight <= 100; weight += 5)
            {
                int w = weight;
                var item = new ToolStripMenuItem($"{weight}%");

                // Create colored square to represent the weight
                int intensity = 255 - (int)((weight / 100.0) * 255);
                item.BackColor = Color.FromArgb(intensity, intensity, intensity);

                item.Click += (s, e) =>
                {
                    if (_mapControl != null)
                    {
                        _mapControl.CurrentDrawMode = MapControl.DrawMode.SetWeight;
                        _mapControl.CurrentWeight = (byte)w;
                        System.Diagnostics.Debug.WriteLine($"Surface weight set to {w}%");
                    }
                };

                weightMenu.DropDownItems.Add(item);
            }

            this.DropDownItems.Add(weightMenu);
        }

        private void CreateStaticObstaclesMenu()
        {
            var staticMenu = new ToolStripMenuItem("🧱 Static Obstacles");
            AddStaticItem(staticMenu, "🧱 Wall", MapElementType.Wall);
            AddStaticItem(staticMenu, "📐 Ramp", MapElementType.Ramp);
            this.DropDownItems.Add(staticMenu);
        }

        private void CreateSemiStaticObstaclesMenu()
        {
            var semiStaticMenu = new ToolStripMenuItem("🪟 Semi-Static Obstacles");
            AddStaticItem(semiStaticMenu, "🚪 Door", MapElementType.Door);
            AddStaticItem(semiStaticMenu, "🪟 Window", MapElementType.Window);
            this.DropDownItems.Add(semiStaticMenu);
        }

        private void CreateDynamicObstaclesMenu()
        {
            var dynamicMenu = new ToolStripMenuItem("👤 Dynamic Obstacles");
            AddDynamicItem(dynamicMenu, "👤 Adult", ObstacleType.Adult);
            AddDynamicItem(dynamicMenu, "🧒 Child", ObstacleType.Child);
            AddDynamicItem(dynamicMenu, "🐕 Animal", ObstacleType.Animal);
            AddDynamicItem(dynamicMenu, "🤖 Other Robot", ObstacleType.OtherRobot);
            AddDynamicItem(dynamicMenu, "🔧 Equipment", ObstacleType.Equipment);
            this.DropDownItems.Add(dynamicMenu);
        }

        private void CreateClearAllItem()
        {
            var clearItem = new ToolStripMenuItem("🗑 Clear All Obstacles");
            clearItem.Click += (s, e) => ClearAllObstacles();
            this.DropDownItems.Add(clearItem);
        }

        private void CreateSeparator()
        {
            this.DropDownItems.Add(new ToolStripSeparator());
        }
        #endregion

        #region Helper Methods
        private void AddStaticItem(ToolStripMenuItem parent, string text, MapElementType element)
        {
            var item = new ToolStripMenuItem(text);
            item.Click += (s, e) =>
            {
                if (_mapControl != null)
                {
                    _mapControl.CurrentDrawMode = MapControl.DrawMode.SetElement;
                    _mapControl.CurrentElement = element;
                    System.Diagnostics.Debug.WriteLine($"Static element selected: {element}");
                }
            };
            parent.DropDownItems.Add(item);
        }

        private void AddDynamicItem(ToolStripMenuItem parent, string text, ObstacleType type)
        {
            var item = new ToolStripMenuItem(text);
            item.Click += (s, e) =>
            {
                if (_mapControl != null)
                {
                    _mapControl.CurrentDrawMode = MapControl.DrawMode.SetDynamicObstacle;
                    _mapControl.CurrentObstacleType = type;
                    ObstacleTypeSelected?.Invoke(this, type);
                    System.Diagnostics.Debug.WriteLine($"Dynamic obstacle selected: {type}");
                }
            };
            parent.DropDownItems.Add(item);
        }

        private void ClearAllObstacles()
        {
            if (_mapControl == null) return;

            // Clear dynamic obstacles
            _mapControl.DynamicObstacles.Clear();

            // Clear static and semi-static obstacles from grid
            if (_mapControl.MapGrid != null)
            {
                for (int x = 0; x < _mapControl.MapGrid.Width; x++)
                {
                    for (int y = 0; y < _mapControl.MapGrid.Height; y++)
                    {
                        var cell = _mapControl.MapGrid[x, y];
                        if (cell.ElementType == MapElementType.Wall ||
                            cell.ElementType == MapElementType.Door ||
                            cell.ElementType == MapElementType.Window ||
                            cell.ElementType == MapElementType.Ramp)
                        {
                            cell.ElementType = MapElementType.Empty;
                        }

                        // Reset surface weights to 1
                        cell.SurfaceWeight = 1;
                        cell.OccupyingObstacle = null;
                        cell.IsWalkable = true;
                    }
                }

                _mapControl.MapGrid.UpdateAllCellProperties();
            }

            _mapControl.Invalidate();
            System.Diagnostics.Debug.WriteLine("All obstacles cleared");
        }
        #endregion
    }
}