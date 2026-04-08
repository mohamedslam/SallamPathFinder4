#region File Header
/// <summary>
/// File: RobotCommunicator.cs
/// Description: Service for communication with physical robot via WiFi
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-04
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Robot;
using static System.Net.Mime.MediaTypeNames;
#endregion

namespace SallamPathFinder4.Services.Robot
{
    #region Class Documentation
    /// <summary>
    /// Service for communication with physical robot via WiFi
    /// Uses TCP/IP protocol for command and data exchange
    /// Supports sending commands, receiving sensor data, and camera frames
    /// </summary>
    #endregion
    public sealed class RobotCommunicator : IRobotCommunicator, IDisposable
    {
        #region Constants
        private const int BUFFER_SIZE = 65536;
        private const int DEFAULT_PORT = 8080;
        private const int RECEIVE_TIMEOUT_MS = 5000;
        #endregion

        #region Private Fields
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected;
        private bool _isDisposed;
        private readonly object _lockObject = new object();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new robot communicator
        /// </summary>
        public RobotCommunicator()
        {
            _isConnected = false;
            _isDisposed = false;
        }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public bool IsConnected
        {
            get
            {
                lock (_lockObject)
                {
                    return _isConnected && _client?.Connected == true;
                }
            }
        }
        #endregion

        #region Events
        /// <inheritdoc/>
        public event Action<SensorData> SensorDataReceived;

        /// <inheritdoc/>
        public event Action<System.Drawing. Image> CameraFrameReceived;

        /// <inheritdoc/>
        public event Action<ObstacleData> ObstacleDetected;
        #endregion

        #region Public Methods - Connection
        /// <inheritdoc/>
        public async Task<bool> ConnectAsync(string ip, int port = DEFAULT_PORT)
        {
            if (string.IsNullOrEmpty(ip))
                throw new ArgumentNullException(nameof(ip));

            try
            {
                lock (_lockObject)
                {
                    _client?.Close();
                    _client = new TcpClient();
                }

                await _client.ConnectAsync(ip, port);

                lock (_lockObject)
                {
                    _stream = _client.GetStream();
                    _stream.ReadTimeout = RECEIVE_TIMEOUT_MS;
                    _isConnected = true;
                }

                // Start listening for data
                _ = Task.Run(ListenForDataAsync);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            lock (_lockObject)
            {
                _isConnected = false;
                _stream?.Close();
                _client?.Close();
                _stream = null;
                _client = null;
            }
        }
        #endregion

        #region Public Methods - Commands
        /// <inheritdoc/>
        public async Task SendCommandAsync(RobotCommand command)
        {
            if (!IsConnected)
                return;

            var commandData = new { command = command.ToString(), timestamp = DateTime.UtcNow };
            var json = JsonSerializer.Serialize(commandData);
            var data = Encoding.UTF8.GetBytes(json);

            try
            {
                lock (_lockObject)
                {
                    _stream?.Write(data, 0, data.Length);
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Send command failed: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public async Task<SensorData> RequestSensorDataAsync()
        {
            await SendCommandAsync(RobotCommand.GetSensorData);

            // Return empty data for now - actual data comes from event
            return new SensorData();
        }

        /// <inheritdoc/>
        public async Task<System.Drawing.Image> RequestCameraFrameAsync()
        {
            await SendCommandAsync(RobotCommand.StartCamera);

            // Return null for now - actual frame comes from event
            return null;
        }
        #endregion

        #region Private Methods
        private async Task ListenForDataAsync()
        {
            var buffer = new byte[BUFFER_SIZE];
            var memoryStream = new MemoryStream();

            while (IsConnected)
            {
                try
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                        ProcessReceivedData(memoryStream);
                    }
                }
                catch (IOException)
                {
                    // Connection closed or timeout
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Receive error: {ex.Message}");
                    await Task.Delay(100);
                }
            }
        }

        private void ProcessReceivedData(MemoryStream stream)
        {
            stream.Position = 0;
            var data = stream.ToArray();
            var message = Encoding.UTF8.GetString(data);

            // Try to parse as JSON
            try
            {
                using var doc = JsonDocument.Parse(message);
                var root = doc.RootElement;

                if (root.TryGetProperty("type", out JsonElement typeElement))
                {
                    string type = typeElement.GetString();

                    switch (type)
                    {
                        case "sensor":
                            ParseSensorData(root);
                            break;
                        case "camera":
                            ParseCameraData(root);
                            break;
                        case "obstacle":
                            ParseObstacleData(root);
                            break;
                    }
                }
            }
            catch (JsonException)
            {
                // Not JSON, might be binary camera data
                ProcessBinaryData(data);
            }

            // Clear stream for next message
            stream.SetLength(0);
        }

        private void ParseSensorData(JsonElement root)
        {
            var sensorData = new SensorData();

            if (root.TryGetProperty("front", out JsonElement front))
                sensorData.FrontDistance = front.GetDouble();
            if (root.TryGetProperty("left", out JsonElement left))
                sensorData.LeftDistance = left.GetDouble();
            if (root.TryGetProperty("right", out JsonElement right))
                sensorData.RightDistance = right.GetDouble();
            if (root.TryGetProperty("back", out JsonElement back))
                sensorData.BackDistance = back.GetDouble();
            if (root.TryGetProperty("pitch", out JsonElement pitch))
                sensorData.Pitch = pitch.GetDouble();
            if (root.TryGetProperty("roll", out JsonElement roll))
                sensorData.Roll = roll.GetDouble();
            if (root.TryGetProperty("yaw", out JsonElement yaw))
                sensorData.Yaw = yaw.GetDouble();

            SensorDataReceived?.Invoke(sensorData);
        }

        private void ParseCameraData(JsonElement root)
        {
            if (root.TryGetProperty("image_base64", out JsonElement imageBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(imageBase64.GetString());
                using var ms = new MemoryStream(imageBytes);
                var image = System.Drawing.Image.FromStream(ms);
                CameraFrameReceived?.Invoke(image);
            }
        }

        private void ParseObstacleData(JsonElement root)
        {
            var obstacleData = new ObstacleData();

            if (root.TryGetProperty("type", out JsonElement type))
                obstacleData.Type = (Core.Enums.ObstacleType)type.GetInt32();
            if (root.TryGetProperty("x", out JsonElement x))
                obstacleData.Location = new Point(x.GetInt32(), obstacleData.Location.Y);
            if (root.TryGetProperty("y", out JsonElement y))
                obstacleData.Location = new Point(obstacleData.Location.X, y.GetInt32());
            if (root.TryGetProperty("distance", out JsonElement distance))
                obstacleData.Distance = distance.GetDouble();

            obstacleData.Timestamp = DateTime.UtcNow;
            ObstacleDetected?.Invoke(obstacleData);
        }

        private void ProcessBinaryData(byte[] data)
        {
            // Try to interpret as JPEG image
            try
            {
                using var ms = new MemoryStream(data);
                var image = System.Drawing.Image.FromStream(ms);
                CameraFrameReceived?.Invoke(image);
            }
            catch (ArgumentException)
            {
                // Not a valid image
            }
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            if (!_isDisposed)
            {
                Disconnect();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}