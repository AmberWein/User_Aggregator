using System.Net.Http.Json;
using UserAggregator.Models;
using System.Text.Json;

namespace UserAggregator.Providers
{
    // Fetches users from the DummyJSON API.
    public class DummyJsonUserProvider : IUserProvider
    {
        private readonly HttpClient _httpClient = new();

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>("https://dummyjson.com/users");

                if (!response.TryGetProperty("users", out var results))
                {
                    Console.WriteLine("Warning: 'users' property not found in DummyJSON response.");
                    return users;
                }

                foreach (var element in results.EnumerateArray())
                {
                    users.Add(new User
                    {
                        FirstName = element.GetProperty("firstName").GetString() ?? string.Empty,
                        LastName = element.GetProperty("lastName").GetString() ?? string.Empty,
                        Email = element.GetProperty("email").GetString() ?? string.Empty,
                        SourceId = element.GetProperty("id").GetInt32().ToString()
                    });
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP error in {nameof(DummyJsonUserProvider)}: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON parsing error in {nameof(DummyJsonUserProvider)}: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in {nameof(DummyJsonUserProvider)}: {ex.Message}");
            }

            return users;
        }
    }
}
