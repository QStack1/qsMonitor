using System.Diagnostics;
using System.Runtime.InteropServices;

namespace qsPlus.Services;

public class SystemPerformanceService
{
    private PerformanceCounter? _cpuCounter;
    private PerformanceCounter? _ramCounter;
    private readonly DateTime _systemStartTime;
    private bool _countersInitialized = false;

    public SystemPerformanceService()
    {
        try
        {
            // Get system boot time - this doesn't use PerformanceCounter so should be safe
            _systemStartTime = DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount64);
        }
        catch (Exception)
        {
            // Fallback if anything goes wrong
            _systemStartTime = DateTime.Now;
        }
    }

    private void EnsureCountersInitialized()
    {
        if (_countersInitialized || !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        try
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            _countersInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to initialize performance counters: {ex.Message}");
            // Keep _countersInitialized as false
        }
    }

    public double GetCpuUsage()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return 0;

        try
        {
            EnsureCountersInitialized();
            if (_cpuCounter != null)
            {
                _cpuCounter.NextValue(); // First call will return 0
                Thread.Sleep(500); // Reduced wait time for better UX
                return Math.Round(_cpuCounter.NextValue(), 2);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting CPU usage: {ex.Message}");
            _countersInitialized = false; // Reset to try again next time
        }
        return 0;
    }

    public double GetAvailableRamInMB()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return 0;

        try
        {
            EnsureCountersInitialized();
            if (_ramCounter != null)
            {
                return _ramCounter.NextValue();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting RAM: {ex.Message}");
            _countersInitialized = false; // Reset to try again next time
        }
        return 0;
    }

    public TimeSpan GetSystemUptime()
    {
        return DateTime.Now - _systemStartTime;
    }

    public string GetFormattedUptime()
    {
        var uptime = GetSystemUptime();
        return $"{uptime.Days} days, {uptime.Hours} hours, {uptime.Minutes} minutes";
    }

    public string GetOperatingSystemInfo()
    {
        return RuntimeInformation.OSDescription;
    }

    public int GetProcessorCount()
    {
        return Environment.ProcessorCount;
    }
}