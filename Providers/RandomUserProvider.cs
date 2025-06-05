using System.Text.Json;
using System.Net.Http.Json;
using UserAggregator.Models;

namespace UserAggregator.Providers
{
    // Fetches users from the RandomUser API.
    public class RandomUserProvider : BaseUserProvider
    {
        private const string ApiUrl = "https://randomuser.me/api/?results=500";

        public RandomUserProvider(HttpClient? httpClient = null) : base(httpClient) { }

        public override async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(ApiUrl);

                if (response.ValueKind != JsonValueKind.Object || !response.TryGetProperty("results", out var results))
                {
                    LogWarning("Unexpected JSON structure from RandomUser API.");
                    return users;
                }

                foreach (var userElement in results.EnumerateArray())
                {
                    users.Add(new User(
                        userElement.GetProperty("name").GetProperty("first").GetString() ?? string.Empty,
                        userElement.GetProperty("name").GetProperty("last").GetString() ?? string.Empty,
                        userElement.GetProperty("email").GetString() ?? string.Empty,
                        userElement.GetProperty("login").GetProperty("uuid").GetString() ?? string.Empty
                    ));
                }
            }
            catch (HttpRequestException ex)
            {
                LogError(nameof(RandomUserProvider), ex);
            }
            catch (JsonException ex)
            {
                LogError(nameof(RandomUserProvider), ex);
            }
            catch (Exception ex)
            {
                LogError(nameof(RandomUserProvider), ex);
            }

            return users;
        }
    }
}
