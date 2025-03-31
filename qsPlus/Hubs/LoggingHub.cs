using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace qsPlus.Hubs
{
    public class LoggingHub : Hub
    {
        private readonly ILogger<LoggingHub> _logger;

        public LoggingHub(ILogger<LoggingHub> logger)
        {
            _logger = logger;
        }

        public async Task SendLogMessage(LogMessage message)
        {
            _logger.LogInformation("Received log message from {Application}", message.ApplicationName);
            await Clients.All.SendAsync("ReceiveLogMessage", message);
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
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