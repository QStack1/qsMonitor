using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace QStackLogger.ProtoTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting QStackLogger Prototype Test...");
            
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<TestService>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    // Set this to Debug to include all debug logs
                    logging.SetMinimumLevel(LogLevel.Debug);
                    
                    logging.AddQStackLogger(options =>
                    {
                        options.HubUrl = "http://localhost:5117/loggingHub";
                        options.ApplicationName = "QStackLogger.ProtoTests";
                    });
                })
                .Build();

            var service = host.Services.GetRequiredService<TestService>();
            
            // Start the host in the background
            var hostTask = host.RunAsync();
            
            // Run our interactive menu
            await RunInteractiveMenu(service);
            
            // Ensure clean host shutdown
            await host.StopAsync();
        }
        
        static async Task RunInteractiveMenu(TestService service)
        {
            bool exit = false;
            
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== QStackLogger Test Menu ===");
                Console.WriteLine("1) Test Error Log (with exception)");
                Console.WriteLine("2) Test Warning Log");
                Console.WriteLine("3) Test Debug Log");
                Console.WriteLine("4) Test Info Log");
                Console.WriteLine("5) Run All Log Tests");
                Console.WriteLine("6) Custom Log Message");
                Console.WriteLine("X) Exit");
                Console.WriteLine();
                Console.Write("Enter your choice: ");
                
                var key = Console.ReadKey();
                Console.WriteLine("\n");
                
                switch (key.KeyChar)
                {
                    case '1':
                        service.ForceErrThrow();
                        break;
                    case '2':
                        service.TestWarnLog();
                        break;
                    case '3':
                        service.TestDebugLog();
                        break;
                    case '4':
                        service.TestInfoLog();
                        break;
                    case '5':
                        service.TestAllLogs();
                        break;
                    case '6':
                        RunCustomLogTest(service);
                        break;
                    case 'x':
                    case 'X':
                        exit = true;
                        Console.WriteLine("Exiting...");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
                
                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
            }
        }
        
        static void RunCustomLogTest(TestService service)
        {
            Console.WriteLine("=== Custom Log Message ===");
            
            Console.WriteLine("Choose log level:");
            Console.WriteLine("1) Error");
            Console.WriteLine("2) Warning");
            Console.WriteLine("3) Information");
            Console.WriteLine("4) Debug");
            Console.Write("Enter choice (1-4): ");
            
            var levelChoice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            
            Console.Write("Enter your log message: ");
            var message = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(message))
                message = "Test custom message";
                
            switch (levelChoice)
            {
                case '1':
                    service.LogCustom(LogLevel.Error, message);
                    break;
                case '2':
                    service.LogCustom(LogLevel.Warning, message);
                    break;
                case '3':
                    service.LogCustom(LogLevel.Information, message);
                    break;
                case '4':
                    service.LogCustom(LogLevel.Debug, message);
                    break;
                default:
                    Console.WriteLine("Invalid level choice. Using Error level.");
                    service.LogCustom(LogLevel.Error, message);
                    break;
            }
        }
    }

    public class TestService
    {
        private readonly ILogger<TestService> _logger;

        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }

        public void ForceErrThrow()
        {
            try
            {
                Console.WriteLine("Attempting to do something...");
                throw new Exception("Something went wrong!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to do something important");
                Console.WriteLine($"Error logged: {ex.Message}");
            }
        }

        public void TestWarnLog()
        {
            _logger.LogWarning("This is a WARNING log message!");
            Console.WriteLine("Warning logged!");
        }

        public void TestDebugLog()
        {
            _logger.LogDebug("This is a DEBUG log message!");
            Console.WriteLine("Debug logged!");
        }

        public void TestInfoLog()
        {
            _logger.LogInformation("This is an INFORMATION log message!");
            Console.WriteLine("Information logged!");
        }
        
        public void LogCustom(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Error:
                    _logger.LogError(message);
                    break;
                case LogLevel.Warning:
                    _logger.LogWarning(message);
                    break;
                case LogLevel.Information:
                    _logger.LogInformation(message);
                    break;
                case LogLevel.Debug:
                    _logger.LogDebug(message);
                    break;
                default:
                    _logger.LogError(message);
                    break;
            }
            
            Console.WriteLine($"{level} level message logged: {message}");
        }

        public void TestAllLogs()
        {
            Console.WriteLine("Running all log tests...");
            ForceErrThrow();
            TestWarnLog();
            TestDebugLog();
            TestInfoLog();
            Console.WriteLine("All log tests completed!");
        }
    }
}