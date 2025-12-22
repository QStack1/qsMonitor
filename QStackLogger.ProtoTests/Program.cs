using System;
using QStackLogger;
using System.Threading.Tasks;
using System.IO;

namespace QStackLogger.ProtoTests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting QStackLogger Prototype Test...");
            
            // Initialize QStack first so it's ready for any early logging
            string logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "QStackLogger.ProtoTests", "Logs");
                
            // Make sure directory exists
            Directory.CreateDirectory(logDirectory);
            
            // Initialize with application name, fallback log path, and hub URL
            QStack.Initialize(
                applicationName: "QStackLogger.ProtoTests", 
                fallbackLogPath: Path.Combine(logDirectory, "qstack-logs.txt"),
                hubUrl: "http://localhost:5117/loggingHub");
                
            // Log startup
            QStack.Log("Application starting...");
            
            // Run our interactive menu
            await RunInteractiveMenu();
            
            // Log shutdown
            QStack.Log("Application shutting down...");
        }
        
        static async Task RunInteractiveMenu()
        {
            bool exit = false;
            
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== QStack Direct Logging Test Menu ===");
                Console.WriteLine("1) Test Error Log (with exception)");
                Console.WriteLine("2) Test Warning Log");
                Console.WriteLine("3) Test Debug Log");
                Console.WriteLine("4) Test Info Log");
                Console.WriteLine("5) Test Critical Log");
                Console.WriteLine("6) Run All Log Tests");
                Console.WriteLine("7) Custom Log Message");
                Console.WriteLine("X) Exit");
                Console.WriteLine();
                Console.Write("Enter your choice: ");
                
                var key = Console.ReadKey();
                Console.WriteLine("\n");
                
                switch (key.KeyChar)
                {
                    case '1':
                        LogErrorWithException();
                        break;
                    case '2':
                        QStack.LogWarning("This is a WARNING log message!");
                        Console.WriteLine("Warning logged!");
                        break;
                    case '3':
                        QStack.LogDebug("This is a DEBUG log message!");
                        Console.WriteLine("Debug logged!");
                        break;
                    case '4':
                        QStack.Log("This is an INFORMATION log message!");
                        Console.WriteLine("Information logged!");
                        break;
                    case '5':
                        LogCriticalWithException();
                        break;
                    case '6':
                        RunAllLogTests();
                        break;
                    case '7':
                        RunCustomLogTest();
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
        
        static void LogErrorWithException()
        {
            try
            {
                Console.WriteLine("Simulating an exception...");
                throw new Exception("Something went wrong!");
            }
            catch (Exception ex)
            {
                QStack.LogError("Failed to do something important", ex);
                Console.WriteLine($"Error logged: {ex.Message}");
            }
        }
        
        static void LogCriticalWithException()
        {
            try
            {
                Console.WriteLine("Simulating a critical failure...");
                throw new InvalidOperationException("Critical system failure");
            }
            catch (Exception ex)
            {
                QStack.LogCritical("Critical system component failure", ex);
                Console.WriteLine($"Critical error logged: {ex.Message}");
            }
        }
        
        static void RunAllLogTests()
        {
            Console.WriteLine("Running all log tests...");
            LogErrorWithException();
            QStack.LogWarning("This is a WARNING log message!");
            Console.WriteLine("Warning logged!");
            QStack.LogDebug("This is a DEBUG log message!");
            Console.WriteLine("Debug logged!");
            QStack.Log("This is an INFORMATION log message!");
            Console.WriteLine("Information logged!");
            LogCriticalWithException();
            Console.WriteLine("All log tests completed!");
        }
        
        static void RunCustomLogTest()
        {
            Console.WriteLine("=== Custom Log Message ===");
            
            Console.WriteLine("Choose log level:");
            Console.WriteLine("1) Error");
            Console.WriteLine("2) Warning");
            Console.WriteLine("3) Information");
            Console.WriteLine("4) Debug");
            Console.WriteLine("5) Critical");
            Console.Write("Enter choice (1-5): ");
            
            var levelChoice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            
            Console.Write("Enter your log message: ");
            var message = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(message))
                message = "Custom test message";
            
            // Should we simulate an exception?
            Console.Write("Include exception details? (y/n): ");
            var includeEx = Console.ReadKey().KeyChar.ToString().ToLower() == "y";
            Console.WriteLine();
            
            Exception ex = null;
            if (includeEx)
            {
                ex = new Exception("Simulated exception for testing");
            }
                
            switch (levelChoice)
            {
                case '1':
                    QStack.LogError(message, ex);
                    break;
                case '2':
                    QStack.LogWarning(message);
                    break;
                case '3':
                    QStack.Log(message);
                    break;
                case '4':
                    QStack.LogDebug(message);
                    break;
                case '5':
                    QStack.LogCritical(message, ex);
                    break;
                default:
                    Console.WriteLine("Invalid level choice. Using Info level.");
                    QStack.Log(message);
                    break;
            }
            
            Console.WriteLine($"Log sent: {message}");
        }
    }
}