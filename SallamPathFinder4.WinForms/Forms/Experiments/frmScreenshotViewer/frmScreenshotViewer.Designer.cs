#region File Header
/// <summary>
/// File: frmScreenshotViewer.Designer.cs
/// Description: Designer file for screenshot viewer form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmScreenshotViewer
{
    partial class frmScreenshotViewer
    {
        #region Private Fields - UI Components
        private System.ComponentModel.IContainer _components;
        private System.Windows.Forms.Panel _topPanel;
        private System.Windows.Forms.Panel _bottomPanel;
        private System.Windows.Forms.Panel _leftPanel;
        private System.Windows.Forms.Panel _rightPanel;
        private System.Windows.Forms.PictureBox _picScreenshot;
        private System.Windows.Forms.ListBox _lstImages;
        private System.Windows.Forms.Button _btnSaveImage;
        private System.Windows.Forms.Button _btnCopyPath;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Button _btnZoomIn;
        private System.Windows.Forms.Button _btnZoomOut;
        private System.Windows.Forms.Button _btnZoomReset;
        private System.Windows.Forms.Button _btnPrevious;
        private System.Windows.Forms.Button _btnNext;
        private System.Windows.Forms.Label _lblZoom;
        private System.Windows.Forms.Label _lblStatus;
        private System.Windows.Forms.Label _lblPathInfo;
        private System.Windows.Forms.Label _lblBatteryInfo;
        private System.Windows.Forms.Label _lblTimeInfo;
        private System.Windows.Forms.Label _lblAlgorithmInfo;
        private System.Windows.Forms.Label _lblCollisionInfo;
        private System.Windows.Forms.Label _lblErrorMessage;
        private System.Windows.Forms.Panel _infoPanel;
        #endregion

        #region Constructor
        public frmScreenshotViewer()
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
            this.Text = "Screenshot Viewer";
            this.Size = new System.Drawing.Size(1000, 750);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = System.Drawing.Color.FromArgb(240, 242, 245);

            // Left Panel (Image List)
            _leftPanel = new System.Windows.Forms.Panel();
            _leftPanel.Dock = DockStyle.Left;
            _leftPanel.Width = 200;
            _leftPanel.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            _leftPanel.Padding = new System.Windows.Forms.Padding(10);

            var lblTitle = new System.Windows.Forms.Label();
            lblTitle.Text = "Screenshots";
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
            lblTitle.Location = new System.Drawing.Point(10, 10);
            lblTitle.Size = new System.Drawing.Size(180, 30);

            _lstImages = new System.Windows.Forms.ListBox();
            _lstImages.Location = new System.Drawing.Point(10, 50);
            _lstImages.Size = new System.Drawing.Size(180, 400);
            _lstImages.Font = new System.Drawing.Font("Segoe UI", 10);
            _lstImages.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            _lstImages.ForeColor = System.Drawing.Color.White;
            _lstImages.BorderStyle = BorderStyle.None;

            _leftPanel.Controls.Add(lblTitle);
            _leftPanel.Controls.Add(_lstImages);

            // Top Panel (Zoom Controls)
            _topPanel = new System.Windows.Forms.Panel();
            _topPanel.Dock = DockStyle.Top;
            _topPanel.Height = 40;
            _topPanel.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);

            _btnZoomOut = new System.Windows.Forms.Button();
            _btnZoomOut.Text = "-";
            _btnZoomOut.Location = new System.Drawing.Point(10, 8);
            _btnZoomOut.Size = new System.Drawing.Size(30, 25);
            _btnZoomOut.FlatStyle = FlatStyle.Flat;
            _btnZoomOut.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            _btnZoomOut.ForeColor = System.Drawing.Color.White;
            _btnZoomOut.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            _lblZoom = new System.Windows.Forms.Label();
            _lblZoom.Text = "100%";
            _lblZoom.Location = new System.Drawing.Point(45, 10);
            _lblZoom.Size = new System.Drawing.Size(50, 20);
            _lblZoom.ForeColor = System.Drawing.Color.White;
            _lblZoom.TextAlign = ContentAlignment.MiddleCenter;

            _btnZoomIn = new System.Windows.Forms.Button();
            _btnZoomIn.Text = "+";
            _btnZoomIn.Location = new System.Drawing.Point(100, 8);
            _btnZoomIn.Size = new System.Drawing.Size(30, 25);
            _btnZoomIn.FlatStyle = FlatStyle.Flat;
            _btnZoomIn.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            _btnZoomIn.ForeColor = System.Drawing.Color.White;
            _btnZoomIn.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            _btnZoomReset = new System.Windows.Forms.Button();
            _btnZoomReset.Text = "Reset";
            _btnZoomReset.Location = new System.Drawing.Point(140, 8);
            _btnZoomReset.Size = new System.Drawing.Size(50, 25);
            _btnZoomReset.FlatStyle = FlatStyle.Flat;
            _btnZoomReset.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            _btnZoomReset.ForeColor = System.Drawing.Color.White;

            _btnPrevious = new System.Windows.Forms.Button();
            _btnPrevious.Text = "◀ Previous";
            _btnPrevious.Location = new System.Drawing.Point(210, 8);
            _btnPrevious.Size = new System.Drawing.Size(80, 25);
            _btnPrevious.FlatStyle = FlatStyle.Flat;
            _btnPrevious.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            _btnPrevious.ForeColor = System.Drawing.Color.White;

            _btnNext = new System.Windows.Forms.Button();
            _btnNext.Text = "Next ▶";
            _btnNext.Location = new System.Drawing.Point(295, 8);
            _btnNext.Size = new System.Drawing.Size(80, 25);
            _btnNext.FlatStyle = FlatStyle.Flat;
            _btnNext.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            _btnNext.ForeColor = System.Drawing.Color.White;

            _topPanel.Controls.Add(_btnZoomOut);
            _topPanel.Controls.Add(_lblZoom);
            _topPanel.Controls.Add(_btnZoomIn);
            _topPanel.Controls.Add(_btnZoomReset);
            _topPanel.Controls.Add(_btnPrevious);
            _topPanel.Controls.Add(_btnNext);

            // Picture Box Container
            var pictureContainer = new System.Windows.Forms.Panel();
            pictureContainer.Dock = DockStyle.Fill;
            pictureContainer.AutoScroll = true;
            pictureContainer.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);

            _picScreenshot = new System.Windows.Forms.PictureBox();
            _picScreenshot.SizeMode = PictureBoxSizeMode.AutoSize;
            _picScreenshot.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);

            pictureContainer.Controls.Add(_picScreenshot);

            // Bottom Panel (Status)
            _bottomPanel = new System.Windows.Forms.Panel();
            _bottomPanel.Dock = DockStyle.Bottom;
            _bottomPanel.Height = 30;
            _bottomPanel.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);

            _lblStatus = new System.Windows.Forms.Label();
            _lblStatus.Text = "Ready";
            _lblStatus.Dock = DockStyle.Fill;
            _lblStatus.ForeColor = System.Drawing.Color.White;
            _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            _lblStatus.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);

            _bottomPanel.Controls.Add(_lblStatus);

            // Right Panel (Info)
            _rightPanel = new System.Windows.Forms.Panel();
            _rightPanel.Dock = DockStyle.Right;
            _rightPanel.Width = 280;
            _rightPanel.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            _rightPanel.Padding = new System.Windows.Forms.Padding(10);

            var lblInfoTitle = new System.Windows.Forms.Label();
            lblInfoTitle.Text = "Experiment Details";
            lblInfoTitle.ForeColor = System.Drawing.Color.White;
            lblInfoTitle.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            lblInfoTitle.Location = new System.Drawing.Point(10, 10);
            lblInfoTitle.Size = new System.Drawing.Size(260, 25);

            _lblAlgorithmInfo = new System.Windows.Forms.Label();
            _lblAlgorithmInfo.Text = "Algorithm: -";
            _lblAlgorithmInfo.Location = new System.Drawing.Point(10, 45);
            _lblAlgorithmInfo.Size = new System.Drawing.Size(260, 20);
            _lblAlgorithmInfo.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);

            _lblPathInfo = new System.Windows.Forms.Label();
            _lblPathInfo.Text = "Path: -";
            _lblPathInfo.Location = new System.Drawing.Point(10, 70);
            _lblPathInfo.Size = new System.Drawing.Size(260, 20);
            _lblPathInfo.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);

            _lblTimeInfo = new System.Windows.Forms.Label();
            _lblTimeInfo.Text = "Time: -";
            _lblTimeInfo.Location = new System.Drawing.Point(10, 95);
            _lblTimeInfo.Size = new System.Drawing.Size(260, 20);
            _lblTimeInfo.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);

            _lblBatteryInfo = new System.Windows.Forms.Label();
            _lblBatteryInfo.Text = "Battery: -";
            _lblBatteryInfo.Location = new System.Drawing.Point(10, 120);
            _lblBatteryInfo.Size = new System.Drawing.Size(260, 20);
            _lblBatteryInfo.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);

            _lblCollisionInfo = new System.Windows.Forms.Label();
            _lblCollisionInfo.Text = "Collisions: -";
            _lblCollisionInfo.Location = new System.Drawing.Point(10, 145);
            _lblCollisionInfo.Size = new System.Drawing.Size(260, 20);
            _lblCollisionInfo.ForeColor = System.Drawing.Color.FromArgb(189, 195, 199);

            _lblErrorMessage = new System.Windows.Forms.Label();
            _lblErrorMessage.Text = "";
            _lblErrorMessage.Location = new System.Drawing.Point(10, 175);
            _lblErrorMessage.Size = new System.Drawing.Size(260, 40);
            _lblErrorMessage.ForeColor = System.Drawing.Color.FromArgb(231, 76, 60);
            _lblErrorMessage.Visible = false;

            var separator = new System.Windows.Forms.Label();
            separator.Text = "─────────────────────";
            separator.Location = new System.Drawing.Point(10, 165);
            separator.Size = new System.Drawing.Size(260, 20);
            separator.ForeColor = System.Drawing.Color.FromArgb(127, 140, 141);

            _btnSaveImage = new System.Windows.Forms.Button();
            _btnSaveImage.Text = "Save Image";
            _btnSaveImage.Location = new System.Drawing.Point(10, 230);
            _btnSaveImage.Size = new System.Drawing.Size(120, 30);
            _btnSaveImage.FlatStyle = FlatStyle.Flat;
            _btnSaveImage.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            _btnSaveImage.ForeColor = System.Drawing.Color.White;

            _btnCopyPath = new System.Windows.Forms.Button();
            _btnCopyPath.Text = "Copy Path";
            _btnCopyPath.Location = new System.Drawing.Point(140, 230);
            _btnCopyPath.Size = new System.Drawing.Size(120, 30);
            _btnCopyPath.FlatStyle = FlatStyle.Flat;
            _btnCopyPath.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            _btnCopyPath.ForeColor = System.Drawing.Color.White;

            _btnClose = new System.Windows.Forms.Button();
            _btnClose.Text = "Close";
            _btnClose.Location = new System.Drawing.Point(10, 270);
            _btnClose.Size = new System.Drawing.Size(250, 30);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            _btnClose.ForeColor = System.Drawing.Color.White;

            _rightPanel.Controls.Add(lblInfoTitle);
            _rightPanel.Controls.Add(_lblAlgorithmInfo);
            _rightPanel.Controls.Add(_lblPathInfo);
            _rightPanel.Controls.Add(_lblTimeInfo);
            _rightPanel.Controls.Add(_lblBatteryInfo);
            _rightPanel.Controls.Add(_lblCollisionInfo);
            _rightPanel.Controls.Add(separator);
            _rightPanel.Controls.Add(_lblErrorMessage);
            _rightPanel.Controls.Add(_btnSaveImage);
            _rightPanel.Controls.Add(_btnCopyPath);
            _rightPanel.Controls.Add(_btnClose);

            // Add controls to form
            this.Controls.Add(pictureContainer);
            this.Controls.Add(_rightPanel);
            this.Controls.Add(_topPanel);
            this.Controls.Add(_leftPanel);
            this.Controls.Add(_bottomPanel);
        }
        #endregion
    }
}