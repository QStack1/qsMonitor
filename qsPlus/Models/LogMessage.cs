namespace qsPlus.Models
{
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