using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using qsPlus.Services;
using qsPlus.Models;
using System.Threading.Tasks;

namespace qsPlus.Hubs
{
    public class LoggingHub : Hub
    {
        private readonly ILogger<LoggingHub> _logger;
        private readonly LogFileManager _logFileManager;

        public LoggingHub(ILogger<LoggingHub> logger, LogFileManager logFileManager)
        {
            _logger = logger;
            _logFileManager = logFileManager;
        }

        public async Task SendLogMessage(LogMessage message)
        {
            _logger.LogInformation("Received log message from {ApplicationName}: {Message}", 
                message.ApplicationName, message.Message);

            // Write to file
            await _logFileManager.WriteLogMessageAsync(message);

            // Broadcast to clients
            await Clients.All.SendAsync("ReceiveLogMessage", message);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}