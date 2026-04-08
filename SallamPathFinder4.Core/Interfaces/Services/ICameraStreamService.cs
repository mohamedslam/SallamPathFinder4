#region File Header
/// <summary>
/// File: ICameraStreamService.cs
/// Description: Interface for camera stream service
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.Threading.Tasks;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    #region Interface Documentation
    /// <summary>
    /// Service interface for receiving camera stream from robot
    /// </summary>
    #endregion
    public interface ICameraStreamService
    {
        #region Properties
        /// <summary>Indicates whether streaming is active</summary>
        bool IsStreaming { get; }
        #endregion

        #region Methods
        /// <summary>Starts camera streaming</summary>
        Task StartStreamingAsync();

        /// <summary>Stops camera streaming</summary>
        void StopStreaming();
        #endregion

        #region Events
        /// <summary>Event raised when a new frame is received</summary>
        event Action<Image> FrameReceived;
        #endregion
    }
}