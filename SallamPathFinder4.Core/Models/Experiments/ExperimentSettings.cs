#region File Header
/// <summary>
/// File: ExperimentSettings.cs
/// Description: Settings for experiment configuration
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Drawing;
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.Core.Models.Experiments
{
    #region Class Documentation
    /// <summary>
    /// Complete settings for an experiment
    /// </summary>
    #endregion
    public sealed class ExperimentSettings
    {
        #region Constructor
        public ExperimentSettings()
        {
            SelectedAlgorithms = new List<string>();
            SelectedMetrics = new List<string>();
            MLSettings = new MLSettings();
            RobotSettings = new RobotSettings();
            CustomStartPoint = new Point(10, 10);
        }
        #endregion 

        #region Properties - Basic
        public string ExperimentName { get; set; }
        public List<string> SelectedAlgorithms { get; set; }
        public List<string> SelectedMetrics { get; set; }         
        public int Iterations { get; set; }
        public bool SaveScreenshots { get; set; }
        public bool SaveReplay { get; set; }
        public bool ShowPathOnScreenshots { get; set; }
        public string SavePath { get; set; } 
  
        #endregion

        #region Properties - Map
        public int GoalCount { get; set; }
        public int ParkingCount { get; set; }
        public int StaticObstacles { get; set; }
        public int DynamicObstacles { get; set; }
        #endregion

        #region Properties - Algorithm
        public int HeuristicWeight { get; set; }
        public int SearchLimit { get; set; }
        public bool AllowDiagonals { get; set; }
        public bool HeavyDiagonals { get; set; }
        public string DistanceMetric { get; set; } = "Manhattan";
        public bool OrderGoalsByDistance { get; set; }
        #endregion

        #region Properties - Start Point
        public bool UseCustomStartPoint { get; set; }
        public Point CustomStartPoint { get; set; }
        #endregion

        #region Properties - Dynamic Charging
        public bool EnableDynamicCharging { get; set; }
        public int ChargingTimeSeconds { get; set; } = 15;
        public double SafetyMarginPercent { get; set; } = 10.0;
        #endregion

        #region Properties - Robot & ML
        public RobotSettings RobotSettings { get; set; }
        public MLSettings MLSettings { get; set; }
        #endregion
    }

    #region Class Documentation
    /// <summary>
    /// Machine learning settings for SPPA-DL
    /// </summary>
    #endregion
    public sealed class MLSettings
    {
        public bool EnableDynamicLearning { get; set; } = true;
        public double LearningRate { get; set; } = 2.0;
        public bool UseNeuralNetwork { get; set; } = false;
        public bool CollectTrainingData { get; set; } = false;
        public bool TrainBeforeExperiment { get; set; } = false;
        public double PredictionWeight { get; set; } = 0.3;
    }
}