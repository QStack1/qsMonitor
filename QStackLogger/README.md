# QStackLogger

A lightweight logging client that sends logs to a QStack monitoring hub using SignalR. It includes automatic reconnection, fallback file logging, and a simple static API.

## Features

- Simple static API (`QStack.Log()`, `QStack.LogError()`, etc.)
- Automatic reconnection to SignalR hub
- Fallback to file logging when hub connection is unavailable
- Queue and resend logs when connection is restored
- No dependencies on specific logging frameworks

## Quick Start

```csharp
// Initialize once at application startup
QStack.Initialize("YourApplicationName");

// Log at different levels
QStack.Log("User logged in successfully");
QStack.LogWarning("Database connection is slow");
QStack.LogError("Failed to process request", exception);
QStack.LogDebug("Processing item #45");
QStack.LogCritical("Server shutting down unexpectedly", exception);
```

## Configuration

```csharp
QStack.Initialize(
    applicationName: "YourApp",
    fallbackLogPath: @"C:\Logs\YourApp\logs.txt", // Optional - defaults to %LocalAppData%\QStack
    hubUrl: "http://yourserver:5117/loggingHub"   // Optional - defaults to localhost:5117
);
```

## Integration with QStack Monitor

This library works seamlessly with the QStack Monitor application to provide real-time log monitoring, visualization, and management.