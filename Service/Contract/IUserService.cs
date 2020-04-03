using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Model;
using ChatServer.Model.ViewModels;

namespace ChatServer.Service.Contract {
    public interface IUserService {
        Task<List<UserChatVM>> GetUserChats (string appId, string userId);
    }
}