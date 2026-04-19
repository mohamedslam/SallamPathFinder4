#region File Header
/// <summary>
/// File: ExperimentData.cs
/// Description: Complete data for a single experiment run
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-06
/// </summary>
#endregion

#region Namespace Imports
using System.Drawing;
#endregion

namespace SallamPathFinder4.Core.Models.Experiments
{
    #region Class Documentation
    /// <summary>
    /// Complete data for a single experiment run
    /// Includes map configuration, algorithm parameters, performance metrics, and results
    /// </summary>
    #endregion
    public sealed class ExperimentData
    {
        #region Constructor
        public ExperimentData()
        {
            ExperimentId = Guid.NewGuid().ToString().Substring(0, 8);
            Timestamp = DateTime.UtcNow;
            ObstacleLog = string.Empty;
            CollisionRecords = new List<CollisionRecord>();
            RobotPathReplay = new List<Point>();
            RobotAnglesReplay = new List<float>();
            ReplayTimestamps = new List<DateTime>();
        }
        #endregion

        #region Properties - Identification
        public string ExperimentId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ExperimentName { get; set; }
        public string MapName { get; set; }
        public string MapFilePath { get; set; }
        public string AlgorithmName { get; set; }
        public string DistanceMetric { get; set; }
        public string AlgorithmFormula { get; set; }
        #endregion

        #region Properties - Map Configuration
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public double ScaleCmPerCell { get; set; }
        public double RealAreaCm2 => GridWidth * GridHeight * ScaleCmPerCell * ScaleCmPerCell;
        #endregion

        #region Properties - Obstacle Statistics
        public int FreeCellsCount { get; set; }
        public double FreeCellsPercentage => (double)FreeCellsCount / (GridWidth * GridHeight) * 100;
        public int StaticObstaclesCount { get; set; }
        public double StaticObstaclesPercentage => (double)StaticObstaclesCount / (GridWidth * GridHeight) * 100;
        public int SemiStaticObstaclesCount { get; set; }
        public double SemiStaticObstaclesPercentage => (double)SemiStaticObstaclesCount / (GridWidth * GridHeight) * 100;
        public int DynamicObstaclesCount { get; set; }
        #endregion

        #region Properties - Surface Difficulty
        public double AvgSurfaceWeight { get; set; }
        public double SurfaceWeightStdDev { get; set; }
        #endregion

        #region Properties - Goals and Parking
        public int GoalCount { get; set; }
        public int ParkingCount { get; set; }
        #endregion

        #region Properties - Performance Metrics
        public double SearchTimeMs { get; set; }
        public int PathLengthCells { get; set; }
        public double PathLengthCm => PathLengthCells * ScaleCmPerCell;
        public int ReplanCount { get; set; }
        public double SimulationTimeSec { get; set; }
        public double BatteryConsumption { get; set; }
        public bool Success { get; set; }
        public string FailureReason { get; set; }
        #endregion

        #region Properties - Algorithm Parameters
        public double RobotSpeedCms { get; set; }
        public double HeuristicWeight { get; set; }
        public bool Diagonals { get; set; }
        #endregion

        #region Properties - Robot Settings
        public string RobotName { get; set; } = "SallamBot";
        public double RobotWidthCm { get; set; } = 60;
        public double RobotLengthCm { get; set; } = 60;
        public double RobotHeightCm { get; set; } = 30;
        public double RobotBatteryPercent { get; set; } = 100;
        public double RobotConsumptionRate { get; set; } = 1.0;
        #endregion

        #region Properties - Advanced Performance
        public double PathfindingTimeMs { get; set; }
        public int PathCellsCount { get; set; }
        public double SimulationTotalTimeSec { get; set; }
        public double AverageActualSpeedCmS { get; set; }
        public double TotalDistanceCm { get; set; }
        public double WeightedTimeSec { get; set; }
        public int CollisionCount { get; set; }
        public int InvalidMoveCount { get; set; }
        public bool IsSuccessful { get; set; }
        #endregion

        #region Properties - Machine Learning
        public bool EnableDynamicLearning { get; set; }
        public double LearningRateAlpha { get; set; }
        public string MemoryFilePath { get; set; }
        public bool UseNeuralNetworkPrediction { get; set; }
        public bool CollectTrainingData { get; set; }
        public bool TrainBeforeExperiment { get; set; }
        public int TrainingEpochs { get; set; }
        public double TrainingAccuracy { get; set; }
        public double PredictionConfidence { get; set; }
        #endregion

        #region Properties - Screenshots
        public string InitialScreenshotPath { get; set; }
        public string PathScreenshotPath { get; set; }
        public string CompletedScreenshotPath { get; set; }
        #endregion

        #region Properties - Replay
        public List<Point> RobotPathReplay { get; set; }
        public List<float> RobotAnglesReplay { get; set; }
        public List<DateTime> ReplayTimestamps { get; set; }
        public string ReplayFilePath { get; set; }
        public string GifFilePath { get; set; }
        #endregion

