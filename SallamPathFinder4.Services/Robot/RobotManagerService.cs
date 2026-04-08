#region File Header
/// <summary>
/// File: RobotManagerService.cs
/// Description: Manages multiple robot profiles and active robot selection
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.Services.Robot
{
    #region Robot Profile Class
    /// <summary>
    /// Complete robot profile including settings and state
    /// </summary>
    public sealed class RobotProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
        public string Name { get; set; } = "New Robot";
        public RobotSettings Settings { get; set; } = new RobotSettings();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUsed { get; set; } = DateTime.UtcNow;
        public bool IsFavorite { get; set; } = false;
        public string Description { get; set; } = string.Empty;
    }
    #endregion

    #region Class Documentation
    /// <summary>
    /// Service for managing multiple robot profiles
    /// Supports create, read, update, delete operations
    /// Persists profiles to JSON file
    /// </summary>
    #endregion
    public sealed class RobotManagerService : IDisposable
    {
        #region Constants
        private const string PROFILES_FILE_NAME = "RobotProfiles.json";
        private const string DEFAULT_PROFILE_NAME = "Default Robot";
        #endregion

        #region Private Fields
        private readonly string _profilesPath;
        private List<RobotProfile> _profiles;
        private RobotProfile _activeProfile;
        private readonly object _lockObject = new object();
        private bool _isDisposed;
        #endregion

        #region Constructor
        public RobotManagerService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appFolder = Path.Combine(appDataPath, "SallamPathFinder4");

            if (!Directory.Exists(appFolder))
                Directory.CreateDirectory(appFolder);

            _profilesPath = Path.Combine(appFolder, PROFILES_FILE_NAME);
            _profiles = new List<RobotProfile>();

            LoadProfilesAsync().Wait();
        }
        #endregion

        #region Properties
        public IReadOnlyList<RobotProfile> Profiles
        {
            get
            {
                lock (_lockObject)
                {
                    return _profiles.AsReadOnly();
                }
            }
        }

        public RobotProfile ActiveProfile
        {
            get
            {
                lock (_lockObject)
                {
                    return _activeProfile;
                }
            }
        }

        public bool HasActiveProfile => _activeProfile != null;
        #endregion

        #region Public Methods - CRUD
        /// <summary>
        /// Creates a new robot profile
        /// </summary>
        public RobotProfile CreateProfile(string name, RobotSettings settings = null)
        {
            lock (_lockObject)
            {
                var profile = new RobotProfile
                {
                    Name = name,
                    Settings = settings?.Clone() ?? new RobotSettings(),
                    CreatedAt = DateTime.UtcNow,
                    LastUsed = DateTime.UtcNow
                };

                _profiles.Add(profile);
                SaveProfilesAsync().Wait();

                return profile;
            }
        }

        /// <summary>
        /// Updates an existing robot profile
        /// </summary>
        public bool UpdateProfile(string profileId, RobotSettings newSettings)
        {
            lock (_lockObject)
            {
                var profile = _profiles.FirstOrDefault(p => p.Id == profileId);
                if (profile == null) return false;

                profile.Settings = newSettings.Clone();
                profile.LastUsed = DateTime.UtcNow;

                SaveProfilesAsync().Wait();
                return true;
            }
        }

        /// <summary>
        /// Deletes a robot profile
        /// </summary>
        public bool DeleteProfile(string profileId)
        {
            lock (_lockObject)
            {
                var profile = _profiles.FirstOrDefault(p => p.Id == profileId);
                if (profile == null) return false;

                if (_activeProfile?.Id == profileId)
                    _activeProfile = null;

                _profiles.Remove(profile);
                SaveProfilesAsync().Wait();

                return true;
            }
        }

        /// <summary>
        /// Duplicates an existing profile
        /// </summary>
        public RobotProfile DuplicateProfile(string profileId, string newName)
        {
            lock (_lockObject)
            {
                var original = _profiles.FirstOrDefault(p => p.Id == profileId);
                if (original == null) return null;

                var clone = new RobotProfile
                {
                    Name = newName,
                    Settings = original.Settings.Clone(),
                    CreatedAt = DateTime.UtcNow,
                    LastUsed = DateTime.UtcNow,
                    IsFavorite = original.IsFavorite
                };

                _profiles.Add(clone);
                SaveProfilesAsync().Wait();

                return clone;
            }
        }
        #endregion

        #region Public Methods - Active Profile
        /// <summary>
        /// Sets the active robot profile
        /// </summary>
        public bool SetActiveProfile(string profileId)
        {
            lock (_lockObject)
            {
                var profile = _profiles.FirstOrDefault(p => p.Id == profileId);
                if (profile == null) return false;

                _activeProfile = profile;
                profile.LastUsed = DateTime.UtcNow;
                SaveProfilesAsync().Wait();

                return true;
            }
        }

        /// <summary>
        /// Gets the active robot settings
        /// </summary>
        public RobotSettings GetActiveSettings()
        {
            lock (_lockObject)
            {
                return _activeProfile?.Settings?.Clone() ?? new RobotSettings();
            }
        }

        /// <summary>
        /// Creates a default profile if none exists
        /// </summary>
        public void EnsureDefaultProfile()
        {
            lock (_lockObject)
            {
                if (_profiles.Count == 0)
                {
                    var defaultProfile = CreateProfile(DEFAULT_PROFILE_NAME, new RobotSettings());
                    _activeProfile = defaultProfile;
                }
                else if (_activeProfile == null)
                {
                    _activeProfile = _profiles.First();
                }
            }
        }
        #endregion

        #region Public Methods - Import/Export
        /// <summary>
        /// Exports a profile to a file
        /// </summary>
        public async Task ExportProfileAsync(string profileId, string filePath)
        {
            RobotProfile profile;
            lock (_lockObject)
            {
                profile = _profiles.FirstOrDefault(p => p.Id == profileId);
            }

            if (profile == null)
                throw new ArgumentException($"Profile {profileId} not found");

            var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// Imports a profile from a file
        /// </summary>
        public async Task<RobotProfile> ImportProfileAsync(string filePath)
        {
            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var profile = JsonSerializer.Deserialize<RobotProfile>(json);

            if (profile == null)
                throw new InvalidDataException("Invalid profile file");

            lock (_lockObject)
            {
                _profiles.Add(profile);
                SaveProfilesAsync().Wait();
            }

            return profile;
        }
        #endregion

        #region Private Methods
        private async Task LoadProfilesAsync()
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    if (System.IO.File.Exists(_profilesPath))
                    {
                        try
                        {
                            var json = System.IO.File.ReadAllText(_profilesPath);
                            _profiles = JsonSerializer.Deserialize<List<RobotProfile>>(json) ?? new List<RobotProfile>();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error loading profiles: {ex.Message}");
                            _profiles = new List<RobotProfile>();
                        }
                    }
                }
            });
        }

        private async Task SaveProfilesAsync()
        {
            await Task.Run(() =>
            {
                lock (_lockObject)
                {
                    try
                    {
                        var json = JsonSerializer.Serialize(_profiles, new JsonSerializerOptions { WriteIndented = true });
                        System.IO. File.WriteAllText(_profilesPath, json);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error saving profiles: {ex.Message}");
                    }
                }
            });
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                SaveProfilesAsync().Wait();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}