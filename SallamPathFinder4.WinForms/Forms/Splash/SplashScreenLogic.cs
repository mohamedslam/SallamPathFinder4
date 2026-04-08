#region File Header
/// <summary>
/// File: SplashScreenLogic.cs
/// Description: Business logic for splash screen
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Threading.Tasks;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Splash
{
    public sealed class SplashScreenLogic
    {
        #region Constants
        private const int SPLASH_DISPLAY_MS = 2000;
        #endregion

        #region Constructor
        public SplashScreenLogic()
        {
        }
        #endregion

        #region Public Methods
        public int GetDisplayDuration()
        {
            return SPLASH_DISPLAY_MS;
        }

        public string GetTitleText()
        {
            return "SallamPathFinder 4";
        }

        public string GetSubtitleText()
        {
            return "Cognitive Mobility Robot";
        }

        public string GetAlgorithmText()
        {
            return "SPPA Algorithm (Shortest Path with Precautionary Avoidance)";
        }

        public string GetDeveloperText()
        {
            return "Developed by: Mohamed Elsayed Sallam";
        }

        public string GetEmailText()
        {
            return "Email: mohamedslam2000@yahoo.com";
        }

        public string GetUniversityText()
        {
            return "PhD Student at South Ural State University (SUSU)";
        }

        public string GetSupervisorText()
        {
            return "Supervisor: Prof. Tatiana Anatolyevna Makarovskikh";
        }

        public string GetLoadingText()
        {
            return "Loading...";
        }

        public string GetVersionText()
        {
            return "Version 4.0.0";
        }
        #endregion
    }
}