#region File Header
/// <summary>
/// File: SimulationServiceFactory.cs
/// Description: Factory for creating SimulationService instances
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System.Collections.Generic;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
#endregion

namespace SallamPathFinder4.Services.Simulation
{
    public sealed class SimulationServiceFactory : ISimulationServiceFactory
    {
        public ISimulationService Create(MapGrid grid, List<DynamicObstacle> dynamicObstacles)
        {
            return new SimulationService(grid, dynamicObstacles);
        }
    }
}