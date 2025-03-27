using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;

namespace qsPlus.Services;

public class SystemPerformanceService
{
    private static bool _isFirstCpuCall = true;
    private PerformanceCounter? _cpuCounter;
    private PerformanceCounter? _ramCounter;
    private readonly DateTime _systemStartTime;
    private bool _countersInitialized = false;

    public SystemPerformanceService()
    {
        try
        {
            _systemStartTime = DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount64);
        }
        catch (Exception)
        {
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
            Debug.WriteLine($"Failed to initialize performance counters: {ex.Message}");
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
                double value = _cpuCounter.NextValue();
                
                if (_isFirstCpuCall)
                {
                    _isFirstCpuCall = false;
                    Thread.Sleep(200);
                    value = _cpuCounter.NextValue();
                }
                
                return Math.Min(100, Math.Round(value, 2));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting CPU usage: {ex.Message}");
            _countersInitialized = false;
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
            Debug.WriteLine($"Error getting RAM: {ex.Message}");
            _countersInitialized = false;
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

    public double GetTotalRamInMB()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return GetWindowsTotalRamInMB();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return GetLinuxTotalRamInMB();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return GetMacOSTotalRamInMB();
        }
        return 8192;
    }

    private double GetWindowsTotalRamInMB()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
            foreach (var obj in searcher.Get())
            {
                var totalBytes = Convert.ToDouble(obj["TotalPhysicalMemory"]);
                return totalBytes / 1024 / 1024;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting total RAM: {ex.Message}");
            
            try
            {
                var memoryStatus = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(memoryStatus))
                {
                    return memoryStatus.ullTotalPhys / 1024 / 1024;
                }
            }
            catch
            {
                // Ignored
            }
        }
        
        return 8192;
    }

    private double GetLinuxTotalRamInMB()
    {
        try
        {
            string[] memInfoLines = File.ReadAllLines("/proc/meminfo");
            foreach (string line in memInfoLines)
            {
                if (line.StartsWith("MemTotal:"))
                {
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && long.TryParse(parts[1], out long memKb))
                    {
                        return memKb / 1024.0;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting Linux RAM: {ex.Message}");
        }
        
        return 8192;
    }

    private double GetMacOSTotalRamInMB()
    {
        try
        {
            using var process = new Process();
            process.StartInfo.FileName = "sysctl";
            process.StartInfo.Arguments = "-n hw.memsize";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (long.TryParse(output.Trim(), out long totalBytes))
            {
                return totalBytes / 1024 / 1024;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting macOS RAM: {ex.Message}");
        }
        
        return 8192;
    }

    [StructLayout(LayoutKind.Sequential)]
    private class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
        
        public MEMORYSTATUSEX()
        {
            this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
}