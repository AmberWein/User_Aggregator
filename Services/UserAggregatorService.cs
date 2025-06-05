using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAggregator.Models;
using UserAggregator.Providers;

namespace UserAggregator.Services
{
    // aggregates users from all configured providers asynchronously.
    public class UserAggregatorService
    {
        private readonly IEnumerable<IUserProvider> _userProviders;

        public UserAggregatorService(IEnumerable<IUserProvider> userProviders)
        {
            _userProviders = userProviders ?? throw new ArgumentNullException(nameof(userProviders));
        }

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
                    Console.Error.WriteLine($"Error fetching users from {provider.GetType().Name}: {ex.Message}");
                    return new List<User>();
                }
            });

            var results = await Task.WhenAll(userTasks);

            var allUsers = results.SelectMany(users => users).ToList();

            return allUsers.DistinctBy(u => u.SourceId).ToList();
        }
    }
}
