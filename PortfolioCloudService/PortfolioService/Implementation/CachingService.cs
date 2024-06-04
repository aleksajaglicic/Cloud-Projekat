using Common.Interfaces;
using System;
using System.IO;

public class CachingService : ICachingService
{
    private readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);

    public bool IsCacheValid(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return false;
        }

        var fileInfo = new FileInfo(filePath);
        return DateTime.Now - fileInfo.LastWriteTime < CacheDuration;
    }

    public string ReadCachedData(string filePath)
    {
        return File.Exists(filePath) ? File.ReadAllText(filePath) : null;
    }

    public void WriteCachedData(string filePath, string data)
    {
        File.WriteAllText(filePath, data);
    }
}
