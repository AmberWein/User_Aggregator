using UserAggregator.Models;
using System.Text.Json;

namespace UserAggregator.Providers;
public class DummyJsonUserProvider : IUserProvider
{
    private readonly HttpClient _httpClient = new();

    public async Task<List<User>> GetUsersAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>("https://dummyjson.com/users");
        var results = response.GetProperty("users");

        var users = new List<User>();

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
        return users;
    }
}