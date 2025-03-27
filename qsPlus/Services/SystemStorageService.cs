using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace qsPlus.Services
{
    public class SystemStorageService
    {
        public class DiskInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Format { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public long TotalSizeBytes { get; set; }
            public long FreeSpaceBytes { get; set; }
            public double UsagePercent => Math.Round(100 - ((double)FreeSpaceBytes / TotalSizeBytes * 100), 1);
            
            public string TotalSizeFormatted => FormatSize(TotalSizeBytes);
            public string FreeSpaceFormatted => FormatSize(FreeSpaceBytes);
        }
        
        public class FolderInfo
        {
            public string Path { get; set; } = string.Empty;
            public string Name => System.IO.Path.GetFileName(Path);
            public long SizeBytes { get; set; }
            public string SizeFormatted => FormatSize(SizeBytes);
            public double Percentage { get; set; }
        }

        public List<DiskInfo> GetDriveInfo()
        {
            var drives = new List<DiskInfo>();
            
            try
            {
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                {
                    drives.Add(new DiskInfo
                    {
                        Name = drive.Name,
                        Format = drive.DriveFormat,
                        Type = drive.DriveType.ToString(),
                        TotalSizeBytes = drive.TotalSize,
                        FreeSpaceBytes = drive.AvailableFreeSpace
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting drive info: {ex.Message}");
            }
            
            return drives;
        }

        public async Task<List<FolderInfo>> GetLargestFoldersAsync(string path, int maxFolders = 5)
        {
            var result = new List<FolderInfo>();
            
            try
            {
                // Get top-level directories only
                var directories = new List<string>();
                try 
                {
                    directories = Directory.GetDirectories(path).ToList();
                }
                catch (UnauthorizedAccessException)
                {
                    // If we can't list the root directory, just return empty result
                    return result;
                }
                
                var folderSizes = new List<FolderInfo>();
                
                // Calculate size for each directory
                foreach (var dir in directories)
                {
                    try
                    {
                        // Skip known protected system folders
                        if (ShouldSkipFolder(dir))
                        {
                            continue;
                        }
                        
                        var size = await Task.Run(() => CalculateFolderSize(dir));
                        folderSizes.Add(new FolderInfo 
                        { 
                            Path = dir,
                            SizeBytes = size
                        });
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip directories we can't access
                        continue;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error calculating size for {dir}: {ex.Message}");
                    }
                }
                
                // Sort by size (largest first) and take the requested number
                result = folderSizes
                    .OrderByDescending(f => f.SizeBytes)
                    .Take(maxFolders)
                    .ToList();
                
                // Calculate percentages
                long totalSize = result.Sum(f => f.SizeBytes);
                if (totalSize > 0)
                {
                    foreach (var folder in result)
                    {
                        folder.Percentage = Math.Round((double)folder.SizeBytes / totalSize * 100, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error analyzing folders: {ex.Message}");
            }
            
            return result;
        }

        private bool ShouldSkipFolder(string folderPath)
        {
            // List of known protected or system folders that should be skipped
            string[] knownProtectedFolders = new[]
            {
                "Config.Msi",
                "System Volume Information",
                "$RECYCLE.BIN",
                "$Recycle.Bin",
                "WindowsApps",
                "Documents and Settings",
                "ProgramData",
                "Recovery",
                "Boot",
                "$Windows.~BT",
                "$Windows.~WS",
                "Windows.old"
            };
            
            string folderName = Path.GetFileName(folderPath);
            
            // Also skip hidden or system folders
            try
            {
                var attr = File.GetAttributes(folderPath);
                if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden ||
                    (attr & FileAttributes.System) == FileAttributes.System)
                {
                    return true;
                }
            }
            catch
            {
                // If we can't get attributes, better to skip
                return true;
            }
            
            // Skip folders from our known list
            return knownProtectedFolders.Contains(folderName);
        }

        private long CalculateFolderSize(string folderPath)
        {
            long size = 0;
            
            try
            {
                // Add size of all files in the current directory
                var files = Directory.GetFiles(folderPath);
                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                    catch
                    {
                        // Skip files we can't access
                    }
                }
                
                // Recursively process all subdirectories
                var directories = Directory.GetDirectories(folderPath);
                foreach (var dir in directories)
                {
                    try
                    {
                        // Skip known protected folders in subdirectories too
                        if (ShouldSkipFolder(dir))
                        {
                            continue;
                        }
                        size += CalculateFolderSize(dir);
                    }
                    catch
                    {
                        // Skip directories we can't access
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip folders we can't access
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating folder size: {ex.Message}");
            }
            
            return size;
        }
        
        private static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            
            return $"{len:0.##} {sizes[order]}";
        }
    }
}