using System.Text.Json;
using UserAggregator.Models;

namespace UserAggregator.Exporters
{
    public class JsonExporter : IExporter
    {
        // Exports a list of users to a specified file path in JSON format.
        public async Task ExportAsync(List<User> users, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(users, options);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to export JSON: {ex.Message}");
            }
        }
    }
}
