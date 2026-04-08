#region File Header
/// <summary>
/// File: ServiceContainer.cs
/// Description: Simple dependency injection container for services
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Map;
using SallamPathFinder4.Core.Models.Obstacles;
using SallamPathFinder4.Services.Battery;
using SallamPathFinder4.Services.Experiment;
using SallamPathFinder4.Services.File;
using SallamPathFinder4.Services.Obstacle;
using SallamPathFinder4.Services.Pathfinding;
using SallamPathFinder4.Services.Simulation;
#endregion

namespace SallamPathFinder4.WinForms.Container
{
    public static class ServiceContainer
    {
        #region Private Fields
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private static readonly object _lockObject = new object();
        #endregion

        #region Public Methods - Registration
        public static void Register<TInterface>(TInterface implementation) where TInterface : class
        {
            lock (_lockObject)
            {
                _services[typeof(TInterface)] = implementation;
            }
        }

        public static void RegisterFactory<TInterface>(Func<TInterface> factory) where TInterface : class
        {
            lock (_lockObject)
            {
                _services[typeof(TInterface)] = factory;
            }
        }
        #endregion

        #region Public Methods - Resolution
        public static TInterface Resolve<TInterface>() where TInterface : class
        {
            lock (_lockObject)
            {
                if (_services.TryGetValue(typeof(TInterface), out var service))
                {
                    if (service is Func<TInterface> factory)
                    {
                        return factory();
                    }
                    return service as TInterface;
                }
                throw new InvalidOperationException($"Service {typeof(TInterface).Name} not registered");
            }
        }

        public static IPathfindingService CreatePathfindingService(MapGrid grid)
        {
            return new PathfindingService(grid);
        }

        public static ISimulationService CreateSimulationService(MapGrid grid, List<DynamicObstacle> obstacles)
        {
            return new SimulationService(grid, obstacles);
        }

        public static bool TryResolve<TInterface>(out TInterface service) where TInterface : class
        {
            lock (_lockObject)
            {
                if (_services.TryGetValue(typeof(TInterface), out var obj))
                {
                    if (obj is Func<TInterface> factory)
                    {
                        service = factory();
                        return service != null;
                    }
                    service = obj as TInterface;
                    return service != null;
                }
                service = null;
                return false;
            }
        }

        public static bool IsRegistered<TInterface>() where TInterface : class
        {
            lock (_lockObject)
            {
                return _services.ContainsKey(typeof(TInterface));
            }
        }
        #endregion

        #region Public Methods - Initialization
        public static void InitializeServices()
        {
            RegisterFactory<IBatteryService>(() => new BatteryService());
            RegisterFactory<IExperimentService>(() => new ExperimentService());
            RegisterFactory<IFileService>(() => new FileService());
            RegisterFactory<IObstacleSettingsService>(() => new ObstacleSettingsService());
        }

        public static void Clear()
        {
            lock (_lockObject)
            {
                _services.Clear();
            }
        }
        #endregion
    }
}