#region File Header
/// <summary>
/// File: ISimulationServiceFactory.cs
/// Description: Factory interface for creating SimulationService with dynamic obstacles
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
#endregion

namespace SallamPathFinder4.Core.Interfaces.Services
{
    public interface ISimulationServiceFactory
    {
        ISimulationService Create(MapGrid grid, List<DynamicObstacle> dynamicObstacles);
    }
}