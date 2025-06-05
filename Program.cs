// using UserAggregator.Models;
// using UserAggregator.Providers;
// using UserAggregator.Exporters;
// using UserAggregator.Services;

// Console.WriteLine("Enter output folder path:");
// string? folderPath = Console.ReadLine()?.Trim();

// if (string.IsNullOrEmpty(folderPath))
// {
//     Console.WriteLine("Folder path cannot be empty.");
//     return;
// }

// if (!Directory.Exists(folderPath))
// {
//     Directory.CreateDirectory(folderPath);
// }

// Console.WriteLine("Choose file format: JSON or CSV");
// string? format = Console.ReadLine()?.Trim().ToLower();

// if (format != "json" && format != "csv")
// {
//     Console.WriteLine("Invalid format selected. Defaulting to JSON.");
//     format = "json";
// }

// string fileName = $"users_output.{format}";
// string fullPath = Path.Combine(folderPath, fileName);

// IExporter exporter = format switch
// {
//     "csv" => new CsvExporter(),
//     _ => new JsonExporter()
// };

// var providers = new List<IUserProvider>
// {
//     new RandomUserProvider(),
//     new JsonPlaceholderUserProvider(),
//     new DummyJsonUserProvider(),
//     new ReqResUserProvider()
// };

// var aggregator = new UserAggregatorService(providers);

// try
// {
//     var users = await aggregator.AggregateUsersAsync();
//     await exporter.ExportAsync(users, fullPath);
//     Console.WriteLine($"Export complete! {users.Count} users saved to {fullPath}");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"An error occurred: {ex.Message}");
// }


using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UserAggregator.Models;
using UserAggregator.Exporters;
using UserAggregator.Providers;
using UserAggregator.Services;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Enter output folder path:");
        string? folderPath = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(folderPath))
        {
            Console.WriteLine("Folder path cannot be empty.");
            return;
        }

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        Console.WriteLine("Choose file format: JSON or CSV");
        string? format = Console.ReadLine()?.Trim().ToLower();

        if (format != "json" && format != "csv")
        {
            Console.WriteLine("Invalid format selected. Defaulting to JSON.");
            format = "json";
        }

        string fileName = $"users_output.{format}";
        string fullPath = Path.Combine(folderPath, fileName);

        IExporter exporter = format switch
        {
            "csv" => new CsvExporter(),
            _ => new JsonExporter()
        };

        var providers = new List<IUserProvider>
        {
            new RandomUserProvider(),
            new JsonPlaceholderUserProvider(),
            new DummyJsonUserProvider(),
            new ReqResUserProvider()
        };

        var aggregator = new UserAggregatorService(providers);

        try
        {
            var users = await aggregator.AggregateUsersAsync();
            await exporter.ExportAsync(users, fullPath);
            Console.WriteLine($"Export complete! {users.Count} users saved to {fullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
