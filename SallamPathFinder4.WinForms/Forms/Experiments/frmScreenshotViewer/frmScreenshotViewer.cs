#region File Header
/// <summary>
/// File: frmScreenshotViewer.cs
/// Description: Form to display experiment screenshots with navigation
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.WinForms.Forms.Experiments.frmScreenshotViewer.Core;
using SallamPathFinder4.WinForms.Models;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmScreenshotViewer
{
    /// <summary>
    /// Form for viewing experiment screenshots with zoom and pan capabilities
    /// </summary>
    public sealed partial class frmScreenshotViewer : Form
    {
        #region Constants
        private const int FORM_WIDTH = 900;
        private const int FORM_HEIGHT = 700;
        private const int INFO_PANEL_HEIGHT = 120;
        private const float ZOOM_STEP = 0.1f;
        private const float MIN_ZOOM = 0.5f;
        private const float MAX_ZOOM = 3.0f;
        #endregion

        #region Private Fields
        private readonly ExperimentResultItem _result;
        private readonly string _resultsFolderPath;
        private readonly ScreenshotViewerLogic _logic;
        private float _currentZoom;
        private Point _imageOffset;
        private Point _lastMousePosition;
        private bool _isPanning;
        private Image _currentImage;
        private string _currentImagePath;

        private string _selectedImagePath;
        private string _selectedImageType;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the screenshot viewer form
        /// </summary>
        public frmScreenshotViewer(ExperimentResultItem resultItem, string folderPath)
        {
            _result = resultItem ?? throw new ArgumentNullException(nameof(resultItem));
            _resultsFolderPath = folderPath ?? string.Empty;
            _logic = new ScreenshotViewerLogic();
            _currentZoom = 1.0f;
            _imageOffset = Point.Empty;

            InitializeComponent();
            WireEvents();
            LoadScreenshots();
        }

        /// <summary>
        /// Initializes a new instance of the screenshot viewer form with a specific image preselected
        /// </summary>
        /// <param name="resultItem">The experiment result item</param>
        /// <param name="folderPath">The results folder path</param>
        /// <param name="selectedImageType">The type of image to preselect (initial, path, completed)</param>
        public frmScreenshotViewer(ExperimentResultItem resultItem, string folderPath, string selectedImageType)
            : this(resultItem, folderPath)
        {
            SelectImageByType(selectedImageType);
        }

        #endregion
 

        #region Private Methods - Initialization
        /// <summary>
        /// Wires up all event handlers
        /// </summary>
        private void WireEvents()
        {
            _btnSaveImage.Click += BtnSaveImage_Click;
            _btnCopyPath.Click += BtnCopyPath_Click;
            _btnClose.Click += (s, e) => Close();
            _btnZoomIn.Click += BtnZoomIn_Click;
            _btnZoomOut.Click += BtnZoomOut_Click;
            _btnZoomReset.Click += BtnZoomReset_Click;
            _btnPrevious.Click += BtnPrevious_Click;
            _btnNext.Click += BtnNext_Click;

            _picScreenshot.MouseWheel += PicScreenshot_MouseWheel;
            _picScreenshot.MouseDown += PicScreenshot_MouseDown;
            _picScreenshot.MouseMove += PicScreenshot_MouseMove;
            _picScreenshot.MouseUp += PicScreenshot_MouseUp;
            _picScreenshot.Paint += PicScreenshot_Paint;

            _lstImages.SelectedIndexChanged += LstImages_SelectedIndexChanged;
        }

        // <summary>
        /// Selects an image by its type (initial, path, completed)
        /// </summary>
        /// <param name="imageType">The type of image to select</param>
        public void SelectImageByType(string imageType)
        {
            if (string.IsNullOrEmpty(imageType) || _lstImages.Items.Count == 0) return;

            int selectedIndex = -1;

            switch (imageType.ToLower())
            {
                case "initial":
                    selectedIndex = 0;
                    break;
                case "path":
                    selectedIndex = 1;
                    break;
                case "completed":
                    selectedIndex = 2;
                    break;
            }

            if (selectedIndex >= 0 && selectedIndex < _lstImages.Items.Count)
            {
                _lstImages.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Loads all available screenshots for the experiment
        /// </summary>
        private void LoadScreenshots()
        {
            _logic.LoadScreenshots(_result, _resultsFolderPath);

            _lstImages.Items.Clear();

            if (!string.IsNullOrEmpty(_logic.InitialPath))
            {
                _lstImages.Items.Add(new ImageListItem { Name = "Initial", Path = _logic.InitialPath });
            }

            if (!string.IsNullOrEmpty(_logic.PathPath))
            {
                _lstImages.Items.Add(new ImageListItem { Name = "Path", Path = _logic.PathPath });
            }

            if (!string.IsNullOrEmpty(_logic.CompletedPath))
            {
                _lstImages.Items.Add(new ImageListItem { Name = "Completed", Path = _logic.CompletedPath });
            }

            if (_lstImages.Items.Count > 0)
            {
                _lstImages.SelectedIndex = 0;
            }
            else
            {
                ShowNoScreenshotAvailable();
            }

            UpdateInfoPanel();
        }

        /// <summary>
        /// Shows message when no screenshot is available
        /// </summary>
        private void ShowNoScreenshotAvailable()
        {
            _lblPathInfo.Text = "No screenshots available for this experiment.";
            _lblBatteryInfo.Text = string.Empty;
            _lblTimeInfo.Text = string.Empty;
            _picScreenshot.BackColor = Color.FromArgb(240, 240, 240);
            _picScreenshot.Image = null;
        }

        /// <summary>
        /// Updates the information panel with experiment details
        /// </summary>
        private void UpdateInfoPanel()
        {
            _lblPathInfo.Text = $"📍 Path: {_result.PathLength} cells | Return: {(_result.ReturnPathLength > 0 ? _result.ReturnPathLength.ToString() : "N/A")} cells";
            _lblBatteryInfo.Text = $"🔋 Battery: {_result.RemainingBattery:F1}% remaining | Success Rate: {_result.SuccessRate:F1}%";
            _lblTimeInfo.Text = $"⏱️ Computation Time: {_result.ComputationTimeMs:F2} ms | Success: {(_result.Success ? "✓" : "✗")}";
            _lblAlgorithmInfo.Text = $"🧠 Algorithm: {_result.Algorithm} | Metric: {_result.Metric} | Iteration: {_result.Iteration}";

            if (_result.CollisionCount > 0)
            {
                _lblCollisionInfo.Text = $"💥 Collisions: {_result.CollisionCount} | Errors: {_result.InvalidMoveCount}";
                _lblCollisionInfo.ForeColor = Color.FromArgb(231, 76, 60);
            }
            else
            {
                _lblCollisionInfo.Text = $"💚 No collisions | Errors: {_result.InvalidMoveCount}";
                _lblCollisionInfo.ForeColor = Color.FromArgb(46, 204, 113);
            }

            if (!string.IsNullOrEmpty(_result.ErrorMessage))
            {
                _lblErrorMessage.Text = $"⚠️ Error: {_result.ErrorMessage}";
                _lblErrorMessage.ForeColor = Color.FromArgb(231, 76, 60);
                _lblErrorMessage.Visible = true;
            }
            else
            {
                _lblErrorMessage.Visible = false;
            }
        }


        #endregion

        #region Private Methods - Image Loading
        /// <summary>
        /// Loads and displays the selected screenshot
        /// </summary>
        private void LoadSelectedScreenshot()
        {
            if (_lstImages.SelectedItem == null) return;

            var item = _lstImages.SelectedItem as ImageListItem;
            if (item == null || string.IsNullOrEmpty(item.Path)) return;

            if (!File.Exists(item.Path))
            {
                _lblStatus.Text = $"File not found: {item.Path}";
                return;
            }

            try
            {
                if (_currentImage != null)
                {
                    _currentImage.Dispose();
                }

                _currentImagePath = item.Path;
                _currentImage = Image.FromFile(item.Path);

                _currentZoom = 1.0f;
                _imageOffset = Point.Empty;

                UpdatePictureBoxSize();
                _picScreenshot.Invalidate();

                _lblStatus.Text = $"Loaded: {item.Name} - {Path.GetFileName(item.Path)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                _lblStatus.Text = "Failed to load image";
            }
        }

        /// <summary>
        /// Updates the picture box size based on current zoom
        /// </summary>
        private void UpdatePictureBoxSize()
        {
            if (_currentImage == null) return;

            int newWidth = (int)(_currentImage.Width * _currentZoom);
            int newHeight = (int)(_currentImage.Height * _currentZoom);

            _picScreenshot.Size = new Size(newWidth, newHeight);
            _picScreenshot.Location = new Point(_imageOffset.X, _imageOffset.Y);
        }
        #endregion

        #region Private Methods - Zoom and Pan
        /// <summary>
        /// Applies zoom to the image
        /// </summary>
        private void ApplyZoom(float delta)
        {
            float oldZoom = _currentZoom;
            _currentZoom += delta;
            _currentZoom = Math.Max(MIN_ZOOM, Math.Min(MAX_ZOOM, _currentZoom));

            if (Math.Abs(oldZoom - _currentZoom) > 0.01f)
            {
                // Adjust offset to keep center point
                Point center = new Point(_picScreenshot.Width / 2, _picScreenshot.Height / 2);
                float ratio = _currentZoom / oldZoom;
                _imageOffset = new Point(
                    (int)(_imageOffset.X * ratio),
                    (int)(_imageOffset.Y * ratio));

                UpdatePictureBoxSize();
                _picScreenshot.Invalidate();

                _lblZoom.Text = $"{_currentZoom * 100:F0}%";
            }
        }

        /// <summary>
        /// Resets zoom to 100%
        /// </summary>
        private void ResetZoom()
        {
            _currentZoom = 1.0f;
            _imageOffset = Point.Empty;
            UpdatePictureBoxSize();
            _picScreenshot.Invalidate();
            _lblZoom.Text = "100%";
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles save image button click
        /// </summary>
        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            if (_currentImage == null)
            {
                MessageBox.Show("No image to save.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var sfd = new SaveFileDialog();
            sfd.Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg)|*.jpg";
            sfd.FileName = Path.GetFileName(_currentImagePath);

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _currentImage.Save(sfd.FileName);
                    MessageBox.Show($"Image saved to:\n{sfd.FileName}", "Save Complete",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving image: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Handles copy path button click
        /// </summary>
        private void BtnCopyPath_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentImagePath) && File.Exists(_currentImagePath))
            {
                Clipboard.SetText(_currentImagePath);
                _lblStatus.Text = "Path copied to clipboard!";
            }
            else
            {
                Clipboard.SetText("No screenshot available");
                MessageBox.Show("No screenshot path to copy.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Handles zoom in button click
        /// </summary>
        private void BtnZoomIn_Click(object sender, EventArgs e)
        {
            ApplyZoom(ZOOM_STEP);
        }

        /// <summary>
        /// Handles zoom out button click
        /// </summary>
        private void BtnZoomOut_Click(object sender, EventArgs e)
        {
            ApplyZoom(-ZOOM_STEP);
        }

        /// <summary>
        /// Handles zoom reset button click
        /// </summary>
        private void BtnZoomReset_Click(object sender, EventArgs e)
        {
            ResetZoom();
        }

        /// <summary>
        /// Handles previous image button click
        /// </summary>
        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_lstImages.SelectedIndex > 0)
            {
                _lstImages.SelectedIndex--;
            }
        }

        /// <summary>
        /// Handles next image button click
        /// </summary>
        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_lstImages.SelectedIndex < _lstImages.Items.Count - 1)
            {
                _lstImages.SelectedIndex++;
            }
        }

        /// <summary>
        /// Handles list box selection change
        /// </summary>
        private void LstImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedScreenshot();
        }

        /// <summary>
        /// Handles mouse wheel for zooming
        /// </summary>
        private void PicScreenshot_MouseWheel(object sender, MouseEventArgs e)
        {
            float delta = e.Delta > 0 ? ZOOM_STEP : -ZOOM_STEP;
            ApplyZoom(delta);
        }

        /// <summary>
        /// Handles mouse down for panning
        /// </summary>
        private void PicScreenshot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle || e.Button == MouseButtons.Left)
            {
                _isPanning = true;
                _lastMousePosition = e.Location;
                _picScreenshot.Cursor = Cursors.SizeAll;
            }
        }

        /// <summary>
        /// Handles mouse move for panning
        /// </summary>
        private void PicScreenshot_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                int dx = e.X - _lastMousePosition.X;
                int dy = e.Y - _lastMousePosition.Y;
                _imageOffset = new Point(_imageOffset.X + dx, _imageOffset.Y + dy);
                _picScreenshot.Location = _imageOffset;
                _lastMousePosition = e.Location;
                _picScreenshot.Invalidate();
            }
        }

        /// <summary>
        /// Handles mouse up to end panning
        /// </summary>
        private void PicScreenshot_MouseUp(object sender, MouseEventArgs e)
        {
            _isPanning = false;
            _picScreenshot.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Handles paint event to draw image
        /// </summary>
        private void PicScreenshot_Paint(object sender, PaintEventArgs e)
        {
            if (_currentImage != null)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(_currentImage, 0, 0, _picScreenshot.Width, _picScreenshot.Height);
            }
        }
        #endregion
    }

    #region Helper Class
    /// <summary>
    /// List item for screenshot list
    /// </summary>
    public class ImageListItem
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
    #endregion
}