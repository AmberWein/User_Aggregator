using System.Text.Json;
using System.Text;
using UserAggregator.Models;
using UserAggregator.Providers;

var httpClient = new HttpClient();
var reqresApiKey = Environment.GetEnvironmentVariable("REQRES_API_KEY");

// Add API key header if available
if (!string.IsNullOrWhiteSpace(reqresApiKey))
{
    httpClient.DefaultRequestHeaders.Add("x-api-key", reqresApiKey);
}

// Create all user providers
var providers = new List<IUserProvider>
{
    new RandomUserProvider(httpClient),
    new JsonPlaceholderUserProvider(httpClient),
    new DummyJsonUserProvider(httpClient),
    new ReqResUserProvider(httpClient)
};

// Prompt user for output folder
Console.Write("Enter output folder path: ");
var folderPath = Console.ReadLine();

while (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
{
    Console.WriteLine("Invalid folder path. Please try again.");
    Console.Write("Enter output folder path: ");
    folderPath = Console.ReadLine();
}

// Prompt user for format
Console.Write("Enter output format (json/csv): ");
var format = Console.ReadLine()?.Trim().ToLower();

while (format != "json" && format != "csv")
{
    Console.WriteLine("Invalid format. Please enter 'json' or 'csv'.");
    Console.Write("Enter output format (json/csv): ");
    format = Console.ReadLine()?.Trim().ToLower();
}

try
{
    // Fetch users in parallel
    var userTasks = providers.Select(p => p.GetUsersAsync()).ToList();
    var results = await Task.WhenAll(userTasks);

    var allUsers = results.SelectMany(u => u).ToList();

    // Deduplicate by Email + SourceId
    var uniqueUsers = allUsers
        .GroupBy(u => (u.Email, u.SourceId))
        .Select(g => g.First())
        .ToList();

    // Save result
    var fileName = "aggregated_users";
    var outputPath = Path.Combine(folderPath, fileName);

    if (format == "json")
    {
        var json = JsonSerializer.Serialize(uniqueUsers, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(outputPath, json);
    }
    else if (format == "csv")
    {
        var csv = new StringBuilder();
        csv.AppendLine("FirstName,LastName,Email,SourceId");

        foreach (var user in uniqueUsers)
        {
            csv.AppendLine($"{user.FirstName},{user.LastName},{user.Email},{user.SourceId}");
        }

        await File.WriteAllTextAsync(outputPath, csv.ToString());
    }

    Console.WriteLine($"Total unique users aggregated: {uniqueUsers.Count}");
    Console.WriteLine($"Saved to: {outputPath}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}