#region File Header
/// <summary>
/// File: frmSplashScreen.Designer.cs
/// Description: Designer file for splash screen form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

namespace SallamPathFinder4.WinForms.Forms
{
    partial class frmSplashScreen
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer closeTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblAlgorithm = new Label();
            lblDeveloper = new Label();
            lblEmail = new Label();
            lblUniversity = new Label();
            lblSupervisor = new Label();
            lblLoading = new Label();
            lblVersion = new Label();
            closeTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 152, 219);
            lblTitle.Location = new Point(0, 40);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(650, 60);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "SallamPathFinder 4";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            lblSubtitle.BackColor = Color.Transparent;
            lblSubtitle.Font = new Font("Segoe UI", 14F, FontStyle.Italic);
            lblSubtitle.ForeColor = Color.FromArgb(236, 240, 241);
            lblSubtitle.Location = new Point(0, 100);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(650, 30);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Cognitive Mobility Robot";
            lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAlgorithm
            // 
            lblAlgorithm.BackColor = Color.Transparent;
            lblAlgorithm.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAlgorithm.ForeColor = Color.FromArgb(52, 152, 219);
            lblAlgorithm.Location = new Point(0, 140);
            lblAlgorithm.Name = "lblAlgorithm";
            lblAlgorithm.Size = new Size(650, 25);
            lblAlgorithm.TabIndex = 2;
            lblAlgorithm.Text = "SPPA Algorithm (Shortest Path with Precautionary Avoidance)";
            lblAlgorithm.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblDeveloper
            // 
            lblDeveloper.BackColor = Color.Transparent;
            lblDeveloper.Font = new Font("Segoe UI", 20F);
            lblDeveloper.ForeColor = Color.FromArgb(189, 195, 199);
            lblDeveloper.Location = new Point(0, 173);
            lblDeveloper.Name = "lblDeveloper";
            lblDeveloper.Size = new Size(650, 41);
            lblDeveloper.TabIndex = 3;
            lblDeveloper.Text = "Developed by: Mohamed Elsayed Sallam";
            lblDeveloper.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblEmail
            // 
            lblEmail.BackColor = Color.Transparent;
            lblEmail.Font = new Font("Segoe UI", 10F);
            lblEmail.ForeColor = Color.FromArgb(189, 195, 199);
            lblEmail.Location = new Point(0, 216);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(650, 25);
            lblEmail.TabIndex = 4;
            lblEmail.Text = "Email: mohamedslam2000@yahoo.com";
            lblEmail.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblUniversity
            // 
            lblUniversity.BackColor = Color.Transparent;
            lblUniversity.Font = new Font("Segoe UI", 10F);
            lblUniversity.ForeColor = Color.FromArgb(189, 195, 199);
            lblUniversity.Location = new Point(0, 240);
            lblUniversity.Name = "lblUniversity";
            lblUniversity.Size = new Size(650, 25);
            lblUniversity.TabIndex = 5;
            lblUniversity.Text = "PhD Student at South Ural State University (SUSU)";
            lblUniversity.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSupervisor
            // 
            lblSupervisor.BackColor = Color.Transparent;
            lblSupervisor.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblSupervisor.ForeColor = Color.FromArgb(52, 152, 219);
            lblSupervisor.Location = new Point(0, 286);
            lblSupervisor.Name = "lblSupervisor";
            lblSupervisor.Size = new Size(650, 47);
            lblSupervisor.TabIndex = 6;
            lblSupervisor.Text = "Supervisor: Prof. Tatiana Anatolyevna Makarovskikh";
            lblSupervisor.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblLoading
            // 
            lblLoading.BackColor = Color.Transparent;
            lblLoading.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblLoading.ForeColor = Color.FromArgb(149, 165, 166);
            lblLoading.Location = new Point(0, 380);
            lblLoading.Name = "lblLoading";
            lblLoading.Size = new Size(650, 25);
            lblLoading.TabIndex = 7;
            lblLoading.Text = "Loading...";
            lblLoading.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            lblVersion.BackColor = Color.Transparent;
            lblVersion.Font = new Font("Segoe UI", 8F);
            lblVersion.ForeColor = Color.FromArgb(127, 140, 141);
            lblVersion.Location = new Point(0, 450);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(640, 20);
            lblVersion.TabIndex = 8;
            lblVersion.Text = "Version 4.0.0";
            lblVersion.TextAlign = ContentAlignment.MiddleRight;
            // 
            // closeTimer
            // 
            closeTimer.Enabled = true;
            closeTimer.Interval = 2000;
            closeTimer.Tick += CloseTimer_Tick;
            // 
            // frmSplashScreen
            // 
            BackColor = Color.FromArgb(44, 62, 80);
            ClientSize = new Size(650, 480);
            Controls.Add(lblTitle);
            Controls.Add(lblSubtitle);
            Controls.Add(lblAlgorithm);
            Controls.Add(lblDeveloper);
            Controls.Add(lblEmail);
            Controls.Add(lblUniversity);
            Controls.Add(lblSupervisor);
            Controls.Add(lblLoading);
            Controls.Add(lblVersion);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmSplashScreen";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SallamPathFinder 4";
            ResumeLayout(false);
        }

        // Controls
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblAlgorithm;
        private System.Windows.Forms.Label lblDeveloper;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblUniversity;
        private System.Windows.Forms.Label lblSupervisor;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Label lblVersion;
    }
}