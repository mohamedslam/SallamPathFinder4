#region File Header
/// <summary>
/// File: RealRobotInterface.cs
/// Description: Implementation of real robot communication via TCP/IP
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using SallamPathFinder4.Core.Interfaces.Services;
using SallamPathFinder4.Core.Models.Robot;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
#endregion

namespace SallamPathFinder4.Services.Robot
{
    #region Class Documentation
    /// <summary>
    /// Implementation of real robot communication via TCP/IP
    /// </summary>
    #endregion
    public sealed class RealRobotInterface : IRealRobotInterface, IDisposable
    {
        #region Constants
        private const int DEFAULT_PORT = 8080;
        private const int BUFFER_SIZE = 65536;
        private const int RECEIVE_TIMEOUT_MS = 5000;
        private const int TELEMETRY_INTERVAL_MS = 100;
        #endregion

        #region Private Fields
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected;
        private bool _isDisposed;
        private CancellationTokenSource _cts;
        private readonly object _lockObject = new object();
        #endregion

        #region Constructor
        public RealRobotInterface()
        {
            _isConnected = false;
            _isDisposed = false;
        }
        #endregion

        #region Properties
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
        public event Action<RobotTelemetry> TelemetryReceived;
        public event Action<RobotCommand, bool> CommandExecuted;
        public event Action<Image> FrameReceived;
        public event Action<string> ErrorOccurred;
        public event Action ConnectionLost;
        public event Action ConnectionRestored;
        #endregion

        #region Public Methods - Connection
        public async Task<bool> ConnectAsync(string ip, int port = DEFAULT_PORT)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new ArgumentNullException(nameof(ip));
            }

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

                _cts = new CancellationTokenSource();
                _ = Task.Run(() => ListenForDataAsync(_cts.Token));
                _ = Task.Run(() => RequestTelemetryLoopAsync(_cts.Token));

                System.Diagnostics.Debug.WriteLine($"[RealRobot] Connected to {ip}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RealRobot] Connection failed: {ex.Message}");
                ErrorOccurred?.Invoke($"Connection failed: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            _cts?.Cancel();

            lock (_lockObject)
            {
                _isConnected = false;
                _stream?.Close();
                _client?.Close();
                _stream = null;
                _client = null;
            }

            System.Diagnostics.Debug.WriteLine("[RealRobot] Disconnected");
        }
        #endregion

        #region Public Methods - Send Commands
        public async Task<bool> SendCommandAsync(RobotCommand command)
        {
            if (!this.IsConnected)
            {
                return false;
            }

            try
            {
                byte[] data = command.ToBinary();

                lock (_lockObject)
                {
                    _stream?.Write(data, 0, data.Length);
                }

                System.Diagnostics.Debug.WriteLine($"[RealRobot] Sent: {command}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[RealRobot] Send failed: {ex.Message}");
                ErrorOccurred?.Invoke($"Send failed: {ex.Message}");
                return false;
            }
        }

        public async Task<int> SendCommandsAsync(List<RobotCommand> commands)
        {
            int successCount = 0;

            foreach (var command in commands)
            {
                if (await this.SendCommandAsync(command))
                {
                    successCount++;
                    await Task.Delay(50); // Small delay between commands
                }
            }

            return successCount;
        }

        public async Task StopAsync()
        {
            await this.SendCommandAsync(new RobotCommand(CommandID.Stop));
        }

        public async Task EmergencyStopAsync()
        {
            await this.SendCommandAsync(new RobotCommand(CommandID.EmergencyStop));
        }
        #endregion

        #region Public Methods - Request Data
        public async Task<RobotTelemetry> RequestTelemetryAsync()
        {
            await this.SendCommandAsync(new RobotCommand(CommandID.RequestTelemetry));
            return new RobotTelemetry(); // Will be updated via event
        }

        public async Task<Image> RequestFrameAsync()
        {
            await this.SendCommandAsync(new RobotCommand(CommandID.RequestFrame));
            return null; // Will be updated via event
        }

        public async Task<SensorData> RequestSensorsAsync()
        {
            await this.SendCommandAsync(new RobotCommand(CommandID.RequestSensors));
            return new SensorData(); // Will be updated via event
        }
        #endregion

