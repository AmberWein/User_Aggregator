using UserAggregator.Models;

namespace UserAggregator.Providers
{
    public interface IUserProvider
    {
        Task<List<User>> GetUsersAsync();
    }
}