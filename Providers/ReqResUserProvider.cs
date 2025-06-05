// This provider fetches users from the ReqRes API, handling pagination to retrieve all users.

using System.Net.Http.Json;
using UserAggregator.Models;
using System.Text.Json;

namespace UserAggregator.Providers;
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

                if (response.TryGetProperty("total_pages", out var totalPagesElement))
                {
                    totalPages = totalPagesElement.GetInt32();
                }

                if (response.TryGetProperty("data", out var dataElement))
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

                currentPage++;
            } while (currentPage <= totalPages);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ReqResUserProvider: {ex.Message}");
        }

        return users;
    }
}
