
using Microsoft.Extensions.Configuration;

namespace Schematics.API.Service
{
    public class LogService : ILogService
    {
        private readonly string _logDirectory;

        public LogService(IConfiguration config)
        {
            _logDirectory = config["Logging:LogDirectory"] ?? "Logs";
        }

        public async Task<IList<string>> ReadLastLinesAsync(string date, int lines = 200)
        {
         
            var fileName = $"logs-{date}.txt";
            var path = Path.Combine(_logDirectory, fileName);

            if (!File.Exists(path))
            {
                return new List<string> { $"Log file not found: {fileName}" };
            }

            try
            {
                var allLines = new List<string>();

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    string? line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        allLines.Add(line);
                    }
                }

               
                var take = Math.Min(lines, allLines.Count);
                return allLines.Skip(allLines.Count - take).ToList();
            }
            catch (Exception ex)
            {
                return new List<string> { $"Error reading log file: {ex.Message}" };
            }
        }
    }
}