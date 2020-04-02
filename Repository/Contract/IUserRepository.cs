using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;

namespace ChatServer.Repository.Contract
{
    public interface IUserRepository
    {
        List<User> GetByAppId(string appId);
        Task<List<User>> GetByAppIdAsync(string appId);
        User Get(string id);
        Task<User> GetAsync(string id);
        User GetByExternalId(string appId, string externalId);
        Task<User> GetByExternalIdAsync(string appId, string externalId);
        User GetByConnectionId(string connectionId);
        Task<User> GetByConnectionIdAsync(string connectionId);
        User Create(User user);
        Task<User> CreateAsync(User user);
        void Update(string id, User userIn);
        Task UpdateAsync(string id, User userIn);
        void AddActivityAndManageConnectionToUser(string userId, Activity activity, Connection connection);
        Task AddActivityAndManageConnectionToUserAsync(string userId, Activity activity, Connection connection);
        void Remove(User userIn);
        Task RemoveAsync(User userIn);
        void Remove(string id);
        Task RemoveAsync(string id);
    }
}
