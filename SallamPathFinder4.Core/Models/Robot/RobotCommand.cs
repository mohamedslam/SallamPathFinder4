#region File Header
/// <summary>
/// File: RobotCommand.cs
/// Description: Robot command definitions for real robot control
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace SallamPathFinder4.Core.Models.Robot
{
    #region Enum - CommandID
    /// <summary>
    /// Command IDs for real robot communication
    /// </summary>
    public enum CommandID : byte
    {
        // Movement Commands (0x01 - 0x0F)
        Forward = 0x01,
        Backward = 0x02,
        TurnLeftTank = 0x03,
        TurnRightTank = 0x04,
        TurnLeftPivot = 0x05,
        TurnRightPivot = 0x06,
        Stop = 0x07,
        SetSpeed = 0x08,
        SetAcceleration = 0x09,

        // Action Commands (0x10 - 0x1F)
        TakePhoto = 0x10,
        StartVideo = 0x11,
        StopVideo = 0x12,
        Speak = 0x13,
        PlaySound = 0x14,
        UnloadCargo = 0x15,
        LoadCargo = 0x16,
        ArmUp = 0x17,
        ArmDown = 0x18,
        ArmExtend = 0x19,
        ArmRetract = 0x1A,
        OpenGripper = 0x1B,
        CloseGripper = 0x1C,
        Wait = 0x1D,
        Charge = 0x1E,
        EmergencyStop = 0x1F,

        // Flow Control (0x20 - 0x2F)
        Label = 0x20,
        Goto = 0x21,
        IfCondition = 0x22,
        EndIf = 0x23,
        LoopStart = 0x24,
        LoopEnd = 0x25,

        // Status Commands (0x30 - 0x3F)
        RequestTelemetry = 0x30,
        RequestFrame = 0x31,
        RequestSensors = 0x32,
        Ack = 0x33,
        Nak = 0x34
    }
    #endregion

    #region Class - RobotCommand
    /// <summary>
    /// Represents a single robot command
    /// </summary>
    public sealed class RobotCommand
    {
        #region Constructor
        public RobotCommand()
        {
            Parameters = new List<object>();
            Timestamp = DateTime.UtcNow;
        }

        public RobotCommand(CommandID id, params object[] parameters) : this()
        {
            this.ID = id;
            foreach (var param in parameters)
            {
                this.Parameters.Add(param);
            }
        }
        #endregion

        #region Properties
        public CommandID ID { get; set; }
        public List<object> Parameters { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsExecuted { get; set; }
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        #endregion

        #region Public Methods - Conversion
        /// <summary>
        /// Converts command to binary format for robot transmission
        /// </summary>
        public byte[] ToBinary()
        {
            using (var ms = new System.IO.MemoryStream())
            {
                // Command ID
                ms.WriteByte((byte)this.ID);

                // Parameter count
                ms.WriteByte((byte)this.Parameters.Count);

                // Parameters
                foreach (var param in this.Parameters)
                {
                    byte[] bytes = param switch
                    {
                        int i => BitConverter.GetBytes(i),
                        float f => BitConverter.GetBytes(f),
                        double d => BitConverter.GetBytes(d),
                        string s => Encoding.ASCII.GetBytes(s),
                        _ => new byte[0]
                    };
                    ms.Write(bytes, 0, bytes.Length);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts command to text format for debugging
        /// </summary>
        public override string ToString()
        {
            return this.ID switch
            {
                CommandID.Forward => $"F,{Parameters[0]},{Parameters[1]}",
                CommandID.Backward => $"B,{Parameters[0]},{Parameters[1]}",
                CommandID.TurnLeftTank => $"TL,{Parameters[0]}",
                CommandID.TurnRightTank => $"TR,{Parameters[0]}",
                CommandID.TurnLeftPivot => $"PL,{Parameters[0]}",
                CommandID.TurnRightPivot => $"PR,{Parameters[0]}",
                CommandID.Stop => "S",
                CommandID.SetSpeed => $"SP,{Parameters[0]}",
                CommandID.SetAcceleration => $"AC,{Parameters[0]}",
                CommandID.TakePhoto => "PH",
                CommandID.StartVideo => "VD",
                CommandID.StopVideo => "VE",
                CommandID.Speak => $"SPK,{Parameters[0]}",
                CommandID.PlaySound => $"SND,{Parameters[0]}",
                CommandID.UnloadCargo => "UL",
                CommandID.LoadCargo => "LD",
                CommandID.ArmUp => "AU",
                CommandID.ArmDown => "AD",
                CommandID.ArmExtend => $"AE,{Parameters[0]}",
                CommandID.ArmRetract => $"AR,{Parameters[0]}",
                CommandID.OpenGripper => "OG",
                CommandID.CloseGripper => "CG",
                CommandID.Wait => $"W,{Parameters[0]}",
                CommandID.Charge => $"CH,{Parameters[0]}",
                CommandID.EmergencyStop => "ES",
                _ => this.ID.ToString()
            };
        }
        #endregion
    }
    #endregion
}