using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
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
        
        // Events
        public event Action<bool>? ConnectionChanged;
        public event Action<LogMessage>? LogReceived;
        
        // Properties
        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
        public string ConnectionError { get; private set; } = string.Empty;
        
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
                    ConnectionError = error?.Message ?? "Connection closed";
                    ConnectionChanged?.Invoke(false);
                    
                    // Reset initialization flag to allow reconnection
                    _connectionInitialized = false;
                    
                    // Attempt to reconnect after delay
                    await Task.Delay(5000);
                    await AttemptReconnectAsync();
                };
                
                // Set up message handler
                _hubConnection.On<LogMessage>("ReceiveLogMessage", (log) =>
                {
                    LogReceived?.Invoke(log);
                });
                
                // Handle reconnected event
                _hubConnection.Reconnected += (connectionId) =>
                {
                    ConnectionError = string.Empty;
                    ConnectionChanged?.Invoke(true);
                    return Task.CompletedTask;
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