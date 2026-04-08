#region File Header
/// <summary>
/// File: RobotState.cs
/// Description: Represents the current state of the robot
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Models.Robot
{
    #region Class Documentation
    /// <summary>
    /// Represents the current state of the robot
    /// Includes position, orientation, battery, speed, and sensor readings
    /// </summary>
    #endregion
    public sealed class RobotState
    {
        #region Constructor
        /// <summary>
        /// Initializes a new robot state at the specified start position
        /// </summary>
        public RobotState(Point startPosition)
        {
            Position = startPosition;
            Angle = 0;
            BatteryLevel = 100.0;
            Speed = 0;
            FrontSensorDistance = 0;
            LeftSensorDistance = 0;
            RightSensorDistance = 0;
            BackSensorDistance = 0;
        }
        #endregion

        #region Properties - Movement
        /// <summary>Current position on grid (cell coordinates)</summary>
        public Point Position { get; set; }

        /// <summary>Current orientation in degrees (0 = right, positive counter-clockwise)</summary>
        public float Angle { get; set; }

        /// <summary>Current speed in centimeters per second</summary>
        public double Speed { get; set; }
        #endregion

        #region Properties - Battery
        /// <summary>Current battery level (0-100 percent)</summary>
        public double BatteryLevel { get; set; }
        #endregion

        #region Properties - Sensors
        /// <summary>Front ultrasonic sensor reading in cells</summary>
        public double FrontSensorDistance { get; set; }

        /// <summary>Left ultrasonic sensor reading in cells</summary>
        public double LeftSensorDistance { get; set; }

        /// <summary>Right ultrasonic sensor reading in cells</summary>
        public double RightSensorDistance { get; set; }

        /// <summary>Back ultrasonic sensor reading in cells</summary>
        public double BackSensorDistance { get; set; }
        #endregion

        #region Properties - Derived
        /// <summary>
        /// Indicates whether the robot is moving
        /// </summary>
        public bool IsMoving => Speed > 0.1;

        /// <summary>
        /// Indicates whether the robot has low battery (less than 20%)
        /// </summary>
        public bool IsLowBattery => BatteryLevel < 20;

        /// <summary>
        /// Indicates whether the robot has critical battery (less than 10%)
        /// </summary>
        public bool IsCriticalBattery => BatteryLevel < 10;

        /// <summary>
        /// Indicates whether the robot is out of battery
        /// </summary>
        public bool IsBatteryEmpty => BatteryLevel <= 0;
        #endregion

        #region Public Methods
        /// <summary>
        /// Consumes battery by the specified amount
        /// </summary>
        /// <returns>True if battery still has charge, false if depleted</returns>
        public bool ConsumeBattery(double amount)
        {
            BatteryLevel = System.Math.Max(0, BatteryLevel - amount);
            return BatteryLevel > 0;
        }

        /// <summary>
        /// Recharges battery to full capacity
        /// </summary>
        public void Recharge()
        {
            BatteryLevel = 100.0;
        }

        /// <summary>
        /// Recharges battery by the specified amount (up to maximum)
        /// </summary>
        public void Recharge(double amount)
        {
            BatteryLevel = System.Math.Min(100.0, BatteryLevel + amount);
        }

        /// <summary>
        /// Updates all sensor readings at once
        /// </summary>
        public void UpdateSensors(double front, double left, double right, double back)
        {
            FrontSensorDistance = front;
            LeftSensorDistance = left;
            RightSensorDistance = right;
            BackSensorDistance = back;
        }

        /// <summary>
        /// Creates a deep copy of the robot state
        /// </summary>
        public RobotState Clone()
        {
            return new RobotState(Position)
            {
                Angle = this.Angle,
                Speed = this.Speed,
                BatteryLevel = this.BatteryLevel,
                FrontSensorDistance = this.FrontSensorDistance,
                LeftSensorDistance = this.LeftSensorDistance,
                RightSensorDistance = this.RightSensorDistance,
                BackSensorDistance = this.BackSensorDistance
            };
        }

        /// <summary>
        /// Resets the robot state to default values
        /// </summary>
        public void Reset(Point startPosition)
        {
            Position = startPosition;
            Angle = 0;
            BatteryLevel = 100.0;
            Speed = 0;
            FrontSensorDistance = 0;
            LeftSensorDistance = 0;
            RightSensorDistance = 0;
            BackSensorDistance = 0;
        }
        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns formatted string for status display
        /// </summary>
        public override string ToString()
        {
            return $"Robot at ({Position.X},{Position.Y}) | Angle: {Angle:F0}° | Speed: {Speed:F1} cm/s | Battery: {BatteryLevel:F1}%";
        }
        #endregion
    }
}