using System.Net.Http.Json;
using UserAggregator.Models;
using System.Text.Json;

namespace UserAggregator.Providers;
public class JsonPlaceholderUserProvider : IUserProvider
{
    private readonly HttpClient _httpClient = new();

    public async Task<List<User>> GetUsersAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<JsonElement>("https://jsonplaceholder.typicode.com/users");
        var users = new List<User>();
        
        foreach (var element in response.EnumerateArray())
        {
            var fullName = element.GetProperty("name").GetString() ?? string.Empty;
            var nameParts = fullName.Split(' ', 2);
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
        return users;
    }
}