using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace QStackLogger
{
    public class LoggerOptions
    {
        public string HubUrl { get; set; } = "http://localhost:5117/loggingHub";
        public string ApplicationName { get; set; } = "Unknown Application";
    }

    public class SignalRLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly HubConnection _hubConnection;
        private readonly LoggerOptions _options;

        public SignalRLogger(string categoryName, LoggerOptions options)
        {
            _categoryName = categoryName;
            _options = options;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_options.HubUrl)
                .WithAutomaticReconnect()
                .Build();

            // Start the connection in the background
            Task.Run(async () =>
            {
                try
                {
                    await _hubConnection.StartAsync();
                }
                catch
                {
                    // Handle connection failure
                    // Could retry or log to local storage
                }
            });
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            Task.Run(async () =>
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    try
                    {
                        await _hubConnection.StartAsync();
                    }
                    catch
                    {
                        return; // Can't log if can't connect
                    }
                }

                var message = new LogMessage
                {
                    Message = formatter(state, exception),
                    ExceptionType = exception?.GetType().Name,
                    StackTrace = exception?.StackTrace,
                    Source = exception?.Source ?? _categoryName,
                    ApplicationName = _options.ApplicationName,
                    LogLevel = logLevel.ToString()
                };

                try
                {
                    await _hubConnection.SendAsync("SendLogMessage", message);
                }
                catch
                {
                    // Handle send failure
                }
            });
        }

        private class LogMessage
        {
            public string Message { get; set; }
            public string ExceptionType { get; set; }
            public string StackTrace { get; set; }
            public string Source { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
            public string ApplicationName { get; set; }
            public string LogLevel { get; set; }
        }
    }

    public class SignalRLoggerProvider : ILoggerProvider
    {
        private readonly LoggerOptions _options;

        public SignalRLoggerProvider(LoggerOptions options)
        {
            _options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SignalRLogger(categoryName, _options);
        }

        public void Dispose() { }
    }

    public static class SignalRLoggerExtensions
    {
        public static ILoggingBuilder AddQStackLogger(this ILoggingBuilder builder, Action<LoggerOptions> configure = null)
        {
            var options = new LoggerOptions();
            configure?.Invoke(options);

            builder.Services.AddSingleton<ILoggerProvider>(new SignalRLoggerProvider(options));
            return builder;
        }
    }
}