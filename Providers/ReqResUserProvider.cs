using System.Net.Http.Json;
using UserAggregator.Models;
using System.Text.Json;

namespace UserAggregator.Providers
{
    /// Fetches paginated user data from the ReqRes API.
    public class ReqResUserProvider : IUserProvider
    {
        private readonly HttpClient _httpClient = new();

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();
            int currentPage = 1;
            int totalPages = 1;

            try
            {
                do
                {
                    var response = await _httpClient.GetFromJsonAsync<JsonElement>($"https://reqres.in/api/users?page={currentPage}");

                    if (response.ValueKind != JsonValueKind.Object)
                    {
                        Console.WriteLine("Warning: Unexpected JSON structure from ReqRes API.");
                        break;
                    }

                    if (response.TryGetProperty("total_pages", out var totalPagesElement))
                    {
                        totalPages = totalPagesElement.GetInt32();
                    }

                    if (response.TryGetProperty("data", out var dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var element in dataElement.EnumerateArray())
                        {
                            users.Add(new User
                            {
                                FirstName = element.GetProperty("first_name").GetString() ?? string.Empty,
                                LastName = element.GetProperty("last_name").GetString() ?? string.Empty,
                                Email = element.GetProperty("email").GetString() ?? string.Empty,
                                SourceId = element.GetProperty("id").GetInt32().ToString()
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Missing or invalid 'data' on page {currentPage}");
                        break;
                    }

                    currentPage++;

                } while (currentPage <= totalPages);
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP error in ReqResUserProvider: {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON parsing error in ReqResUserProvider: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in ReqResUserProvider: {ex.Message}");
            }

            return users;
        }
    }
}
