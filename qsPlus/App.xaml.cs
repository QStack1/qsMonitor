using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace qsPlus;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<App> _logger;

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logger = serviceProvider.GetRequiredService<ILogger<App>>();
        InitializeComponent();
        
        MainPage = new MainPage();
    }

    protected override async void OnStart()
    {
        _logger.LogInformation("Application starting");
        
        // Get the SignalR hosted service and ensure it's started
        var hostedServices = _serviceProvider.GetServices<IHostedService>();
        foreach (var service in hostedServices)
        {
            if (service is SignalRHostedService)
            {
                _logger.LogInformation("Starting SignalR hosted service");
                await service.StartAsync(CancellationToken.None);
                _logger.LogInformation("SignalR hosted service started");
            }
        }
    }
}
