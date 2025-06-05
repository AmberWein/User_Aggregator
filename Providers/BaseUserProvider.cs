using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using UserAggregator.Models;

namespace UserAggregator.Providers
{
    // This abstract class provides a base for user providers, allowing for shared functionality like logging and HTTP client management.
    public abstract class BaseUserProvider : IUserProvider
    {
        protected readonly HttpClient _httpClient;

        protected BaseUserProvider(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public abstract Task<List<User>> GetUsersAsync();

        protected void LogWarning(string message) => Console.WriteLine($"Warning: {message}");

        protected void LogError(string providerName, Exception ex) =>
            Console.WriteLine($"Error in {providerName}: {ex.Message}");
    }
}
