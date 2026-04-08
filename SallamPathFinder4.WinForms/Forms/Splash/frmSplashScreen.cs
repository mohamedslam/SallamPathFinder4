#region File Header
/// <summary>
/// File: frmSplashScreen.cs
/// Description: Splash screen displayed at application startup
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Windows.Forms;
using SallamPathFinder4.WinForms.Forms.Splash;
#endregion

namespace SallamPathFinder4.WinForms.Forms
{
    public partial class frmSplashScreen : Form
    {
        #region Private Fields
        private readonly SplashScreenLogic _logic;
        #endregion

        #region Constructor
        public frmSplashScreen()
        {
            _logic = new SplashScreenLogic();
            InitializeComponent();
            SetTextsFromLogic();
        }
        #endregion

        #region Private Methods
        private void SetTextsFromLogic()
        {
            lblTitle.Text = _logic.GetTitleText();
            lblSubtitle.Text = _logic.GetSubtitleText();
            lblAlgorithm.Text = _logic.GetAlgorithmText();
            lblDeveloper.Text = _logic.GetDeveloperText();
            lblEmail.Text = _logic.GetEmailText();
            lblUniversity.Text = _logic.GetUniversityText();
            lblSupervisor.Text = _logic.GetSupervisorText();
            lblLoading.Text = _logic.GetLoadingText();
            lblVersion.Text = _logic.GetVersionText();

            closeTimer.Interval = _logic.GetDisplayDuration();
        }
        #endregion

        #region Event Handlers
        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            closeTimer.Stop();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        #endregion
    }
}