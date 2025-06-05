using System.Net.Http.Json;
using UserAggregator.Models;
using System.Text.Json;

namespace UserAggregator.Providers;
public class RandomUserProvider : IUserProvider
{
    private readonly HttpClient _httpClient = new();

    public async Task<List<User>> GetUsersAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>("https://randomuser.me/api/?results=500");
        // response.EnsureSuccessStatusCode();
        var results = response.GetProperty("results");

        var users = new List<User>();
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
        return users;
    }
}