        #region Private Methods - Listeners
        private async Task ListenForDataAsync(CancellationToken token)
        {
            var buffer = new byte[BUFFER_SIZE];
            var memoryStream = new MemoryStream();

            while (!token.IsCancellationRequested && this.IsConnected)
            {
                try
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);

                    if (bytesRead > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                        ProcessReceivedData(memoryStream);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[RealRobot] Receive error: {ex.Message}");
                    await Task.Delay(100, token);
                }
            }

            ConnectionLost?.Invoke();
        }

        private async Task RequestTelemetryLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && this.IsConnected)
            {
                await this.RequestTelemetryAsync();
                await Task.Delay(TELEMETRY_INTERVAL_MS, token);
            }
        }

        private void ProcessReceivedData(MemoryStream stream)
        {
            stream.Position = 0;
            var data = stream.ToArray();
            var message = Encoding.UTF8.GetString(data);

            try
            {
                using var doc = JsonDocument.Parse(message);
                var root = doc.RootElement;

                if (root.TryGetProperty("type", out JsonElement typeElement))
                {
                    string type = typeElement.GetString();

                    switch (type)
                    {
                        case "telemetry":
                            ParseTelemetry(root);
                            break;
                        case "frame":
                            ParseFrame(root);
                            break;
                        case "sensor":
                            ParseSensor(root);
                            break;
                        case "ack":
                            ParseAcknowledgment(root);
                            break;
                    }
                }
            }
            catch (JsonException)
            {
                // Not JSON, might be binary data
                ProcessBinaryData(data);
            }

            stream.SetLength(0);
        }

        private void ParseTelemetry(JsonElement root)
        {
            var telemetry = new RobotTelemetry();

            if (root.TryGetProperty("x", out JsonElement x))
                telemetry.Position = new Point(x.GetInt32(), telemetry.Position.Y);
            if (root.TryGetProperty("y", out JsonElement y))
                telemetry.Position = new Point(telemetry.Position.X, y.GetInt32());
            if (root.TryGetProperty("angle", out JsonElement angle))
                telemetry.Angle = angle.GetSingle();
            if (root.TryGetProperty("speed", out JsonElement speed))
                telemetry.Speed = speed.GetDouble();
            if (root.TryGetProperty("battery", out JsonElement battery))
                telemetry.BatteryPercent = battery.GetDouble();
            if (root.TryGetProperty("state", out JsonElement state))
                telemetry.State = state.GetString();

            TelemetryReceived?.Invoke(telemetry);
        }

        private void ParseFrame(JsonElement root)
        {
            if (root.TryGetProperty("image_base64", out JsonElement imageBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(imageBase64.GetString());
                using var ms = new MemoryStream(imageBytes);
                var image = Image.FromStream(ms);
                FrameReceived?.Invoke(image);
            }
        }

        private void ParseSensor(JsonElement root)
        {
            var sensors = new SensorData();

            if (root.TryGetProperty("front", out JsonElement front))
                sensors.FrontDistance = front.GetDouble();
            if (root.TryGetProperty("left", out JsonElement left))
                sensors.LeftDistance = left.GetDouble();
            if (root.TryGetProperty("right", out JsonElement right))
                sensors.RightDistance = right.GetDouble();
            if (root.TryGetProperty("back", out JsonElement back))
                sensors.BackDistance = back.GetDouble();

            // Forward sensor data to existing event
            if (_existingSensorEvent != null)
            {
                _existingSensorEvent?.Invoke(sensors);
            }
        }

        private void ParseAcknowledgment(JsonElement root)
        {
            if (root.TryGetProperty("command", out JsonElement command))
            {
                bool success = root.TryGetProperty("success", out JsonElement successElem) && successElem.GetBoolean();
                var robotCommand = new RobotCommand();
                // Parse command...
                CommandExecuted?.Invoke(robotCommand, success);
            }
        }

        private void ProcessBinaryData(byte[] data)
        {
            try
            {
                using var ms = new MemoryStream(data);
                var image = Image.FromStream(ms);
                FrameReceived?.Invoke(image);
            }
            catch
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
                this.Disconnect();
                _cts?.Dispose();
                _isDisposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Temporary - For compatibility with existing code
        private Action<SensorData> _existingSensorEvent;
        #endregion
    }
}