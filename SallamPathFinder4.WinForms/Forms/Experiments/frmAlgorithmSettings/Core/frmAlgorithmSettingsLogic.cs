#region File Header
/// <summary>
/// File: frmAlgorithmSettingsLogic.cs
/// Description: Business logic for algorithm settings form
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-08
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SallamPathFinder4.Core.Models.Algorithms;
#endregion

namespace SallamPathFinder4.WinForms.Forms.Experiments.frmAlgorithmSettings
{
    #region Class Documentation
    /// <summary>
    /// Business logic for algorithm settings form
    /// Handles parameter loading, validation, and formatting
    /// </summary>
    #endregion
    public sealed class frmAlgorithmSettingsLogic
    {
        #region Private Fields
        private readonly string _algorithmName;
        private readonly List<AlgorithmParameterDefinition> _parameterDefinitions;
        private Dictionary<string, object> _currentValues;
        private Dictionary<string, object> _modifiedValues;
        #endregion

        #region Constructor
        public frmAlgorithmSettingsLogic(string algorithmName, Dictionary<string, object> currentValues = null)
        {
            _algorithmName = algorithmName;
            _parameterDefinitions = AlgorithmParametersRegistry.GetParameters(algorithmName);
            _currentValues = currentValues ?? AlgorithmParametersRegistry.GetDefaultParameters(algorithmName);
            _modifiedValues = new Dictionary<string, object>();
        }
        #endregion

        #region Properties
        public Dictionary<string, object> ModifiedValues => _modifiedValues;
        public List<AlgorithmParameterDefinition> ParameterDefinitions => _parameterDefinitions;
        public string AlgorithmFormula => AlgorithmParametersRegistry.GetAlgorithmFormula(_algorithmName);
        public string AlgorithmDescription => AlgorithmParametersRegistry.GetAlgorithmDescription(_algorithmName);
        public string AlgorithmDisplayName => GetDisplayName();
        public string ModifiedMetric { get; private set; } = null;

        #endregion

        #region Public Methods - Parameter Management
        /// <summary>
        /// Gets the current value for a parameter
        /// </summary>
        public object GetCurrentValue(string parameterName)
        {
            return _currentValues.GetValueOrDefault(parameterName);
        }

        /// <summary>
        /// Sets a modified value for a parameter
        /// </summary>
        public void SetModifiedValue(string parameterName, object value)
        {
            _modifiedValues[parameterName] = value;
        }

        /// <summary>
        /// Resets all parameters to default values
        /// </summary>
        public void ResetToDefaults()
        {
            _modifiedValues.Clear();
            _currentValues = AlgorithmParametersRegistry.GetDefaultParameters(_algorithmName);
        }

        /// <summary>
        /// Applies all modified values to current values
        /// </summary>
        public void ApplyChanges()
        {
            foreach (var kvp in _modifiedValues)
            {
                _currentValues[kvp.Key] = kvp.Value;
            }
           // _modifiedValues.Clear();
        }
        public Dictionary<string, object> GetModifiedValuesAndClear()
        {
            var result = new Dictionary<string, object>(_modifiedValues);
            _modifiedValues.Clear();
            return result;
        }
        /// <summary>
        /// Gets the parameter definition by name
        /// </summary>
        public AlgorithmParameterDefinition GetParameterDefinition(string parameterName)
        {
            return _parameterDefinitions.FirstOrDefault(p => p.Name == parameterName);
        }
        #endregion

        #region Public Methods - Display Formatting
        /// <summary>
        /// Formats a value for display in the grid
        /// </summary>
        public string FormatValueForDisplay(object value, AlgorithmParameterDefinition param)
        {
            if (value == null) return "";

            if (param.Type == ParameterType.Boolean)
                return ((bool)value) ? "✓ Yes" : "☐ No";

            if (param.Type == ParameterType.Double)
                return ((double)value).ToString("F3");

            return value.ToString();
        }

        /// <summary>
        /// Parses a display value back to its original type
        /// </summary>
        public object ParseDisplayValue(string displayValue, AlgorithmParameterDefinition param)
        {
            if (string.IsNullOrEmpty(displayValue))
                return param.DefaultValue;

