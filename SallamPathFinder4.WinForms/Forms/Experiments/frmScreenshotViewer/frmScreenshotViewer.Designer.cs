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
            _leftPanel = new Panel();
            lblTitle = new Label();
            _lstImages = new ListBox();
            _topPanel = new Panel();
            _btnZoomOut = new Button();
            _lblZoom = new Label();
            _btnZoomIn = new Button();
            _btnZoomReset = new Button();
            _btnPrevious = new Button();
            _btnNext = new Button();
            pictureContainer = new Panel();
            _picScreenshot = new PictureBox();
            _bottomPanel = new Panel();
            _lblStatus = new Label();
            _rightPanel = new Panel();
            lblInfoTitle = new Label();
            _lblAlgorithmInfo = new Label();
            _lblPathInfo = new Label();
            _lblTimeInfo = new Label();
            _lblBatteryInfo = new Label();
            _lblCollisionInfo = new Label();
            separator = new Label();
            _lblErrorMessage = new Label();
            _btnSaveImage = new Button();
            _btnCopyPath = new Button();
            _btnClose = new Button();
            _leftPanel.SuspendLayout();
            _topPanel.SuspendLayout();
            pictureContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_picScreenshot).BeginInit();
            _bottomPanel.SuspendLayout();
            _rightPanel.SuspendLayout();
            SuspendLayout();
            // 
            // _leftPanel
            // 
            _leftPanel.BackColor = Color.FromArgb(52, 73, 94);
            _leftPanel.Controls.Add(lblTitle);
            _leftPanel.Controls.Add(_lstImages);
            _leftPanel.Dock = DockStyle.Left;
            _leftPanel.Location = new Point(0, 0);
            _leftPanel.Name = "_leftPanel";
            _leftPanel.Padding = new Padding(10);
            _leftPanel.Size = new Size(200, 681);
            _leftPanel.TabIndex = 3;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(10, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(180, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Screenshots";
            // 
            // _lstImages
            // 
            _lstImages.BackColor = Color.FromArgb(44, 62, 80);
            _lstImages.BorderStyle = BorderStyle.None;
            _lstImages.Font = new Font("Segoe UI", 10F);
            _lstImages.ForeColor = Color.White;
            _lstImages.ItemHeight = 17;
            _lstImages.Location = new Point(10, 50);
            _lstImages.Name = "_lstImages";
            _lstImages.Size = new Size(180, 153);
            _lstImages.TabIndex = 1;
            // 
            // _topPanel
            // 
            _topPanel.BackColor = Color.FromArgb(44, 62, 80);
            _topPanel.Controls.Add(_btnZoomOut);
            _topPanel.Controls.Add(_lblZoom);
            _topPanel.Controls.Add(_btnZoomIn);
            _topPanel.Controls.Add(_btnZoomReset);
            _topPanel.Controls.Add(_btnPrevious);
            _topPanel.Controls.Add(_btnNext);
            _topPanel.Dock = DockStyle.Top;
            _topPanel.Location = new Point(200, 0);
            _topPanel.Name = "_topPanel";
            _topPanel.Size = new Size(784, 40);
            _topPanel.TabIndex = 2;
            // 
            // _btnZoomOut
            // 
            _btnZoomOut.BackColor = Color.FromArgb(52, 73, 94);
            _btnZoomOut.FlatStyle = FlatStyle.Flat;
            _btnZoomOut.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnZoomOut.ForeColor = Color.White;
            _btnZoomOut.Location = new Point(10, 8);
            _btnZoomOut.Name = "_btnZoomOut";
            _btnZoomOut.Size = new Size(30, 25);
            _btnZoomOut.TabIndex = 0;
            _btnZoomOut.Text = "-";
            _btnZoomOut.UseVisualStyleBackColor = false;
            // 
            // _lblZoom
            // 
            _lblZoom.ForeColor = Color.White;
            _lblZoom.Location = new Point(45, 10);
            _lblZoom.Name = "_lblZoom";
            _lblZoom.Size = new Size(50, 20);
            _lblZoom.TabIndex = 1;
            _lblZoom.Text = "100%";
            _lblZoom.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // _btnZoomIn
            // 
            _btnZoomIn.BackColor = Color.FromArgb(52, 73, 94);
            _btnZoomIn.FlatStyle = FlatStyle.Flat;
            _btnZoomIn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnZoomIn.ForeColor = Color.White;
            _btnZoomIn.Location = new Point(100, 8);
            _btnZoomIn.Name = "_btnZoomIn";
            _btnZoomIn.Size = new Size(30, 25);
            _btnZoomIn.TabIndex = 2;
            _btnZoomIn.Text = "+";
            _btnZoomIn.UseVisualStyleBackColor = false;
            // 
            // _btnZoomReset
            // 
            _btnZoomReset.BackColor = Color.FromArgb(52, 152, 219);
            _btnZoomReset.FlatStyle = FlatStyle.Flat;
            _btnZoomReset.ForeColor = Color.White;
            _btnZoomReset.Location = new Point(140, 8);
            _btnZoomReset.Name = "_btnZoomReset";
            _btnZoomReset.Size = new Size(50, 25);
            _btnZoomReset.TabIndex = 3;
            _btnZoomReset.Text = "Reset";
            _btnZoomReset.UseVisualStyleBackColor = false;
            // 
            // _btnPrevious
            // 
            _btnPrevious.BackColor = Color.FromArgb(52, 73, 94);
            _btnPrevious.FlatStyle = FlatStyle.Flat;
            _btnPrevious.ForeColor = Color.White;
            _btnPrevious.Location = new Point(210, 8);
            _btnPrevious.Name = "_btnPrevious";
            _btnPrevious.Size = new Size(80, 25);
            _btnPrevious.TabIndex = 4;
            _btnPrevious.Text = "◀ Previous";
            _btnPrevious.UseVisualStyleBackColor = false;
            // 
            // _btnNext
            // 
            _btnNext.BackColor = Color.FromArgb(52, 73, 94);
            _btnNext.FlatStyle = FlatStyle.Flat;
            _btnNext.ForeColor = Color.White;
            _btnNext.Location = new Point(295, 8);
            _btnNext.Name = "_btnNext";
            _btnNext.Size = new Size(80, 25);
            _btnNext.TabIndex = 5;
            _btnNext.Text = "Next ▶";
            _btnNext.UseVisualStyleBackColor = false;
            // 
            // pictureContainer
            // 
            pictureContainer.AutoScroll = true;
            pictureContainer.BackColor = Color.FromArgb(44, 62, 80);
            pictureContainer.Controls.Add(_picScreenshot);
            pictureContainer.Dock = DockStyle.Fill;
            pictureContainer.Location = new Point(200, 40);
            pictureContainer.Name = "pictureContainer";
            pictureContainer.Size = new Size(504, 641);
            pictureContainer.TabIndex = 0;
            // 
            // _picScreenshot
            // 
            _picScreenshot.BackColor = Color.FromArgb(44, 62, 80);
            _picScreenshot.Location = new Point(0, 0);
            _picScreenshot.Name = "_picScreenshot";
            _picScreenshot.Size = new Size(100, 50);
            _picScreenshot.SizeMode = PictureBoxSizeMode.AutoSize;
            _picScreenshot.TabIndex = 0;
            _picScreenshot.TabStop = false;
            // 
            // _bottomPanel
            // 
            _bottomPanel.BackColor = Color.FromArgb(44, 62, 80);
            _bottomPanel.Controls.Add(_lblStatus);
            _bottomPanel.Dock = DockStyle.Bottom;
            _bottomPanel.Location = new Point(0, 681);
            _bottomPanel.Name = "_bottomPanel";
            _bottomPanel.Size = new Size(984, 30);
            _bottomPanel.TabIndex = 4;
            // 
            // _lblStatus
            // 
            _lblStatus.Dock = DockStyle.Fill;
            _lblStatus.ForeColor = Color.White;
            _lblStatus.Location = new Point(0, 0);
            _lblStatus.Name = "_lblStatus";
            _lblStatus.Padding = new Padding(5, 0, 0, 0);
            _lblStatus.Size = new Size(984, 30);
            _lblStatus.TabIndex = 0;
            _lblStatus.Text = "Ready";
            _lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _rightPanel
            // 
            _rightPanel.BackColor = Color.FromArgb(52, 73, 94);
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
            _rightPanel.Dock = DockStyle.Right;
            _rightPanel.Location = new Point(704, 40);
            _rightPanel.Name = "_rightPanel";
            _rightPanel.Padding = new Padding(10);
            _rightPanel.Size = new Size(280, 641);
            _rightPanel.TabIndex = 1;
            // 
            // lblInfoTitle
            // 
            lblInfoTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblInfoTitle.ForeColor = Color.White;
            lblInfoTitle.Location = new Point(10, 10);
            lblInfoTitle.Name = "lblInfoTitle";
            lblInfoTitle.Size = new Size(260, 25);
            lblInfoTitle.TabIndex = 0;
            lblInfoTitle.Text = "Experiment Details";
            // 
            // _lblAlgorithmInfo
            // 
            _lblAlgorithmInfo.ForeColor = Color.FromArgb(189, 195, 199);
            _lblAlgorithmInfo.Location = new Point(10, 45);
            _lblAlgorithmInfo.Name = "_lblAlgorithmInfo";
            _lblAlgorithmInfo.Size = new Size(260, 20);
            _lblAlgorithmInfo.TabIndex = 1;
            _lblAlgorithmInfo.Text = "Algorithm: -";
            // 
            // _lblPathInfo
            // 
            _lblPathInfo.ForeColor = Color.FromArgb(189, 195, 199);
            _lblPathInfo.Location = new Point(10, 70);
            _lblPathInfo.Name = "_lblPathInfo";
            _lblPathInfo.Size = new Size(260, 20);
            _lblPathInfo.TabIndex = 2;
            _lblPathInfo.Text = "Path: -";
            // 
            // _lblTimeInfo
            // 
            _lblTimeInfo.ForeColor = Color.FromArgb(189, 195, 199);
            _lblTimeInfo.Location = new Point(10, 95);
            _lblTimeInfo.Name = "_lblTimeInfo";
            _lblTimeInfo.Size = new Size(260, 20);
            _lblTimeInfo.TabIndex = 3;
            _lblTimeInfo.Text = "Time: -";
            // 
            // _lblBatteryInfo
            // 
            _lblBatteryInfo.ForeColor = Color.FromArgb(189, 195, 199);
            _lblBatteryInfo.Location = new Point(10, 120);
            _lblBatteryInfo.Name = "_lblBatteryInfo";
            _lblBatteryInfo.Size = new Size(260, 20);
            _lblBatteryInfo.TabIndex = 4;
            _lblBatteryInfo.Text = "Battery: -";
            // 
            // _lblCollisionInfo
            // 
            _lblCollisionInfo.ForeColor = Color.FromArgb(189, 195, 199);
            _lblCollisionInfo.Location = new Point(10, 145);
            _lblCollisionInfo.Name = "_lblCollisionInfo";
            _lblCollisionInfo.Size = new Size(260, 20);
            _lblCollisionInfo.TabIndex = 5;
            _lblCollisionInfo.Text = "Collisions: -";
            // 
            // separator
            // 
            separator.ForeColor = Color.FromArgb(127, 140, 141);
            separator.Location = new Point(0, 523);
            separator.Name = "separator";
            separator.Size = new Size(260, 20);
            separator.TabIndex = 6;
            separator.Text = "─────────────────────";
            // 
            // _lblErrorMessage
            // 
            _lblErrorMessage.ForeColor = Color.FromArgb(231, 76, 60);
            _lblErrorMessage.Location = new Point(10, 175);
            _lblErrorMessage.Name = "_lblErrorMessage";
            _lblErrorMessage.Size = new Size(260, 40);
            _lblErrorMessage.TabIndex = 7;
            _lblErrorMessage.Visible = false;
            // 
            // _btnSaveImage
            // 
            _btnSaveImage.BackColor = Color.FromArgb(46, 204, 113);
            _btnSaveImage.FlatStyle = FlatStyle.Flat;
            _btnSaveImage.ForeColor = Color.White;
            _btnSaveImage.Location = new Point(13, 555);
            _btnSaveImage.Name = "_btnSaveImage";
            _btnSaveImage.Size = new Size(120, 30);
            _btnSaveImage.TabIndex = 8;
            _btnSaveImage.Text = "Save Image";
            _btnSaveImage.UseVisualStyleBackColor = false;
            // 
            // _btnCopyPath
            // 
            _btnCopyPath.BackColor = Color.FromArgb(52, 152, 219);
            _btnCopyPath.FlatStyle = FlatStyle.Flat;
            _btnCopyPath.ForeColor = Color.White;
            _btnCopyPath.Location = new Point(143, 555);
            _btnCopyPath.Name = "_btnCopyPath";
            _btnCopyPath.Size = new Size(120, 30);
            _btnCopyPath.TabIndex = 9;
            _btnCopyPath.Text = "Copy Path";
            _btnCopyPath.UseVisualStyleBackColor = false;
            // 
            // _btnClose
            // 
            _btnClose.BackColor = Color.FromArgb(149, 165, 166);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.ForeColor = Color.White;
            _btnClose.Location = new Point(13, 595);
            _btnClose.Name = "_btnClose";
            _btnClose.Size = new Size(250, 30);
            _btnClose.TabIndex = 10;
            _btnClose.Text = "Close";
            _btnClose.UseVisualStyleBackColor = false;
            // 
            // frmScreenshotViewer
            // 
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(984, 711);
            Controls.Add(pictureContainer);
            Controls.Add(_rightPanel);
            Controls.Add(_topPanel);
            Controls.Add(_leftPanel);
            Controls.Add(_bottomPanel);
            MinimumSize = new Size(800, 600);
            Name = "frmScreenshotViewer";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Screenshot Viewer";
            _leftPanel.ResumeLayout(false);
            _topPanel.ResumeLayout(false);
            pictureContainer.ResumeLayout(false);
            pictureContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_picScreenshot).EndInit();
            _bottomPanel.ResumeLayout(false);
            _rightPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
        #endregion

        private Label lblTitle;
        private Panel pictureContainer;
        private Label lblInfoTitle;
        private Label separator;
    }
}