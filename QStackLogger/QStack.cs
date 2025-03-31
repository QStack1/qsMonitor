using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace QStackLogger
{
    public static class QStack
    {
        private static HubConnection _hubConnection;
        private static bool _initialized;
        private static readonly object _initLock = new object();
        private static string _applicationName = "Unknown Application";
        private static string _hubUrl = "http://localhost:5117/loggingHub";
        private static string _fallbackLogPath;
        private static readonly ConcurrentQueue<LogMessage> _pendingLogs = new ConcurrentQueue<LogMessage>();
        private static readonly int MaxPendingLogs = 1000;
        private static bool _processingPendingLogs;

        /// <summary>
        /// Initialize the QStack logging system. Call this once at application startup.
        /// </summary>
        /// <param name="applicationName">Name of your application</param>
        /// <param name="hubUrl">URL to the SignalR hub</param>
        /// <param name="fallbackLogPath">Optional path for fallback logging when hub is unavailable</param>
        public static void Initialize(string applicationName, string fallbackLogPath = null, string hubUrl = null)
        {
            lock (_initLock)
            {
                if (_initialized)
                    return;

                _applicationName = applicationName;
                _hubUrl = hubUrl ?? _hubUrl;
                
                // Set default fallback path if not provided
                if (string.IsNullOrEmpty(fallbackLogPath))
                {
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    _fallbackLogPath = Path.Combine(appDataPath, "QStack", $"{_applicationName}-logs.txt");
                }
                else
                {
                    _fallbackLogPath = fallbackLogPath;
                }

                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(_fallbackLogPath));

                InitializeHubConnection();
                _initialized = true;
            }
        }

        private static void InitializeHubConnection()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30) })
                .Build();

            _hubConnection.Closed += OnConnectionClosed;
            _hubConnection.Reconnected += OnConnectionReconnected;

            // Initial connection attempt
            Task.Run(async () =>
            {
                try
                {
                    await _hubConnection.StartAsync();
                }
                catch
                {
                    // Connection failed - will use file logging
                }
            });
        }

        private static async Task OnConnectionClosed(Exception error)
        {
            // Connection lost - logs will be written to file until reconnected
            await Task.Delay(5000);
            try
            {
                await _hubConnection.StartAsync();
                // On reconnection, try to send any pending logs
                ProcessPendingLogs();
            }
            catch
            {
                // Continue with file-based logging
            }
        }

        private static Task OnConnectionReconnected(string connectionId)
        {
            // Connection restored - attempt to send any queued logs
            ProcessPendingLogs();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Log an informational message to QStack
        /// </summary>
        public static void Log(string message)
        {
            LogWithLevel(message, LogLevel.Information);
        }

        /// <summary>
        /// Log a warning message to QStack
        /// </summary>
        public static void LogWarning(string message)
        {
            LogWithLevel(message, LogLevel.Warning);
        }

        /// <summary>
        /// Log an error message to QStack
        /// </summary>
        public static void LogError(string message, Exception ex = null)
        {
            LogWithLevel(message, LogLevel.Error, ex);
        }

        /// <summary>
        /// Log a debug message to QStack
        /// </summary>
        public static void LogDebug(string message)
        {
            LogWithLevel(message, LogLevel.Debug);
        }

        /// <summary>
        /// Log a critical error message to QStack
        /// </summary>
        public static void LogCritical(string message, Exception ex = null)
        {
            LogWithLevel(message, LogLevel.Critical, ex);
        }

        private static void LogWithLevel(string message, LogLevel logLevel, Exception ex = null)
        {
            if (!_initialized)
            {
                // Auto-initialize with default values if not done explicitly
                Initialize(AppDomain.CurrentDomain.FriendlyName);
            }

            var logMessage = new LogMessage
            {
                Message = message,
                LogLevel = logLevel.ToString(),
                ApplicationName = _applicationName,
                Timestamp = DateTime.UtcNow,
                ExceptionType = ex?.GetType().Name,
                StackTrace = ex?.StackTrace,
                Source = ex?.Source ?? "QStack"
            };

            SendLogMessage(logMessage);
        }

        private static void SendLogMessage(LogMessage message)
        {
            // Fire and forget
            Task.Run(async () =>
            {
                if (_hubConnection?.State == HubConnectionState.Connected)
                {
                    try
                    {
                        // Convert the LogMessage to an anonymous object with exact property names
                        var logObj = new
                        {
                            Message = message.Message,
                            ExceptionType = message.ExceptionType,
                            StackTrace = message.StackTrace,
                            Source = message.Source,
                            Timestamp = message.Timestamp,
                            ApplicationName = message.ApplicationName,
                            LogLevel = message.LogLevel
                        };
                        
                        await _hubConnection.SendAsync("SendLogMessage", logObj);
                        return; // Success
                    }
                    catch (Exception ex)
                    {
                        // Log the exception
                        Console.WriteLine($"Failed to send log to hub: {ex.Message}");
                        // Fall through to queue/file logging
                    }
                }

                // Add to pending queue and write to file
                EnqueuePendingLog(message);
                WriteToFile(message);
            });
        }

        private static void EnqueuePendingLog(LogMessage message)
        {
            // Add to pending queue while keeping queue size under control
            _pendingLogs.Enqueue(message);
            
            // Trim queue if needed
            while (_pendingLogs.Count > MaxPendingLogs && _pendingLogs.TryDequeue(out _))
            {
                // Discard oldest log
            }
        }

        private static void ProcessPendingLogs()
        {
            if (_processingPendingLogs || _hubConnection?.State != HubConnectionState.Connected)
                return;

            _processingPendingLogs = true;

            Task.Run(async () =>
            {
                try
                {
                    while (_pendingLogs.TryDequeue(out var log) && 
                           _hubConnection?.State == HubConnectionState.Connected)
                    {
                        try
                        {
                            await _hubConnection.SendAsync("SendLogMessage", log);
                            // Success - move to next log
                        }
                        catch
                        {
                            // Re-enqueue and stop processing for now
                            EnqueuePendingLog(log);
                            break; 
                        }
                    }
                }
                finally
                {
                    _processingPendingLogs = false;
                }
            });
        }

        private static void WriteToFile(LogMessage message)
        {
            try
            {
                var logLine = $"{message.Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{message.LogLevel}] {message.Source}: {message.Message}";
                
                // Add exception details if present
                if (!string.IsNullOrEmpty(message.ExceptionType))
                {
                    logLine += $"{Environment.NewLine}Exception: {message.ExceptionType}";
                    
                    if (!string.IsNullOrEmpty(message.StackTrace))
                    {
                        logLine += $"{Environment.NewLine}{message.StackTrace}";
                    }
                }
                
                File.AppendAllText(_fallbackLogPath, logLine + Environment.NewLine);
            }
            catch
            {
                // Last resort - we can't even write to file
                // Nothing more we can do here
            }
        }

        public class LogMessage
        {
            public string? Message { get; set; }
            public string? ExceptionType { get; set; }
            public string? StackTrace { get; set; }
            public string? Source { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
            public string? ApplicationName { get; set; }
            public string? LogLevel { get; set; }
        }
    }
}