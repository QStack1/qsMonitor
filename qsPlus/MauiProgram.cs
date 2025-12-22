using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using qsPlus.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using qsPlus.Services;
using Radzen;

namespace qsPlus
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddScoped<ThemeService>();
            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<SignalRConnectionService>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<SignalRConnectionService>());
            
            // Configure local SignalR server
            builder.Services.AddSingleton<IHostedService, SignalRHostedService>();
            builder.Services.AddRadzenComponents();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }

    // Class to host SignalR as a background service
    public class SignalRHostedService : IHostedService
    {
        private WebApplication? _app;
        public const int HubPort = 5117;
        private readonly ILogger<SignalRHostedService> _logger;

        public SignalRHostedService(ILogger<SignalRHostedService> logger)
        {
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting SignalR service on port {Port}", HubPort);
                
                var builder = WebApplication.CreateBuilder();
                builder.WebHost.UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, HubPort);
                });
                
                builder.Services.AddSignalR();
                
                _app = builder.Build();
                
                _app.MapHub<LoggingHub>("/loggingHub");
                
                await _app.StartAsync(cancellationToken);
                
                _logger.LogInformation("SignalR service started successfully on port {Port}", HubPort);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start SignalR service");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_app != null)
            {
                _logger.LogInformation("Stopping SignalR service");
                await _app.StopAsync(cancellationToken);
                _logger.LogInformation("SignalR service stopped");
            }
        }
    }
}
