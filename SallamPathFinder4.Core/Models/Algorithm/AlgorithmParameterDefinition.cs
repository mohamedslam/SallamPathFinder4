#region File Header
/// <summary>
/// File: AlgorithmParameterDefinition.cs
/// Description: Defines the structure for configurable algorithm parameters
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-08
/// </summary>
#endregion

#region Namespace Imports
#endregion

namespace SallamPathFinder4.Core.Models.Algorithms
{
    #region Enums
    /// <summary>
    /// Type of parameter value
    /// </summary>
    public enum ParameterType
    {
        Integer,
        Double,
        Boolean,
        Enum,
        String
    }
    #endregion

    #region Class Documentation
    /// <summary>
    /// Defines a configurable parameter for an algorithm
    /// Used for dynamic UI generation and parameter validation
    /// </summary>
    #endregion
    public class AlgorithmParameterDefinition
    {
        #region Properties
        /// <summary>
        /// Unique internal name (used in code)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name shown to user
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Type of parameter (Integer, Double, Boolean, Enum, String)
        /// </summary>
        public ParameterType Type { get; set; }

        /// <summary>
        /// Default value for this parameter
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Minimum allowed value (for numeric types)
        /// </summary>
        public object MinValue { get; set; }

        /// <summary>
        /// Maximum allowed value (for numeric types)
        /// </summary>
        public object MaxValue { get; set; }

        /// <summary>
        /// Unit of measurement (e.g., "%", "cells", "ms")
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// User-friendly description of the parameter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Mathematical formula or explanation
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Available options for Enum types
        /// </summary>
        public List<string> EnumOptions { get; set; }

        public bool OrderGoalsByDistance { get; set; } = false;
        #endregion

        #region Constructor
        public AlgorithmParameterDefinition()
        {
            EnumOptions = new List<string>();
            Unit = string.Empty;
            Description = string.Empty;
            Formula = string.Empty;
        }
        #endregion

        #region Clone Method
        /// <summary>
        /// Creates a deep copy of this parameter definition
        /// </summary>
        public AlgorithmParameterDefinition Clone()
        {
            return new AlgorithmParameterDefinition
            {
                Name = this.Name,
                DisplayName = this.DisplayName,
                Type = this.Type,
                DefaultValue = this.DefaultValue,
                MinValue = this.MinValue,
                MaxValue = this.MaxValue,
                Unit = this.Unit,
                Description = this.Description,
                Formula = this.Formula,
                EnumOptions = new List<string>(this.EnumOptions)
            };
        }
        #endregion

        #region Validation Method
        /// <summary>
        /// Validates if a value is within the allowed range
        /// </summary>
        public bool IsValid(object value)
        {
            if (value == null) return false;

            try
            {
                switch (Type)
                {
                    case ParameterType.Integer:
                        int intValue = Convert.ToInt32(value);
                        int minInt = MinValue != null ? Convert.ToInt32(MinValue) : int.MinValue;
                        int maxInt = MaxValue != null ? Convert.ToInt32(MaxValue) : int.MaxValue;
                        return intValue >= minInt && intValue <= maxInt;

                    case ParameterType.Double:
                        double dblValue = Convert.ToDouble(value);
                        double minDbl = MinValue != null ? Convert.ToDouble(MinValue) : double.MinValue;
                        double maxDbl = MaxValue != null ? Convert.ToDouble(MaxValue) : double.MaxValue;
                        return dblValue >= minDbl && dblValue <= maxDbl;

                    case ParameterType.Boolean:
                        return true;

                    case ParameterType.Enum:
                        return EnumOptions.Contains(value?.ToString() ?? string.Empty);

                    case ParameterType.String:
                        return true;

                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}