        #region Properties - Logs
        public string ObstacleLog { get; set; }
        public List<CollisionRecord> CollisionRecords { get; set; }
        #endregion

        #region Properties - Iteration Tracking
        /// <summary>نقاط البداية لكل تكرار (JSON)</summary>
        public string StartPointsJson { get; set; }

        /// <summary>نقاط النهاية لكل تكرار (JSON)</summary>
        public string EndPointsJson { get; set; }

        /// <summary>بيانات المسارات الكاملة لكل تكرار (JSON)</summary>
        public string PathsDataJson { get; set; }

        /// <summary>إعدادات الشحن المستخدمة (JSON)</summary>
        public string ChargingSettingsJson { get; set; }
        #endregion

        #region Properties - Dynamic Charging Settings
        public bool EnableDynamicCharging { get; set; }
        public int ChargingTimeSeconds { get; set; }
        public double SafetyMarginPercent { get; set; }
        #endregion

        #region Properties - Algorithm Settings
        public bool OrderGoalsByDistance { get; set; }
        #endregion

        #region Public Methods
        public static string GetCsvHeader()
        {
            return "ExperimentId,Timestamp,ExperimentName,MapName,Algorithm,DistanceMetric,AlgorithmFormula," +
                   "GridWidth,GridHeight,ScaleCmPerCell,RealAreaCm2," +
                   "FreeCellsCount,FreeCellsPercentage," +
                   "StaticObstaclesCount,StaticObstaclesPercentage," +
                   "SemiStaticObstaclesCount,SemiStaticObstaclesPercentage," +
                   "DynamicObstaclesCount," +
                   "AvgSurfaceWeight,SurfaceWeightStdDev," +
                   "GoalCount,ParkingCount," +
                   "SearchTimeMs,PathLengthCells,PathLengthCm,ReplanCount," +
                   "SimulationTimeSec,BatteryConsumption,Success,FailureReason," +
                   "RobotSpeedCms,HeuristicWeight,Diagonals," +
                   "RobotName,RobotWidthCm,RobotLengthCm,RobotHeightCm,RobotBatteryPercent,RobotConsumptionRate," +
                   "PathfindingTimeMs,PathCellsCount,SimulationTotalTimeSec,AverageActualSpeedCmS,TotalDistanceCm,WeightedTimeSec," +
                   "CollisionCount,InvalidMoveCount,IsSuccessful," +
                   "EnableDynamicLearning,LearningRateAlpha,UseNeuralNetworkPrediction,CollectTrainingData,PredictionConfidence," +
                   "InitialScreenshotPath,PathScreenshotPath,CompletedScreenshotPath,ReplayFilePath,GifFilePath," +
                   "ObstacleLog";
        }

        public string ToCsvLine()
        {
            return $"{ExperimentId},{Timestamp:yyyy-MM-dd HH:mm:ss},{ExperimentName},{MapName},{AlgorithmName},{DistanceMetric},{AlgorithmFormula}," +
                   $"{GridWidth},{GridHeight},{ScaleCmPerCell:F2},{RealAreaCm2:F2}," +
                   $"{FreeCellsCount},{FreeCellsPercentage:F2}," +
                   $"{StaticObstaclesCount},{StaticObstaclesPercentage:F2}," +
                   $"{SemiStaticObstaclesCount},{SemiStaticObstaclesPercentage:F2}," +
                   $"{DynamicObstaclesCount}," +
                   $"{AvgSurfaceWeight:F2},{SurfaceWeightStdDev:F2}," +
                   $"{GoalCount},{ParkingCount}," +
                   $"{SearchTimeMs:F3},{PathLengthCells},{PathLengthCm:F2},{ReplanCount}," +
                   $"{SimulationTimeSec:F3},{BatteryConsumption:F2},{Success},{FailureReason}," +
                   $"{RobotSpeedCms:F2},{HeuristicWeight:F2},{Diagonals}," +
                   $"{RobotName},{RobotWidthCm:F1},{RobotLengthCm:F1},{RobotHeightCm:F1},{RobotBatteryPercent:F1},{RobotConsumptionRate:F1}," +
                   $"{PathfindingTimeMs:F3},{PathCellsCount},{SimulationTotalTimeSec:F3},{AverageActualSpeedCmS:F2},{TotalDistanceCm:F2},{WeightedTimeSec:F3}," +
                   $"{CollisionCount},{InvalidMoveCount},{IsSuccessful}," +
                   $"{EnableDynamicLearning},{LearningRateAlpha:F2},{UseNeuralNetworkPrediction},{CollectTrainingData},{PredictionConfidence:F2}," +
                   $"\"{InitialScreenshotPath}\",\"{PathScreenshotPath}\",\"{CompletedScreenshotPath}\",\"{ReplayFilePath}\",\"{GifFilePath}\"," +
                   $"\"{ObstacleLog}\"";
        }
        #endregion
    }
}