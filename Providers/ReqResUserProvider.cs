using System.Text.Json;
using System.Net.Http.Json;
using UserAggregator.Models;
using DotNetEnv;

namespace UserAggregator.Providers
{
    // Fetches paginated user data from the ReqRes API.
    public class ReqResUserProvider : BaseUserProvider
    {
        private const string ApiUrlTemplate = "https://reqres.in/api/users?page={0}";

        public ReqResUserProvider(HttpClient? httpClient = null) : base(httpClient)
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("UserAggregatorApp/1.0");

            Env.Load();

            var apiKey = Environment.GetEnvironmentVariable("REQRES_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                LogWarning("Missing REQRES_API_KEY environment variable");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
            }
        }

        public override async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();
            int currentPage = 1;
            int totalPages = 1;

            try
            {
                do
                {
                    var url = string.Format(ApiUrlTemplate, currentPage);
                    using var responseMessage = await _httpClient.GetAsync(url);

                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        LogWarning($"HTTP {(int)responseMessage.StatusCode} {responseMessage.ReasonPhrase} for page {currentPage}");
                        break;
                    }

                    var response = await responseMessage.Content.ReadFromJsonAsync<JsonElement>();

                    if (response.ValueKind != JsonValueKind.Object)
                    {
                        LogWarning("Unexpected JSON structure from ReqRes API.");
                        break;
                    }

                    if (response.TryGetProperty("total_pages", out var totalPagesElement))
                    {
                        totalPages = totalPagesElement.GetInt32();
                    }

                    if (response.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var userElement in dataElement.EnumerateArray())
                        {
                            users.Add(new User(
                                firstName: userElement.GetProperty("first_name").GetString() ?? string.Empty,
                                lastName: userElement.GetProperty("last_name").GetString() ?? string.Empty,
                                email: userElement.GetProperty("email").GetString() ?? string.Empty,
                                sourceId: userElement.GetProperty("id").GetInt32().ToString()
                            ));
                        }
                    }
                    else
                    {
                        LogWarning($"Missing or invalid 'data' array on page {currentPage}");
                        break;
                    }

                    currentPage++;
                }
                while (currentPage <= totalPages);
            }
            catch (HttpRequestException ex)
            {
                LogError(nameof(ReqResUserProvider), ex);
            }
            catch (JsonException ex)
            {
                LogError(nameof(ReqResUserProvider), ex);
            }
            catch (Exception ex)
            {
                LogError(nameof(ReqResUserProvider), ex);
            }

            return users;
        }
    }
}
