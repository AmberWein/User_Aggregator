using System.Text;
using UserAggregator.Models;

namespace UserAggregator.Exporters
{
    public class CsvExporter : IExporter
    {
        // Exports a list of users to a specified file path in CSV format.
        public async Task ExportAsync(List<User> users, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var csvBuilder = new StringBuilder(users.Count * 50); // Initial capacity to reduce memory allocations
                csvBuilder.AppendLine("FirstName,LastName,Email,SourceId");

                foreach (var user in users)
                {
                    csvBuilder.AppendLine($"{EscapeCsvValue(user.FirstName)},{EscapeCsvValue(user.LastName)},{EscapeCsvValue(user.Email)},{EscapeCsvValue(user.SourceId)}");
                }

                await File.WriteAllTextAsync(filePath, csvBuilder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to export CSV: {ex.Message}");
            }
        }

        private string EscapeCsvValue(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}
