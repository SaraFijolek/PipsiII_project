
using Microsoft.Extensions.Configuration;
namespace Schematics.API.Service
{
    public class LogService : ILogService
    {
        private readonly string _logFilePath;

        public LogService(IConfiguration config)
        {
            _logFilePath = config["Logging:LogFilePath"] ?? "Logs/logs-";
        }

        public async Task<IList<string>> ReadLastLinesAsync(string date, int lines = 200)
        {
            var path = $"{_logFilePath}/{date}.txt";
            if (!File.Exists(path)) return new List<string> { "Log file not found." };

            
            var result = new List<string>();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                var fileLines = new List<string>();
                while (!sr.EndOfStream)
                    fileLines.Add(await sr.ReadLineAsync() ?? string.Empty);

                var take = Math.Min(lines, fileLines.Count);
                return fileLines.Skip(Math.Max(0, fileLines.Count - take)).ToList();
            }
        }
    }
}
