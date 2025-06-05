using System.Text.Json;
using System.Net.Http.Json;
using UserAggregator.Models;

namespace UserAggregator.Providers
{
    // Fetches users from the DummyJSON API.
    public class DummyJsonUserProvider : BaseUserProvider
    {
        private const string ApiUrl = "https://dummyjson.com/users";

        public DummyJsonUserProvider(HttpClient? httpClient = null) : base(httpClient) { }

        public override async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(ApiUrl);

                if (!response.TryGetProperty("users", out var usersArray))
                {
                    LogWarning("'users' property not found in DummyJSON response.");
                    return users;
                }

                foreach (var userElement in usersArray.EnumerateArray())
                {
                    users.Add(new User(
                        userElement.GetProperty("firstName").GetString() ?? string.Empty,
                        userElement.GetProperty("lastName").GetString() ?? string.Empty,
                        userElement.GetProperty("email").GetString() ?? string.Empty,
                        userElement.GetProperty("id").GetInt32().ToString()
                    ));
                }
            }
            catch (HttpRequestException ex)
            {
                LogError(nameof(DummyJsonUserProvider), ex);
            }
            catch (JsonException ex)
            {
                LogError(nameof(DummyJsonUserProvider), ex);
            }
            catch (Exception ex)
            {
                LogError(nameof(DummyJsonUserProvider), ex);
            }

            return users;
        }
    }
}
