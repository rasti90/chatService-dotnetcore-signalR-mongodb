using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.ViewModels;

namespace ChatServer.Service.Contract
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetUserInformation(string userId);
    }
}