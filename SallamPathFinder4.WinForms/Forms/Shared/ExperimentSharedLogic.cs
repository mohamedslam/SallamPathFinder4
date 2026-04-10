#region File Header
/// <summary>
/// File: ExperimentSharedLogic.cs
/// Description: Shared business logic for all experiment-related forms
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-07
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Enums;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Shared
{
    /// <summary>
    /// Provides shared business logic for experiment operations
    /// </summary>
    public static class ExperimentSharedLogic
    {
        #region Algorithm Operations
        /// <summary>
        /// Converts algorithm name string to AlgorithmType enum
        /// </summary>
        public static AlgorithmType GetAlgorithmType(string algorithmName)
        {
            return algorithmName switch
            {
                "AStar" => AlgorithmType.AStar,
                "SPPA" => AlgorithmType.SPPA,
                "SPPA_DL" => AlgorithmType.SPPA_DL,
                "ACO" => AlgorithmType.ACO,
                "DStar" => AlgorithmType.DStar,
                "KNN" => AlgorithmType.KNN,
                "BruteForce" => AlgorithmType.BruteForce,
                _ => AlgorithmType.AStar
            };
        }

        /// <summary>
        /// Gets the display name of an algorithm
        /// </summary>
        public static string GetAlgorithmDisplayName(AlgorithmType type)
        {
            return type switch
            {
                AlgorithmType.AStar => "A* (A-Star)",
                AlgorithmType.SPPA => "SPPA",
                AlgorithmType.SPPA_DL => "SPPA-DL",
                AlgorithmType.ACO => "Ant Colony Optimization",
                AlgorithmType.DStar => "D* (Dynamic A*)",
                AlgorithmType.KNN => "K-Nearest Neighbors",
                AlgorithmType.BruteForce => "Brute Force",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Gets the mathematical formula for an algorithm
        /// </summary>
        public static string GetAlgorithmFormula(AlgorithmType type)
        {
            return type switch
            {
                AlgorithmType.AStar => "f(n) = g(n) + h(n)",
                AlgorithmType.SPPA => "f(n) = g(n) + h(n) + λ·o(n)",
                AlgorithmType.SPPA_DL => "f(n) = g(n) + h(n) + λ·o(n) + α·m(n) + β·p(n)",
                AlgorithmType.ACO => "P_ij = [τ_ij]^α · [η_ij]^β / Σ[τ_il]^α · [η_il]^β",
                AlgorithmType.DStar => "f(n) = g(n) + h(n) + d(n)",
                AlgorithmType.KNN => "d(x,y) = √Σ(x_i - y_i)²",
                AlgorithmType.BruteForce => "min Σ cost(path)",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Gets the parameter description for an algorithm
        /// </summary>
        public static string GetAlgorithmParameterDescription(AlgorithmType type)
        {
            return type switch
            {
                AlgorithmType.AStar => "g(n): Path Cost, h(n): Heuristic Cost",
                AlgorithmType.SPPA => "g(n): Path Cost, h(n): Heuristic Cost, λ: Obstacle Coefficient, o(n): Obstacle Factor",
                AlgorithmType.SPPA_DL => "g(n): Path Cost, h(n): Heuristic Cost, λ: Obstacle Coefficient, o(n): Obstacle Factor, α: Learning Rate, m(n): Memory Coefficient, β: Prediction Weight, p(n): Prediction Risk",
                AlgorithmType.ACO => "τ_ij: Pheromone, η_ij: Visibility (1/distance), α: Pheromone Influence, β: Heuristic Influence",
                AlgorithmType.DStar => "g(n): Path Cost, h(n): Heuristic Cost, d(n): Dynamic Obstacle Cost",
                AlgorithmType.KNN => "Euclidean distance between points",
                AlgorithmType.BruteForce => "Exhaustive search for minimum cost path",
                _ => "Unknown"
            };
        }
        #endregion

        #region Distance Metric Operations
        /// <summary>
        /// Converts metric name string to DistanceMetric enum
        /// </summary>
        public static DistanceMetric GetDistanceMetric(string metricName)
        {
            return metricName switch
            {
                "Manhattan" => DistanceMetric.Manhattan,
                "Euclidean" => DistanceMetric.Euclidean,
                "MaxDXDY" => DistanceMetric.MaxDXDY,
                "DiagonalShortcut" => DistanceMetric.DiagonalShortcut,
                "EuclideanNoSQR" => DistanceMetric.EuclideanNoSQR,
                "Custom" => DistanceMetric.Custom,
                _ => DistanceMetric.Manhattan
            };
        }

        /// <summary>
        /// Gets the formula for a distance metric
        /// </summary>
        public static string GetDistanceMetricFormula(string metricName)
        {
            return metricName switch
            {
                "Manhattan" => "|dx| + |dy|",
                "Euclidean" => "√(dx² + dy²)",
                "MaxDXDY" => "max(|dx|, |dy|)",
                "DiagonalShortcut" => "2·min(dx,dy) + |dx-dy|",
                "EuclideanNoSQR" => "dx² + dy²",
                "Custom" => "Custom formula",
                _ => "Unknown"
            };
        }
        #endregion

        #region Validation
        /// <summary>
        /// Validates search limit value
        /// </summary>
        public static bool IsValidSearchLimit(int value, out string warning)
        {
            warning = null;

            if (value < 1000)
            {
                warning = "Search limit is very low. May not find a path.";
                return true;
            }

            if (value > 100000)
            {
                warning = "High search limit may cause long computation time (up to several minutes).";
                return true;
            }

            return true;
        }

        /// <summary>
        /// Validates heuristic weight value
        /// </summary>
        public static bool IsValidHeuristicWeight(int value, out string warning)
        {
            warning = null;

            if (value > 5)
            {
                warning = "High heuristic weight may produce suboptimal (non-shortest) paths.";
                return true;
            }

            return true;
        }

        /// <summary>
        /// Validates ACO parameters
        /// </summary>
        public static bool AreValidACOParameters(int ants, int iterations, out string warning)
        {
            warning = null;

            if (ants > 100)
            {
                warning = "Too many ants may slow down the search significantly.";
                return true;
            }

            if (iterations > 300)
            {
                warning = "High iteration count may take very long time to complete.";
                return true;
            }

            return true;
        }

        /// <summary>
        /// Validates KNN parameters
        /// </summary>
        public static bool IsValidKNNRadius(int radius, out string warning)
        {
            warning = null;

            if (radius > 10)
            {
                warning = "Large search radius may explore too many cells and slow down the search.";
                return true;
            }

            return true;
        }

        /// <summary>
        /// Validates Brute Force parameters
        /// </summary>
        public static bool AreValidBruteForceParameters(int maxDepth, int maxIterations, out string warning)
        {
            warning = null;

            if (maxDepth > 20000)
            {
                warning = "Very deep search may cause timeout.";
                return true;
            }

            if (maxIterations > 200000)
            {
                warning = "Too many iterations may freeze the application.";
                return true;
            }

            return true;
        }
        #endregion

        #region File Operations
        /// <summary>
        /// Gets a safe file name by removing invalid characters
        /// </summary>
        public static string GetSafeFileName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Experiment";

            char[] invalidChars = Path.GetInvalidFileNameChars();
            return new string(name.Where(c => !invalidChars.Contains(c)).ToArray());
        }

        /// <summary>
        /// Creates a timestamped experiment ID
        /// </summary>
        public static string CreateExperimentId(string prefix = "Exp")
        {
            return $"{prefix}_{DateTime.Now:yyyyMMdd_HHmmss}";
        }

        /// <summary>
        /// Formats a timestamp for display
        /// </summary>
        public static string FormatTimestamp(DateTime timestamp)
        {
            return timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Formats a timestamp for file names
        /// </summary>
        public static string FormatTimestampForFile(DateTime timestamp)
        {
            return timestamp.ToString("yyyyMMdd_HHmmss");
        }
        #endregion

        #region Path Calculation
        /// <summary>
        /// Calculates estimated battery consumption for a path
        /// </summary>
        public static double CalculateBatteryConsumption(int pathLength, double consumptionRate, double baseBattery)
        {
            double consumption = pathLength * consumptionRate;
            return Math.Max(0, baseBattery - consumption);
        }

        /// <summary>
        /// Calculates average speed considering surface weights
        /// </summary>
        public static double CalculateAverageSpeed(double baseSpeed, double averageSurfaceWeight)
        {
            double speedFactor = 1.0 - averageSurfaceWeight / 100.0;
            return baseSpeed * Math.Max(0.1, speedFactor);
        }

        /// <summary>
        /// Estimates time to complete a path
        /// </summary>
        public static double EstimatePathTime(int pathLength, double averageSpeed)
        {
            if (averageSpeed <= 0) return 0;
            return pathLength / averageSpeed;
        }
        #endregion

        #region Statistics
        /// <summary>
        /// Calculates standard deviation of a list of values
        /// </summary>
        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            var list = values.ToList();
            if (list.Count == 0) return 0;

            double avg = list.Average();
            double sumOfSquares = list.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sumOfSquares / list.Count);
        }

        /// <summary>
        /// Calculates median of a list of values
        /// </summary>
        public static double CalculateMedian(IEnumerable<double> values)
        {
            var sorted = values.OrderBy(v => v).ToList();
            if (sorted.Count == 0) return 0;

            int mid = sorted.Count / 2;
            return sorted.Count % 2 == 0 ? (sorted[mid - 1] + sorted[mid]) / 2 : sorted[mid];
        }

        /// <summary>
        /// Calculates percentiles of a list of values
        /// </summary>
        public static double CalculatePercentile(IEnumerable<double> values, double percentile)
        {
            var sorted = values.OrderBy(v => v).ToList();
            if (sorted.Count == 0) return 0;

            double index = percentile / 100.0 * (sorted.Count - 1);
            int lowerIndex = (int)Math.Floor(index);
            int upperIndex = (int)Math.Ceiling(index);

            if (lowerIndex == upperIndex)
                return sorted[lowerIndex];

            double lowerValue = sorted[lowerIndex];
            double upperValue = sorted[upperIndex];
            double fraction = index - lowerIndex;

            return lowerValue + (upperValue - lowerValue) * fraction;
        }
        #endregion
    }
}