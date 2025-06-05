using System.Net.Http.Json;
using UserAggregator.Models;
using System.Text.Json;

namespace UserAggregator.Providers
{
    // Fetches users from the JSONPlaceholder API.
    public class JsonPlaceholderUserProvider : IUserProvider
    {
        private readonly HttpClient _httpClient = new();

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>("https://jsonplaceholder.typicode.com/users");

                if (response.ValueKind != JsonValueKind.Array)
                {
                    Console.WriteLine("Warning: Unexpected JSON format from JSONPlaceholder.");
                    return users;
                }

                foreach (var element in response.EnumerateArray())
                {
                    var fullName = element.GetProperty("name").GetString() ?? string.Empty;
                    var nameParts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                    var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                    users.Add(new User
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = element.GetProperty("email").GetString() ?? string.Empty,
                        SourceId = element.GetProperty("id").GetRawText()
                    });
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP error in {nameof(JsonPlaceholderUserProvider)}: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON parsing error in {nameof(JsonPlaceholderUserProvider)}: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in {nameof(JsonPlaceholderUserProvider)}: {ex.Message}");
            }

            return users;
        }
    }
}
