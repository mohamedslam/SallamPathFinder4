#region File Header
/// <summary>
/// File: TerrainType.cs
/// Description: Defines terrain types that affect pathfinding cost
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-05-12
/// </summary>
#endregion

namespace SallamPathFinder4.Core.Models.Map
{
    /// <summary>
    /// Terrain types that affect movement cost
    /// </summary>
    public enum TerrainType
    {
        /// <summary>
        /// Normal terrain with standard movement cost (1.0)
        /// </summary>
        Normal = 0,

        /// <summary>
        /// Rough terrain with increased movement cost (1.5 - 2.0)
        /// </summary>
        Rough = 1,

        /// <summary>
        /// Wet/slippery terrain with moderate increased cost (1.3 - 1.6)
        /// </summary>
        Wet = 2,

        /// <summary>
        /// Sandy terrain with significant increased cost (1.4 - 1.8)
        /// </summary>
        Sandy = 3,

        /// <summary>
        /// Muddy terrain with high movement cost (1.6 - 2.2)
        /// </summary>
        Muddy = 4,

        /// <summary>
        /// Rocky terrain with very high movement cost (1.8 - 2.5)
        /// </summary>
        Rocky = 5,

        /// <summary>
        /// Vegetation/dense grass with moderate increased cost (1.2 - 1.5)
        /// </summary>
        Vegetation = 6
    }
}