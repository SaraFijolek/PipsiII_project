namespace Schematics.API.Service
{
    public interface ILogService
    {
        Task<IList<string>> ReadLastLinesAsync(int lines = 200);
    }
}
