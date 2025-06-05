using UserAggregator.Models;

namespace UserAggregator.Exporters
{
    public interface IExporter
    {
        // Exports a list of users to a specified file path.
        Task ExportAsync(List<User> users, string filePath);
    }
}