using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using qsPlus.Hubs;
using qsPlus.Models;

namespace qsPlus.Services
{
    public class LogFileManager
    {
        private readonly ILogger<LogFileManager> _logger;
        private readonly string _baseLogDirectory;
        private string _currentLogFile;
        
        public event Action<IEnumerable<LogFileInfo>> LogFilesChanged;
        
        public LogFileManager(ILogger<LogFileManager> logger)
        {
            _logger = logger;
            
            // Set up base log directory
            _baseLogDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "qsPlus", "Logs");
                
            // Ensure directory exists
            Directory.CreateDirectory(_baseLogDirectory);
            
            // Set initial log file
            _currentLogFile = Path.Combine(_baseLogDirectory, $"qsPlus-{DateTime.Now:yyyy-MM-dd}.log");
        }
        
        /// <summary>
        /// Gets the current log file path
        /// </summary>
        public string CurrentLogFile => _currentLogFile;
        
        /// <summary>
        /// Gets all log files in the log directory
        /// </summary>
        public IEnumerable<LogFileInfo> GetLogFiles()
        {
            try
            {
                if (!Directory.Exists(_baseLogDirectory))
                    return Enumerable.Empty<LogFileInfo>();
                    
                return Directory.GetFiles(_baseLogDirectory, "*.log")
                    .Select(f => new LogFileInfo
                    {
                        FilePath = f,
                        FileName = Path.GetFileName(f),
                        CreationTime = File.GetCreationTime(f),
                        LastWriteTime = File.GetLastWriteTime(f),
                        SizeInBytes = new FileInfo(f).Length,
                        IsCurrentLogFile = f == _currentLogFile
                    })
                    .OrderByDescending(f => f.LastWriteTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log files");
                return Enumerable.Empty<LogFileInfo>();
            }
        }
        
        /// <summary>
        /// Creates a new log file and sets it as the current file
        /// </summary>
        public async Task<bool> CreateNewLogFileAsync(string namePrefix = "qsPlus")
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
                string newLogPath = Path.Combine(_baseLogDirectory, $"{namePrefix}-{timestamp}.log");
                
                // Create the file if it doesn't exist
                using (var fs = File.Create(newLogPath))
                {
                    // Write a header
                    byte[] header = Encoding.UTF8.GetBytes(
                        $"QS+ Log File\r\n" +
                        $"Created: {DateTime.Now}\r\n" +
                        $"----------------------------------------\r\n\r\n");
                    await fs.WriteAsync(header, 0, header.Length);
                }
                
                _currentLogFile = newLogPath;
                _logger.LogInformation("Created new log file: {LogFile}", newLogPath);
                
                // Notify subscribers
                LogFilesChanged?.Invoke(GetLogFiles());
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new log file");
                return false;
            }
        }
        
        /// <summary>
        /// Clears the contents of the current log file
        /// </summary>
        public async Task<bool> ClearCurrentLogFileAsync()
        {
            try
            {
                if (!File.Exists(_currentLogFile))
                    return false;
                    
                // Clear the file
                using (var fs = new FileStream(_currentLogFile, FileMode.Truncate))
                {
                    // Write a header
                    byte[] header = Encoding.UTF8.GetBytes(
                        $"QS+ Log File (Cleared)\r\n" +
                        $"Cleared: {DateTime.Now}\r\n" +
                        $"----------------------------------------\r\n\r\n");
                    await fs.WriteAsync(header, 0, header.Length);
                }
                
                _logger.LogInformation("Cleared current log file: {LogFile}", _currentLogFile);
                
                // Notify subscribers
                LogFilesChanged?.Invoke(GetLogFiles());
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing current log file");
                return false;
            }
        }
        
        /// <summary>
        /// Deletes a specific log file
        /// </summary>
        public async Task<bool> DeleteLogFileAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;
                    
                // Make sure the file is in our logs directory
                if (!filePath.StartsWith(_baseLogDirectory))
                {
                    _logger.LogWarning("Attempt to delete file outside log directory: {FilePath}", filePath);
                    return false;
                }
                
                // If it's the current log file, create a new one first
                bool isCurrentLog = filePath == _currentLogFile;
                if (isCurrentLog)
                {
                    await CreateNewLogFileAsync();
                }
                
                // Delete the file
                File.Delete(filePath);
                
                _logger.LogInformation("Deleted log file: {LogFile}", filePath);
                
                // Notify subscribers
                LogFilesChanged?.Invoke(GetLogFiles());
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting log file: {FilePath}", filePath);
                return false;
            }
        }
        
        /// <summary>
        /// Deletes all log files except the current one
        /// </summary>
        public async Task<bool> DeleteAllLogFilesAsync(bool keepCurrent = true)
        {
            try
            {
                var logFiles = Directory.GetFiles(_baseLogDirectory, "*.log");
                int deletedCount = 0;
                
                foreach (var file in logFiles)
                {
                    if (keepCurrent && file == _currentLogFile)
                        continue;
                        
                    try
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete log file: {FilePath}", file);
                    }
                }
                
                _logger.LogInformation("Deleted {Count} log files", deletedCount);
                
                // Notify subscribers
                LogFilesChanged?.Invoke(GetLogFiles());
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting all log files");
                return false;
            }
        }
        
        /// <summary>
        /// Writes a log message to the current log file
        /// </summary>
        public async Task WriteLogMessageAsync(LogMessage logMessage)
        {
            try
            {
                if (!File.Exists(_currentLogFile))
                {
                    await CreateNewLogFileAsync();
                }
                
                // Format the log message
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"[{logMessage.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{logMessage.LogLevel}] {logMessage.ApplicationName} - {logMessage.Source}");
                sb.AppendLine($"Message: {logMessage.Message}");
                
                if (!string.IsNullOrEmpty(logMessage.ExceptionType))
                {
                    sb.AppendLine($"Exception: {logMessage.ExceptionType}");
                    
                    if (!string.IsNullOrEmpty(logMessage.StackTrace))
                    {
                        sb.AppendLine($"StackTrace: {logMessage.StackTrace}");
                    }
                }
                
                sb.AppendLine();
                
                // Write to file
                await File.AppendAllTextAsync(_currentLogFile, sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing log message to file");
            }
        }
        
        /// <summary>
        /// Gets the content of a log file
        /// </summary>
        public async Task<string> GetLogFileContentAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return "Log file not found.";
                    
                // Make sure the file is in our logs directory
                if (!filePath.StartsWith(_baseLogDirectory))
                {
                    _logger.LogWarning("Attempt to read file outside log directory: {FilePath}", filePath);
                    return "Access denied: File is outside the logs directory.";
                }
                
                return await File.ReadAllTextAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading log file: {FilePath}", filePath);
                return $"Error reading log file: {ex.Message}";
            }
        }
    }
    
    public class LogFileInfo
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public long SizeInBytes { get; set; }
        public bool IsCurrentLogFile { get; set; }
        
        public string FormattedSize
        {
            get
            {
                if (SizeInBytes < 1024)
                    return $"{SizeInBytes} B";
                else if (SizeInBytes < 1024 * 1024)
                    return $"{SizeInBytes / 1024.0:F1} KB";
                else if (SizeInBytes < 1024 * 1024 * 1024)
                    return $"{SizeInBytes / (1024.0 * 1024.0):F1} MB";
                else
                    return $"{SizeInBytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
            }
        }
    }
}