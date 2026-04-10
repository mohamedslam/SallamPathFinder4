#region File Header
/// <summary>
/// File: ObservableRobotState.cs
/// Description: Observable robot state for binding to UI
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace SallamPathFinder4.WinForms.ViewModels
{
    public sealed class ObservableRobotState : INotifyPropertyChanged
    {
        #region Private Fields
        private Point _position;
        private float _angle;
        private double _batteryLevel;
        private double _speed;
        private double _frontSensor;
        private double _leftSensor;
        private double _rightSensor;
        private double _backSensor;
        #endregion

        #region Properties
        public Point Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PositionText));
            }
        }

        public float Angle
        {
            get => _angle;
            set
            {
                _angle = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AngleText));
            }
        }

        public double BatteryLevel
        {
            get => _batteryLevel;
            set
            {
                _batteryLevel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BatteryPercentage));
                OnPropertyChanged(nameof(BatteryStatus));
                OnPropertyChanged(nameof(BatteryStatusColor));
            }
        }

        public double Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SpeedText));
            }
        }

        public double FrontSensor
        {
            get => _frontSensor;
            set { _frontSensor = value; OnPropertyChanged(); }
        }

        public double LeftSensor
        {
            get => _leftSensor;
            set { _leftSensor = value; OnPropertyChanged(); }
        }

        public double RightSensor
        {
            get => _rightSensor;
            set { _rightSensor = value; OnPropertyChanged(); }
        }

        public double BackSensor
        {
            get => _backSensor;
            set { _backSensor = value; OnPropertyChanged(); }
        }
        #endregion

        #region Derived Properties
        public string PositionText => $"({_position.X}, {_position.Y})";
        public string AngleText => $"{_angle:F0}°";
        public string BatteryPercentage => $"{_batteryLevel:F1}%";
        public string SpeedText => $"{_speed:F1} cm/s";

        public string BatteryStatus
        {
            get
            {
                if (_batteryLevel <= 0) return "EMPTY";
                if (_batteryLevel < 10) return "CRITICAL";
                if (_batteryLevel < 20) return "LOW";
                return "NORMAL";
            }
        }

        public Color BatteryStatusColor
        {
            get
            {
                if (_batteryLevel <= 0) return Color.FromArgb(231, 76, 60);
                if (_batteryLevel < 10) return Color.FromArgb(231, 76, 60);
                if (_batteryLevel < 20) return Color.FromArgb(241, 196, 15);
                return Color.FromArgb(46, 204, 113);
            }
        }
        #endregion

        #region Public Methods
        public void UpdateSensors(double front, double left, double right, double back)
        {
            FrontSensor = front;
            LeftSensor = left;
            RightSensor = right;
            BackSensor = back;
        }

        public void Reset(Point startPosition)
        {
            Position = startPosition;
            Angle = 0;
            BatteryLevel = 100;
            Speed = 0;
            FrontSensor = 0;
            LeftSensor = 0;
            RightSensor = 0;
            BackSensor = 0;
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}