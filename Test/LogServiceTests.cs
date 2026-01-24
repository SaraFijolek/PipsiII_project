using Microsoft.Extensions.Configuration;
using Schematics.API.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

public class LogServiceTests : IDisposable
{
    private readonly string _tempFile;

    public LogServiceTests()
    {
        _tempFile = Path.GetTempFileName();
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
            File.Delete(_tempFile);
    }

    private LogService GetService(string? path = null)
    {
        var inMemoryConfig = new Dictionary<string, string>
        {
            { "Logging:LogFilePath", path ?? _tempFile }
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        return new LogService(config);
    }

    [Fact]
    public async Task ReadLastLinesAsync_ReturnsLines_WhenFileExists()
    {
        var lines = new List<string> { "Line1", "Line2", "Line3" };
        await File.WriteAllLinesAsync(_tempFile, lines);

        var service = GetService();

        var result = await service.ReadLastLinesAsync(2);

        Assert.Equal(2, result.Count);
        Assert.Equal("Line2", result[0]);
        Assert.Equal("Line3", result[1]);
    }

    [Fact]
    public async Task ReadLastLinesAsync_ReturnsAllLines_IfRequestedMoreThanFile()
    {
        var lines = new List<string> { "A", "B" };
        await File.WriteAllLinesAsync(_tempFile, lines);

        var service = GetService();

        var result = await service.ReadLastLinesAsync(10);

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0]);
        Assert.Equal("B", result[1]);
    }

    [Fact]
    public async Task ReadLastLinesAsync_ReturnsMessage_IfFileNotExists()
    {
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".log");
        var service = GetService(nonExistentPath);

        var result = await service.ReadLastLinesAsync(5);

        Assert.Single(result);
        Assert.Equal("Log file not found.", result[0]);
    }

    [Fact]
    public async Task ReadLastLinesAsync_HandlesEmptyFile()
    {
        File.WriteAllText(_tempFile, "");
        var service = GetService();

        var result = await service.ReadLastLinesAsync(5);

        Assert.Empty(result);
    }
}

