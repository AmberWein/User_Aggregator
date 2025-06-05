using UserAggregator.Models;
using UserAggregator.Providers;

namespace UserAggregator.Services
{
    public class UserAggregatorService
    {
        private readonly IEnumerable<IUserProvider> _userProviders;

        public UserAggregatorService(IEnumerable<IUserProvider> userProviders)
        {
            _userProviders = userProviders;
        }

        // aggregates users from all configured providers asynchronously.
        // <returns>A list of distinct users aggregated from all providers.</returns>
        public async Task<List<User>> AggregateUsersAsync()
        {
            var userTasks = _userProviders.Select(async provider =>
            {
                try
                {
                    return await provider.GetUsersAsync() ?? new List<User>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching users from {provider.GetType().Name}: {ex.Message}");
                    return new List<User>();
                }
            });

            var results = await Task.WhenAll(userTasks);

            var allUsers = results.SelectMany(users => users).ToList();

            // remove duplicate users by SourceId
            return allUsers.DistinctBy(u => u.SourceId).ToList();
        }
    }
}