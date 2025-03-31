using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using qsPlus.Models;
using qsPlus.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace qsPlus.Services
{
    // Implement IHostedService to make it start automatically when the app starts
    public class SignalRConnectionService : IHostedService, IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private bool _connectionInitialized = false;
        private readonly object _lock = new object();
        private readonly LogFileManager _logFileManager;
        private readonly ILogger<SignalRConnectionService> _logger;
        private readonly SignalRHostedService _serverService;

        // Events
        public event Action<bool>? ConnectionChanged;
        public event Action<LogMessage>? LogReceived;

        // Properties
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public string ConnectionError { get; private set; } = string.Empty;

        // Constructor
        public SignalRConnectionService(
            ILogger<SignalRConnectionService> logger,
            SignalRHostedService serverService,
            LogFileManager logFileManager)
        {
            _logger = logger;
            _serverService = serverService;
            _logFileManager = logFileManager;
        }

        // IHostedService implementation - starts when the app initializes
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Start connection when app starts
                await EnsureConnectedAsync();
            }
            catch (Exception ex)
            {
                // Log error but don't throw - allows app to continue
                ConnectionError = $"Failed to start connection: {ex.Message}";
                ConnectionChanged?.Invoke(false);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }

        public async Task<bool> EnsureConnectedAsync()
        {
            if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
                return true;

            return await InitializeConnectionAsync();
        }

        private async Task<bool> InitializeConnectionAsync()
        {
            // Use a lock to prevent multiple initialization attempts
            lock (_lock)
            {
                if (_connectionInitialized)
                    return false;

                _connectionInitialized = true;
            }

            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"http://localhost:{SignalRHostedService.HubPort}/loggingHub")
                    .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                    .Build();

                // Set up connection closed handler
                _hubConnection.Closed += async (error) =>
                {
                    await OnConnectionClosed(error);
                };

                // Set up message handler
                _hubConnection.On<LogMessage>("ReceiveLogMessage", (log) =>
                {
                    LogReceived?.Invoke(log);
                });

                // Handle reconnected event
                _hubConnection.Reconnected += async (connectionId) =>
                {
                    await OnConnectionReconnected(connectionId);
                };

                await _hubConnection.StartAsync();
                ConnectionError = string.Empty;
                ConnectionChanged?.Invoke(true);
                return true;
            }
            catch (Exception ex)
            {
                ConnectionError = ex.Message;
                _connectionInitialized = false; // Reset to allow retry
                ConnectionChanged?.Invoke(false);
                return false;
            }
        }

        private async Task OnConnectionClosed(Exception error)
        {
            ConnectionError = error?.Message ?? "Connection closed";
            ConnectionChanged?.Invoke(false);

            // Log disconnection to file
            var message = new LogMessage
            {
                Message = $"SignalR connection closed: {error?.Message ?? "Unknown reason"}",
                LogLevel = "Warning",
                ApplicationName = "qsPlus",
                Source = "SignalRConnectionService",
                ExceptionType = error?.GetType().Name,
                StackTrace = error?.StackTrace,
                Timestamp = DateTime.UtcNow
            };

            await _logFileManager.WriteLogMessageAsync(message);

            // Reset initialization flag to allow reconnection
            _connectionInitialized = false;

            // Attempt to reconnect after delay
            await Task.Delay(5000);
            await AttemptReconnectAsync();
        }

        private Task OnConnectionReconnected(string connectionId)
        {
            ConnectionError = string.Empty;
            ConnectionChanged?.Invoke(true);

            // Log reconnection to file as well
            var message = new LogMessage
            {
                Message = $"SignalR connection re-established with ID: {connectionId}",
                LogLevel = "Information",
                ApplicationName = "qsPlus",
                Source = "SignalRConnectionService",
                Timestamp = DateTime.UtcNow
            };

            _ = _logFileManager.WriteLogMessageAsync(message);

            return Task.CompletedTask;
        }

        public async Task<bool> AttemptReconnectAsync()
        {
            if (_hubConnection == null)
                return await InitializeConnectionAsync();

            try
            {
                await _hubConnection.StartAsync();
                ConnectionError = string.Empty;
                ConnectionChanged?.Invoke(true);
                return true;
            }
            catch (Exception ex)
            {
                ConnectionError = ex.Message;
                ConnectionChanged?.Invoke(false);
                return false;
            }
        }

        public async Task SendLogMessageAsync(LogMessage log)
        {
            if (_hubConnection?.State != HubConnectionState.Connected)
            {
                if (!await EnsureConnectedAsync())
                    throw new InvalidOperationException("Cannot send log: Not connected to hub");
            }

            await _hubConnection!.SendAsync("SendLogMessage", log);
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
            }
        }
    }
}