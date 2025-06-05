using System.Text.Json;
using System.Net.Http.Json;
using UserAggregator.Models;

namespace UserAggregator.Providers
{
    // Fetches users from the JSONPlaceholder API.
    public class JsonPlaceholderUserProvider : BaseUserProvider
    {
        private const string ApiUrl = "https://jsonplaceholder.typicode.com/users";

        public JsonPlaceholderUserProvider(HttpClient? httpClient = null) : base(httpClient) { }

        public override async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>(ApiUrl);

                if (response.ValueKind != JsonValueKind.Array)
                {
                    LogWarning("Unexpected JSON format from JSONPlaceholder.");
                    return users;
                }

                foreach (var userElement in response.EnumerateArray())
                {
                    var fullName = userElement.GetProperty("name").GetString() ?? string.Empty;
                    var nameParts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
                    var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

                    users.Add(new User(
                        firstName,
                        lastName,
                        userElement.GetProperty("email").GetString() ?? string.Empty,
                        userElement.GetProperty("id").GetRawText()
                    ));
                }
            }
            catch (HttpRequestException ex)
            {
                LogError(nameof(JsonPlaceholderUserProvider), ex);
            }
            catch (JsonException ex)
            {
                LogError(nameof(JsonPlaceholderUserProvider), ex);
            }
            catch (Exception ex)
            {
                LogError(nameof(JsonPlaceholderUserProvider), ex);
            }

            return users;
        }
    }
}
