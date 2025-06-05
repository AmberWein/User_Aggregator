using System.Net.Http.Json;
using UserAggregator.Models;
using System.Text.Json;

namespace UserAggregator.Providers
{
    // Fetches users from the RandomUser API.
    public class RandomUserProvider : IUserProvider
    {
        private readonly HttpClient _httpClient = new();

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>("https://randomuser.me/api/?results=500");

                if (response.ValueKind != JsonValueKind.Object || !response.TryGetProperty("results", out var results))
                {
                    Console.WriteLine("Warning: Unexpected JSON structure from RandomUser API.");
                    return users;
                }

                foreach (var element in results.EnumerateArray())
                {
                    users.Add(new User
                    {
                        FirstName = element.GetProperty("name").GetProperty("first").GetString() ?? string.Empty,
                        LastName = element.GetProperty("name").GetProperty("last").GetString() ?? string.Empty,
                        Email = element.GetProperty("email").GetString() ?? string.Empty,
                        SourceId = element.GetProperty("login").GetProperty("uuid").GetString() ?? string.Empty
                    });
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP error in {nameof(RandomUserProvider)}: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON parsing error in {nameof(RandomUserProvider)}: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in {nameof(RandomUserProvider)}: {ex.Message}");
            }

            return users;
        }
    }
}
