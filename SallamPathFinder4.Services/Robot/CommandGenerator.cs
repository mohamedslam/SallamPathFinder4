#region File Header
/// <summary>
/// File: CommandGenerator.cs
/// Description: Generates robot commands from path
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SallamPathFinder4.Core.Models.Path;
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.Services.Robot
{
    #region Class Documentation
    /// <summary>
    /// Generates robot commands from path
    /// </summary>
    #endregion
    public sealed class CommandGenerator
    {
        #region Constants
        private const double CELL_SIZE_CM = 10.0;
        private const double DEFAULT_SPEED_CM_S = 10.0;
        private const double DEFAULT_ACCELERATION_CM_S2 = 10.0;
        #endregion

        #region Private Fields
        private double _cellSizeCm;
        private double _robotSpeedCmS;
        private double _robotAccelerationCmS2;
        private float _currentAngle;
        #endregion

        #region Constructor
        public CommandGenerator(double cellSizeCm = CELL_SIZE_CM,
                                double robotSpeedCmS = DEFAULT_SPEED_CM_S,
                                double robotAccelerationCmS2 = DEFAULT_ACCELERATION_CM_S2)
        {
            _cellSizeCm = cellSizeCm;
            _robotSpeedCmS = robotSpeedCmS;
            _robotAccelerationCmS2 = robotAccelerationCmS2;
            _currentAngle = 0;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Generates commands from path
        /// </summary>
        public List<RobotCommand> GenerateFromPath(List<PathNode> path)
        {
            if (path == null || path.Count < 2)
            {
                return new List<RobotCommand>();
            }

            var commands = new List<RobotCommand>();
            _currentAngle = 0;

            for (int i = 1; i < path.Count; i++)
            {
                var from = path[i - 1];
                var to = path[i];

                // Calculate angle for this step
                float targetAngle = (float)(Math.Atan2(to.Y - from.Y, to.X - from.X) * 180 / Math.PI);

                // Add turn command if angle changed
                if (Math.Abs(_currentAngle - targetAngle) > 0.1f)
                {
                    commands.Add(CreateTurnCommand(_currentAngle, targetAngle));
                    _currentAngle = targetAngle;
                }

                // Add forward command
                commands.Add(CreateForwardCommand());
            }

            return commands;
        }

        /// <summary>
        /// Generates commands with step counting (compressed format)
        /// </summary>
        public List<RobotCommand> GenerateCompressedCommands(List<PathNode> path)
        {
            var rawCommands = this.GenerateFromPath(path);
            return this.CompressCommands(rawCommands);
        }
        #endregion

        #region Private Methods
        private RobotCommand CreateForwardCommand()
        {
            return new RobotCommand(CommandID.Forward, 1, (int)_currentAngle);
        }

        private RobotCommand CreateTurnCommand(float fromAngle, float toAngle)
        {
            float angleDiff = toAngle - fromAngle;

            // Normalize angle difference to -180 to 180
            if (angleDiff > 180) angleDiff -= 360;
            if (angleDiff < -180) angleDiff += 360;

            // Use tank turn by default (faster)
            if (angleDiff > 0)
            {
                return new RobotCommand(CommandID.TurnRightTank, (int)Math.Abs(angleDiff));
            }
            else
            {
                return new RobotCommand(CommandID.TurnLeftTank, (int)Math.Abs(angleDiff));
            }
        }

        private List<RobotCommand> CompressCommands(List<RobotCommand> commands)
        {
            if (commands == null || commands.Count == 0)
            {
                return commands;
            }

            var compressed = new List<RobotCommand>();
            int i = 0;

            while (i < commands.Count)
            {
                var current = commands[i];
                int count = 1;

                // Count consecutive identical commands
                while (i + count < commands.Count &&
                       commands[i + count].ID == current.ID &&
                       commands[i + count].Parameters.Count == current.Parameters.Count)
                {
                    bool sameParams = true;
                    for (int p = 0; p < current.Parameters.Count; p++)
                    {
                        if (!commands[i + count].Parameters[p].Equals(current.Parameters[p]))
                        {
                            sameParams = false;
                            break;
                        }
                    }

                    if (sameParams)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (count > 1 && current.ID == CommandID.Forward)
                {
                    // Compress multiple forward commands into one
                    int totalSteps = count;
                    compressed.Add(new RobotCommand(CommandID.Forward, totalSteps, (int)current.Parameters[1]));
                }
                else
                {
                    // Add commands individually
                    for (int j = 0; j < count; j++)
                    {
                        compressed.Add(commands[i + j]);
                    }
                }

                i += count;
            }

            return compressed;
        }
        #endregion
    }
}