namespace Schematics.API.Service
{
    public interface ILogService
    {
        Task<IList<string>> ReadLastLinesAsync(string date, int lines = 200);
    }
}