            if (param.Type == ParameterType.Boolean)
            {
                if (displayValue == "✓ Yes")
                    return true;
                if (displayValue == "☐ No")
                    return false;
                if (bool.TryParse(displayValue, out bool boolResult))
                    return boolResult;
                return false;
            }

            if (param.Type == ParameterType.Integer)
                return int.TryParse(displayValue, out int intVal) ? intVal : param.DefaultValue;

            if (param.Type == ParameterType.Double)
                return double.TryParse(displayValue, out double dblVal) ? dblVal : param.DefaultValue;

            return displayValue;
        }

        /// <summary>
        /// Gets the range text for display
        /// </summary>
        public string GetRangeText(AlgorithmParameterDefinition param)
        {
            if (param.Type == ParameterType.Boolean)
                return "True / False";

            if (param.Type == ParameterType.Enum)
                return string.Join(" | ", param.EnumOptions);

            if (param.MinValue != null && param.MaxValue != null)
                return $"{param.MinValue} - {param.MaxValue}";

            if (param.MinValue != null)
                return $">= {param.MinValue}";

            if (param.MaxValue != null)
                return $"<= {param.MaxValue}";

            return "-";
        }

        /// <summary>
        /// Gets the type text for display
        /// </summary>
        public string GetTypeText(AlgorithmParameterDefinition param)
        {
            return param.Type switch
            {
                ParameterType.Integer => "Integer",
                ParameterType.Double => "Decimal",
                ParameterType.Boolean => "Boolean",
                ParameterType.Enum => "Selection",
                ParameterType.String => "Text",
                _ => "Unknown"
            };
        }
        #endregion

        #region Public Methods - Validation
        /// <summary>
        /// Validates a value against parameter constraints
        /// </summary>
        public bool ValidateValue(string value, AlgorithmParameterDefinition param, out string errorMessage)
        {
            errorMessage = null;

            if (string.IsNullOrEmpty(value))
            {
                errorMessage = $"Value cannot be empty for {param.DisplayName}.";
                return false;
            }

            try
            {
                switch (param.Type)
                {
                    case ParameterType.Integer:
                        if (!int.TryParse(value, out int intValue))
                        {
                            errorMessage = $"Value must be an integer for {param.DisplayName}.";
                            return false;
                        }
                        if (!param.IsValid(intValue))
                        {
                            errorMessage = $"Value must be between {param.MinValue} and {param.MaxValue} for {param.DisplayName}.";
                            return false;
                        }
                        break;

                    case ParameterType.Double:
                        if (!double.TryParse(value, out double dblValue))
                        {
                            errorMessage = $"Value must be a number for {param.DisplayName}.";
                            return false;
                        }
                        if (!param.IsValid(dblValue))
                        {
                            errorMessage = $"Value must be between {param.MinValue} and {param.MaxValue} for {param.DisplayName}.";
                            return false;
                        }
                        break;

                    case ParameterType.Boolean:
                    case ParameterType.Enum:
                    case ParameterType.String:
                        // No additional validation needed
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Validation error: {ex.Message}";
                return false;
            }
        }
        #endregion

        #region Private Methods
        private string GetDisplayName()
        {
            return _algorithmName switch
            {
                "AStar" => "A* (A-Star)",
                "SPPA" => "SPPA (Shortest Path with Precautionary Avoidance)",
                "SPPA_DL" => "SPPA-DL (with Dynamic Learning)",
                "ACO" => "ACO (Ant Colony Optimization)",
                "DStar" => "D* (Dynamic A*)",
                "KNN" => "KNN (K-Nearest Neighbors)",
                "BruteForce" => "Brute Force Search",
                "RRT" => "RRT (Rapidly-exploring Random Tree)",
                "PRM" => "PRM (Probabilistic Roadmap)",
                "PSO" => "PSO (Particle Swarm Optimization)",
                "GA" => "GA (Genetic Algorithm)",
                "RRTStar" => "RRT* (RRT-Star)",
                _ => _algorithmName
            };
        }
        #endregion
        public object GetDefaultValue(string parameterName)
        {
            var param = _parameterDefinitions.FirstOrDefault(p => p.Name == parameterName);
            return param?.DefaultValue;
        }
    }
}